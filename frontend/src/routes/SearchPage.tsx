import { useState, type FormEvent } from "react";
import { useTranslation } from "react-i18next";
import { useParams } from "react-router";
import { createWatchEntry, searchAnime, type AnimeDto, type UpdateWatchEntryRequest } from "../lib/api";
import { urlSegmentToLocale, type Locale } from "../i18n/locales";
import { localizedTitle } from "../lib/animeTitle";
import Modal from "../components/Modal";
import WatchEntryForm from "../components/WatchEntryForm";

type SearchState =
  | { kind: "idle" }
  | { kind: "loading" }
  | { kind: "error"; message: string }
  | { kind: "results"; items: AnimeDto[] };

type AddState = { kind: "idle" } | { kind: "saving" } | { kind: "error"; message: string };

export default function SearchPage() {
  const { t } = useTranslation();
  const { localeSegment } = useParams();
  const locale: Locale = (localeSegment && urlSegmentToLocale[localeSegment]) || "en";

  const [query, setQuery] = useState("");
  const [state, setState] = useState<SearchState>({ kind: "idle" });
  const [selected, setSelected] = useState<AnimeDto | null>(null);
  const [addState, setAddState] = useState<AddState>({ kind: "idle" });

  async function handleSearch(event: FormEvent) {
    event.preventDefault();
    if (query.trim() === "") return;

    setState({ kind: "loading" });
    try {
      const items = await searchAnime(query.trim());
      setState({ kind: "results", items });
    } catch (error) {
      setState({ kind: "error", message: describeError(error) });
    }
  }

  function openAddModal(anime: AnimeDto) {
    setAddState({ kind: "idle" });
    setSelected(anime);
  }

  async function handleAdd(request: UpdateWatchEntryRequest) {
    if (!selected) return;

    setAddState({ kind: "saving" });
    try {
      await createWatchEntry({ ...request, provider: selected.provider, externalId: selected.externalId });
      setSelected(null);
      setAddState({ kind: "idle" });
    } catch (error) {
      setAddState({ kind: "error", message: describeError(error) });
    }
  }

  return (
    <div className="mx-auto w-full max-w-4xl flex-1 px-4 py-8">
      <h1 className="text-xl font-semibold text-zinc-900 dark:text-zinc-50">{t("search.title")}</h1>

      <form onSubmit={handleSearch} className="mt-4 flex gap-2">
        <input
          type="text"
          value={query}
          onChange={(event) => setQuery(event.target.value)}
          placeholder={t("search.placeholder")}
          className="flex-1 rounded-md border border-zinc-300 bg-white px-3 py-2 text-sm text-zinc-900 dark:border-zinc-700 dark:bg-zinc-900 dark:text-zinc-100"
        />
        <button
          type="submit"
          className="rounded-md bg-zinc-900 px-4 py-2 text-sm font-medium text-white hover:bg-zinc-700 dark:bg-zinc-100 dark:text-zinc-900 dark:hover:bg-zinc-300"
        >
          {t("search.button")}
        </button>
      </form>

      <div className="mt-6">
        {state.kind === "idle" && <p className="text-sm text-zinc-500 dark:text-zinc-400">{t("search.prompt")}</p>}
        {state.kind === "loading" && (
          <p className="text-sm text-zinc-500 dark:text-zinc-400">{t("search.searching")}</p>
        )}
        {state.kind === "error" && (
          <p className="rounded-lg border border-red-200 bg-red-50 px-4 py-3 text-sm text-red-700 dark:border-red-900 dark:bg-red-950 dark:text-red-300">
            {state.message}
          </p>
        )}
        {state.kind === "results" && state.items.length === 0 && (
          <p className="text-sm text-zinc-500 dark:text-zinc-400">{t("search.noResults")}</p>
        )}
        {state.kind === "results" && state.items.length > 0 && (
          <ul className="grid grid-cols-2 gap-4 sm:grid-cols-3 md:grid-cols-4">
            {state.items.map((anime) => (
              <li
                key={`${anime.provider}-${anime.externalId}`}
                className="flex flex-col overflow-hidden rounded-lg border border-zinc-200 dark:border-zinc-800"
              >
                {anime.coverImageUrl && (
                  <img src={anime.coverImageUrl} alt="" className="aspect-2/3 w-full object-cover" />
                )}
                <div className="flex flex-1 flex-col gap-2 p-3">
                  <p className="line-clamp-2 text-sm font-medium text-zinc-900 dark:text-zinc-50">
                    {localizedTitle(anime, locale)}
                  </p>
                  <button
                    type="button"
                    onClick={() => openAddModal(anime)}
                    className="mt-auto rounded-md bg-zinc-900 px-3 py-1.5 text-xs font-medium text-white hover:bg-zinc-700 dark:bg-zinc-100 dark:text-zinc-900 dark:hover:bg-zinc-300"
                  >
                    {t("search.addToList")}
                  </button>
                </div>
              </li>
            ))}
          </ul>
        )}
      </div>

      {selected && (
        <Modal onClose={() => setSelected(null)}>
          <h2 className="mb-4 text-lg font-semibold text-zinc-900 dark:text-zinc-50">
            {t("watchEntry.form.addTitle")} — {localizedTitle(selected, locale)}
          </h2>
          <WatchEntryForm
            submitLabel={t("common.add")}
            busy={addState.kind === "saving"}
            onSubmit={handleAdd}
            onCancel={() => setSelected(null)}
          />
          {addState.kind === "error" && (
            <p className="mt-3 text-sm text-red-600 dark:text-red-400">{addState.message}</p>
          )}
        </Modal>
      )}
    </div>
  );
}

function describeError(error: unknown): string {
  return error instanceof Error ? error.message : "Unknown error";
}
