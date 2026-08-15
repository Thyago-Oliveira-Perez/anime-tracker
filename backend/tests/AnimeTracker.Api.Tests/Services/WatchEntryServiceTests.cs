using AnimeTracker.Api.Data;
using AnimeTracker.Api.Models.Dtos;
using AnimeTracker.Api.Models.Entities;
using AnimeTracker.Api.Services;
using AnimeTracker.Api.Services.Providers;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace AnimeTracker.Api.Tests.Services;

public class WatchEntryServiceTests
{
    private static readonly DateTimeOffset FixedNow = new(2026, 1, 1, 12, 0, 0, TimeSpan.Zero);

    private class FakeTimeProvider : TimeProvider
    {
        public override DateTimeOffset GetUtcNow() => FixedNow;
    }

    private class FakeAnimeProvider(AnimeProvider providerType) : IAnimeProvider
    {
        /// <summary>Set to control what <see cref="GetByExternalIdAsync"/> returns; null means "not found".</summary>
        public AnimeDto? Response { get; set; }

        /// <summary>How many times the backing "API" was actually hit — used to assert on cache reuse.</summary>
        public int LookupCount { get; private set; }

        public AnimeProvider ProviderType => providerType;

        public Task<IReadOnlyList<AnimeDto>> SearchAsync(
            string query, int page = 1, int perPage = 20, CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyList<AnimeDto>>([]);

        public Task<AnimeDto?> GetByExternalIdAsync(string externalId, CancellationToken cancellationToken = default)
        {
            LookupCount++;
            return Task.FromResult(Response);
        }
    }

    private static AnimeDto SampleAnimeDto(AnimeProvider provider = AnimeProvider.AniList, string externalId = "154587") =>
        new(provider, externalId, "Sousou no Frieren", "Frieren", "葬送のフリーレン", null, "TV", 28, ["Adventure"]);

    /// <summary>Wires up a service against a fresh in-memory database and a single fake provider.</summary>
    private static (WatchEntryService Service, AppDbContext Db, FakeAnimeProvider Provider) CreateSut(
        AnimeProvider provider = AnimeProvider.AniList)
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        var db = new AppDbContext(options);
        var fakeProvider = new FakeAnimeProvider(provider);
        var registry = new AnimeProviderRegistry([fakeProvider]);
        var service = new WatchEntryService(db, registry, new FakeTimeProvider());
        return (service, db, fakeProvider);
    }

    private static CreateWatchEntryRequest SampleCreateRequest(
        AnimeProvider provider = AnimeProvider.AniList, string externalId = "154587", string[]? tags = null) => new(
        provider, externalId, WatchStatus.Watching, null, null, 5, null, null, 0, false, tags);

    [Fact]
    public async Task CreateAsync_FetchesAndCachesAnimeOnFirstUse()
    {
        var (service, db, provider) = CreateSut();
        provider.Response = SampleAnimeDto();

        var entry = await service.CreateAsync(SampleCreateRequest());

        Assert.NotNull(entry);
        Assert.Equal(1, provider.LookupCount);
        Assert.Single(db.AnimeCaches);
        Assert.Equal("154587", entry.Anime.ExternalId);
        Assert.Equal(FixedNow, entry.CreatedAt);
        Assert.Equal(FixedNow, entry.UpdatedAt);
    }

    [Fact]
    public async Task CreateAsync_ReusesAnExistingCacheEntryInsteadOfCallingTheProviderAgain()
    {
        var (service, _, provider) = CreateSut();
        provider.Response = SampleAnimeDto();

        await service.CreateAsync(SampleCreateRequest());
        await service.CreateAsync(SampleCreateRequest(tags: ["rewatch"]));

        Assert.Equal(1, provider.LookupCount);
    }

    [Fact]
    public async Task CreateAsync_ReturnsNullWhenTheProviderDoesNotKnowTheAnime()
    {
        var (service, db, provider) = CreateSut();
        provider.Response = null;

        var entry = await service.CreateAsync(SampleCreateRequest());

        Assert.Null(entry);
        Assert.Empty(db.WatchEntries);
    }

    [Fact]
    public async Task CreateAsync_AssignsNewAndReusesExistingTagsCaseInsensitively()
    {
        var (service, db, provider) = CreateSut();
        provider.Response = SampleAnimeDto();
        db.Tags.Add(new Tag { Name = "comfort" });
        await db.SaveChangesAsync();

        var entry = await service.CreateAsync(SampleCreateRequest(tags: ["Comfort", "favorite"]));

        Assert.NotNull(entry);
        Assert.Equal(2, entry.Tags.Count);
        // "Comfort" (different casing) must reuse the pre-existing "comfort" tag, not duplicate it.
        Assert.Single(db.Tags, t => t.Name == "comfort");
        Assert.Single(db.Tags, t => t.Name == "favorite");
    }

    [Fact]
    public async Task ListAsync_FiltersByStatusAndFavorite()
    {
        var (service, _, provider) = CreateSut();
        provider.Response = SampleAnimeDto();
        await service.CreateAsync(SampleCreateRequest() with { Status = WatchStatus.Watching, Favorite = true });
        provider.Response = SampleAnimeDto(externalId: "2");
        await service.CreateAsync(SampleCreateRequest(externalId: "2") with { Status = WatchStatus.Completed, Favorite = false });

        var watchingOnly = await service.ListAsync(WatchStatus.Watching, null);
        var favoritesOnly = await service.ListAsync(null, true);
        var all = await service.ListAsync(null, null);

        Assert.Single(watchingOnly);
        Assert.Single(favoritesOnly);
        Assert.Equal(2, all.Count);
    }

    [Fact]
    public async Task UpdateAsync_UpdatesFieldsAndReplacesTags()
    {
        var (service, _, provider) = CreateSut();
        provider.Response = SampleAnimeDto();
        var created = await service.CreateAsync(SampleCreateRequest(tags: ["old-tag"]));

        var updated = await service.UpdateAsync(
            created!.Id,
            new UpdateWatchEntryRequest(WatchStatus.Completed, 9, "Great!", 28, null, null, 1, true, ["new-tag"]));

        Assert.NotNull(updated);
        Assert.Equal(WatchStatus.Completed, updated.Status);
        Assert.Equal(9, updated.Rating);
        Assert.Equal("Great!", updated.Review);
        Assert.True(updated.Favorite);
        Assert.Equal(["new-tag"], updated.Tags.Select(t => t.Name));
    }

    [Fact]
    public async Task UpdateAsync_ReturnsNullForAnUnknownId()
    {
        var (service, _, _) = CreateSut();

        var updated = await service.UpdateAsync(
            999, new UpdateWatchEntryRequest(WatchStatus.Completed, null, null, 0, null, null, 0, false, null));

        Assert.Null(updated);
    }

    [Fact]
    public async Task DeleteAsync_RemovesAnExistingEntryAndReturnsTrue()
    {
        var (service, db, provider) = CreateSut();
        provider.Response = SampleAnimeDto();
        var created = await service.CreateAsync(SampleCreateRequest());

        var deleted = await service.DeleteAsync(created!.Id);

        Assert.True(deleted);
        Assert.Empty(db.WatchEntries);
    }

    [Fact]
    public async Task DeleteAsync_ReturnsFalseForAnUnknownId()
    {
        var (service, _, _) = CreateSut();

        var deleted = await service.DeleteAsync(999);

        Assert.False(deleted);
    }
}
