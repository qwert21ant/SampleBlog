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
        <button 
          class="bg-gray-600 hover:bg-gray-700 text-white px-4 py-2 rounded-lg transition-colors"
          @click="goBack"
        >
          Back to Posts
        </button>
      </div>
    </div>

    <!-- Post Content -->
    <article
      v-else-if="post"
      class="prose prose-lg max-w-none"
    >
      <!-- Back Navigation -->
      <nav class="mb-8">
        <button 
          class="inline-flex items-center text-blue-600 hover:text-blue-700 font-medium"
          @click="goBack"
        >
          <ArrowLeftIcon class="h-5 w-5 mr-2" />
          Back to Posts
        </button>
      </nav>

      <!-- Post Header -->
      <header class="mb-8">
        <!-- Title -->
        <h1 class="text-4xl md:text-5xl font-bold text-gray-900 mb-4 leading-tight">
          {{ post.title }}
        </h1>

        <!-- Subtitle -->
        <div
          v-if="post.subtitle"
          class="mb-6"
        >
          <h2 class="text-xl md:text-2xl text-gray-600 font-light italic leading-relaxed">
            {{ post.subtitle }}
          </h2>
        </div>

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

        <!-- Images Slider -->
        <div
          v-if="postImages.length > 0"
          class="mb-8"
        >
          <!-- Single Image -->
          <div
            v-if="postImages.length === 1"
            class="w-full"
          >
            <img 
              :src="postImages[0].url" 
              :alt="postImages[0].altText || post.title"
              class="w-full h-96 object-cover rounded-lg shadow-lg"
            >
          </div>

          <!-- Multiple Images Slider -->
          <div
            v-else
            class="relative"
          >
            <!-- Main Image Display -->
            <div 
              class="relative overflow-hidden rounded-lg shadow-lg"
              @touchstart="handleTouchStart"
              @touchend="handleTouchEnd"
            >
              <img 
                :src="postImages[currentImageIndex].url"
                :alt="postImages[currentImageIndex].altText || `${post.title} - Image ${currentImageIndex + 1}`"
                class="w-full h-64 sm:h-80 md:h-96 object-cover transition-all duration-300"
              >
              
              <!-- Image Counter -->
              <div class="absolute top-4 right-4 bg-black bg-opacity-50 text-white px-3 py-1 rounded-full text-sm">
                {{ currentImageIndex + 1 }} / {{ postImages.length }}
              </div>

              <!-- Navigation Arrows -->
              <button
                v-if="postImages.length > 1"
                class="absolute left-4 top-1/2 transform -translate-y-1/2 bg-black bg-opacity-50 hover:bg-opacity-75 text-white p-2 rounded-full transition-all duration-200"
                @click="previousImage"
              >
                <ArrowLeftIcon class="h-6 w-6" />
              </button>
              <button
                v-if="postImages.length > 1"
                class="absolute right-4 top-1/2 transform -translate-y-1/2 bg-black bg-opacity-50 hover:bg-opacity-75 text-white p-2 rounded-full transition-all duration-200"
                @click="nextImage"
              >
                <ArrowRightIcon class="h-6 w-6" />
              </button>
            </div>

            <!-- Image Thumbnails -->
            <div
              v-if="postImages.length > 1"
              class="flex space-x-2 mt-4 overflow-x-auto pb-2 scrollbar-hide"
            >
              <button
                v-for="(image, index) in postImages"
                :key="image.id"
                class="flex-shrink-0 relative focus:outline-none focus:ring-2 focus:ring-blue-500 focus:ring-offset-2 rounded"
                @click="currentImageIndex = index"
              >
                <img 
                  :src="image.url"
                  :alt="`Thumbnail ${index + 1}`"
                  class="w-16 h-12 sm:w-20 sm:h-16 object-cover rounded border-2 transition-all duration-200"
                  :class="index === currentImageIndex ? 'border-blue-500 shadow-md' : 'border-gray-300 hover:border-gray-400'"
                >
                <!-- Active indicator for small screens -->
                <div
                  v-if="index === currentImageIndex"
                  class="absolute -bottom-1 left-1/2 transform -translate-x-1/2 w-2 h-2 bg-blue-500 rounded-full"
                />
              </button>
            </div>
          </div>
        </div>

        <!-- Fallback Featured Image (if no post images but has imageUrl) -->
        <div
          v-else-if="post.imageUrl"
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
        <div class="flex justify-center">
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
import { ref, computed, onMounted, onUnmounted, watch } from "vue";
import { useRoute, useRouter } from "vue-router";
import { 
  ExclamationTriangleIcon, 
  ArrowLeftIcon, 
  ArrowRightIcon,
  UserCircleIcon, 
  CalendarIcon
} from "@heroicons/vue/24/outline";
import { postService, type PublicPost, type ImageDetailsDto } from "@/services";

