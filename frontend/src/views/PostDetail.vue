<template>
  <div class="max-w-4xl mx-auto px-4 py-8">
    <!-- Loading State -->
    <div
      v-if="isLoading"
      class="flex justify-center items-center py-16"
    >
      <div class="animate-spin rounded-full h-12 w-12 border-b-2 border-blue-600" />
      <span class="ml-3 text-lg text-gray-600">Loading post...</span>
    </div>

    <!-- Error State -->
    <div
      v-else-if="error"
      class="py-8"
    >
      <div class="bg-red-50 border border-red-200 rounded-lg p-6 text-center">
        <ExclamationTriangleIcon class="h-12 w-12 text-red-500 mx-auto mb-4" />
        <h3 class="text-lg font-semibold text-red-800 mb-2">Failed to load post</h3>
        <p class="text-red-600 mb-4">{{ error }}</p>
        <button 
          class="bg-red-600 hover:bg-red-700 text-white px-4 py-2 rounded-lg transition-colors mr-4"
          @click="fetchPost"
        >
          Try Again
        </button>
        <router-link 
          to="/"
          class="bg-gray-600 hover:bg-gray-700 text-white px-4 py-2 rounded-lg transition-colors"
        >
          Back to Home
        </router-link>
      </div>
    </div>

    <!-- Post Content -->
    <article
      v-else-if="post"
      class="prose prose-lg max-w-none"
    >
      <!-- Back Navigation -->
      <nav class="mb-8">
        <router-link 
          to="/" 
          class="inline-flex items-center text-blue-600 hover:text-blue-700 font-medium"
        >
          <ArrowLeftIcon class="h-5 w-5 mr-2" />
          Back to Posts
        </router-link>
      </nav>

      <!-- Post Header -->
      <header class="mb-8">
        <!-- Title -->
        <h1 class="text-4xl md:text-5xl font-bold text-gray-900 mb-4 leading-tight">
          {{ post.title }}
        </h1>

        <!-- Meta Information -->
        <div class="flex flex-wrap items-center gap-4 text-gray-600 mb-6">
          <div class="flex items-center">
            <UserCircleIcon class="h-5 w-5 mr-2" />
            <span>{{ post.author || 'Anonymous' }}</span>
          </div>
          <div class="flex items-center">
            <CalendarIcon class="h-5 w-5 mr-2" />
            <time :datetime="post.publishedAt">{{ formattedDate }}</time>
          </div>
        </div>

        <!-- Featured Image -->
        <div
          v-if="post.imageUrl"
          class="mb-8"
        >
          <img 
            :src="post.imageUrl" 
            :alt="post.title"
            class="w-full h-96 object-cover rounded-lg shadow-lg"
          >
        </div>
      </header>

      <!-- Post Content -->
      <div class="prose prose-lg max-w-none">
        <div
          class="text-gray-800 leading-relaxed"
          v-html="formattedContent"
        />
      </div>



      <!-- Navigation -->
      <div class="mt-12 pt-8 border-t border-gray-200">
        <div class="flex justify-between items-center">
          <router-link 
            to="/" 
            class="bg-blue-600 hover:bg-blue-700 text-white px-6 py-3 rounded-lg transition-colors font-medium"
          >
            ← More Posts
          </router-link>
          
          <button 
            class="bg-gray-100 hover:bg-gray-200 text-gray-700 px-6 py-3 rounded-lg transition-colors font-medium"
            @click="scrollToTop"
          >
            ↑ Back to Top
          </button>
        </div>
      </div>
    </article>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted, watch } from "vue";
import { useRoute } from "vue-router";
import { 
  ExclamationTriangleIcon, 
  ArrowLeftIcon, 
  UserCircleIcon, 
  CalendarIcon
} from "@heroicons/vue/24/outline";
import { postService, type PublicPost } from "@/services";

// Props
interface Props {
  id: string
}

const props = defineProps<Props>();
const route = useRoute();

// Reactive state
const post = ref<PublicPost | null>(null);
const isLoading = ref(true);
const error = ref<string | null>(null);

// Computed
const formattedDate = computed(() => {
  if (!post.value) return "";
  try {
    return new Date(post.value.publishedAt).toLocaleDateString("en-US", {
      year: "numeric",
      month: "long",
      day: "numeric"
    });
  } catch {
    return "Invalid date";
  }
});

const formattedContent = computed(() => {
  if (!post.value?.content) return "";
  // Convert line breaks to paragraphs
  return post.value.content
    .split("\n\n")
    .map(paragraph => `<p>${paragraph.replace(/\n/g, "<br>")}</p>`)
    .join("");
});

// Methods
const fetchPost = async () => {
  try {
    isLoading.value = true;
    error.value = null;
    const postId = parseInt(props.id || route.params.id as string);
    
    if (isNaN(postId)) {
      throw new Error("Invalid post ID");
    }
    
    post.value = await postService.getPostById(postId);
  } catch (err) {
    error.value = err instanceof Error ? err.message : "An unexpected error occurred";
    console.error("Failed to fetch post:", err);
  } finally {
    isLoading.value = false;
  }
};

const scrollToTop = () => {
  window.scrollTo({ top: 0, behavior: "smooth" });
};

// Watch for route changes
watch(() => props.id, () => {
  if (props.id) {
    fetchPost();
  }
});

watch(() => route.params.id, () => {
  if (route.params.id) {
    fetchPost();
  }
});

// Lifecycle
onMounted(() => {
  fetchPost();
});
</script>

<style scoped>
.prose {
  max-width: none;
}

.prose p {
  margin-bottom: 1.25rem;
  line-height: 1.75;
}

.prose h2, .prose h3, .prose h4 {
  margin-top: 2rem;
  margin-bottom: 1rem;
  font-weight: 600;
}

.prose h2 {
  font-size: 1.75rem;
}

.prose h3 {
  font-size: 1.5rem;
}

.prose ul, .prose ol {
  margin: 1.25rem 0;
  padding-left: 1.75rem;
}

.prose li {
  margin: 0.5rem 0;
}

.prose blockquote {
  border-left: 4px solid #e5e7eb;
  padding-left: 1rem;
  margin: 1.5rem 0;
  font-style: italic;
  color: #6b7280;
}

.prose code {
  background-color: #f3f4f6;
  padding: 0.125rem 0.25rem;
  border-radius: 0.25rem;
  font-size: 0.875rem;
}
</style>