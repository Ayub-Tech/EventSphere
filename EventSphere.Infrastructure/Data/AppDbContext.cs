using EventSphere.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EventSphere.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    // En DbSet per tabell i databasen
    public DbSet<User> Users => Set<User>();
    public DbSet<Event> Events => Set<Event>();
    public DbSet<Venue> Venues => Set<Venue>();
    public DbSet<Registration> Registrations => Set<Registration>();
    public DbSet<Review> Reviews => Set<Review>();
    public DbSet<Category> Categories => Set<Category>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            // Unikt index – ingen kan registrera samma email två gånger
            entity.HasIndex(e => e.Email).IsUnique();
            entity.Property(e => e.Email).HasMaxLength(255).IsRequired();
            entity.Property(e => e.FirstName).HasMaxLength(100).IsRequired();
            entity.Property(e => e.LastName).HasMaxLength(100).IsRequired();
            entity.Property(e => e.Role).HasMaxLength(20).HasDefaultValue("Attendee");
            // Soft-delete filter – borttagna users syns aldrig i queries
            entity.HasQueryFilter(e => !e.IsDeleted);
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Name).IsUnique();
            entity.Property(e => e.Name).HasMaxLength(100).IsRequired();
            entity.HasQueryFilter(e => !e.IsDeleted);
        });

        modelBuilder.Entity<Venue>(entity =>
        {
            entity.HasKey(e => e.Id);
            // Index för snabb platssökning
            entity.HasIndex(e => new { e.City, e.Country });
            entity.Property(e => e.Name).HasMaxLength(200).IsRequired();
            entity.Property(e => e.Address).HasMaxLength(500).IsRequired();
            entity.Property(e => e.City).HasMaxLength(100).IsRequired();
            entity.Property(e => e.Country).HasMaxLength(100).IsRequired();
            entity.HasQueryFilter(e => !e.IsDeleted);
        });

        modelBuilder.Entity<Event>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.StartDate);
            entity.HasIndex(e => e.Status);
            entity.HasIndex(e => new { e.CategoryId, e.Status });
            entity.Property(e => e.Title).HasMaxLength(200).IsRequired();
            // decimal(18,2) = standard för pengar
            entity.Property(e => e.Price).HasColumnType("decimal(18,2)");
            entity.Property(e => e.Status).HasMaxLength(20).HasDefaultValue("Draft");

            // Restrict = kan inte radera organizer/venue/category om de har events
            entity.HasOne(e => e.Organizer)
                .WithMany(u => u.OrganizedEvents)
                .HasForeignKey(e => e.OrganizerId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Venue)
                .WithMany(v => v.Events)
                .HasForeignKey(e => e.VenueId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Category)
                .WithMany(c => c.Events)
                .HasForeignKey(e => e.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasQueryFilter(e => !e.IsDeleted);
        });

        modelBuilder.Entity<Registration>(entity =>
        {
            entity.HasKey(e => e.Id);
            // En användare kan bara ha EN aktiv bokning per event
            entity.HasIndex(e => new { e.UserId, e.EventId }).IsUnique();
            entity.HasIndex(e => e.EventId);
            entity.Property(e => e.Status).HasMaxLength(20).HasDefaultValue("Pending");
            entity.Property(e => e.TicketCode).HasMaxLength(50);

            entity.HasOne(e => e.User)
                .WithMany(u => u.Registrations)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Cascade = om event raderas, raderas även bokningarna
            entity.HasOne(e => e.Event)
                .WithMany(ev => ev.Registrations)
                .HasForeignKey(e => e.EventId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasQueryFilter(e => !e.IsDeleted);
        });

        modelBuilder.Entity<Review>(entity =>
        {
            entity.HasKey(e => e.Id);
            // En användare kan bara recensera ett event en gång
            entity.HasIndex(e => new { e.UserId, e.EventId }).IsUnique();
            entity.HasIndex(e => e.EventId);
            entity.Property(e => e.Comment).HasMaxLength(1000).IsRequired();

            entity.HasOne(e => e.User)
                .WithMany(u => u.Reviews)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Event)
                .WithMany(ev => ev.Reviews)
                .HasForeignKey(e => e.EventId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasQueryFilter(e => !e.IsDeleted);
        });

        SeedData(modelBuilder);
    }

    // Lägg till grundkategorier direkt när databasen skapas
    private static void SeedData(ModelBuilder modelBuilder)
    {
        var categoryId1 = Guid.Parse("11111111-1111-1111-1111-111111111111");
        var categoryId2 = Guid.Parse("22222222-2222-2222-2222-222222222222");
        var categoryId3 = Guid.Parse("33333333-3333-3333-3333-333333333333");

        modelBuilder.Entity<Category>().HasData(
            new Category { Id = categoryId1, Name = "Music", Description = "Concerts, festivals, live music", IconUrl = "🎵" },
            new Category { Id = categoryId2, Name = "Technology", Description = "Tech talks, hackathons, workshops", IconUrl = "💻" },
            new Category { Id = categoryId3, Name = "Art & Culture", Description = "Exhibitions, theater, film", IconUrl = "🎨" }
        );
    }
}