import AnimeProviderSettings from "./components/AnimeProviderSettings";

// The only page this frontend has so far: the anime-provider feature flag. The rest of the
// app (search, watch log, i18n) is built separately — see project README.
export default function App() {
  return (
    <div className="flex min-h-svh flex-1 items-center justify-center bg-zinc-50 px-4 py-16 dark:bg-black">
      <main className="w-full max-w-md rounded-2xl border border-zinc-200 bg-white p-8 shadow-sm dark:border-zinc-800 dark:bg-zinc-950">
        <h1 className="text-xl font-semibold text-zinc-900 dark:text-zinc-50">
          Anime data source
        </h1>
        <p className="mt-1 text-sm text-zinc-500 dark:text-zinc-400">
          Choose which provider search and lookups use. Switch here if one goes down.
        </p>

        <div className="mt-6">
          <AnimeProviderSettings />
        </div>
      </main>
    </div>
  );
}
