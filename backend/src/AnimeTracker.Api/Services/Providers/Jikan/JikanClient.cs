using System.Net;
using System.Net.Http.Json;
using AnimeTracker.Api.Models.Entities;
using AnimeTracker.Api.Models.Jikan;

namespace AnimeTracker.Api.Services.Providers.Jikan;

/// <summary>
/// Thin wrapper around Jikan (https://docs.api.jikan.moe/), the unofficial MyAnimeList REST API.
/// No API key required, but it enforces a strict rate limit (~3 req/s, 60/min).
/// </summary>
public class JikanClient(HttpClient httpClient, ILogger<JikanClient> logger) : IJikanClient
{
    private const int MaxPerPage = 25; // Jikan's hard limit for the "limit" query param.

    public async Task<IReadOnlyList<JikanAnimeDto>> SearchAsync(
        string query, int page = 1, int perPage = 20, CancellationToken cancellationToken = default)
    {
        var limit = Math.Clamp(perPage, 1, MaxPerPage);
        var url = $"anime?q={Uri.EscapeDataString(query)}&page={page}&limit={limit}";

        var result = await GetAsync<JikanSearchResponse>(url, cancellationToken);
        return result?.Data ?? [];
    }

    public async Task<JikanAnimeDto?> GetByIdAsync(int malId, CancellationToken cancellationToken = default)
    {
        var result = await GetAsync<JikanSingleResponse>($"anime/{malId}", cancellationToken, treat404AsNull: true);
        return result?.Data;
    }

    private async Task<TResponse?> GetAsync<TResponse>(
        string relativeUrl, CancellationToken cancellationToken, bool treat404AsNull = false)
    {
        using var response = await httpClient.GetAsync(relativeUrl, cancellationToken);

        if (treat404AsNull && response.StatusCode == HttpStatusCode.NotFound)
            return default;

        if (response.StatusCode == HttpStatusCode.TooManyRequests)
        {
            logger.LogWarning("Jikan rate limit hit while querying {Url}.", relativeUrl);
            throw new AnimeProviderRateLimitException(AnimeProvider.Jikan);
        }

        if (!response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadAsStringAsync(cancellationToken);
            logger.LogError("Jikan API returned {StatusCode}: {Body}", response.StatusCode, body);
            throw new AnimeProviderUnavailableException(AnimeProvider.Jikan, $"HTTP {(int)response.StatusCode}");
        }

        return await response.Content.ReadFromJsonAsync<TResponse>(cancellationToken);
    }
}
