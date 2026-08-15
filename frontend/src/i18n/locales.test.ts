import { afterEach, describe, expect, it, vi } from "vitest";
import { detectPreferredLocale, detectPreferredLocaleSegment } from "./locales";

afterEach(() => {
  vi.unstubAllGlobals();
});

function stubBrowserLanguages(...languages: string[]) {
  vi.stubGlobal("navigator", { languages, language: languages[0] });
}

describe("detectPreferredLocale", () => {
  it("returns an exact match when the browser language is one of the supported locales", () => {
    stubBrowserLanguages("pt-BR", "en");
    expect(detectPreferredLocale()).toBe("pt-BR");
  });

  it("matches by language prefix when only the region differs", () => {
    stubBrowserLanguages("pt-PT"); // Portuguese from Portugal, not Brazil — only pt-BR is supported
    expect(detectPreferredLocale()).toBe("pt-BR");
  });

  it("falls through the browser language list until it finds a supported one", () => {
    stubBrowserLanguages("fr-FR", "de-DE", "ja");
    expect(detectPreferredLocale()).toBe("ja");
  });

  it("defaults to English when nothing matches", () => {
    stubBrowserLanguages("fr-FR", "de-DE");
    expect(detectPreferredLocale()).toBe("en");
  });

  it("defaults to English when navigator is unavailable", () => {
    vi.stubGlobal("navigator", undefined);
    expect(detectPreferredLocale()).toBe("en");
  });
});

describe("detectPreferredLocaleSegment", () => {
  it("returns the lowercase-hyphen URL segment, not the raw locale code", () => {
    stubBrowserLanguages("pt-BR");
    expect(detectPreferredLocaleSegment()).toBe("pt-br");
  });
});
