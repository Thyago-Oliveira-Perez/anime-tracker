using AnimeTracker.Api.Models.Dtos;
using AnimeTracker.Api.Models.Entities;

namespace AnimeTracker.Api.Services.Providers;

/// <summary>
/// Strategy interface every anime data source (AniList, Jikan, ...) implements. Callers only
/// ever talk to this abstraction — never to a provider's own client — so the active source
/// can be swapped via the provider-settings feature flag without touching calling code.
/// </summary>
public interface IAnimeProvider
{
    AnimeProvider ProviderType { get; }

    Task<IReadOnlyList<AnimeDto>> SearchAsync(
        string query, int page = 1, int perPage = 20, CancellationToken cancellationToken = default);

    /// <summary>Fetches a single anime by the id it has *in this provider*, or null if not found.</summary>
    Task<AnimeDto?> GetByExternalIdAsync(string externalId, CancellationToken cancellationToken = default);
}
