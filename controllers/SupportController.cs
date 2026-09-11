using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TradeSpace.Data;
using TradeSpace.Models;

namespace TradeSpace.Controllers;

[Authorize]
public class SupportController : Controller
{
    private readonly ApplicationDbContext _context;

    public SupportController(ApplicationDbContext context)
    {
        _context = context;
    }

    private Guid GetCurrentUserId()
    {
        var idClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return Guid.TryParse(idClaim, out var id) ? id : Guid.Empty;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var userId = GetCurrentUserId();
        var isAdmin = User.IsInRole("Admin");

        var query = _context.SupportTickets
            .Include(t => t.User)
            .AsQueryable();

        if (!isAdmin)
        {
            query = query.Where(t => t.UserId == userId);
        }

        var tickets = await query.OrderByDescending(t => t.UpdatedAt).ToListAsync();
        return View(tickets);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(string subject, string message)
    {
        if (string.IsNullOrWhiteSpace(subject) || string.IsNullOrWhiteSpace(message))
        {
            ModelState.AddModelError("", "Заполните все поля.");
            return View();
        }

        var userId = GetCurrentUserId();
        var ticket = new SupportTicket
        {
            UserId = userId,
            Subject = subject,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var ticketMessage = new TicketMessage
        {
            TicketId = ticket.Id,
            SenderId = userId,
            Text = message,
            SentAt = DateTime.UtcNow
        };

        _context.SupportTickets.Add(ticket);
        _context.TicketMessages.Add(ticketMessage);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Details), new { id = ticket.Id });
    }

    [HttpGet]
    public async Task<IActionResult> Details(Guid id)
    {
        var userId = GetCurrentUserId();
        var isAdmin = User.IsInRole("Admin");

        var ticket = await _context.SupportTickets
            .Include(t => t.User)
            .Include(t => t.Messages)
                .ThenInclude(m => m.Sender)
            .FirstOrDefaultAsync(t => t.Id == id);

        if (ticket == null) return NotFound();
        if (!isAdmin && ticket.UserId != userId) return Forbid();

        return View(ticket);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SendMessage(Guid ticketId, string text)
    {
        if (string.IsNullOrWhiteSpace(text)) return RedirectToAction(nameof(Details), new { id = ticketId });

        var userId = GetCurrentUserId();
        var isAdmin = User.IsInRole("Admin");

        var ticket = await _context.SupportTickets.FindAsync(ticketId);
        if (ticket == null) return NotFound();
        if (!isAdmin && ticket.UserId != userId) return Forbid();

        var message = new TicketMessage
        {
            TicketId = ticketId,
            SenderId = userId,
            Text = text,
            SentAt = DateTime.UtcNow
        };

        ticket.UpdatedAt = DateTime.UtcNow;
        _context.TicketMessages.Add(message);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Details), new { id = ticketId });
    }
}