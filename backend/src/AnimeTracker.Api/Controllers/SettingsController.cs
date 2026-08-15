using AnimeTracker.Api.Models.Dtos;
using AnimeTracker.Api.Services.Providers;
using AnimeTracker.Api.Services.Settings;
using Microsoft.AspNetCore.Mvc;

namespace AnimeTracker.Api.Controllers;

/// <summary>Runtime feature flags / config. Starts with just the active anime provider.</summary>
[ApiController]
[Route("api/settings")]
public class SettingsController(IActiveAnimeProviderService activeProvider, IAnimeProviderRegistry providers) : ControllerBase
{
    [HttpGet("anime-provider")]
    public async Task<ActionResult<AnimeProviderSettingDto>> GetAnimeProvider(CancellationToken cancellationToken)
    {
        var active = await activeProvider.GetActiveAsync(cancellationToken);
        return Ok(new AnimeProviderSettingDto(active, providers.All));
    }

    [HttpPut("anime-provider")]
    public async Task<IActionResult> SetAnimeProvider(
        [FromBody] SetAnimeProviderRequest request, CancellationToken cancellationToken)
    {
        await activeProvider.SetActiveAsync(request.Provider, cancellationToken);
        return NoContent();
    }
}
