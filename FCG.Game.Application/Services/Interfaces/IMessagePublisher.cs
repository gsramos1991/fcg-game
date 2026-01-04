namespace FCG.Game.Application.Services.Interfaces
{
    public interface IMessagePublisher
    {
        void Publish(string message, string queueName);
    }
}
