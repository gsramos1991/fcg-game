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
        if (Status != OrderStatus.PENDING)
            throw new InvalidOperationException("Pedido j� foi processado");

        Status = OrderStatus.SUCCESS;
        CompletedAt = DateTime.UtcNow;
    }

    public void Cancel()
    {
        if (Status == OrderStatus.SUCCESS)
            throw new InvalidOperationException("N�o � poss�vel cancelar pedido completado");

        Status = OrderStatus.CANCELLED;
    }

    public void Fail(string reason)
    {
        Status = OrderStatus.FAIL;
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

//public enum OrderStatus
//{
//    Pending = 0,
//    Completed = 1,
//    CANCELLED = 2,
//    Failed = 3
//}

public enum OrderStatus
{
    PENDING = 1,
    SUCCESS = 2,
    CANCELLED = 3,
    REJECTED = 4,

    ERROR = -1,
    FAIL = -2
}