// Props
interface Props {
  id: string
}

const props = defineProps<Props>();
const route = useRoute();
const router = useRouter();

// Reactive state
const post = ref<PublicPost | null>(null);
const postImages = ref<ImageDetailsDto[]>([]);
const currentImageIndex = ref(0);
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
    
    // Fetch post and images in parallel
    const [postData, images] = await Promise.all([
      postService.getPostById(postId),
      postService.getImagesByPost(postId).catch((err) => {
        console.warn("Failed to load post images:", err);
        return []; // Don't fail if images can't be loaded
      })
    ]);
    
    post.value = postData;
    postImages.value = images;
    currentImageIndex.value = 0; // Reset image index when loading new post
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

const goBack = () => {
  // Check if we came from within the app (referrer includes our domain or is empty for direct navigation)
  const referrer = document.referrer;
  const currentOrigin = window.location.origin;
  
  if (referrer && referrer.startsWith(currentOrigin)) {
    // We came from within our app, go back
    router.go(-1);
  } else if (window.history.length > 1) {
    // We have history but unclear origin, still try to go back
    router.go(-1);
  } else {
    // Fallback to posts grid page, or home if grid doesn't exist
    router.push('/posts').catch(() => router.push('/'));
  }
};

const nextImage = () => {
  if (postImages.value.length > 1) {
    currentImageIndex.value = (currentImageIndex.value + 1) % postImages.value.length;
  }
};

const previousImage = () => {
  if (postImages.value.length > 1) {
    currentImageIndex.value = currentImageIndex.value === 0 
      ? postImages.value.length - 1 
      : currentImageIndex.value - 1;
  }
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

// Handle keyboard navigation for images
const handleKeydown = (event: KeyboardEvent) => {
  if (postImages.value.length <= 1) return;
  
  switch (event.key) {
    case 'ArrowLeft':
      event.preventDefault();
      previousImage();
      break;
    case 'ArrowRight':
      event.preventDefault();
      nextImage();
      break;
  }
};

// Touch/swipe support
const touchStartX = ref(0);
const touchEndX = ref(0);

const handleTouchStart = (event: TouchEvent) => {
  touchStartX.value = event.changedTouches[0].screenX;
};

const handleTouchEnd = (event: TouchEvent) => {
  touchEndX.value = event.changedTouches[0].screenX;
  handleSwipe();
};

const handleSwipe = () => {
  if (postImages.value.length <= 1) return;
  
  const swipeThreshold = 50;
  const diff = touchStartX.value - touchEndX.value;
  
  if (Math.abs(diff) > swipeThreshold) {
    if (diff > 0) {
      // Swiped left - show next image
      nextImage();
    } else {
      // Swiped right - show previous image
      previousImage();
    }
  }
};

// Lifecycle
onMounted(() => {
  fetchPost();
  document.addEventListener('keydown', handleKeydown);
});

onUnmounted(() => {
  document.removeEventListener('keydown', handleKeydown);
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

/* Custom scrollbar for thumbnails */
.scrollbar-hide {
  -ms-overflow-style: none;  /* Internet Explorer 10+ */
  scrollbar-width: none;  /* Firefox */
}
.scrollbar-hide::-webkit-scrollbar { 
  display: none;  /* Safari and Chrome */
}
</style>