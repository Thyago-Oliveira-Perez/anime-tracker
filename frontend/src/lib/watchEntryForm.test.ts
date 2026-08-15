import { describe, expect, it } from "vitest";
import { entryToFormValues } from "./watchEntryForm";
import type { WatchEntryDto } from "./api";

function makeEntry(overrides: Partial<WatchEntryDto> = {}): WatchEntryDto {
  return {
    id: 1,
    anime: {
      provider: "AniList",
      externalId: "154587",
      titleRomaji: "Sousou no Frieren",
      titleEnglish: "Frieren",
      titleNative: "葬送のフリーレン",
      coverImageUrl: null,
      format: "TV",
      episodesTotal: 28,
      genres: [],
    },
    status: "Watching",
    rating: null,
    review: null,
    episodesWatched: 5,
    startedAt: null,
    finishedAt: null,
    rewatchCount: 0,
    favorite: false,
    tags: [],
    createdAt: "2026-01-01T00:00:00Z",
    updatedAt: "2026-01-01T00:00:00Z",
    ...overrides,
  };
}

describe("entryToFormValues", () => {
  it("converts null fields to empty strings for controlled inputs", () => {
    const values = entryToFormValues(makeEntry());

    expect(values.review).toBe("");
    expect(values.startedAt).toBe("");
    expect(values.finishedAt).toBe("");
  });

  it("joins tags into a comma-separated string", () => {
    const values = entryToFormValues(makeEntry({ tags: ["comfort", "favorite"] }));

    expect(values.tags).toBe("comfort, favorite");
  });

  it("passes through non-null scalar fields unchanged", () => {
    const values = entryToFormValues(
      makeEntry({ status: "Completed", rating: 9, review: "Great!", startedAt: "2026-01-01", favorite: true }),
    );

    expect(values.status).toBe("Completed");
    expect(values.rating).toBe(9);
    expect(values.review).toBe("Great!");
    expect(values.startedAt).toBe("2026-01-01");
    expect(values.favorite).toBe(true);
  });
});
