<template>
  <div class="w-full">
    <!-- Hero Section -->
    <div class="bg-gradient-to-r from-blue-600 to-purple-600 text-white py-16 mb-8">
      <div class="max-w-4xl mx-auto px-4 text-center">
        <h1 class="text-4xl md:text-6xl font-bold mb-4">Welcome to SampleBlog</h1>
        <p class="text-xl md:text-2xl opacity-90">Discover amazing stories and insights</p>
      </div>
    </div>

    <!-- Loading State -->
    <div
      v-if="isLoading"
      class="flex justify-center items-center py-16"
    >
      <div class="animate-spin rounded-full h-12 w-12 border-b-2 border-blue-600" />
      <span class="ml-3 text-lg text-gray-600">Loading latest posts...</span>
    </div>

    <!-- Error State -->
    <div
      v-else-if="error"
      class="max-w-4xl mx-auto px-4 py-8"
    >
      <div class="bg-red-50 border border-red-200 rounded-lg p-6 text-center">
        <ExclamationTriangleIcon class="h-12 w-12 text-red-500 mx-auto mb-4" />
        <h3 class="text-lg font-semibold text-red-800 mb-2">Failed to load posts</h3>
        <p class="text-red-600 mb-4">{{ error }}</p>
        <button 
          class="bg-red-600 hover:bg-red-700 text-white px-4 py-2 rounded-lg transition-colors"
          @click="fetchPosts"
        >
          Try Again
        </button>
      </div>
    </div>

    <!-- Posts Grid -->
    <div
      v-else
      class="max-w-7xl mx-auto px-4 py-8"
    >
      <div
        v-if="posts.length === 0"
        class="text-center py-16"
      >
        <DocumentIcon class="h-16 w-16 text-gray-400 mx-auto mb-4" />
        <h3 class="text-xl font-semibold text-gray-600 mb-2">No posts yet</h3>
        <p class="text-gray-500">Check back later for new content!</p>
      </div>
      
      <div v-else>
        <h2 class="text-3xl font-bold text-gray-900 mb-8 text-center">Latest Posts</h2>
        <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-8">
          <PostCard 
            v-for="post in posts" 
            :key="post.id" 
            :post="post"
            @click="navigateToPost(post.id)"
          />
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from "vue";
import { useRouter } from "vue-router";
import { ExclamationTriangleIcon, DocumentIcon } from "@heroicons/vue/24/outline";
import PostCard from "@/components/PostCard.vue";
import { postService, type PublicPost } from "@/services";

const router = useRouter();

// Reactive state
const posts = ref<PublicPost[]>([]);
const isLoading = ref(true);
const error = ref<string | null>(null);

// Methods
const fetchPosts = async () => {
  try {
    isLoading.value = true;
    error.value = null;
    posts.value = await postService.getLatestPosts(6); // Get 6 latest posts
  } catch (err) {
    error.value = err instanceof Error ? err.message : "An unexpected error occurred";
    console.error("Failed to fetch posts:", err);
  } finally {
    isLoading.value = false;
  }
};

const navigateToPost = (postId: number) => {
  router.push(`/posts/${postId}`);
};

// Lifecycle
onMounted(() => {
  fetchPosts();
});
</script>