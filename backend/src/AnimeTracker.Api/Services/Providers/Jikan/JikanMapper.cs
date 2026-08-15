using AnimeTracker.Api.Models.Dtos;
using AnimeTracker.Api.Models.Entities;
using AnimeTracker.Api.Models.Jikan;

namespace AnimeTracker.Api.Services.Providers.Jikan;

public static class JikanMapper
{
    /// <summary>Maps a raw Jikan anime DTO to the provider-agnostic <see cref="AnimeDto"/>.</summary>
    public static AnimeDto ToAnimeDto(this JikanAnimeDto dto) => new(
        AnimeProvider.Jikan,
        dto.MalId.ToString(),
        dto.Title ?? dto.TitleEnglish ?? dto.TitleJapanese ?? "Untitled",
        dto.TitleEnglish,
        dto.TitleJapanese,
        dto.Images?.Jpg?.LargeImageUrl ?? dto.Images?.Jpg?.ImageUrl,
        dto.Type,
        dto.Episodes,
        dto.Genres?.Select(g => g.Name).ToArray() ?? [],
        dto.Synopsis,
        // Jikan scores are 0-10 with one decimal; AnimeDto.AverageScore follows AniList's
        // 0-100 convention, so scale it up for a consistent meaning across providers.
        dto.Score.HasValue ? (int)Math.Round(dto.Score.Value * 10) : null);
}
