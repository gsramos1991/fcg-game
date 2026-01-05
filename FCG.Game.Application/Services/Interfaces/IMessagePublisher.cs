namespace FCG.Game.Application.Services.Interfaces
{
    public interface IMessagePublisher
    {
        Task Publish(string message, string queueName);
    }
}
