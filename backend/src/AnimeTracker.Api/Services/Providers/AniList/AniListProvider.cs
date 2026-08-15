using AnimeTracker.Api.Models.Dtos;
using AnimeTracker.Api.Models.Entities;

namespace AnimeTracker.Api.Services.Providers.AniList;

/// <summary>Adapts <see cref="IAniListClient"/> (AniList-shaped) to the normalized <see cref="IAnimeProvider"/> contract.</summary>
public class AniListProvider(IAniListClient client) : IAnimeProvider
{
    public AnimeProvider ProviderType => AnimeProvider.AniList;

    public async Task<IReadOnlyList<AnimeDto>> SearchAsync(
        string query, int page = 1, int perPage = 20, CancellationToken cancellationToken = default)
    {
        var results = await client.SearchAsync(query, page, perPage, cancellationToken);
        return results.Select(r => r.ToAnimeDto()).ToList();
    }

    public async Task<AnimeDto?> GetByExternalIdAsync(string externalId, CancellationToken cancellationToken = default)
    {
        if (!int.TryParse(externalId, out var id))
            return null;

        var media = await client.GetByIdAsync(id, cancellationToken);
        return media?.ToAnimeDto();
    }
}
