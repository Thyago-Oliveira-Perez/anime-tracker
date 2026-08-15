using AnimeTracker.Api.Models.AniList;
using AnimeTracker.Api.Models.Entities;

namespace AnimeTracker.Api.Models.Dtos;

public static class DtoMapper
{
    public static AnimeDto ToDto(this AnimeCache anime) => new(
        anime.AniListId,
        anime.TitleRomaji,
        anime.TitleEnglish,
        anime.TitleNative,
        anime.CoverImageUrl,
        anime.Format,
        anime.EpisodesTotal,
        anime.Genres);

    public static AnimeDto ToDto(this AniListMediaDto media) => new(
        media.Id,
        media.Title.Romaji ?? media.Title.English ?? media.Title.Native ?? "Untitled",
        media.Title.English,
        media.Title.Native,
        media.CoverImage?.Large,
        media.Format,
        media.Episodes,
        media.Genres ?? [],
        media.Description,
        media.AverageScore);

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
