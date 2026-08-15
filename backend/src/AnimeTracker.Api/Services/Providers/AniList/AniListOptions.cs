namespace AnimeTracker.Api.Services.Providers.AniList;

public class AniListOptions
{
    public const string SectionName = "AniList";

    public string BaseUrl { get; set; } = "https://graphql.anilist.co";
}
