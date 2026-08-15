namespace AnimeTracker.Api.Models.Dtos;

/// <summary>
/// Anime info shown to the client. All three title variants are included so the frontend
/// can pick the right one for the active locale (romaji/english for en & pt-BR, native for ja).
/// </summary>
public record AnimeDto(
    int AniListId,
    string TitleRomaji,
    string? TitleEnglish,
    string? TitleNative,
    string? CoverImageUrl,
    string? Format,
    int? EpisodesTotal,
    string[] Genres,
    string? Description = null,
    int? AverageScore = null);
