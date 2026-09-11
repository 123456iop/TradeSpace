using System;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TradeSpace.Data;
using TradeSpace.Models;

namespace TradeSpace.Controllers;

[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    private readonly ApplicationDbContext _context;

    public AdminController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        ViewBag.TotalUsers = await _context.Users.CountAsync();
        ViewBag.TotalProducts = await _context.Products.CountAsync();
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> Users()
    {
        var users = await _context.Users
            .OrderByDescending(u => u.CreatedAt)
            .ToListAsync();
            
        return View(users);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangeRole(Guid userId, UserRole newRole)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user != null)
        {
            user.Role = newRole;
            await _context.SaveChangesAsync();
        }
        
        return RedirectToAction(nameof(Users));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteUser(Guid userId)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user != null)
        {
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
        }
        
        return RedirectToAction(nameof(Users));
    }
    
    private Guid GetCurrentUserId()
    {
        var idClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return Guid.TryParse(idClaim, out var id) ? id : Guid.Empty;
    }

    [HttpGet]
    public async Task<IActionResult> Tickets()
    {
        var tickets = await _context.SupportTickets
            .Include(t => t.User)
            .OrderBy(t => t.IsClosed) 
            .ThenByDescending(t => t.CreatedAt)
            .ToListAsync();
        
        return View(tickets);
    }

    [HttpGet]
    public async Task<IActionResult> TicketDetails(Guid id)
    {
        var ticket = await _context.SupportTickets
            .Include(t => t.User)
            .Include(t => t.Messages)
            .ThenInclude(m => m.Sender) // Используем Sender согласно модели TicketMessage[cite: 7]
            .FirstOrDefaultAsync(t => t.Id == id);

        if (ticket == null) return NotFound();
        return View(ticket);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ReplyTicket(Guid ticketId, string message)
    {
        if (!string.IsNullOrWhiteSpace(message))
        {
            var ticketMessage = new TicketMessage
            {
                TicketId = ticketId,
                SenderId = GetCurrentUserId(), // Используем SenderId[cite: 7]
                Text = message,
                SentAt = DateTime.UtcNow // Используем SentAt[cite: 7]
            };

            _context.TicketMessages.Add(ticketMessage);
            await _context.SaveChangesAsync();
        }
        return RedirectToAction(nameof(TicketDetails), new { id = ticketId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CloseTicket(Guid ticketId)
    {
        var ticket = await _context.SupportTickets.FindAsync(ticketId);
        if (ticket != null)
        {
            ticket.IsClosed = true;
            await _context.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Tickets));
    }
}