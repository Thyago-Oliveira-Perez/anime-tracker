using AnimeTracker.Api.Models.Entities;

namespace AnimeTracker.Api.Services.Settings;

/// <summary>The "which anime source is active" feature flag: reads/writes it via <see cref="ISettingsService"/>.</summary>
public interface IActiveAnimeProviderService
{
    Task<AnimeProvider> GetActiveAsync(CancellationToken cancellationToken = default);
    Task SetActiveAsync(AnimeProvider provider, CancellationToken cancellationToken = default);
}
