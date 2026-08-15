using AnimeTracker.Api.Models.Entities;
using AnimeTracker.Api.Services.Providers;
using Microsoft.Extensions.Options;

namespace AnimeTracker.Api.Services.Settings;

public class ActiveAnimeProviderService(ISettingsService settings, IOptions<AnimeProvidersOptions> options)
    : IActiveAnimeProviderService
{
    private const string SettingKey = "ActiveAnimeProvider";

    public async Task<AnimeProvider> GetActiveAsync(CancellationToken cancellationToken = default)
    {
        var stored = await settings.GetAsync(SettingKey, cancellationToken);
        if (stored is not null && Enum.TryParse<AnimeProvider>(stored, out var parsed))
            return parsed;

        return options.Value.DefaultProvider;
    }

    public Task SetActiveAsync(AnimeProvider provider, CancellationToken cancellationToken = default) =>
        settings.SetAsync(SettingKey, provider.ToString(), cancellationToken);
}
