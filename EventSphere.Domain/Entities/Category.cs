// Kategorisering av events, t.ex. Musik, Tech, Konst.
// Varje event tillhör en kategori.
// Tre grundkategorier skapas automatiskt när databasen startas.

using EventSphere.Domain.Common;

namespace EventSphere.Domain.Entities;

public class Category : BaseEntity
{
    // Kategorinamnet måste vara unikt
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    // Emoji eller URL till ikon som visas i frontend, t.ex. "🎵"
    public string IconUrl { get; set; } = string.Empty;

    // En kategori kan ha många events
    public ICollection<Event> Events { get; set; } = new List<Event>();
}