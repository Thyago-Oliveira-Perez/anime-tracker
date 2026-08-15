using AnimeTracker.Api.Models.Entities;

namespace AnimeTracker.Api.Services.Providers;

/// <summary>Base type for anything that goes wrong talking to an external anime provider.</summary>
public abstract class AnimeProviderException(AnimeProvider provider, string message) : Exception(message)
{
    public AnimeProvider Provider { get; } = provider;
}

public class AnimeProviderRateLimitException(AnimeProvider provider)
    : AnimeProviderException(provider, $"{provider} rate limit exceeded, try again shortly.");

public class AnimeProviderUnavailableException(AnimeProvider provider, string details)
    : AnimeProviderException(provider, $"{provider} API is unavailable: {details}");
