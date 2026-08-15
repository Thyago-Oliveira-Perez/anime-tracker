import { NavLink, useParams } from "react-router";
import { useTranslation } from "react-i18next";
import LocaleSwitcher from "./LocaleSwitcher";

export default function Nav() {
  const { t } = useTranslation();
  const { localeSegment } = useParams();

  const linkClass = ({ isActive }: { isActive: boolean }) =>
    `rounded-md px-3 py-1.5 text-sm font-medium transition-colors ${
      isActive
        ? "bg-zinc-900 text-white dark:bg-zinc-100 dark:text-zinc-900"
        : "text-zinc-600 hover:bg-zinc-100 dark:text-zinc-400 dark:hover:bg-zinc-900"
    }`;

  return (
    <nav className="flex items-center justify-between border-b border-zinc-200 bg-white px-4 py-3 dark:border-zinc-800 dark:bg-zinc-950">
      <div className="flex items-center gap-1">
        <NavLink to={`/${localeSegment}/search`} className={linkClass}>
          {t("nav.search")}
        </NavLink>
        <NavLink to={`/${localeSegment}/my-list`} className={linkClass}>
          {t("nav.myList")}
        </NavLink>
        <NavLink to={`/${localeSegment}/settings`} className={linkClass}>
          {t("nav.settings")}
        </NavLink>
      </div>
      <LocaleSwitcher />
    </nav>
  );
}
