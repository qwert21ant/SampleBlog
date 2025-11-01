<template>
  <div class="max-w-7xl mx-auto px-4 py-8">
    <!-- Header -->
    <div class="mb-8">
      <div class="flex items-center justify-between">
        <div>
          <h1 class="text-3xl font-bold text-gray-900">Admin Dashboard</h1>
          <p class="text-gray-600 mt-1">Manage your blog content and settings</p>
        </div>
        <div class="flex items-center space-x-3">
          <span class="text-sm text-gray-500">Welcome back,</span>
          <span class="font-medium text-gray-900">{{ user?.username || user?.email }}</span>
        </div>
      </div>
    </div>

    <!-- Stats Cards -->
    <div class="grid grid-cols-1 md:grid-cols-3 gap-6 mb-8">
      <div class="bg-white rounded-lg shadow p-6">
        <div class="flex items-center">
          <DocumentTextIcon class="h-8 w-8 text-blue-500" />
          <div class="ml-4">
            <p class="text-sm font-medium text-gray-500">Total Posts</p>
            <p class="text-2xl font-bold text-gray-900">{{ stats.totalPosts || 0 }}</p>
          </div>
        </div>
      </div>

      <div class="bg-white rounded-lg shadow p-6">
        <div class="flex items-center">
          <EyeIcon class="h-8 w-8 text-green-500" />
          <div class="ml-4">
            <p class="text-sm font-medium text-gray-500">Published</p>
            <p class="text-2xl font-bold text-gray-900">{{ stats.publishedPosts || 0 }}</p>
          </div>
        </div>
      </div>

      <div class="bg-white rounded-lg shadow p-6">
        <div class="flex items-center">
          <PencilIcon class="h-8 w-8 text-yellow-500" />
          <div class="ml-4">
            <p class="text-sm font-medium text-gray-500">Drafts</p>
            <p class="text-2xl font-bold text-gray-900">{{ stats.draftPosts || 0 }}</p>
          </div>
        </div>
      </div>
    </div>

    <!-- Quick Actions -->
    <div class="bg-white rounded-lg shadow mb-8">
      <div class="px-6 py-4 border-b border-gray-200 flex items-center justify-between">
        <h2 class="text-lg font-medium text-gray-900">Posts Management</h2>
        <button
          class="inline-flex items-center px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700 transition-colors"
          @click="createNewPost"
        >
          <PlusIcon class="h-5 w-5 mr-2" />
          Create New Post
        </button>
      </div>
    </div>

    <!-- Posts List -->
    <div class="bg-white rounded-lg shadow">
      <div class="px-6 py-4 border-b border-gray-200">
        <h2 class="text-lg font-medium text-gray-900">All Posts</h2>
      </div>
      <div class="p-6">
        <div
          v-if="isLoadingPosts"
          class="flex justify-center py-8"
        >
          <div class="animate-spin rounded-full h-8 w-8 border-b-2 border-blue-600" />
        </div>
        <div
          v-else-if="!filteredPosts || filteredPosts.length === 0"
          class="text-center py-8"
        >
          <DocumentTextIcon class="h-12 w-12 text-gray-400 mx-auto mb-4" />
          <h3 class="text-lg font-medium text-gray-900 mb-2">No Posts Yet</h3>
          <p class="text-gray-500">Create your first blog post to get started.</p>
        </div>
        <div
          v-else
          class="space-y-6"
        >
          <!-- Post Cards -->
          <AdminPostCard
            v-for="post in filteredPosts"
            :key="post.id"
            :post="post"
            @edit="editPost"
            @publish="publishPost"
            @unpublish="unpublishPost"
            @delete="deletePost"
          />
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, computed } from "vue";
import { useRouter } from "vue-router";
import {
  DocumentTextIcon,
  EyeIcon,
  PencilIcon,
  PlusIcon
} from "@heroicons/vue/24/outline";
import AdminPostCard from "@/components/AdminPostCard.vue";
import { useAuth } from "@/composables/useAuth";
import { adminService } from "@/services";
import type { PostStatsDto, AdminPost } from "@/types";

// Composables
const { user, requireAdmin } = useAuth();
const router = useRouter();

// Reactive state
const isLoadingStats = ref(true);
const isLoadingPosts = ref(true);
const stats = ref<Partial<PostStatsDto>>({});
const posts = ref<AdminPost[]>([]);
const error = ref<string | null>(null);

// Filters for posts
const filters = ref({
  search: "",
  category: "",
  status: "all" as "all" | "published" | "draft"
});

