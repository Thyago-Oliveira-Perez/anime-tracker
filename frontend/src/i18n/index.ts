import i18next from "i18next";
import { initReactI18next } from "react-i18next";
import en from "./locales/en.json";
import ptBR from "./locales/pt-BR.json";
import ja from "./locales/ja.json";
import { DEFAULT_LOCALE } from "./locales";

void i18next.use(initReactI18next).init({
  resources: {
    en: { translation: en },
    "pt-BR": { translation: ptBR },
    ja: { translation: ja },
  },
  lng: DEFAULT_LOCALE,
  fallbackLng: DEFAULT_LOCALE,
  interpolation: { escapeValue: false }, // React already escapes output.
});

export default i18next;
