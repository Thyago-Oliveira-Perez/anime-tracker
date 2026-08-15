using AnimeTracker.Api.Models.Dtos;
using AnimeTracker.Api.Models.Entities;
using AnimeTracker.Api.Services.Providers;
using AnimeTracker.Api.Services.Settings;
using Microsoft.AspNetCore.Mvc;

namespace AnimeTracker.Api.Controllers;

/// <summary>Read-only proxy over the currently active anime provider, used to look anime up before adding it to your list.</summary>
[ApiController]
[Route("api/anime")]
public class AnimeSearchController(
    IAnimeProviderRegistry providers, IActiveAnimeProviderService activeProvider) : ControllerBase
{
    /// <summary>Searches the active provider for anime matching <paramref name="q"/>. Nothing is persisted here.</summary>
    [HttpGet("search")]
    public async Task<ActionResult<IReadOnlyList<AnimeDto>>> Search(
        [FromQuery] string q, [FromQuery] int page = 1, [FromQuery] int perPage = 20, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(q))
            return BadRequest("Query parameter 'q' is required.");

        var provider = providers.Get(await activeProvider.GetActiveAsync(cancellationToken));
        var results = await provider.SearchAsync(q, page, perPage, cancellationToken);
        return Ok(results);
    }

    /// <summary>Fetches full details for a single anime from a specific provider by its id there.</summary>
    [HttpGet("{provider}/{externalId}")]
    public async Task<ActionResult<AnimeDto>> GetById(
        AnimeProvider provider, string externalId, CancellationToken cancellationToken)
    {
        var result = await providers.Get(provider).GetByExternalIdAsync(externalId, cancellationToken);
        return result is null ? NotFound() : Ok(result);
    }
}
