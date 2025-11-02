<template>
  <div class="border border-gray-200 rounded-lg p-6 hover:shadow-md transition-shadow">
    <div class="flex items-start justify-between">
      <div class="flex-1 min-w-0">
        <div class="flex items-center space-x-3 mb-2">
          <h3 class="text-lg font-medium text-gray-900 truncate">
            {{ post.title }}
          </h3>
          <span
            class="inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium"
            :class="post.isPublished 
              ? 'bg-green-100 text-green-800' 
              : 'bg-yellow-100 text-yellow-800'"
          >
            {{ post.isPublished ? $t('admin.published') : $t('admin.draft') }}
          </span>
        </div>
        
        <p 
          class="text-gray-600 text-sm mb-3 overflow-hidden" 
          style="display: -webkit-box; -webkit-line-clamp: 2; line-clamp: 2; -webkit-box-orient: vertical;"
        >
          {{ post.subtitle || post.text?.substring(0, 150) + '...' }}
        </p>
        
        <div class="flex items-center text-sm text-gray-500 space-x-4">
          <span>{{ getAuthorName(post.author) }}</span>
          <span>•</span>
          <span>{{ formatDate(post.createdAt || post.publishedAt) }}</span>
        </div>
      </div>
      
      <div class="flex items-center space-x-2 ml-4">
        <button
          class="inline-flex items-center px-3 py-1.5 bg-blue-50 text-blue-700 rounded-md hover:bg-blue-100 transition-colors text-sm"
          @click="$emit('edit', post)"
        >
          <PencilIcon class="h-4 w-4 mr-1" />
          {{ $t('admin.editPost') }}
        </button>
        
        <button
          v-if="post.isPublished"
          class="inline-flex items-center px-3 py-1.5 bg-yellow-50 text-yellow-700 rounded-md hover:bg-yellow-100 transition-colors text-sm"
          @click="$emit('unpublish', post)"
        >
          <EyeSlashIcon class="h-4 w-4 mr-1" />
          {{ $t('admin.unpublishPost') }}
        </button>
        
        <button
          v-else
          class="inline-flex items-center px-3 py-1.5 bg-green-50 text-green-700 rounded-md hover:bg-green-100 transition-colors text-sm"
          @click="$emit('publish', post)"
        >
          <EyeIcon class="h-4 w-4 mr-1" />
          {{ $t('admin.publishPost') }}
        </button>
        
        <button
          class="inline-flex items-center px-3 py-1.5 bg-red-50 text-red-700 rounded-md hover:bg-red-100 transition-colors text-sm"
          @click="$emit('delete', post)"
        >
          <TrashIcon class="h-4 w-4 mr-1" />
          {{ $t('admin.deletePost') }}
        </button>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { useI18n } from "vue-i18n";
import {
  EyeIcon,
  EyeSlashIcon,
  PencilIcon,
  TrashIcon
} from "@heroicons/vue/24/outline";
import type { AdminPost } from "@/types";

// Props
interface Props {
  post: AdminPost;
}

defineProps<Props>();
const { locale } = useI18n();

// Emits
defineEmits<{
  edit: [post: AdminPost];
  publish: [post: AdminPost];
  unpublish: [post: AdminPost];
  delete: [post: AdminPost];
}>();

// Utility functions
const formatDate = (dateString: string) => {
  return new Date(dateString).toLocaleDateString(locale.value === "ru" ? "ru-RU" : "en-US", {
    year: "numeric",
    month: "short",
    day: "numeric",
    hour: "2-digit",
    minute: "2-digit"
  });
};

const getAuthorName = (author: AdminPost["author"]) => {
  return typeof author === "string" ? author : author.username;
};
</script>