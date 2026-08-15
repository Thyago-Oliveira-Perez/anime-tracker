import { useCallback, useEffect, useState } from "react";
import { useTranslation } from "react-i18next";
import { useParams } from "react-router";
import {
  WATCH_STATUSES,
  deleteWatchEntry,
  listWatchEntries,
  updateWatchEntry,
  type UpdateWatchEntryRequest,
  type WatchEntryDto,
  type WatchStatus,
} from "../lib/api";
import { urlSegmentToLocale, type Locale } from "../i18n/locales";
import { localizedTitle } from "../lib/animeTitle";
import Modal from "../components/Modal";
import WatchEntryForm from "../components/WatchEntryForm";
import { entryToFormValues } from "../lib/watchEntryForm";

type ListState =
  | { kind: "loading" }
  | { kind: "error"; message: string }
  | { kind: "loaded"; items: WatchEntryDto[] };

type EditState = { kind: "idle" } | { kind: "saving" } | { kind: "error"; message: string };

export default function MyListPage() {
  const { t } = useTranslation();
  const { localeSegment } = useParams();
  const locale: Locale = (localeSegment && urlSegmentToLocale[localeSegment]) || "en";

  const [statusFilter, setStatusFilter] = useState<WatchStatus | "">("");
  const [favoritesOnly, setFavoritesOnly] = useState(false);
  const [state, setState] = useState<ListState>({ kind: "loading" });
  const [editing, setEditing] = useState<WatchEntryDto | null>(null);
  const [editState, setEditState] = useState<EditState>({ kind: "idle" });

  const refresh = useCallback(async () => {
    setState({ kind: "loading" });
    try {
      const items = await listWatchEntries({
        status: statusFilter === "" ? undefined : statusFilter,
        favorite: favoritesOnly ? true : undefined,
      });
      setState({ kind: "loaded", items });
    } catch (error) {
      setState({ kind: "error", message: describeError(error) });
    }
  }, [statusFilter, favoritesOnly]);

  useEffect(() => {
    void refresh();
  }, [refresh]);

  async function handleEditSubmit(request: UpdateWatchEntryRequest) {
    if (!editing) return;

    setEditState({ kind: "saving" });
    try {
      await updateWatchEntry(editing.id, request);
      setEditing(null);
      setEditState({ kind: "idle" });
      await refresh();
    } catch (error) {
      setEditState({ kind: "error", message: describeError(error) });
    }
  }

  async function handleDelete(entry: WatchEntryDto) {
    await deleteWatchEntry(entry.id);
    await refresh();
  }

  return (
    <div className="mx-auto w-full max-w-4xl flex-1 px-4 py-8">
      <h1 className="text-xl font-semibold text-zinc-900 dark:text-zinc-50">{t("myList.title")}</h1>

      <div className="mt-4 flex flex-wrap items-center gap-3">
        <select
          value={statusFilter}
          onChange={(event) => setStatusFilter(event.target.value as WatchStatus | "")}
          className="rounded-md border border-zinc-300 bg-white px-3 py-1.5 text-sm text-zinc-900 dark:border-zinc-700 dark:bg-zinc-900 dark:text-zinc-100"
        >
          <option value="">{t("myList.filters.allStatuses")}</option>
          {WATCH_STATUSES.map((status) => (
            <option key={status} value={status}>
              {t(`watchEntry.status.${status}`)}
            </option>
          ))}
        </select>

        <label className="flex items-center gap-2 text-sm text-zinc-700 dark:text-zinc-300">
          <input
            type="checkbox"
            checked={favoritesOnly}
            onChange={(event) => setFavoritesOnly(event.target.checked)}
          />
          {t("myList.filters.favoritesOnly")}
        </label>
      </div>

      <div className="mt-6">
        {state.kind === "loading" && <p className="text-sm text-zinc-500 dark:text-zinc-400">{t("common.loading")}</p>}
        {state.kind === "error" && (
          <p className="rounded-lg border border-red-200 bg-red-50 px-4 py-3 text-sm text-red-700 dark:border-red-900 dark:bg-red-950 dark:text-red-300">
            {state.message}
          </p>
        )}
        {state.kind === "loaded" && state.items.length === 0 && (
          <p className="text-sm text-zinc-500 dark:text-zinc-400">{t("myList.empty")}</p>
        )}
        {state.kind === "loaded" && state.items.length > 0 && (
          <ul className="flex flex-col gap-3">
            {state.items.map((entry) => (
              <li
                key={entry.id}
                className="flex items-center gap-4 rounded-lg border border-zinc-200 p-3 dark:border-zinc-800"
              >
                {entry.anime.coverImageUrl && (
                  <img
                    src={entry.anime.coverImageUrl}
                    alt=""
                    className="h-20 w-14 shrink-0 rounded object-cover"
                  />
                )}
                <div className="flex flex-1 flex-col gap-1">
                  <p className="font-medium text-zinc-900 dark:text-zinc-50">
                    {localizedTitle(entry.anime, locale)}
                    {entry.favorite && " ★"}
                  </p>
                  <p className="text-sm text-zinc-500 dark:text-zinc-400">
                    {t(`watchEntry.status.${entry.status}`)}
                    {entry.rating !== null && ` · ${entry.rating}/10`}
                  </p>
                </div>
                <div className="flex gap-2">
                  <button
                    type="button"
                    onClick={() => {
                      setEditState({ kind: "idle" });
                      setEditing(entry);
                    }}
                    className="rounded-md px-3 py-1.5 text-xs font-medium text-zinc-600 hover:bg-zinc-100 dark:text-zinc-400 dark:hover:bg-zinc-900"
                  >
                    {t("common.edit")}
                  </button>
                  <button
                    type="button"
                    onClick={() => void handleDelete(entry)}
                    className="rounded-md px-3 py-1.5 text-xs font-medium text-red-600 hover:bg-red-50 dark:text-red-400 dark:hover:bg-red-950"
                  >
                    {t("common.delete")}
                  </button>
                </div>
              </li>
            ))}
          </ul>
        )}
      </div>

      {editing && (
        <Modal onClose={() => setEditing(null)}>
          <h2 className="mb-4 text-lg font-semibold text-zinc-900 dark:text-zinc-50">
            {t("watchEntry.form.editTitle")} — {localizedTitle(editing.anime, locale)}
          </h2>
          <WatchEntryForm
            initialValues={entryToFormValues(editing)}
            submitLabel={t("common.save")}
            busy={editState.kind === "saving"}
            onSubmit={handleEditSubmit}
            onCancel={() => setEditing(null)}
          />
          {editState.kind === "error" && (
            <p className="mt-3 text-sm text-red-600 dark:text-red-400">{editState.message}</p>
          )}
        </Modal>
      )}
    </div>
  );
}

function describeError(error: unknown): string {
  return error instanceof Error ? error.message : "Unknown error";
}
