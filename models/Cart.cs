namespace TradeSpace.Models;

public class Cart
{
    // Унікальний ідентифікатор кошика
    public Guid Id { get; set; } = Guid.NewGuid();
    
    // Зв'язок з користувачем (власником кошика)
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    // Список товарів у кошику (ініціалізований за замовчуванням)
    public ICollection<CartItem> Items { get; set; } = new List<CartItem>();
}

public class CartItem
{
    // Унікальний ідентифікатор запису в кошику
    public Guid Id { get; set; } = Guid.NewGuid();
    
    // Кількість обраного товару
    public int Quantity { get; set; }

    // Зв'язок з батьківським кошиком
    public Guid CartId { get; set; }
    public Cart Cart { get; set; } = null!;

    // Зв'язок з конкретним товаром
    public Guid ProductId { get; set; }
    public Product Product { get; set; } = null!;
}