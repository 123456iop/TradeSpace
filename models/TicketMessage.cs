using System;

namespace TradeSpace.Models;

public class TicketMessage
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TicketId { get; set; }
    public SupportTicket Ticket { get; set; } = null!;
    public Guid SenderId { get; set; }
    public User Sender { get; set; } = null!;
    public string Text { get; set; } = string.Empty;
    public DateTime SentAt { get; set; } = DateTime.UtcNow;
}