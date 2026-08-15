using AnimeTracker.Api.Models.Entities;

namespace AnimeTracker.Api.Services.Providers;

public interface IAnimeProviderRegistry
{
    /// <summary>Resolves the provider instance for a given source, regardless of which one is currently active.</summary>
    IAnimeProvider Get(AnimeProvider provider);

    IReadOnlyList<AnimeProvider> All { get; }
}

/// <summary>Collects every registered <see cref="IAnimeProvider"/> so callers can look one up by type.</summary>
public class AnimeProviderRegistry : IAnimeProviderRegistry
{
    private readonly Dictionary<AnimeProvider, IAnimeProvider> _providers;

    public AnimeProviderRegistry(IEnumerable<IAnimeProvider> providers)
    {
        _providers = providers.ToDictionary(p => p.ProviderType);
    }

    public IReadOnlyList<AnimeProvider> All => _providers.Keys.ToList();

    public IAnimeProvider Get(AnimeProvider provider) =>
        _providers.TryGetValue(provider, out var instance)
            ? instance
            : throw new InvalidOperationException($"No IAnimeProvider registered for '{provider}'.");
}
