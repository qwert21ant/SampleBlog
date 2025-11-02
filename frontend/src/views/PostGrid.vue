<template>
  <div class="min-h-screen bg-gray-50">
    <!-- Header Section -->
    <div class="bg-white shadow-sm border-b border-gray-200">
      <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
        <div class="text-center">
          <h1 class="text-4xl font-bold text-gray-900 mb-4">
            {{ $t('posts.allPosts') }}
          </h1>
          <p class="text-lg text-gray-600 max-w-2xl mx-auto">
            {{ $t('posts.exploreCollection') }}
          </p>
        </div>
      </div>
    </div>

    <!-- Main Content -->
    <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
      <!-- Loading State -->
      <div
        v-if="isLoading && posts.length === 0"
        class="flex justify-center items-center py-16"
      >
        <div class="animate-spin rounded-full h-12 w-12 border-b-2 border-blue-600" />
        <span class="ml-3 text-lg text-gray-600">{{ $t('posts.loading') }}</span>
      </div>

      <!-- Error State -->
      <div
        v-else-if="error"
        class="py-8"
      >
        <div class="bg-red-50 border border-red-200 rounded-lg p-6 text-center max-w-md mx-auto">
          <ExclamationTriangleIcon class="h-12 w-12 text-red-500 mx-auto mb-4" />
          <h3 class="text-lg font-semibold text-red-800 mb-2">{{ $t('posts.failedToLoad') }}</h3>
          <p class="text-red-600 mb-4">{{ error }}</p>
          <button 
            class="bg-red-600 hover:bg-red-700 text-white px-4 py-2 rounded-lg transition-colors"
            @click="() => fetchPosts()"
          >
            {{ $t('posts.tryAgain') }}
          </button>
        </div>
      </div>

      <!-- No Results -->
      <div
        v-else-if="!isLoading && posts.length === 0"
        class="py-16 text-center"
      >
        <DocumentTextIcon class="h-16 w-16 text-gray-400 mx-auto mb-4" />
        <h3 class="text-lg font-medium text-gray-900 mb-2">
          {{ $t('posts.noPosts') }}
        </h3>
        <p class="text-gray-600">
          {{ $t('posts.checkBackLater') }}
        </p>
      </div>

      <!-- Posts Grid -->
      <div v-else>
        <!-- Results Info -->
        <div class="flex justify-between items-center mb-6">
          <p class="text-gray-600">
            {{ $t('posts.showingResults', { count: posts.length, total: totalCount }) }}
          </p>
          
          <!-- View Toggle -->
          <div class="flex items-center space-x-2">
            <span class="text-sm text-gray-600">{{ $t('posts.view') }}:</span>
            <button
              class="p-2 rounded-lg transition-colors"
              :class="viewMode === 'grid' ? 'bg-blue-100 text-blue-600' : 'text-gray-400 hover:text-gray-600'"
              @click="viewMode = 'grid'"
            >
              <Squares2X2Icon class="h-5 w-5" />
            </button>
            <button
              class="p-2 rounded-lg transition-colors"
              :class="viewMode === 'list' ? 'bg-blue-100 text-blue-600' : 'text-gray-400 hover:text-gray-600'"
              @click="viewMode = 'list'"
            >
              <ListBulletIcon class="h-5 w-5" />
            </button>
          </div>
        </div>

        <!-- Grid View -->
        <div
          v-if="viewMode === 'grid'"
          class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-6"
        >
          <PostCard
            v-for="post in posts"
            :key="post.id"
            :post="post"
            class="transform hover:scale-105 transition-transform duration-200"
            @click="$router.push(`/posts/${post.id}`)"
          />
        </div>

        <!-- List View -->
        <div
          v-else
          class="space-y-6"
        >
          <div
            v-for="post in posts"
            :key="post.id"
            class="bg-white rounded-lg shadow-sm border border-gray-200 overflow-hidden hover:shadow-md transition-shadow duration-200"
          >
            <div class="md:flex">
              <!-- Image -->
              <div class="md:w-48 md:flex-shrink-0">
                <img
                  v-if="post.imageUrl"
                  :src="post.imageUrl"
                  :alt="post.title"
                  class="h-48 w-full md:h-full object-cover"
                >
                <div
                  v-else
                  class="h-48 w-full md:h-full bg-gradient-to-br from-blue-500 to-purple-600 flex items-center justify-center"
                >
                  <DocumentTextIcon class="h-12 w-12 text-white opacity-50" />
                </div>
              </div>

              <!-- Content -->
              <div class="p-6 flex-1">
                <div class="flex flex-col h-full">
                  <div class="flex-1">
                    <router-link
                      :to="`/posts/${post.id}`"
                      class="block hover:text-blue-600 transition-colors"
                    >
                      <h3 class="text-xl font-bold text-gray-900 mb-2">
                        {{ post.title }}
                      </h3>
                    </router-link>
                    
                    <p class="text-gray-600 mb-4 line-clamp-3">
                      {{ post.content }}
                    </p>
                  </div>

                  <div class="flex items-center justify-between text-sm text-gray-500">
                    <div class="flex items-center">
                      <UserCircleIcon class="h-4 w-4 mr-2" />
                      <span>{{ post.author }}</span>
                    </div>
                    <div class="flex items-center">
                      <CalendarIcon class="h-4 w-4 mr-2" />
                      <time :datetime="post.publishedAt">
                        {{ formatDate(post.publishedAt) }}
                      </time>
                    </div>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>

        <!-- Loading More Posts -->
        <div
          v-if="isLoading && posts.length > 0"
          class="flex justify-center items-center py-8"
        >
          <div class="animate-spin rounded-full h-8 w-8 border-b-2 border-blue-600" />
          <span class="ml-3 text-gray-600">Loading more posts...</span>
        </div>

        <!-- Pagination -->
        <div
          v-if="totalPages > 1"
          class="mt-12 flex justify-center"
        >
          <nav class="flex items-center space-x-2">
            <!-- Previous Button -->
            <button
              :disabled="currentPage === 1"
              class="px-3 py-2 text-sm font-medium text-gray-500 bg-white border border-gray-300 rounded-l-md hover:bg-gray-50 disabled:opacity-50 disabled:cursor-not-allowed"
              @click="goToPage(currentPage - 1)"
            >
              <ChevronLeftIcon class="h-4 w-4" />
            </button>

            <!-- Page Numbers -->
            <template
              v-for="page in visiblePages"
              :key="page"
            >
              <button
                v-if="typeof page === 'number'"
                class="px-3 py-2 text-sm font-medium border border-gray-300 hover:bg-gray-50 transition-colors"
                :class="page === currentPage 
                  ? 'bg-blue-600 text-white border-blue-600 hover:bg-blue-700' 
                  : 'text-gray-700 bg-white'"
                @click="goToPage(page)"
              >
                {{ page }}
              </button>
              <span
                v-else
                class="px-3 py-2 text-sm text-gray-500"
              >
                ...
              </span>
            </template>

            <!-- Next Button -->
            <button
              :disabled="currentPage === totalPages"
              class="px-3 py-2 text-sm font-medium text-gray-500 bg-white border border-gray-300 rounded-r-md hover:bg-gray-50 disabled:opacity-50 disabled:cursor-not-allowed"
              @click="goToPage(currentPage + 1)"
            >
              <ChevronRightIcon class="h-4 w-4" />
            </button>
          </nav>
        </div>

        <!-- Page Info -->
        <div class="mt-4 text-center text-sm text-gray-600">
          {{ $t('common.page') }} {{ currentPage }} {{ $t('common.of') }} {{ totalPages }}
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted, watch } from "vue";
import { useRoute, useRouter } from "vue-router";
import {
  ExclamationTriangleIcon,
  DocumentTextIcon,
  UserCircleIcon,
  CalendarIcon,
  ChevronLeftIcon,
  ChevronRightIcon,
  Squares2X2Icon,
  ListBulletIcon
} from "@heroicons/vue/24/outline";
import { postService, type PublicPost } from "@/services";
import PostCard from "@/components/PostCard.vue";

