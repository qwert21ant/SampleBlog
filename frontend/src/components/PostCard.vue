<template>
  <article 
    class="bg-white rounded-lg shadow-lg overflow-hidden hover:shadow-xl transition-shadow duration-300 cursor-pointer group"
    @click="$emit('click')"
  >
    <!-- Post Image -->
    <div class="relative h-48 bg-gray-200 overflow-hidden">
      <img 
        v-if="post.imageUrl" 
        :src="post.imageUrl" 
        :alt="post.title"
        class="w-full h-full object-cover group-hover:scale-105 transition-transform duration-300"
        loading="lazy"
      >
      <div
        v-else
        class="w-full h-full flex items-center justify-center bg-gradient-to-br from-gray-100 to-gray-200"
      >
        <PhotoIcon class="h-16 w-16 text-gray-400" />
      </div>
    </div>

    <!-- Post Content -->
    <div class="p-6">
      <!-- Post Meta -->
      <div class="flex items-center text-sm text-gray-500 mb-3">
        <CalendarIcon class="h-4 w-4 mr-1" />
        <time :datetime="post.publishedAt">{{ formattedDate }}</time>
      </div>

      <!-- Post Title -->
      <h3 class="text-xl font-bold text-gray-900 mb-3 group-hover:text-blue-600 transition-colors line-clamp-2">
        {{ post.title }}
      </h3>

      <!-- Post Excerpt -->
      <p class="text-gray-600 text-sm leading-relaxed mb-4 line-clamp-3">
        {{ post.subtitle || truncateContent(post.content) }}
      </p>

      <!-- Post Footer -->
      <div class="flex items-center justify-between">
        <!-- Author Info -->
        <div class="flex items-center">
          <UserCircleIcon class="h-8 w-8 text-gray-400 mr-2" />
          <div>
            <p class="text-sm font-medium text-gray-900">{{ post.author || $t('posts.anonymous') }}</p>
          </div>
        </div>

        <!-- Read More Link -->
        <div class="flex items-center text-blue-600 font-medium text-sm group-hover:text-blue-700 transition-colors">
          <span>{{ $t('posts.readMore') }}</span>
          <ArrowRightIcon class="h-4 w-4 ml-1 group-hover:translate-x-1 transition-transform" />
        </div>
      </div>
    </div>
  </article>
</template>

<script setup lang="ts">
import { computed } from "vue";
import { useI18n } from "vue-i18n";
import { 
  PhotoIcon, 
  CalendarIcon, 
  UserCircleIcon, 
  ArrowRightIcon 
} from "@heroicons/vue/24/outline";
import type { PublicPost } from "@/services";

// Props
interface Props {
  post: PublicPost
}

const props = defineProps<Props>();
const { t, locale } = useI18n();

// Emits
defineEmits<{
  click: []
}>();

// Computed
const formattedDate = computed(() => {
  try {
    return new Date(props.post.publishedAt).toLocaleDateString(locale.value === "ru" ? "ru-RU" : "en-US", {
      year: "numeric",
      month: "long",
      day: "numeric"
    });
  } catch {
    return t("common.invalidDate");
  }
});

// Methods
const truncateContent = (content: string, maxLength: number = 150): string => {
  if (!content) return "";
  if (content.length <= maxLength) return content;
  return content.substring(0, maxLength).trim() + "...";
};
</script>

<style scoped>
.line-clamp-2 {
  display: -webkit-box;
  -webkit-line-clamp: 2;
  line-clamp: 2;
  -webkit-box-orient: vertical;
  overflow: hidden;
}

.line-clamp-3 {
  display: -webkit-box;
  -webkit-line-clamp: 3;
  line-clamp: 3;
  -webkit-box-orient: vertical;
  overflow: hidden;
}
</style>