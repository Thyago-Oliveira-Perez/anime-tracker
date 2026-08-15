// Thin fetch wrapper for the AnimeTracker.Api backend.

export type AnimeProvider = "AniList" | "Jikan";

export interface AnimeProviderSetting {
  active: AnimeProvider;
  available: AnimeProvider[];
}

export type WatchStatus = "Planned" | "Watching" | "Completed" | "Dropped" | "OnHold";

export const WATCH_STATUSES: WatchStatus[] = ["Planned", "Watching", "Completed", "Dropped", "OnHold"];

export interface AnimeDto {
  provider: AnimeProvider;
  externalId: string;
  titleRomaji: string;
  titleEnglish: string | null;
  titleNative: string | null;
  coverImageUrl: string | null;
  format: string | null;
  episodesTotal: number | null;
  genres: string[];
  description?: string | null;
  averageScore?: number | null;
}

export interface WatchEntryDto {
  id: number;
  anime: AnimeDto;
  status: WatchStatus;
  rating: number | null;
  review: string | null;
  episodesWatched: number;
  startedAt: string | null; // "yyyy-MM-dd"
  finishedAt: string | null;
  rewatchCount: number;
  favorite: boolean;
  tags: string[];
  createdAt: string;
  updatedAt: string;
}

export interface CreateWatchEntryRequest {
  provider: AnimeProvider;
  externalId: string;
  status: WatchStatus;
  rating: number | null;
  review: string | null;
  episodesWatched: number;
  startedAt: string | null;
  finishedAt: string | null;
  rewatchCount: number;
  favorite: boolean;
  tags: string[] | null;
}

export type UpdateWatchEntryRequest = Omit<CreateWatchEntryRequest, "provider" | "externalId">;

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

export const searchAnime = (query: string, page = 1, perPage = 20) =>
  apiFetch<AnimeDto[]>(
    `/api/anime/search?q=${encodeURIComponent(query)}&page=${page}&perPage=${perPage}`,
  );

export function listWatchEntries(params?: { status?: WatchStatus; favorite?: boolean }) {
  const search = new URLSearchParams();
  if (params?.status) search.set("status", params.status);
  if (params?.favorite !== undefined) search.set("favorite", String(params.favorite));
  const qs = search.toString();
  return apiFetch<WatchEntryDto[]>(`/api/watch-entries${qs ? `?${qs}` : ""}`);
}

export const createWatchEntry = (request: CreateWatchEntryRequest) =>
  apiFetch<WatchEntryDto>("/api/watch-entries", {
    method: "POST",
    body: JSON.stringify(request),
  });

export const updateWatchEntry = (id: number, request: UpdateWatchEntryRequest) =>
  apiFetch<WatchEntryDto>(`/api/watch-entries/${id}`, {
    method: "PUT",
    body: JSON.stringify(request),
  });

export const deleteWatchEntry = (id: number) =>
  apiFetch<void>(`/api/watch-entries/${id}`, { method: "DELETE" });
