using AnimeTracker.Api.Models.Jikan;

namespace AnimeTracker.Api.Services.Providers.Jikan;

/// <summary>Raw REST client for Jikan (unofficial MyAnimeList API). Returns Jikan-shaped DTOs;
/// see <see cref="JikanProvider"/> for the normalized <c>IAnimeProvider</c> adapter callers should use instead.</summary>
public interface IJikanClient
{
    Task<IReadOnlyList<JikanAnimeDto>> SearchAsync(
        string query, int page = 1, int perPage = 20, CancellationToken cancellationToken = default);

    Task<JikanAnimeDto?> GetByIdAsync(int malId, CancellationToken cancellationToken = default);
}
