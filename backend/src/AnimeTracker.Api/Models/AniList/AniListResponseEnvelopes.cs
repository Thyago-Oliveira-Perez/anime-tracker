using System.Text.Json.Serialization;

namespace AnimeTracker.Api.Models.AniList;

/// <summary>Envelope for a `Page { media { ... } }` GraphQL response (used by search).</summary>
public class AniListSearchResponse
{
    [JsonPropertyName("data")]
    public AniListSearchData? Data { get; set; }
}

public class AniListSearchData
{
    [JsonPropertyName("Page")]
    public AniListPage? Page { get; set; }
}

public class AniListPage
{
    [JsonPropertyName("media")]
    public List<AniListMediaDto> Media { get; set; } = [];
}

/// <summary>Envelope for a single `Media { ... }` GraphQL response (used by anime detail lookup).</summary>
public class AniListMediaResponse
{
    [JsonPropertyName("data")]
    public AniListMediaData? Data { get; set; }
}

public class AniListMediaData
{
    [JsonPropertyName("Media")]
    public AniListMediaDto? Media { get; set; }
}

/// <summary>Request body for AniList's GraphQL endpoint.</summary>
public class AniListGraphQlRequest
{
    [JsonPropertyName("query")]
    public string Query { get; set; } = string.Empty;

    [JsonPropertyName("variables")]
    public Dictionary<string, object?> Variables { get; set; } = [];
}
