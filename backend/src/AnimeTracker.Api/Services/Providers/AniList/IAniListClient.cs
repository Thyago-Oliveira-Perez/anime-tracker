using AnimeTracker.Api.Models.AniList;

namespace AnimeTracker.Api.Services.Providers.AniList;

/// <summary>Raw GraphQL client for AniList. Returns AniList-shaped DTOs; see <see cref="AniListProvider"/>
/// for the normalized <c>IAnimeProvider</c> adapter callers should use instead.</summary>
public interface IAniListClient
{
    Task<IReadOnlyList<AniListMediaDto>> SearchAsync(
        string query, int page = 1, int perPage = 20, CancellationToken cancellationToken = default);

    Task<AniListMediaDto?> GetByIdAsync(int aniListId, CancellationToken cancellationToken = default);
}
