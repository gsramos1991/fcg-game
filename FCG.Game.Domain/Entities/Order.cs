namespace FCG.Game.Domain.Entities;

public class Order
{
    public Guid Id { get;  set; }
    public Guid UserId { get;  set; }
    public Guid? PaymentId { get;  set; }
    public decimal TotalAmount { get;  set; }
    public OrderStatus Status { get;  set; }
    public DateTime CreatedAt { get;  set; }
    public DateTime? CompletedAt { get;  set; }

    public void Complete()
    {
        if (Status != OrderStatus.Pending)
            throw new InvalidOperationException("Pedido j� foi processado");

        Status = OrderStatus.Completed;
        CompletedAt = DateTime.UtcNow;
    }

    public void Cancel()
    {
        if (Status == OrderStatus.Completed)
            throw new InvalidOperationException("N�o � poss�vel cancelar pedido completado");

        Status = OrderStatus.Cancelled;
    }

    public void Fail(string reason)
    {
        Status = OrderStatus.Failed;
    }
}

public class OrderItem
{
    public Guid Id { get; set; }
    public Guid GameId { get; set; }
    public string GameTitle { get; set; }
    public decimal Price { get; set; }
    public int Quantity { get; set; }
}

public enum OrderStatus
{
    Pending = 0,
    Completed = 1,
    Cancelled = 2,
    Failed = 3
}