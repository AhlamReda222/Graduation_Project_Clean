using Graduation_Project.BLL.Services.Interfaces;
using Graduation_Project.DAL.Models.Entities;
using Graduation_Project.DAL.Repositories.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Mail;

namespace Graduation_Project.BLL.Services.Implementations
{
    /// <summary>
    /// بيبعت الإيميل في مكانين دايماً:
    /// 1. InAppEmail في الـ DB عشان يظهر جوه الموقع
    /// 2. Gmail حقيقي على إيميل اليوزر
    /// </summary>
    public class SmartEmailSender : IEmailSender
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _config;
        private int _receiverId;

        public SmartEmailSender(IUnitOfWork unitOfWork, IConfiguration config)
        {
            _unitOfWork = unitOfWork;
            _config = config;
        }

        public void SetReceiverId(int userId)
        {
            _receiverId = userId;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            // ── 1. InApp Email في DB ──
            if (_receiverId > 0)
            {
                try
                {
                    var inAppEmail = new InAppEmail
                    {
                        UserId = _receiverId,
                        Subject = subject,
                        Body = body,
                        IsRead = false,
                        CreatedAt = DateTime.UtcNow
                    };

                    await _unitOfWork.InAppEmails.AddAsync(inAppEmail);
                    await _unitOfWork.SaveAsync();
                }
                catch { /* InApp failure doesn't block Gmail */ }
            }

            // ── 2. Gmail حقيقي ──
            try
            {
                var smtpHost = _config["Email:SmtpHost"];
                var smtpPort = int.Parse(_config["Email:SmtpPort"]);
                var smtpUser = _config["Email:SmtpUser"];
                var smtpPass = _config["Email:SmtpPass"];

                if (string.IsNullOrEmpty(smtpHost) || string.IsNullOrEmpty(smtpUser))
                    return;

                using var client = new SmtpClient(smtpHost, smtpPort)
                {
                    Credentials = new NetworkCredential(smtpUser, smtpPass),
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,  // ✅ أضيفي
                    UseDefaultCredentials = false
                };

                var mail = new MailMessage(smtpUser, toEmail, subject, body)
                {
                    IsBodyHtml = true
                };

                await client.SendMailAsync(mail);
            }
            catch (Exception ex)
            {
                // ✅ غيري catch فاضي لده عشان تشوفي الـ error
                Console.WriteLine($"Gmail Error: {ex.Message}");
            }
        }
    }
}