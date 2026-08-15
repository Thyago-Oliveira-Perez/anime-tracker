using AnimeTracker.Api.Models.Entities;

namespace AnimeTracker.Api.Models.Dtos;

public static class DtoMapper
{
    public static AnimeDto ToDto(this AnimeCache anime) => new(
        anime.Provider,
        anime.ExternalId,
        anime.TitleRomaji,
        anime.TitleEnglish,
        anime.TitleNative,
        anime.CoverImageUrl,
        anime.Format,
        anime.EpisodesTotal,
        anime.Genres);

    /// <summary>Turns a normalized provider search/lookup result into a cache row ready to persist.</summary>
    public static AnimeCache ToAnimeCache(this AnimeDto dto, TimeProvider timeProvider) => new()
    {
        Provider = dto.Provider,
        ExternalId = dto.ExternalId,
        TitleRomaji = dto.TitleRomaji,
        TitleEnglish = dto.TitleEnglish,
        TitleNative = dto.TitleNative,
        CoverImageUrl = dto.CoverImageUrl,
        Format = dto.Format,
        EpisodesTotal = dto.EpisodesTotal,
        Genres = dto.Genres,
        SyncedAt = timeProvider.GetUtcNow(),
    };

    public static WatchEntryDto ToDto(this WatchEntry entry) => new(
        entry.Id,
        entry.Anime.ToDto(),
        entry.Status,
        entry.Rating,
        entry.Review,
        entry.EpisodesWatched,
        entry.StartedAt,
        entry.FinishedAt,
        entry.RewatchCount,
        entry.Favorite,
        entry.Tags.Select(t => t.Name).OrderBy(name => name).ToArray(),
        entry.CreatedAt,
        entry.UpdatedAt);
}
