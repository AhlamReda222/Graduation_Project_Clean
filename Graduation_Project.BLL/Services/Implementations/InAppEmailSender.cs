using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Graduation_Project.BLL.Services.Interfaces;
using Graduation_Project.DAL.Models.Entities;
using Graduation_Project.DAL.Repositories.Interfaces;

namespace Graduation_Project.BLL.Services.Implementations
{
    // ده بيحل محل الـ EmailSender القديم
    // بدل ما يبعت email حقيقي، بيحفظه في DB عشان يظهر جوه الموقع
    public class InAppEmailSender : IEmailSender
    {
        private readonly IUnitOfWork _unitOfWork;
        private int _currentUserId;

        public InAppEmailSender(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // بيتسيت من الـ Service قبل الاستخدام
        public void SetReceiverId(int userId)
        {
            _currentUserId = userId;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            var email = new InAppEmail
            {
                UserId = _currentUserId,
                Subject = subject,
                Body = body,
                IsRead = false,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.InAppEmails.AddAsync(email);
            await _unitOfWork.SaveAsync();
        }
    }
}