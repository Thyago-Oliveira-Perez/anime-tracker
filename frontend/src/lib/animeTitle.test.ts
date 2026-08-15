import { describe, expect, it } from "vitest";
import { localizedTitle } from "./animeTitle";
import type { AnimeDto } from "./api";

function makeAnime(overrides: Partial<AnimeDto> = {}): AnimeDto {
  return {
    provider: "AniList",
    externalId: "154587",
    titleRomaji: "Sousou no Frieren",
    titleEnglish: "Frieren: Beyond Journey's End",
    titleNative: "葬送のフリーレン",
    coverImageUrl: null,
    format: "TV",
    episodesTotal: 28,
    genres: [],
    ...overrides,
  };
}

describe("localizedTitle", () => {
  it("uses the English title for en", () => {
    expect(localizedTitle(makeAnime(), "en")).toBe("Frieren: Beyond Journey's End");
  });

  it("uses the English title for pt-BR too, since there's no Portuguese field", () => {
    expect(localizedTitle(makeAnime(), "pt-BR")).toBe("Frieren: Beyond Journey's End");
  });

  it("uses the native script for ja", () => {
    expect(localizedTitle(makeAnime(), "ja")).toBe("葬送のフリーレン");
  });

  it("falls back to romaji when the English title is missing", () => {
    expect(localizedTitle(makeAnime({ titleEnglish: null }), "en")).toBe("Sousou no Frieren");
  });

  it("falls back to romaji when the native title is missing", () => {
    expect(localizedTitle(makeAnime({ titleNative: null }), "ja")).toBe("Sousou no Frieren");
  });
});
