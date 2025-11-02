<template>
  <div class="relative">
    <button
      class="flex items-center space-x-2 px-3 py-2 rounded-md text-sm font-medium hover:bg-slate-700 transition-colors"
      @click="toggleDropdown"
    >
      <GlobeAltIcon class="h-4 w-4" />
      <span class="hidden sm:block">{{ currentLanguageLabel }}</span>
      <ChevronDownIcon class="h-4 w-4" />
    </button>

    <!-- Dropdown Menu -->
    <div
      v-show="showDropdown"
      class="absolute right-0 mt-2 w-40 bg-white rounded-lg shadow-lg border border-gray-200 py-1 z-50"
    >
      <button
        v-for="language in languages"
        :key="language.code"
        class="flex items-center w-full px-4 py-2 text-sm text-gray-700 hover:bg-gray-100 transition-colors"
        :class="{ 'bg-blue-50 text-blue-600': currentLocale === language.code }"
        @click="selectLanguage(language.code)"
      >
        <span class="mr-3 text-lg">{{ language.flag }}</span>
        <span>{{ language.name }}</span>
        <CheckIcon
          v-if="currentLocale === language.code"
          class="h-4 w-4 ml-auto text-blue-600"
        />
      </button>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted, onUnmounted } from "vue";
import { useI18n } from "vue-i18n";
import {
  GlobeAltIcon,
  ChevronDownIcon,
  CheckIcon
} from "@heroicons/vue/24/outline";
import { changeLanguage } from "@/i18n";

const { locale } = useI18n();

// Reactive state
const showDropdown = ref(false);

// Language options
const languages = [
  {
    code: "en",
    name: "English",
    flag: "🇺🇸"
  },
  {
    code: "ru",
    name: "Русский",
    flag: "🇷🇺"
  }
];

// Computed properties
const currentLocale = computed(() => locale.value);
const currentLanguageLabel = computed(() => {
  const current = languages.find(lang => lang.code === currentLocale.value);
  return current?.name || "English";
});

// Methods
const toggleDropdown = () => {
  showDropdown.value = !showDropdown.value;
};

const selectLanguage = (languageCode: string) => {
  changeLanguage(languageCode);
  showDropdown.value = false;
};

const closeDropdown = (event: Event) => {
  const target = event.target as HTMLElement;
  if (!target.closest(".relative")) {
    showDropdown.value = false;
  }
};

// Lifecycle
onMounted(() => {
  document.addEventListener("click", closeDropdown);
});

onUnmounted(() => {
  document.removeEventListener("click", closeDropdown);
});
</script>