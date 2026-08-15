using AnimeTracker.Api.Models.Entities;

namespace AnimeTracker.Api.Models.Dtos;

public record WatchEntryDto(
    int Id,
    AnimeDto Anime,
    WatchStatus Status,
    int? Rating,
    string? Review,
    int EpisodesWatched,
    DateOnly? StartedAt,
    DateOnly? FinishedAt,
    int RewatchCount,
    bool Favorite,
    string[] Tags,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt);

public record CreateWatchEntryRequest(
    int AniListId,
    WatchStatus Status,
    int? Rating,
    string? Review,
    int EpisodesWatched,
    DateOnly? StartedAt,
    DateOnly? FinishedAt,
    int RewatchCount,
    bool Favorite,
    string[]? Tags);

public record UpdateWatchEntryRequest(
    WatchStatus Status,
    int? Rating,
    string? Review,
    int EpisodesWatched,
    DateOnly? StartedAt,
    DateOnly? FinishedAt,
    int RewatchCount,
    bool Favorite,
    string[]? Tags);
