// Central place for everything about which locales exist and how they map to URLs.
// URL segments are lowercase-hyphen (/pt-br/...) while i18next/BCP-47 locale codes keep the
// standard casing (pt-BR) — these maps translate between the two.

export const SUPPORTED_LOCALES = ["en", "pt-BR", "ja"] as const;
export type Locale = (typeof SUPPORTED_LOCALES)[number];
export const DEFAULT_LOCALE: Locale = "en";

export const LOCALE_LABELS: Record<Locale, string> = {
  en: "English",
  "pt-BR": "Português",
  ja: "日本語",
};

export const localeToUrlSegment: Record<Locale, string> = {
  en: "en",
  "pt-BR": "pt-br",
  ja: "ja",
};

export const urlSegmentToLocale: Record<string, Locale> = Object.fromEntries(
  SUPPORTED_LOCALES.map((locale) => [localeToUrlSegment[locale], locale]),
);

/** Picks the best supported locale for the browser's language list, defaulting to English. */
export function detectPreferredLocale(): Locale {
  const browserLanguages = typeof navigator !== "undefined" ? navigator.languages ?? [navigator.language] : [];

  for (const browserLanguage of browserLanguages) {
    const exactMatch = SUPPORTED_LOCALES.find(
      (locale) => locale.toLowerCase() === browserLanguage.toLowerCase(),
    );
    if (exactMatch) return exactMatch;

    const languagePrefix = browserLanguage.split("-")[0].toLowerCase();
    const prefixMatch = SUPPORTED_LOCALES.find(
      (locale) => locale.split("-")[0].toLowerCase() === languagePrefix,
    );
    if (prefixMatch) return prefixMatch;
  }

  return DEFAULT_LOCALE;
}

export function detectPreferredLocaleSegment(): string {
  return localeToUrlSegment[detectPreferredLocale()];
}
