using Shared;


namespace UserService.Service
{
    public interface IMessagePublisher
    {
        void PublishUserCreated(UserNotification notification);
    }
}
