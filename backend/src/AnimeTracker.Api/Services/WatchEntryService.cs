using AnimeTracker.Api.Data;
using AnimeTracker.Api.Models.Dtos;
using AnimeTracker.Api.Models.Entities;
using AnimeTracker.Api.Services.AniList;
using Microsoft.EntityFrameworkCore;

namespace AnimeTracker.Api.Services;

public class WatchEntryService(AppDbContext db, IAniListClient aniListClient, TimeProvider timeProvider)
{
    public async Task<List<WatchEntry>> ListAsync(
        WatchStatus? status, bool? favorite, CancellationToken cancellationToken = default)
    {
        var query = db.WatchEntries.Include(w => w.Anime).Include(w => w.Tags).AsQueryable();

        if (status is not null)
            query = query.Where(w => w.Status == status);

        if (favorite is not null)
            query = query.Where(w => w.Favorite == favorite);

        return await query.OrderByDescending(w => w.UpdatedAt).ToListAsync(cancellationToken);
    }

    public async Task<WatchEntry?> GetByIdAsync(int id, CancellationToken cancellationToken = default) =>
        await db.WatchEntries.Include(w => w.Anime).Include(w => w.Tags)
            .FirstOrDefaultAsync(w => w.Id == id, cancellationToken);

    /// <summary>
    /// Creates a new watch entry. Also makes sure the referenced anime is cached locally,
    /// fetching it from AniList on the fly if this is the first time it's ever been added.
    /// </summary>
    /// <returns>The created entry, or null if the AniList id doesn't exist.</returns>
    public async Task<WatchEntry?> CreateAsync(CreateWatchEntryRequest request, CancellationToken cancellationToken = default)
    {
        var anime = await EnsureAnimeCachedAsync(request.AniListId, cancellationToken);
        if (anime is null)
            return null;

        var now = timeProvider.GetUtcNow();
        var entry = new WatchEntry
        {
            AniListId = anime.AniListId,
            Status = request.Status,
            Rating = request.Rating,
            Review = request.Review,
            EpisodesWatched = request.EpisodesWatched,
            StartedAt = request.StartedAt,
            FinishedAt = request.FinishedAt,
            RewatchCount = request.RewatchCount,
            Favorite = request.Favorite,
            CreatedAt = now,
            UpdatedAt = now,
        };

        await ApplyTagsAsync(entry, request.Tags, cancellationToken);

        db.WatchEntries.Add(entry);
        await db.SaveChangesAsync(cancellationToken);

        entry.Anime = anime;
        return entry;
    }

    public async Task<WatchEntry?> UpdateAsync(int id, UpdateWatchEntryRequest request, CancellationToken cancellationToken = default)
    {
        var entry = await db.WatchEntries.Include(w => w.Anime).Include(w => w.Tags)
            .FirstOrDefaultAsync(w => w.Id == id, cancellationToken);
        if (entry is null)
            return null;

        entry.Status = request.Status;
        entry.Rating = request.Rating;
        entry.Review = request.Review;
        entry.EpisodesWatched = request.EpisodesWatched;
        entry.StartedAt = request.StartedAt;
        entry.FinishedAt = request.FinishedAt;
        entry.RewatchCount = request.RewatchCount;
        entry.Favorite = request.Favorite;
        entry.UpdatedAt = timeProvider.GetUtcNow();

        await ApplyTagsAsync(entry, request.Tags, cancellationToken);

        await db.SaveChangesAsync(cancellationToken);
        return entry;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var entry = await db.WatchEntries.FindAsync([id], cancellationToken);
        if (entry is null)
            return false;

        db.WatchEntries.Remove(entry);
        await db.SaveChangesAsync(cancellationToken);
        return true;
    }

    private async Task<AnimeCache?> EnsureAnimeCachedAsync(int aniListId, CancellationToken cancellationToken)
    {
        var cached = await db.AnimeCaches.FirstOrDefaultAsync(a => a.AniListId == aniListId, cancellationToken);
        if (cached is not null)
            return cached;

        var media = await aniListClient.GetByIdAsync(aniListId, cancellationToken);
        if (media is null)
            return null;

        var anime = media.ToAnimeCache(timeProvider);
        db.AnimeCaches.Add(anime);
        return anime;
    }

    private async Task ApplyTagsAsync(WatchEntry entry, string[]? tagNames, CancellationToken cancellationToken)
    {
        entry.Tags.Clear();
        if (tagNames is null or [])
            return;

        var distinctNames = tagNames
            .Select(n => n.Trim())
            .Where(n => n.Length > 0)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        var existingTags = await db.Tags
            .Where(t => distinctNames.Contains(t.Name))
            .ToListAsync(cancellationToken);

        foreach (var name in distinctNames)
        {
            var tag = existingTags.FirstOrDefault(t => t.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
            if (tag is null)
            {
                tag = new Tag { Name = name };
                db.Tags.Add(tag);
            }

            entry.Tags.Add(tag);
        }
    }
}
