import type { WatchEntryDto, WatchStatus } from "./api";

export interface WatchEntryFormValues {
  status: WatchStatus;
  rating: number | null;
  review: string;
  episodesWatched: number;
  startedAt: string;
  finishedAt: string;
  rewatchCount: number;
  favorite: boolean;
  tags: string;
}

export const DEFAULT_WATCH_ENTRY_FORM_VALUES: WatchEntryFormValues = {
  status: "Planned",
  rating: null,
  review: "",
  episodesWatched: 0,
  startedAt: "",
  finishedAt: "",
  rewatchCount: 0,
  favorite: false,
  tags: "",
};

/** Converts a persisted entry back into editable form state (nulls become empty strings). */
export function entryToFormValues(entry: WatchEntryDto): WatchEntryFormValues {
  return {
    status: entry.status,
    rating: entry.rating,
    review: entry.review ?? "",
    episodesWatched: entry.episodesWatched,
    startedAt: entry.startedAt ?? "",
    finishedAt: entry.finishedAt ?? "",
    rewatchCount: entry.rewatchCount,
    favorite: entry.favorite,
    tags: entry.tags.join(", "),
  };
}
