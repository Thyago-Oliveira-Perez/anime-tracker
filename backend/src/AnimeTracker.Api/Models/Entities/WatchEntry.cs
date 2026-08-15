namespace AnimeTracker.Api.Models.Entities;

/// <summary>
/// A personal record for an anime: your watch status, rating, review and dates.
/// This is the entity the whole app revolves around.
/// </summary>
public class WatchEntry
{
    public int Id { get; set; }

    public int AnimeCacheId { get; set; }
    public AnimeCache Anime { get; set; } = null!;

    public WatchStatus Status { get; set; } = WatchStatus.Planned;

    /// <summary>Personal rating from 0 to 10. Null until you've formed an opinion.</summary>
    public int? Rating { get; set; }

    /// <summary>Free-form personal review text, written in whatever language you want.</summary>
    public string? Review { get; set; }

    public int EpisodesWatched { get; set; }

    public DateOnly? StartedAt { get; set; }
    public DateOnly? FinishedAt { get; set; }

    public int RewatchCount { get; set; }
    public bool Favorite { get; set; }

    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }

    public ICollection<Tag> Tags { get; set; } = new List<Tag>();
}
