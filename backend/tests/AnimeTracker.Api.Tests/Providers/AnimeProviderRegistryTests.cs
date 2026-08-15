using AnimeTracker.Api.Models.Dtos;
using AnimeTracker.Api.Models.Entities;
using AnimeTracker.Api.Services.Providers;
using Xunit;

namespace AnimeTracker.Api.Tests.Providers;

public class AnimeProviderRegistryTests
{
    private class FakeProvider(AnimeProvider providerType) : IAnimeProvider
    {
        public AnimeProvider ProviderType => providerType;

        public Task<IReadOnlyList<AnimeDto>> SearchAsync(
            string query, int page = 1, int perPage = 20, CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyList<AnimeDto>>([]);

        public Task<AnimeDto?> GetByExternalIdAsync(string externalId, CancellationToken cancellationToken = default) =>
            Task.FromResult<AnimeDto?>(null);
    }

    [Fact]
    public void Get_ReturnsTheMatchingRegisteredProvider()
    {
        var aniList = new FakeProvider(AnimeProvider.AniList);
        var jikan = new FakeProvider(AnimeProvider.Jikan);
        var registry = new AnimeProviderRegistry([aniList, jikan]);

        Assert.Same(aniList, registry.Get(AnimeProvider.AniList));
        Assert.Same(jikan, registry.Get(AnimeProvider.Jikan));
    }

    [Fact]
    public void All_ListsEveryRegisteredProviderType()
    {
        var registry = new AnimeProviderRegistry([new FakeProvider(AnimeProvider.AniList), new FakeProvider(AnimeProvider.Jikan)]);

        Assert.Equal([AnimeProvider.AniList, AnimeProvider.Jikan], registry.All.OrderBy(p => p));
    }

    [Fact]
    public void Get_ThrowsWhenNoProviderIsRegisteredForThatType()
    {
        var registry = new AnimeProviderRegistry([new FakeProvider(AnimeProvider.AniList)]);

        Assert.Throws<InvalidOperationException>(() => registry.Get(AnimeProvider.Jikan));
    }
}
