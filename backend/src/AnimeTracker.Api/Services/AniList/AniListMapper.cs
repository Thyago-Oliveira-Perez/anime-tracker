using AnimeTracker.Api.Models.AniList;
using AnimeTracker.Api.Models.Entities;

namespace AnimeTracker.Api.Services.AniList;

public static class AniListMapper
{
    /// <summary>Maps a raw AniList media DTO to the local cache entity, ready to be upserted.</summary>
    public static AnimeCache ToAnimeCache(this AniListMediaDto dto, TimeProvider timeProvider) => new()
    {
        AniListId = dto.Id,
        TitleRomaji = dto.Title.Romaji ?? dto.Title.English ?? dto.Title.Native ?? "Untitled",
        TitleEnglish = dto.Title.English,
        TitleNative = dto.Title.Native,
        CoverImageUrl = dto.CoverImage?.Large,
        Format = dto.Format,
        EpisodesTotal = dto.Episodes,
        Genres = dto.Genres ?? [],
        SyncedAt = timeProvider.GetUtcNow(),
    };
}
