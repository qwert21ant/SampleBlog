import { createI18n } from "vue-i18n";
import en from "./locales/en.json";
import ru from "./locales/ru.json";

// Get saved language from localStorage or default to English
const savedLanguage = localStorage.getItem("language") || "en";

// Validate that the saved language is supported
const supportedLanguages = ["en", "ru"];
const defaultLanguage = supportedLanguages.includes(savedLanguage) ? savedLanguage : "en";

export const i18n = createI18n({
  legacy: false,
  locale: defaultLanguage,
  fallbackLocale: "en",
  globalInjection: true,
  silentFallbackWarn: true,
  missingWarn: false,
  messages: {
    en,
    ru
  },
  pluralizationRules: {
    // Russian pluralization rules
    ru: (choice: number, choicesLength: number) => {
      if (choice === 0) {
        return 0;
      }

      const teen = choice > 10 && choice < 20;
      const endsWithOne = choice % 10 === 1;

      if (choicesLength < 4) {
        return !teen && endsWithOne ? 1 : 2;
      }
      if (!teen && endsWithOne) {
        return 1;
      }
      if (!teen && choice % 10 >= 2 && choice % 10 <= 4) {
        return 2;
      }

      return choicesLength < 4 ? 2 : 3;
    }
  }
});

// Function to change language
export const changeLanguage = (locale: string) => {
  if (supportedLanguages.includes(locale)) {
    i18n.global.locale.value = locale as "en" | "ru";
    localStorage.setItem("language", locale);
    
    // Update document language attribute
    document.documentElement.lang = locale;
  }
};

// Initialize document language
document.documentElement.lang = defaultLanguage;

export default i18n;