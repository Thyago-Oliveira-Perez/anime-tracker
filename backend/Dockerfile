FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY src/AnimeTracker.Api/AnimeTracker.Api.csproj src/AnimeTracker.Api/
RUN dotnet restore src/AnimeTracker.Api/AnimeTracker.Api.csproj

COPY src/AnimeTracker.Api/ src/AnimeTracker.Api/
RUN dotnet publish src/AnimeTracker.Api/AnimeTracker.Api.csproj -c Release -o /app --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

# Npgsql probes for GSSAPI/Kerberos support at startup; without this lib it's just a harmless
# warning, but installing it keeps the logs clean.
RUN apt-get update \
    && apt-get install -y --no-install-recommends libgssapi-krb5-2 \
    && rm -rf /var/lib/apt/lists/*

COPY --from=build /app .

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "AnimeTracker.Api.dll"]
