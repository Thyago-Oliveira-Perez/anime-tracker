using System.Text.Json.Serialization;
using AnimeTracker.Api.Data;
using AnimeTracker.Api.Services;
using AnimeTracker.Api.Services.Providers;
using AnimeTracker.Api.Services.Providers.AniList;
using AnimeTracker.Api.Services.Providers.Jikan;
using AnimeTracker.Api.Services.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers()
    // Serialize enums (e.g. WatchStatus, AnimeProvider) as their string names instead of raw
    // ints, so the API stays readable and the frontend doesn't need to hardcode numeric mappings.
    .AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Default")));

builder.Services.AddSingleton(TimeProvider.System);
builder.Services.AddScoped<WatchEntryService>();
builder.Services.AddScoped<ISettingsService, SettingsService>();
builder.Services.AddScoped<IActiveAnimeProviderService, ActiveAnimeProviderService>();
builder.Services.Configure<AnimeProvidersOptions>(builder.Configuration.GetSection(AnimeProvidersOptions.SectionName));

// Anime providers: each source is a raw HTTP client plus an IAnimeProvider adapter over it.
// The registry collects every adapter so callers can resolve one by AnimeProvider at runtime,
// independently of which one the "active provider" feature flag currently points to.
builder.Services.Configure<AniListOptions>(builder.Configuration.GetSection(AniListOptions.SectionName));
builder.Services.AddHttpClient<IAniListClient, AniListClient>((serviceProvider, client) =>
{
    var options = serviceProvider.GetRequiredService<IOptions<AniListOptions>>().Value;
    client.BaseAddress = new Uri(options.BaseUrl);
});
builder.Services.AddScoped<IAnimeProvider, AniListProvider>();

builder.Services.Configure<JikanOptions>(builder.Configuration.GetSection(JikanOptions.SectionName));
builder.Services.AddHttpClient<IJikanClient, JikanClient>((serviceProvider, client) =>
{
    var options = serviceProvider.GetRequiredService<IOptions<JikanOptions>>().Value;
    client.BaseAddress = new Uri(options.BaseUrl);
});
builder.Services.AddScoped<IAnimeProvider, JikanProvider>();

builder.Services.AddScoped<IAnimeProviderRegistry, AnimeProviderRegistry>();

var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? [];
builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
        policy.WithOrigins(allowedOrigins)
            .AllowAnyHeader()
            .AllowAnyMethod());
});

var app = builder.Build();

// Turn provider outages/rate limits into a clean 502 instead of an unhandled 500, since
// these are external dependencies the app has no control over.
app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        var exception = context.Features.Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerFeature>()?.Error;
        if (exception is AnimeProviderException providerException)
        {
            context.Response.StatusCode = StatusCodes.Status502BadGateway;
            await context.Response.WriteAsJsonAsync(new { error = providerException.Message });
            return;
        }

        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        await context.Response.WriteAsJsonAsync(new { error = "An unexpected error occurred." });
    });
});

// Apply pending EF Core migrations on startup. Simple and safe for a single-instance
// personal deployment; revisit if this ever needs to run as multiple replicas.
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseCors("Frontend");

app.UseAuthorization();

app.MapControllers();

app.Run();
