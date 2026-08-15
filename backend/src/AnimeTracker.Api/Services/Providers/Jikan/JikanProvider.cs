using AnimeTracker.Api.Models.Dtos;
using AnimeTracker.Api.Models.Entities;

namespace AnimeTracker.Api.Services.Providers.Jikan;

/// <summary>Adapts <see cref="IJikanClient"/> (Jikan-shaped) to the normalized <see cref="IAnimeProvider"/> contract.</summary>
public class JikanProvider(IJikanClient client) : IAnimeProvider
{
    public AnimeProvider ProviderType => AnimeProvider.Jikan;

    public async Task<IReadOnlyList<AnimeDto>> SearchAsync(
        string query, int page = 1, int perPage = 20, CancellationToken cancellationToken = default)
    {
        var results = await client.SearchAsync(query, page, perPage, cancellationToken);
        return results.Select(r => r.ToAnimeDto()).ToList();
    }

    public async Task<AnimeDto?> GetByExternalIdAsync(string externalId, CancellationToken cancellationToken = default)
    {
        if (!int.TryParse(externalId, out var malId))
            return null;

        var media = await client.GetByIdAsync(malId, cancellationToken);
        return media?.ToAnimeDto();
    }
}
