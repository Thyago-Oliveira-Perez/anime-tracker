namespace AnimeTracker.Api.Models.Entities;

/// <summary>
/// A generic persisted key-value setting, the storage backing every feature flag / runtime
/// config the app exposes through an admin endpoint (starting with the active anime provider).
/// </summary>
public class Setting
{
    public string Key { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public DateTimeOffset UpdatedAt { get; set; }
}
