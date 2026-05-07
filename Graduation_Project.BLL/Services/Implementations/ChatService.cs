using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Graduation_Project.BLL.Common;
using Graduation_Project.BLL.DTOs.Chat;
using Graduation_Project.BLL.Services.Interfaces;
using Graduation_Project.DAL.Models.Entities;
using Graduation_Project.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Graduation_Project.BLL.Services.Implementations
{
    public class ChatService : IChatService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ChatService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // ── Get existing conversation or create new one ──
        public async Task<ServiceResult<int>> GetOrCreateConversationAsync(int customerId, int brandOwnerId)
        {
            try
            {
                var existing = await _unitOfWork.Conversations
                    .GetQueryable()
                    .FirstOrDefaultAsync(c =>
                        (c.CustomerId == customerId && c.BrandOwnerId == brandOwnerId) ||
                        (c.CustomerId == brandOwnerId && c.BrandOwnerId == customerId));

                if (existing != null)
                    return ServiceResult<int>.Success(existing.ConversationId);

                var conversation = new Conversation
                {
                    CustomerId = customerId,
                    BrandOwnerId = brandOwnerId,
                    CreatedAt = DateTime.UtcNow
                };

                await _unitOfWork.Conversations.AddAsync(conversation);
                await _unitOfWork.SaveAsync();

                return ServiceResult<int>.Success(conversation.ConversationId);
            }
            catch (Exception ex)
            {
                return ServiceResult<int>.Failure($"Error: {ex.Message}");
            }
        }

        // ── Send a message ──
        public async Task<ServiceResult<MessageDto>> SendMessageAsync(int senderId, SendMessageDto dto)
        {
            try
            {
                var sender = await _unitOfWork.ApplicationUsers.GetByIdAsync(senderId);
                if (sender == null)
                    return ServiceResult<MessageDto>.Failure("Sender not found.");

                var receiver = await _unitOfWork.ApplicationUsers.GetByIdAsync(dto.ReceiverId);
                if (receiver == null)
                    return ServiceResult<MessageDto>.Failure("Receiver not found.");

                // Get or create conversation
                var convResult = await GetOrCreateConversationAsync(senderId, dto.ReceiverId);
                if (!convResult.Succeeded)
                    return ServiceResult<MessageDto>.Failure(convResult.Message);

                var message = new Message
                {
                    ConversationId = convResult.Data,
                    SenderId = senderId,
                    MessageText = dto.MessageText,
                    SentAt = DateTime.UtcNow,
                    IsRead = false
                };

                await _unitOfWork.Messages.AddAsync(message);

                // Update LastMessageAt
                var conversation = await _unitOfWork.Conversations.GetByIdAsync(convResult.Data);
                if (conversation != null)
                {
                    conversation.LastMessageAt = DateTime.UtcNow;
                    _unitOfWork.Conversations.Update(conversation);
                }

                await _unitOfWork.SaveAsync();

                return ServiceResult<MessageDto>.Success(new MessageDto
                {
                    MessageId = message.MessageId,
                    ConversationId = message.ConversationId,
                    SenderId = senderId,
                    SenderName = sender.UserName,
                    MessageText = message.MessageText,
                    SentAt = message.SentAt,
                    IsRead = false
                });
            }
            catch (Exception ex)
            {
                return ServiceResult<MessageDto>.Failure($"Error: {ex.Message}");
            }
        }

        // ── Get all messages in a conversation ──
        public async Task<ServiceResult<IEnumerable<MessageDto>>> GetConversationMessagesAsync(int conversationId, int userId)
        {
            try
            {
                // تأكد إن اليوزر جزء من المحادثة
                var conversation = await _unitOfWork.Conversations
                    .GetQueryable()
                    .FirstOrDefaultAsync(c => c.ConversationId == conversationId &&
                                             (c.CustomerId == userId || c.BrandOwnerId == userId));

                if (conversation == null)
                    return ServiceResult<IEnumerable<MessageDto>>.Failure("Conversation not found or access denied.");

                var messages = await _unitOfWork.Messages
                    .GetQueryable()
                    .Include(m => m.Sender)
                    .Where(m => m.ConversationId == conversationId)
                    .OrderBy(m => m.SentAt)
                    .ToListAsync();

                var messageDtos = messages.Select(m => new MessageDto
                {
                    MessageId = m.MessageId,
                    ConversationId = m.ConversationId,
                    SenderId = m.SenderId,
                    SenderName = m.Sender?.UserName,
                    MessageText = m.MessageText,
                    SentAt = m.SentAt,
                    IsRead = m.IsRead
                });

                return ServiceResult<IEnumerable<MessageDto>>.Success(messageDtos);
            }
            catch (Exception ex)
            {
                return ServiceResult<IEnumerable<MessageDto>>.Failure($"Error: {ex.Message}");
            }
        }

        // ── Get all conversations for a user ──
        public async Task<ServiceResult<IEnumerable<ConversationDto>>> GetMyConversationsAsync(int userId)
        {
            try
            {
                var conversations = await _unitOfWork.Conversations
                    .GetQueryable()
                    .Include(c => c.Customer)
                    .Include(c => c.BrandOwner)
                    .Include(c => c.Messages)
                    .Where(c => c.CustomerId == userId || c.BrandOwnerId == userId)
                    .OrderByDescending(c => c.LastMessageAt)
                    .ToListAsync();

                var result = conversations.Select(c =>
                {
                    var otherUser = c.CustomerId == userId ? c.BrandOwner : c.Customer;
                    var lastMsg = c.Messages?.OrderByDescending(m => m.SentAt).FirstOrDefault();

                    return new ConversationDto
                    {
                        ConversationId = c.ConversationId,
                        OtherUserId = otherUser?.Id ?? 0,
                        OtherUserName = otherUser?.UserName,
                        LastMessage = lastMsg?.MessageText,
                        LastMessageAt = c.LastMessageAt,
                        UnreadCount = c.Messages?.Count(m => !m.IsRead && m.SenderId != userId) ?? 0
                    };
                });

                return ServiceResult<IEnumerable<ConversationDto>>.Success(result);
            }
            catch (Exception ex)
            {
                return ServiceResult<IEnumerable<ConversationDto>>.Failure($"Error: {ex.Message}");
            }
        }

        // ── Mark all messages as read ──
        public async Task<ServiceResult<bool>> MarkMessagesAsReadAsync(int conversationId, int userId)
        {
            try
            {
                var unread = await _unitOfWork.Messages
                    .FindAsync(m => m.ConversationId == conversationId
                                 && m.SenderId != userId
                                 && !m.IsRead);

                foreach (var msg in unread)
                {
                    msg.IsRead = true;
                    _unitOfWork.Messages.Update(msg);
                }

                await _unitOfWork.SaveAsync();

                return ServiceResult<bool>.Success(true);
            }
            catch (Exception ex)
            {
                return ServiceResult<bool>.Failure($"Error: {ex.Message}");
            }
        }
    }
}