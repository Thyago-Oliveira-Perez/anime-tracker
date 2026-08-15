namespace AnimeTracker.Api.Models.Entities;

/// <summary>A free-form personal tag (e.g. "comfort rewatch", "cried") you can attach to watch entries.</summary>
public class Tag
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;

    public ICollection<WatchEntry> WatchEntries { get; set; } = new List<WatchEntry>();
}
