namespace AnimeTracker.Api.Services.Providers.Jikan;

public class JikanOptions
{
    public const string SectionName = "Jikan";

    public string BaseUrl { get; set; } = "https://api.jikan.moe/v4";
}
