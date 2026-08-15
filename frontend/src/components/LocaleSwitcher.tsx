import type { ChangeEvent } from "react";
import { useLocation, useNavigate, useParams } from "react-router";
import { LOCALE_LABELS, SUPPORTED_LOCALES, localeToUrlSegment, urlSegmentToLocale } from "../i18n/locales";

/** Swaps the locale segment of the current URL, keeping the rest of the path and query intact. */
export default function LocaleSwitcher() {
  const { localeSegment } = useParams();
  const location = useLocation();
  const navigate = useNavigate();
  const currentLocale = localeSegment ? urlSegmentToLocale[localeSegment] : undefined;

  function handleChange(event: ChangeEvent<HTMLSelectElement>) {
    const nextSegment = event.target.value;
    const restOfPath = location.pathname.split("/").slice(2).join("/");
    navigate(`/${nextSegment}${restOfPath ? `/${restOfPath}` : ""}${location.search}`);
  }

  return (
    <select
      value={currentLocale ? localeToUrlSegment[currentLocale] : ""}
      onChange={handleChange}
      aria-label="Language"
      className="rounded-md border border-zinc-300 bg-white px-2 py-1 text-sm text-zinc-900 dark:border-zinc-700 dark:bg-zinc-950 dark:text-zinc-100"
    >
      {SUPPORTED_LOCALES.map((locale) => (
        <option key={locale} value={localeToUrlSegment[locale]}>
          {LOCALE_LABELS[locale]}
        </option>
      ))}
    </select>
  );
}
