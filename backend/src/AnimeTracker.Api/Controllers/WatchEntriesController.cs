using AnimeTracker.Api.Models.Dtos;
using AnimeTracker.Api.Models.Entities;
using AnimeTracker.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace AnimeTracker.Api.Controllers;

/// <summary>CRUD over your personal watch log: status, rating, review and dates per anime.</summary>
[ApiController]
[Route("api/watch-entries")]
public class WatchEntriesController(WatchEntryService watchEntryService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<WatchEntryDto>>> List(
        [FromQuery] WatchStatus? status, [FromQuery] bool? favorite, CancellationToken cancellationToken)
    {
        var entries = await watchEntryService.ListAsync(status, favorite, cancellationToken);
        return Ok(entries.Select(e => e.ToDto()).ToList());
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<WatchEntryDto>> GetById(int id, CancellationToken cancellationToken)
    {
        var entry = await watchEntryService.GetByIdAsync(id, cancellationToken);
        return entry is null ? NotFound() : Ok(entry.ToDto());
    }

    [HttpPost]
    public async Task<ActionResult<WatchEntryDto>> Create(
        [FromBody] CreateWatchEntryRequest request, CancellationToken cancellationToken)
    {
        var entry = await watchEntryService.CreateAsync(request, cancellationToken);
        if (entry is null)
            return NotFound($"No anime found on AniList with id {request.AniListId}.");

        return CreatedAtAction(nameof(GetById), new { id = entry.Id }, entry.ToDto());
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<WatchEntryDto>> Update(
        int id, [FromBody] UpdateWatchEntryRequest request, CancellationToken cancellationToken)
    {
        var entry = await watchEntryService.UpdateAsync(id, request, cancellationToken);
        return entry is null ? NotFound() : Ok(entry.ToDto());
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var deleted = await watchEntryService.DeleteAsync(id, cancellationToken);
        return deleted ? NoContent() : NotFound();
    }
}
