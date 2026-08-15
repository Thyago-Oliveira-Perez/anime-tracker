using AnimeTracker.Api.Models.Dtos;
using AnimeTracker.Api.Services.AniList;
using Microsoft.AspNetCore.Mvc;

namespace AnimeTracker.Api.Controllers;

/// <summary>Read-only proxy over the AniList API used to look anime up before adding it to your list.</summary>
[ApiController]
[Route("api/anime")]
public class AnimeSearchController(IAniListClient aniListClient) : ControllerBase
{
    /// <summary>Searches AniList for anime matching <paramref name="q"/>. Nothing is persisted here.</summary>
    [HttpGet("search")]
    public async Task<ActionResult<IReadOnlyList<AnimeDto>>> Search(
        [FromQuery] string q, [FromQuery] int page = 1, [FromQuery] int perPage = 20, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(q))
            return BadRequest("Query parameter 'q' is required.");

        var results = await aniListClient.SearchAsync(q, page, perPage, cancellationToken);
        return Ok(results.Select(r => r.ToDto()).ToList());
    }

    /// <summary>Fetches full details for a single anime by its AniList id.</summary>
    [HttpGet("{aniListId:int}")]
    public async Task<ActionResult<AnimeDto>> GetById(int aniListId, CancellationToken cancellationToken)
    {
        var media = await aniListClient.GetByIdAsync(aniListId, cancellationToken);
        return media is null ? NotFound() : Ok(media.ToDto());
    }
}