// Computed properties
const filteredPosts = computed(() => {
  return posts.value.map(post => ({
    ...post,
    // Add computed properties for backward compatibility
    content: post.text,
    excerpt: post.subtitle
  }));
});

// Methods
const loadStats = async () => {
  try {
    isLoadingStats.value = true;
    error.value = null;
    stats.value = await adminService.getPostStats();
  } catch (err) {
    console.error("Failed to load stats:", err);
    error.value = "Failed to load statistics. Using fallback data.";
  } finally {
    isLoadingStats.value = false;
  }
};

const loadPosts = async (refreshing = false) => {
  try {
    isLoadingPosts.value = true;
    error.value = null;
    
    // Convert status filter to isPublished boolean
    let isPublished: boolean | undefined = undefined;
    if (filters.value.status === "published") {
      isPublished = true;
    } else if (filters.value.status === "draft") {
      isPublished = false;
    }
    
    const result = await adminService.getAllPosts({
      pageSize: 50, // Get more posts for admin view
      isPublished
    });
    
    posts.value = result.items;
    
    if (refreshing) {
      // Show success message for manual refresh
      console.log("Posts refreshed successfully");
    }
  } catch (err) {
    console.error("Failed to load posts:", err);
    error.value = "Failed to load posts. Using fallback data.";
    
    // Use mock data as fallback
    posts.value = [
      {
        id: 1,
        title: "Welcome to Our Blog",
        subtitle: "Welcome to our new blog! We're excited to share our thoughts...",
        text: "This is the content of our first blog post...",
        author: {
          id: 1,
          email: user.value?.email || "admin@example.com",
          username: user.value?.username || "Admin"
        },
        isPublished: true,
        publishedAt: "2025-11-01T10:00:00Z",
        createdAt: "2025-11-01T09:00:00Z",
        updatedAt: "2025-11-01T10:00:00Z"
      },
      {
        id: 2,
        title: "Draft Post",
        subtitle: "This post is still being worked on...",
        text: "This is a draft post that hasn't been published yet...",
        author: {
          id: 1,
          email: user.value?.email || "admin@example.com",
          username: user.value?.username || "Admin"
        },
        isPublished: false,
        createdAt: "2025-11-01T11:00:00Z",
        updatedAt: "2025-11-01T11:30:00Z"
      }
    ];
  } finally {
    isLoadingPosts.value = false;
  }
};

// Refresh all data
const refreshData = async () => {
  await Promise.all([
    loadStats(),
    loadPosts(true)
  ]);
};

// Action handlers
const createNewPost = () => {
  router.push("/admin/posts/new");
};

const editPost = (post: AdminPost) => {
  router.push(`/admin/posts/edit/${post.id}`);
};

const publishPost = async (post: AdminPost) => {
  try {
    console.log("Publishing post:", post.id);
    const updatedPost = await adminService.publishPost(post.id);
    
    // Update the post in the local array
    const index = posts.value.findIndex(p => p.id === post.id);
    if (index > -1) {
      posts.value[index] = updatedPost;
    }
    
    alert(`Post "${post.title}" published successfully!`);
    await loadStats(); // Refresh stats
  } catch (error) {
    console.error("Failed to publish post:", error);
    alert("Failed to publish post. Please try again.");
  }
};

const unpublishPost = async (post: AdminPost) => {
  try {
    console.log("Unpublishing post:", post.id);
    const updatedPost = await adminService.unpublishPost(post.id);
    
    // Update the post in the local array
    const index = posts.value.findIndex(p => p.id === post.id);
    if (index > -1) {
      posts.value[index] = updatedPost;
    }
    
    alert(`Post "${post.title}" unpublished successfully!`);
    await loadStats(); // Refresh stats
  } catch (error) {
    console.error("Failed to unpublish post:", error);
    alert("Failed to unpublish post. Please try again.");
  }
};

const deletePost = async (post: AdminPost) => {
  if (!confirm(`Are you sure you want to delete "${post.title}"? This action cannot be undone.`)) {
    return;
  }
  
  try {
    console.log("Deleting post:", post.id);
    await adminService.deletePost(post.id);
    
    // Remove the post from the local array
    const index = posts.value.findIndex(p => p.id === post.id);
    if (index > -1) {
      posts.value.splice(index, 1);
    }
    
    alert(`Post "${post.title}" deleted successfully!`);
    await loadStats(); // Refresh stats
  } catch (error) {
    console.error("Failed to delete post:", error);
    alert("Failed to delete post. Please try again.");
  }
};

// Lifecycle
onMounted(async () => {
  // Check admin permissions
  if (!requireAdmin()) {
    return;
  }
  
  await Promise.all([loadStats(), loadPosts()]);
});
</script>