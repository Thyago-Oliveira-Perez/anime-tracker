// Thin fetch wrapper for the AnimeTracker.Api backend. Kept deliberately small — this
// frontend only has one feature so far (the anime-provider setting page).

export type AnimeProvider = "AniList" | "Jikan";

export interface AnimeProviderSetting {
  active: AnimeProvider;
  available: AnimeProvider[];
}

const API_BASE_URL = import.meta.env.VITE_API_URL ?? "http://localhost:8080";

async function apiFetch<T>(path: string, init?: RequestInit): Promise<T> {
  const response = await fetch(`${API_BASE_URL}${path}`, {
    ...init,
    headers: { "Content-Type": "application/json", ...init?.headers },
  });

  if (!response.ok) {
    const body = await response.text();
    throw new Error(`API request to ${path} failed with ${response.status}: ${body}`);
  }

  if (response.status === 204) {
    return undefined as T;
  }

  return (await response.json()) as T;
}

export const getAnimeProviderSetting = () =>
  apiFetch<AnimeProviderSetting>("/api/settings/anime-provider");

export const setAnimeProviderSetting = (provider: AnimeProvider) =>
  apiFetch<void>("/api/settings/anime-provider", {
    method: "PUT",
    body: JSON.stringify({ provider }),
  });
