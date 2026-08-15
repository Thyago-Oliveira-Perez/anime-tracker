using AnimeTracker.Api.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace AnimeTracker.Api.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<AnimeCache> AnimeCaches => Set<AnimeCache>();
    public DbSet<WatchEntry> WatchEntries => Set<WatchEntry>();
    public DbSet<Tag> Tags => Set<Tag>();
    public DbSet<Setting> Settings => Set<Setting>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AnimeCache>(entity =>
        {
            entity.HasKey(a => a.Id);
            entity.Property(a => a.TitleRomaji).IsRequired();
            entity.Property(a => a.ExternalId).IsRequired();

            // The natural key: a given provider never reuses an external id, but two
            // providers can (and will) both use "1" for unrelated anime.
            entity.HasIndex(a => new { a.Provider, a.ExternalId }).IsUnique();
        });

        modelBuilder.Entity<WatchEntry>(entity =>
        {
            entity.HasOne(w => w.Anime)
                .WithMany(a => a.WatchEntries)
                .HasForeignKey(w => w.AnimeCacheId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(w => w.Status);
            entity.HasIndex(w => w.AnimeCacheId);

            // Implicit many-to-many; EF Core creates and manages the "WatchEntryTag" join table.
            entity.HasMany(w => w.Tags)
                .WithMany(t => t.WatchEntries)
                .UsingEntity(j => j.ToTable("WatchEntryTag"));
        });

        modelBuilder.Entity<Tag>(entity =>
        {
            entity.HasIndex(t => t.Name).IsUnique();
        });

        modelBuilder.Entity<Setting>(entity =>
        {
            entity.HasKey(s => s.Key);
        });
    }
}
