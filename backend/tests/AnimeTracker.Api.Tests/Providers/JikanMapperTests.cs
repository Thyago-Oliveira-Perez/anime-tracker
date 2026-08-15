using AnimeTracker.Api.Models.Entities;
using AnimeTracker.Api.Models.Jikan;
using AnimeTracker.Api.Services.Providers.Jikan;
using Xunit;

namespace AnimeTracker.Api.Tests.Providers;

public class JikanMapperTests
{
    [Fact]
    public void ToAnimeDto_MapsAllFieldsAndScalesScoreToAniListRange()
    {
        var dto = new JikanAnimeDto
        {
            MalId = 52991,
            Title = "Sousou no Frieren",
            TitleEnglish = "Frieren: Beyond Journey's End",
            TitleJapanese = "葬送のフリーレン",
            Images = new JikanImagesDto { Jpg = new JikanImageDto { LargeImageUrl = "https://example.com/large.jpg" } },
            Type = "TV",
            Episodes = 28,
            Genres = [new JikanGenreDto { Name = "Adventure" }, new JikanGenreDto { Name = "Drama" }],
            Synopsis = "A story about an elf mage.",
            Score = 8.99,
        };

        var result = dto.ToAnimeDto();

        Assert.Equal(AnimeProvider.Jikan, result.Provider);
        Assert.Equal("52991", result.ExternalId);
        Assert.Equal("Sousou no Frieren", result.TitleRomaji);
        Assert.Equal("Frieren: Beyond Journey's End", result.TitleEnglish);
        Assert.Equal("葬送のフリーレン", result.TitleNative);
        Assert.Equal("https://example.com/large.jpg", result.CoverImageUrl);
        Assert.Equal("TV", result.Format);
        Assert.Equal(28, result.EpisodesTotal);
        Assert.Equal(["Adventure", "Drama"], result.Genres);
        // 8.99 * 10 rounded, to match AniList's 0-100 convention.
        Assert.Equal(90, result.AverageScore);
    }

    [Fact]
    public void ToAnimeDto_FallsBackFromImageUrlWhenLargeIsMissing()
    {
        var dto = new JikanAnimeDto
        {
            MalId = 1,
            Title = "X",
            Images = new JikanImagesDto { Jpg = new JikanImageDto { ImageUrl = "https://example.com/small.jpg" } },
        };

        Assert.Equal("https://example.com/small.jpg", dto.ToAnimeDto().CoverImageUrl);
    }

    [Fact]
    public void ToAnimeDto_LeavesAverageScoreNullWhenJikanScoreIsNull()
    {
        var dto = new JikanAnimeDto { MalId = 1, Title = "X", Score = null };

        Assert.Null(dto.ToAnimeDto().AverageScore);
    }

    [Fact]
    public void ToAnimeDto_DefaultsGenresToEmptyArrayWhenNull()
    {
        var dto = new JikanAnimeDto { MalId = 1, Title = "X", Genres = null };

        Assert.Empty(dto.ToAnimeDto().Genres);
    }
}
