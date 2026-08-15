namespace AnimeTracker.Api.Models.Entities;

/// <summary>
/// Lightweight local mirror of an AniList anime entry. Only holds the fields the app
/// needs to render search results and list items without re-querying AniList every time.
/// </summary>
public class AnimeCache
{
    /// <summary>The AniList media id. Used as the primary key so lookups need no mapping table.</summary>
    public int AniListId { get; set; }

    public string TitleRomaji { get; set; } = string.Empty;
    public string? TitleEnglish { get; set; }
    public string? TitleNative { get; set; }

    public string? CoverImageUrl { get; set; }
    public string? Format { get; set; }
    public int? EpisodesTotal { get; set; }
    public string[] Genres { get; set; } = [];

    /// <summary>When this cache row was last refreshed from AniList.</summary>
    public DateTimeOffset SyncedAt { get; set; }

    public ICollection<WatchEntry> WatchEntries { get; set; } = new List<WatchEntry>();
}
