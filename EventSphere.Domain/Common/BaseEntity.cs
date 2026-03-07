// Alla entiteter ärver från denna klass.
// Det betyder att varje tabell i databasen automatiskt får:
// - Ett unikt ID
// - Datum för när den skapades och uppdaterades
// - En soft-delete flagga (IsDeleted) så inget raderas fysiskt

namespace EventSphere.Domain.Common;

public abstract class BaseEntity
{
    // Unikt ID – genereras automatiskt
    public Guid Id { get; set; } = Guid.NewGuid();

    // Sätts automatiskt när objektet skapas
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Uppdateras när något ändras
    public DateTime? UpdatedAt { get; set; }

    // Om true = "borttagen" men finns kvar i databasen
    public bool IsDeleted { get; set; } = false;
}
