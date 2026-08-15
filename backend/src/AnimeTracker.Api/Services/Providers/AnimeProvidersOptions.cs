using AnimeTracker.Api.Models.Entities;

namespace AnimeTracker.Api.Services.Providers;

public class AnimeProvidersOptions
{
    public const string SectionName = "AnimeProviders";

    /// <summary>Used the very first time the app runs, before anyone has touched the feature flag.</summary>
    public AnimeProvider DefaultProvider { get; set; } = AnimeProvider.AniList;
}
