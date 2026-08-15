namespace AnimeTracker.Api.Models.Entities;

/// <summary>
/// A pluggable anime data source. Each value must have a matching <c>IAnimeProvider</c>
/// implementation registered in DI (see <c>AnimeProviderRegistry</c>).
/// </summary>
public enum AnimeProvider
{
    AniList = 0,
    Jikan = 1,
}
