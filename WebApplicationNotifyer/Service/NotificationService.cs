using WebApplicationNotifyer.Interface;

namespace WebApplicationNotifyer.Service
{
    public class NotificationService : INotificationService
    {
        private readonly IEmailService _emailService;

        public NotificationService(IEmailService emailService)
        {
            _emailService = emailService;
        }

        public void NotifyerUser(string email, string subject, string body)
        {
            _emailService.SendEmailAsync(email,subject, body);
        }

       
    }
}
