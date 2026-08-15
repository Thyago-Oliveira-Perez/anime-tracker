import { useState, type FormEvent } from "react";
import { useTranslation } from "react-i18next";
import { WATCH_STATUSES, type UpdateWatchEntryRequest, type WatchStatus } from "../lib/api";
import { DEFAULT_WATCH_ENTRY_FORM_VALUES, type WatchEntryFormValues } from "../lib/watchEntryForm";

interface WatchEntryFormProps {
  initialValues?: Partial<WatchEntryFormValues>;
  submitLabel: string;
  busy?: boolean;
  onSubmit: (request: UpdateWatchEntryRequest) => void | Promise<void>;
  onCancel: () => void;
}

export default function WatchEntryForm({
  initialValues,
  submitLabel,
  busy,
  onSubmit,
  onCancel,
}: WatchEntryFormProps) {
  const { t } = useTranslation();
  const [values, setValues] = useState<WatchEntryFormValues>({
    ...DEFAULT_WATCH_ENTRY_FORM_VALUES,
    ...initialValues,
  });

  function update<K extends keyof WatchEntryFormValues>(key: K, value: WatchEntryFormValues[K]) {
    setValues((prev) => ({ ...prev, [key]: value }));
  }

  function handleSubmit(event: FormEvent) {
    event.preventDefault();
    void onSubmit({
      status: values.status,
      rating: values.rating,
      review: values.review.trim() === "" ? null : values.review,
      episodesWatched: values.episodesWatched,
      startedAt: values.startedAt === "" ? null : values.startedAt,
      finishedAt: values.finishedAt === "" ? null : values.finishedAt,
      rewatchCount: values.rewatchCount,
      favorite: values.favorite,
      tags: values.tags
        .split(",")
        .map((tag) => tag.trim())
        .filter((tag) => tag.length > 0),
    });
  }

  const inputClass =
    "w-full rounded-md border border-zinc-300 bg-white px-3 py-1.5 text-sm text-zinc-900 dark:border-zinc-700 dark:bg-zinc-900 dark:text-zinc-100";
  const labelClass = "flex flex-col gap-1 text-sm";
  const labelTextClass = "font-medium text-zinc-700 dark:text-zinc-300";

  return (
    <form onSubmit={handleSubmit} className="flex flex-col gap-4">
      <div className="grid grid-cols-2 gap-4">
        <label className={labelClass}>
          <span className={labelTextClass}>{t("watchEntry.fields.status")}</span>
          <select
            className={inputClass}
            value={values.status}
            onChange={(event) => update("status", event.target.value as WatchStatus)}
          >
            {WATCH_STATUSES.map((status) => (
              <option key={status} value={status}>
                {t(`watchEntry.status.${status}`)}
              </option>
            ))}
          </select>
        </label>

        <label className={labelClass}>
          <span className={labelTextClass}>{t("watchEntry.fields.rating")}</span>
          <input
            type="number"
            min={0}
            max={10}
            className={inputClass}
            value={values.rating ?? ""}
            onChange={(event) =>
              update("rating", event.target.value === "" ? null : Number(event.target.value))
            }
          />
        </label>

        <label className={labelClass}>
          <span className={labelTextClass}>{t("watchEntry.fields.episodesWatched")}</span>
          <input
            type="number"
            min={0}
            className={inputClass}
            value={values.episodesWatched}
            onChange={(event) => update("episodesWatched", Number(event.target.value))}
          />
        </label>

        <label className={labelClass}>
          <span className={labelTextClass}>{t("watchEntry.fields.rewatchCount")}</span>
          <input
            type="number"
            min={0}
            className={inputClass}
            value={values.rewatchCount}
            onChange={(event) => update("rewatchCount", Number(event.target.value))}
          />
        </label>

        <label className={labelClass}>
          <span className={labelTextClass}>{t("watchEntry.fields.startedAt")}</span>
          <input
            type="date"
            className={inputClass}
            value={values.startedAt}
            onChange={(event) => update("startedAt", event.target.value)}
          />
        </label>

        <label className={labelClass}>
          <span className={labelTextClass}>{t("watchEntry.fields.finishedAt")}</span>
          <input
            type="date"
            className={inputClass}
            value={values.finishedAt}
            onChange={(event) => update("finishedAt", event.target.value)}
          />
        </label>
      </div>

      <label className={labelClass}>
        <span className={labelTextClass}>{t("watchEntry.fields.review")}</span>
        <textarea
          className={inputClass}
          rows={3}
          value={values.review}
          onChange={(event) => update("review", event.target.value)}
        />
      </label>

      <label className={labelClass}>
        <span className={labelTextClass}>{t("watchEntry.fields.tags")}</span>
        <input
          type="text"
          className={inputClass}
          placeholder={t("watchEntry.form.tagsPlaceholder")}
          value={values.tags}
          onChange={(event) => update("tags", event.target.value)}
        />
      </label>

      <label className="flex items-center gap-2 text-sm">
        <input
          type="checkbox"
          checked={values.favorite}
          onChange={(event) => update("favorite", event.target.checked)}
        />
        <span className={labelTextClass}>{t("watchEntry.fields.favorite")}</span>
      </label>

      <div className="flex justify-end gap-2 pt-2">
        <button
          type="button"
          onClick={onCancel}
          className="rounded-md px-4 py-1.5 text-sm font-medium text-zinc-600 hover:bg-zinc-100 dark:text-zinc-400 dark:hover:bg-zinc-900"
        >
          {t("common.cancel")}
        </button>
        <button
          type="submit"
          disabled={busy}
          className="rounded-md bg-zinc-900 px-4 py-1.5 text-sm font-medium text-white hover:bg-zinc-700 disabled:opacity-50 dark:bg-zinc-100 dark:text-zinc-900 dark:hover:bg-zinc-300"
        >
          {submitLabel}
        </button>
      </div>
    </form>
  );
}
