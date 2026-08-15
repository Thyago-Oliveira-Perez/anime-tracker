using AnimeTracker.Api.Data;
using AnimeTracker.Api.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace AnimeTracker.Api.Services.Settings;

public class SettingsService(AppDbContext db, TimeProvider timeProvider) : ISettingsService
{
    public async Task<string?> GetAsync(string key, CancellationToken cancellationToken = default)
    {
        var setting = await db.Settings.FirstOrDefaultAsync(s => s.Key == key, cancellationToken);
        return setting?.Value;
    }

    public async Task SetAsync(string key, string value, CancellationToken cancellationToken = default)
    {
        var setting = await db.Settings.FirstOrDefaultAsync(s => s.Key == key, cancellationToken);
        if (setting is null)
        {
            setting = new Setting { Key = key };
            db.Settings.Add(setting);
        }

        setting.Value = value;
        setting.UpdatedAt = timeProvider.GetUtcNow();
        await db.SaveChangesAsync(cancellationToken);
    }
}
