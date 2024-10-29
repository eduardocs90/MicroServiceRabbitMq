namespace WebApplicationNotifyer.Interface
{
    public interface INotificationService
    {
        void NotifyerUser(string email, string subject, string body);
    }
}
