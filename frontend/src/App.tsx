import { Navigate, Route, Routes } from "react-router";
import LocaleLayout from "./routes/LocaleLayout";
import SearchPage from "./routes/SearchPage";
import MyListPage from "./routes/MyListPage";
import SettingsPage from "./routes/SettingsPage";
import { detectPreferredLocaleSegment } from "./i18n/locales";

export default function App() {
  const homeRedirect = `/${detectPreferredLocaleSegment()}/search`;

  return (
    <Routes>
      <Route path="/" element={<Navigate to={homeRedirect} replace />} />
      <Route path="/:localeSegment" element={<LocaleLayout />}>
        <Route index element={<Navigate to="search" replace />} />
        <Route path="search" element={<SearchPage />} />
        <Route path="my-list" element={<MyListPage />} />
        <Route path="settings" element={<SettingsPage />} />
      </Route>
      <Route path="*" element={<Navigate to={homeRedirect} replace />} />
    </Routes>
  );
}
