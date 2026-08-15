using System.Text.Json.Serialization;

namespace AnimeTracker.Api.Models.Jikan;

/// <summary>Raw shape of a Jikan (MyAnimeList) "anime" object, trimmed to the fields this app uses.</summary>
public class JikanAnimeDto
{
    [JsonPropertyName("mal_id")]
    public int MalId { get; set; }

    [JsonPropertyName("title")]
    public string? Title { get; set; }

    [JsonPropertyName("title_english")]
    public string? TitleEnglish { get; set; }

    [JsonPropertyName("title_japanese")]
    public string? TitleJapanese { get; set; }

    [JsonPropertyName("images")]
    public JikanImagesDto? Images { get; set; }

    [JsonPropertyName("type")]
    public string? Type { get; set; }

    [JsonPropertyName("episodes")]
    public int? Episodes { get; set; }

    [JsonPropertyName("genres")]
    public List<JikanGenreDto>? Genres { get; set; }

    [JsonPropertyName("synopsis")]
    public string? Synopsis { get; set; }

    [JsonPropertyName("score")]
    public double? Score { get; set; }
}

public class JikanImagesDto
{
    [JsonPropertyName("jpg")]
    public JikanImageDto? Jpg { get; set; }
}

public class JikanImageDto
{
    [JsonPropertyName("large_image_url")]
    public string? LargeImageUrl { get; set; }

    [JsonPropertyName("image_url")]
    public string? ImageUrl { get; set; }
}

public class JikanGenreDto
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
}
