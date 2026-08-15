using AnimeTracker.Api.Models.Entities;

namespace AnimeTracker.Api.Models.Dtos;

public record AnimeProviderSettingDto(AnimeProvider Active, IReadOnlyList<AnimeProvider> Available);

public record SetAnimeProviderRequest(AnimeProvider Provider);
