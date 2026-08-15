using AnimeTracker.Api.Models.Entities;
using AnimeTracker.Api.Services.Providers;
using AnimeTracker.Api.Services.Settings;
using Microsoft.Extensions.Options;
using Xunit;

namespace AnimeTracker.Api.Tests.Settings;

public class ActiveAnimeProviderServiceTests
{
    private class InMemorySettingsService : ISettingsService
    {
        private readonly Dictionary<string, string> _values = [];

        public Task<string?> GetAsync(string key, CancellationToken cancellationToken = default) =>
            Task.FromResult(_values.GetValueOrDefault(key));

        public Task SetAsync(string key, string value, CancellationToken cancellationToken = default)
        {
            _values[key] = value;
            return Task.CompletedTask;
        }
    }

    [Fact]
    public async Task GetActiveAsync_ReturnsConfiguredDefaultWhenNothingIsStoredYet()
    {
        var options = Options.Create(new AnimeProvidersOptions { DefaultProvider = AnimeProvider.Jikan });
        var service = new ActiveAnimeProviderService(new InMemorySettingsService(), options);

        Assert.Equal(AnimeProvider.Jikan, await service.GetActiveAsync());
    }

    [Fact]
    public async Task SetActiveAsync_ThenGetActiveAsync_RoundTripsThePersistedValue()
    {
        var options = Options.Create(new AnimeProvidersOptions { DefaultProvider = AnimeProvider.AniList });
        var service = new ActiveAnimeProviderService(new InMemorySettingsService(), options);

        await service.SetActiveAsync(AnimeProvider.Jikan);

        Assert.Equal(AnimeProvider.Jikan, await service.GetActiveAsync());
    }
}
