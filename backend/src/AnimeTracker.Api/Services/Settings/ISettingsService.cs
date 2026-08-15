namespace AnimeTracker.Api.Services.Settings;

/// <summary>
/// Generic persisted key-value store backing every runtime setting / feature flag the app
/// exposes. Higher-level services (like <see cref="IActiveAnimeProviderService"/>) build on
/// top of this instead of touching <c>Setting</c> rows directly.
/// </summary>
public interface ISettingsService
{
    Task<string?> GetAsync(string key, CancellationToken cancellationToken = default);
    Task SetAsync(string key, string value, CancellationToken cancellationToken = default);
}
