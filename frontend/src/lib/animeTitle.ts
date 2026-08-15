import type { AnimeDto } from "./api";
import type { Locale } from "../i18n/locales";

/**
 * Picks the title variant matching the active locale: English (falling back to romaji) for
 * en/pt-BR — AniList/Jikan have no Portuguese field — and native script for ja.
 */
export function localizedTitle(anime: AnimeDto, locale: Locale): string {
  if (locale === "ja") {
    return anime.titleNative ?? anime.titleRomaji;
  }
  return anime.titleEnglish ?? anime.titleRomaji;
}
