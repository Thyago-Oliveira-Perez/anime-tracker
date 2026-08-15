import { useTranslation } from "react-i18next";
import AnimeProviderSettings from "../components/AnimeProviderSettings";

export default function SettingsPage() {
  const { t } = useTranslation();

  return (
    <div className="flex flex-1 items-start justify-center px-4 py-12">
      <section className="w-full max-w-md rounded-2xl border border-zinc-200 bg-white p-8 shadow-sm dark:border-zinc-800 dark:bg-zinc-950">
        <h1 className="text-xl font-semibold text-zinc-900 dark:text-zinc-50">
          {t("settings.animeProvider.title")}
        </h1>
        <p className="mt-1 text-sm text-zinc-500 dark:text-zinc-400">
          {t("settings.animeProvider.description")}
        </p>

        <div className="mt-6">
          <AnimeProviderSettings />
        </div>
      </section>
    </div>
  );
}
