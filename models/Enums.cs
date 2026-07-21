namespace TradeSpace.Models;

public enum UserRole
{
    Customer = 1,
    Seller = 2,
    Admin = 3
}

public enum OrderStatus
{
    Pending = 1,     // Чекає на оплату
    Paid = 2,        // Оплачено
    Processing = 3,  // У списку продавця
    Shipped = 4,     // Відправлено
    Delivered = 5,   // Доставлено
    Cancelled = 6    // Відмінено
}

public enum PaymentStatus
{
    Pending = 1,
    Completed = 2,
    Failed = 3,
    Refunded = 4
}