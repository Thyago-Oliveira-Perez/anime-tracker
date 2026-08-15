using AnimeTracker.Api.Models.Entities;

namespace AnimeTracker.Api.Models.Dtos;

/// <summary>
/// Provider-agnostic anime info shown to the client. All three title variants are included
/// so the frontend can pick the right one for the active locale (romaji/english for en &amp;
/// pt-BR, native for ja). <see cref="Provider"/> + <see cref="ExternalId"/> identify the
/// anime in whichever source it came from — the client sends both back when adding it to
/// the watch log.
/// </summary>
public record AnimeDto(
    AnimeProvider Provider,
    string ExternalId,
    string TitleRomaji,
    string? TitleEnglish,
    string? TitleNative,
    string? CoverImageUrl,
    string? Format,
    int? EpisodesTotal,
    string[] Genres,
    string? Description = null,
    int? AverageScore = null);
