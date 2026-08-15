import { useEffect, useState } from "react";
import { useTranslation } from "react-i18next";
import {
  type AnimeProvider,
  type AnimeProviderSetting,
  getAnimeProviderSetting,
  setAnimeProviderSetting,
} from "../lib/api";

type Status = { kind: "idle" } | { kind: "error"; message: string } | { kind: "saving" };

export default function AnimeProviderSettings() {
  const { t } = useTranslation();
  const [setting, setSetting] = useState<AnimeProviderSetting | null>(null);
  const [status, setStatus] = useState<Status>({ kind: "idle" });

  useEffect(() => {
    getAnimeProviderSetting()
      .then(setSetting)
      .catch((error: unknown) => setStatus({ kind: "error", message: describeError(error) }));
  }, []);

  async function handleSelect(provider: AnimeProvider) {
    if (!setting || provider === setting.active) return;

    const previous = setting;
    setSetting({ ...setting, active: provider }); // optimistic
    setStatus({ kind: "saving" });

    try {
      await setAnimeProviderSetting(provider);
      setStatus({ kind: "idle" });
    } catch (error) {
      setSetting(previous);
      setStatus({ kind: "error", message: describeError(error) });
    }
  }

  if (status.kind === "error" && !setting) {
    return (
      <p className="rounded-lg border border-red-200 bg-red-50 px-4 py-3 text-sm text-red-700 dark:border-red-900 dark:bg-red-950 dark:text-red-300">
        {t("settings.animeProvider.loadError")} {status.message}
      </p>
    );
  }

  if (!setting) {
    return <p className="text-sm text-zinc-500 dark:text-zinc-400">{t("common.loading")}</p>;
  }

  return (
    <div className="flex flex-col gap-4">
      <fieldset className="flex flex-col gap-3" disabled={status.kind === "saving"}>
        {setting.available.map((provider) => {
          const isActive = provider === setting.active;
          return (
            <label
              key={provider}
              className={`flex cursor-pointer items-start gap-3 rounded-lg border px-4 py-3 transition-colors ${
                isActive
                  ? "border-zinc-900 bg-zinc-50 dark:border-zinc-100 dark:bg-zinc-900"
                  : "border-zinc-200 hover:bg-zinc-50 dark:border-zinc-800 dark:hover:bg-zinc-900/50"
              }`}
            >
              <input
                type="radio"
                name="anime-provider"
                className="mt-1"
                checked={isActive}
                onChange={() => handleSelect(provider)}
              />
              <span className="flex flex-col">
                <span className="font-medium text-zinc-900 dark:text-zinc-50">
                  {t(`settings.animeProvider.providers.${provider}.label`)}
                </span>
                <span className="text-sm text-zinc-500 dark:text-zinc-400">
                  {t(`settings.animeProvider.providers.${provider}.description`)}
                </span>
              </span>
            </label>
          );
        })}
      </fieldset>

      <div className="min-h-5 text-sm">
        {status.kind === "saving" && (
          <span className="text-zinc-500 dark:text-zinc-400">{t("settings.animeProvider.saving")}</span>
        )}
        {status.kind === "error" && <span className="text-red-600 dark:text-red-400">{status.message}</span>}
      </div>
    </div>
  );
}

function describeError(error: unknown): string {
  return error instanceof Error ? error.message : "Unknown error";
}
