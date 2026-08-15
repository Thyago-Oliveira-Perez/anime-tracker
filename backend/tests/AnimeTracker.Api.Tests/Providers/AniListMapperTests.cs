using AnimeTracker.Api.Models.AniList;
using AnimeTracker.Api.Models.Entities;
using AnimeTracker.Api.Services.Providers.AniList;
using Xunit;

namespace AnimeTracker.Api.Tests.Providers;

public class AniListMapperTests
{
    [Fact]
    public void ToAnimeDto_MapsAllFieldsAndTagsAsAniList()
    {
        var dto = new AniListMediaDto
        {
            Id = 154587,
            Title = new AniListTitleDto { Romaji = "Sousou no Frieren", English = "Frieren: Beyond Journey's End", Native = "葬送のフリーレン" },
            CoverImage = new AniListCoverImageDto { Large = "https://example.com/cover.jpg" },
            Format = "TV",
            Episodes = 28,
            Genres = ["Adventure", "Drama"],
            Description = "A story about an elf mage.",
            AverageScore = 92,
        };

        var result = dto.ToAnimeDto();

        Assert.Equal(AnimeProvider.AniList, result.Provider);
        Assert.Equal("154587", result.ExternalId);
        Assert.Equal("Sousou no Frieren", result.TitleRomaji);
        Assert.Equal("Frieren: Beyond Journey's End", result.TitleEnglish);
        Assert.Equal("葬送のフリーレン", result.TitleNative);
        Assert.Equal("https://example.com/cover.jpg", result.CoverImageUrl);
        Assert.Equal("TV", result.Format);
        Assert.Equal(28, result.EpisodesTotal);
        Assert.Equal(["Adventure", "Drama"], result.Genres);
        Assert.Equal(92, result.AverageScore);
    }

    [Fact]
    public void ToAnimeDto_FallsBackToEnglishThenNativeWhenRomajiMissing()
    {
        var dtoWithEnglishOnly = new AniListMediaDto
        {
            Id = 1,
            Title = new AniListTitleDto { English = "Only English" },
        };
        Assert.Equal("Only English", dtoWithEnglishOnly.ToAnimeDto().TitleRomaji);

        var dtoWithNativeOnly = new AniListMediaDto
        {
            Id = 2,
            Title = new AniListTitleDto { Native = "ネイティブ" },
        };
        Assert.Equal("ネイティブ", dtoWithNativeOnly.ToAnimeDto().TitleRomaji);

        var dtoWithNoTitle = new AniListMediaDto { Id = 3, Title = new AniListTitleDto() };
        Assert.Equal("Untitled", dtoWithNoTitle.ToAnimeDto().TitleRomaji);
    }

    [Fact]
    public void ToAnimeDto_DefaultsGenresToEmptyArrayWhenNull()
    {
        var dto = new AniListMediaDto { Id = 1, Title = new AniListTitleDto { Romaji = "X" }, Genres = null };

        Assert.Empty(dto.ToAnimeDto().Genres);
    }
}
