using AnimeTracker.Api.Models.AniList;

namespace AnimeTracker.Api.Services.AniList;

public interface IAniListClient
{
    /// <summary>Searches AniList for anime matching the given text query.</summary>
    Task<IReadOnlyList<AniListMediaDto>> SearchAsync(
        string query, int page = 1, int perPage = 20, CancellationToken cancellationToken = default);

    /// <summary>Fetches a single anime by its AniList id, or null if it doesn't exist.</summary>
    Task<AniListMediaDto?> GetByIdAsync(int aniListId, CancellationToken cancellationToken = default);
}
