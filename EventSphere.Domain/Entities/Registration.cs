// Representerar en bokning – en användare bokar ett event.
// 
// Status-flödet:
// Pending → Confirmed → Attended
//               ↓
//           Cancelled
//
// TicketCode är en unik biljettkod, t.ex. "EVT-A3F9B21C"

using EventSphere.Domain.Common;

namespace EventSphere.Domain.Entities;

public class Registration : BaseEntity
{
    // Vem som bokat
    public Guid UserId { get; set; }

    // Vilket event som bokats
    public Guid EventId { get; set; }

    // Bokningsstatus
    public string Status { get; set; } = "Pending";

    // När bokningen gjordes
    public DateTime RegisteredAt { get; set; } = DateTime.UtcNow;

    // Unik biljettkod som genereras automatiskt vid bokning
    public string? TicketCode { get; set; }

    // Sätts till true när deltagaren checkar in på plats
    public bool CheckedIn { get; set; } = false;
    public DateTime? CheckedInAt { get; set; }

    // Navigationsegenskap – laddas med Include() i repository
    public User User { get; set; } = null!;
    public Event Event { get; set; } = null!;
}