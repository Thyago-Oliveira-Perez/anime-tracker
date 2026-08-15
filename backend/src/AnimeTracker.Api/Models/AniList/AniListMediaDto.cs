using System.Text.Json.Serialization;

namespace AnimeTracker.Api.Models.AniList;

/// <summary>Raw shape of an AniList "Media" object, trimmed to the fields this app uses.</summary>
public class AniListMediaDto
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("title")]
    public AniListTitleDto Title { get; set; } = new();

    [JsonPropertyName("coverImage")]
    public AniListCoverImageDto? CoverImage { get; set; }

    [JsonPropertyName("format")]
    public string? Format { get; set; }

    [JsonPropertyName("episodes")]
    public int? Episodes { get; set; }

    [JsonPropertyName("genres")]
    public string[]? Genres { get; set; }

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("averageScore")]
    public int? AverageScore { get; set; }
}

public class AniListTitleDto
{
    [JsonPropertyName("romaji")]
    public string? Romaji { get; set; }

    [JsonPropertyName("english")]
    public string? English { get; set; }

    [JsonPropertyName("native")]
    public string? Native { get; set; }
}

public class AniListCoverImageDto
{
    [JsonPropertyName("large")]
    public string? Large { get; set; }
}
