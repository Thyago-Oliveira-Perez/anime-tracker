using AnimeTracker.Api.Models.AniList;
using AnimeTracker.Api.Models.Dtos;
using AnimeTracker.Api.Models.Entities;

namespace AnimeTracker.Api.Services.Providers.AniList;

public static class AniListMapper
{
    /// <summary>Maps a raw AniList media DTO to the provider-agnostic <see cref="AnimeDto"/>.</summary>
    public static AnimeDto ToAnimeDto(this AniListMediaDto dto) => new(
        AnimeProvider.AniList,
        dto.Id.ToString(),
        dto.Title.Romaji ?? dto.Title.English ?? dto.Title.Native ?? "Untitled",
        dto.Title.English,
        dto.Title.Native,
        dto.CoverImage?.Large,
        dto.Format,
        dto.Episodes,
        dto.Genres ?? [],
        dto.Description,
        dto.AverageScore);
}