// Router
const route = useRoute();
const router = useRouter();

// Reactive state
const posts = ref<PublicPost[]>([]);
const isLoading = ref(false);
const error = ref<string | null>(null);
const viewMode = ref<"grid" | "list">("grid");

// Pagination state
const currentPage = ref(1);
const pageSize = ref(12);
const totalCount = ref(0);
const totalPages = computed(() => Math.ceil(totalCount.value / pageSize.value));

// Computed properties
const visiblePages = computed(() => {
  const pages: (number | string)[] = [];
  const maxVisible = 7;
  
  if (totalPages.value <= maxVisible) {
    // Show all pages if total is small
    for (let i = 1; i <= totalPages.value; i++) {
      pages.push(i);
    }
  } else {
    // Complex pagination logic
    const current = currentPage.value;
    const total = totalPages.value;
    
    if (current <= 4) {
      // Show first pages
      for (let i = 1; i <= 5; i++) {
        pages.push(i);
      }
      pages.push("...");
      pages.push(total);
    } else if (current >= total - 3) {
      // Show last pages
      pages.push(1);
      pages.push("...");
      for (let i = total - 4; i <= total; i++) {
        pages.push(i);
      }
    } else {
      // Show middle pages
      pages.push(1);
      pages.push("...");
      for (let i = current - 1; i <= current + 1; i++) {
        pages.push(i);
      }
      pages.push("...");
      pages.push(total);
    }
  }
  
  return pages;
});

// Methods
const fetchPosts = async (resetPage = false) => {
  if (resetPage) {
    currentPage.value = 1;
  }

  isLoading.value = true;
  error.value = null;

  try {
    const params = {
      page: currentPage.value,
      pageSize: pageSize.value
    };

    const result = await postService.getPublishedPosts(params);
    
    posts.value = result.items;
    totalCount.value = result.totalCount;
    
    // Update URL with current page
    const query: Record<string, string> = {};
    if (currentPage.value > 1) query.page = currentPage.value.toString();
    
    router.replace({ query });
  } catch (err) {
    error.value = err instanceof Error ? err.message : "Failed to load posts";
    console.error("Failed to fetch posts:", err);
  } finally {
    isLoading.value = false;
  }
};

const goToPage = (page: number) => {
  if (page >= 1 && page <= totalPages.value && page !== currentPage.value) {
    currentPage.value = page;
    fetchPosts();
    
    // Scroll to top
    window.scrollTo({ top: 0, behavior: "smooth" });
  }
};

const formatDate = (dateString: string) => {
  try {
    return new Date(dateString).toLocaleDateString("en-US", {
      year: "numeric",
      month: "short",
      day: "numeric"
    });
  } catch {
    return "Invalid date";
  }
};



// Initialize from URL params
const initializeFromUrl = () => {
  const pageParam = route.query.page;
  
  if (pageParam && typeof pageParam === "string") {
    const page = parseInt(pageParam);
    if (!isNaN(page) && page > 0) {
      currentPage.value = page;
    }
  }
};

// Watchers
watch(() => route.query, () => {
  if (route.name === "PostGrid") {
    initializeFromUrl();
  }
});

// Lifecycle
onMounted(() => {
  initializeFromUrl();
  fetchPosts();
});
</script>

<style scoped>
.line-clamp-3 {
  display: -webkit-box;
  -webkit-line-clamp: 3;
  -webkit-box-orient: vertical;
  overflow: hidden;
}
</style>