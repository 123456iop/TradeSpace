using Microsoft.EntityFrameworkCore;
using TradeSpace.Models;

namespace TradeSpace.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; } = null!;
    public DbSet<Order> Orders { get; set; } = null!;
    public DbSet<Product> Products { get; set; } = null!;
    public DbSet<Category> Categories { get; set; } = null!;
    public DbSet<Cart> Carts { get; set; } = null!;
    public DbSet<Review> Reviews { get; set; } = null!;
    public DbSet<Store> Stores { get; set; } = null!;
    public DbSet<SupportTicket> SupportTickets { get; set; } = null!;
    public DbSet<TicketMessage> TicketMessages { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Налаштування каскадного видалення або поведінки для категорій магазину
        modelBuilder.Entity<Category>()
            .HasOne(c => c.Store)
            .WithMany()
            .HasForeignKey(c => c.StoreId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}