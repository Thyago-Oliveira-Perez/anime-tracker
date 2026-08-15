namespace AnimeTracker.Api.Models.Entities;

/// <summary>
/// Lightweight local mirror of an anime as reported by one of the pluggable providers
/// (AniList, Jikan, ...). Only holds the fields the app needs to render search results and
/// list items without re-querying the provider every time.
/// </summary>
/// <remarks>
/// The same real-world anime found through two different providers becomes two separate
/// rows here (there's no cross-provider identity matching) — an accepted trade-off for a
/// personal project. <see cref="Provider"/> + <see cref="ExternalId"/> is what's unique,
/// not <see cref="Id"/> alone.
/// </remarks>
public class AnimeCache
{
    /// <summary>Internal surrogate key. Never exposed to a provider, only used for local FKs.</summary>
    public int Id { get; set; }

    public AnimeProvider Provider { get; set; }

    /// <summary>The id this anime has in its source provider (e.g. AniList's numeric id, MAL's mal_id).</summary>
    public string ExternalId { get; set; } = string.Empty;

    public string TitleRomaji { get; set; } = string.Empty;
    public string? TitleEnglish { get; set; }
    public string? TitleNative { get; set; }

    public string? CoverImageUrl { get; set; }
    public string? Format { get; set; }
    public int? EpisodesTotal { get; set; }
    public string[] Genres { get; set; } = [];

    /// <summary>When this cache row was last refreshed from its provider.</summary>
    public DateTimeOffset SyncedAt { get; set; }

    public ICollection<WatchEntry> WatchEntries { get; set; } = new List<WatchEntry>();
}
