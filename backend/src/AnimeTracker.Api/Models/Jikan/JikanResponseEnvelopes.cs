using System.Text.Json.Serialization;

namespace AnimeTracker.Api.Models.Jikan;

/// <summary>Envelope for `GET /anime?q=...` (search returns a list under "data").</summary>
public class JikanSearchResponse
{
    [JsonPropertyName("data")]
    public List<JikanAnimeDto> Data { get; set; } = [];
}

/// <summary>Envelope for `GET /anime/{id}` (a single anime under "data").</summary>
public class JikanSingleResponse
{
    [JsonPropertyName("data")]
    public JikanAnimeDto? Data { get; set; }
}
