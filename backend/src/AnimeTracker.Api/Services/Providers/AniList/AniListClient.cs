using System.Net;
using System.Net.Http.Json;
using AnimeTracker.Api.Models.AniList;
using AnimeTracker.Api.Models.Entities;

namespace AnimeTracker.Api.Services.Providers.AniList;

/// <summary>
/// Thin wrapper around AniList's public GraphQL API (https://anilist.gitbook.io/anilist-apiv2-docs/).
/// No API key is required for read-only queries like search and media lookup.
/// </summary>
public class AniListClient(HttpClient httpClient, ILogger<AniListClient> logger) : IAniListClient
{
    private const string SearchQuery = """
        query ($search: String, $page: Int, $perPage: Int) {
          Page(page: $page, perPage: $perPage) {
            media(search: $search, type: ANIME, sort: SEARCH_MATCH) {
              id
              title { romaji english native }
              coverImage { large }
              format
              episodes
              genres
              description
              averageScore
            }
          }
        }
        """;

    private const string MediaByIdQuery = """
        query ($id: Int) {
          Media(id: $id, type: ANIME) {
            id
            title { romaji english native }
            coverImage { large }
            format
            episodes
            genres
            description
            averageScore
          }
        }
        """;

    public async Task<IReadOnlyList<AniListMediaDto>> SearchAsync(
        string query, int page = 1, int perPage = 20, CancellationToken cancellationToken = default)
    {
        var request = new AniListGraphQlRequest
        {
            Query = SearchQuery,
            Variables = new Dictionary<string, object?>
            {
                ["search"] = query,
                ["page"] = page,
                ["perPage"] = perPage,
            },
        };

        var result = await PostAsync<AniListSearchResponse>(request, cancellationToken);
        return result?.Data?.Page?.Media ?? [];
    }

    public async Task<AniListMediaDto?> GetByIdAsync(int aniListId, CancellationToken cancellationToken = default)
    {
        var request = new AniListGraphQlRequest
        {
            Query = MediaByIdQuery,
            Variables = new Dictionary<string, object?> { ["id"] = aniListId },
        };

        var result = await PostAsync<AniListMediaResponse>(request, cancellationToken);
        return result?.Data?.Media;
    }

    private async Task<TResponse?> PostAsync<TResponse>(
        AniListGraphQlRequest request, CancellationToken cancellationToken)
    {
        using var response = await httpClient.PostAsJsonAsync(string.Empty, request, cancellationToken);

        if (response.StatusCode == HttpStatusCode.TooManyRequests)
        {
            logger.LogWarning("AniList rate limit hit while querying the GraphQL API.");
            throw new AnimeProviderRateLimitException(AnimeProvider.AniList);
        }

        if (!response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadAsStringAsync(cancellationToken);
            logger.LogError("AniList API returned {StatusCode}: {Body}", response.StatusCode, body);
            throw new AnimeProviderUnavailableException(AnimeProvider.AniList, $"HTTP {(int)response.StatusCode}");
        }

        return await response.Content.ReadFromJsonAsync<TResponse>(cancellationToken);
    }
}
