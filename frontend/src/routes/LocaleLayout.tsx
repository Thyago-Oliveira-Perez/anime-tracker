import { useEffect } from "react";
import { Navigate, Outlet, useParams } from "react-router";
import { useTranslation } from "react-i18next";
import Nav from "../components/Nav";
import { DEFAULT_LOCALE, localeToUrlSegment, urlSegmentToLocale } from "../i18n/locales";

/** Validates the :localeSegment route param, syncs i18next to it, and renders the shared chrome. */
export default function LocaleLayout() {
  const { localeSegment } = useParams();
  const { i18n } = useTranslation();
  const locale = localeSegment ? urlSegmentToLocale[localeSegment] : undefined;

  useEffect(() => {
    if (locale) void i18n.changeLanguage(locale);
  }, [locale, i18n]);

  if (!locale) {
    return <Navigate to={`/${localeToUrlSegment[DEFAULT_LOCALE]}/search`} replace />;
  }

  return (
    <div className="flex min-h-svh flex-col bg-zinc-50 dark:bg-black">
      <Nav />
      <main className="flex flex-1 flex-col">
        <Outlet />
      </main>
    </div>
  );
}
