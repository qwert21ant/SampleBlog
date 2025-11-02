<template>
  <div class="min-h-screen bg-gray-50 py-8">
    <div class="max-w-4xl mx-auto px-4 sm:px-6 lg:px-8">
      <!-- Header -->
      <div class="bg-white shadow rounded-lg mb-6">
        <div class="px-6 py-4 border-b border-gray-200">
          <div class="flex items-center justify-between">
            <div>
              <h1 class="text-2xl font-bold text-gray-900">
                {{ isEditing ? 'Edit Post' : 'Create New Post' }}
              </h1>
              <p class="text-sm text-gray-500 mt-1">
                {{ isEditing ? 'Update your existing post' : 'Write and publish a new blog post' }}
              </p>
            </div>
            <button
              class="inline-flex items-center px-4 py-2 border border-gray-300 rounded-md shadow-sm text-sm font-medium text-gray-700 bg-white hover:bg-gray-50 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-blue-500"
              @click="goBack"
            >
              <ArrowLeftIcon class="h-4 w-4 mr-2" />
              Back to Admin
            </button>
          </div>
        </div>
      </div>

      <!-- Loading State -->
      <div
        v-if="isLoading"
        class="bg-white shadow rounded-lg p-8 text-center"
      >
        <div class="animate-spin rounded-full h-8 w-8 border-b-2 border-blue-600 mx-auto mb-4" />
        <p class="text-gray-500">Loading post data...</p>
      </div>

      <!-- Load Error -->
      <div
        v-else-if="loadError"
        class="bg-red-50 border border-red-200 rounded-lg p-4 mb-6"
      >
        <div class="flex">
          <div class="flex-shrink-0">
            <svg
              class="h-5 w-5 text-red-400"
              viewBox="0 0 20 20"
              fill="currentColor"
            >
              <path
                fill-rule="evenodd"
                d="M10 18a8 8 0 100-16 8 8 0 000 16zM8.707 7.293a1 1 0 00-1.414 1.414L8.586 10l-1.293 1.293a1 1 0 101.414 1.414L10 11.414l1.293 1.293a1 1 0 001.414-1.414L11.414 10l1.293-1.293a1 1 0 00-1.414-1.414L10 8.586 8.707 7.293z"
                clip-rule="evenodd"
              />
            </svg>
          </div>
          <div class="ml-3">
            <p class="text-sm text-red-800">{{ loadError }}</p>
          </div>
        </div>
      </div>

      <!-- Form -->
      <form
        v-else
        class="space-y-6"
        @submit.prevent="handleSubmit"
      >
        <!-- Title -->
        <div class="bg-white shadow rounded-lg p-6">
          <label
            for="title"
            class="block text-sm font-medium text-gray-700 mb-2"
          >
            Title <span class="text-red-500">*</span>
          </label>
          <input
            id="title"
            v-model="form.title"
            type="text"
            required
            placeholder="Enter post title..."
            class="block w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-blue-500 focus:border-blue-500 text-lg"
          >
        </div>

        <!-- Subtitle -->
        <div class="bg-white shadow rounded-lg p-6">
          <label
            for="subtitle"
            class="block text-sm font-medium text-gray-700 mb-2"
          >
            Subtitle <span class="text-red-500">*</span>
          </label>
          <input
            id="subtitle"
            v-model="form.subtitle"
            type="text"
            required
            placeholder="Enter post subtitle..."
            class="block w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-blue-500 focus:border-blue-500"
          >
        </div>

        <!-- Images Slider -->
        <div class="bg-white shadow rounded-lg p-6">
          <label class="block text-sm font-medium text-gray-700 mb-4">
            Images
            <span class="text-gray-500 font-normal">(Optional - drag & drop or click to upload)</span>
          </label>
          
          <div class="relative">
            <!-- Image Slider Container -->
            <div
              ref="sliderContainer"
              class="flex overflow-x-auto space-x-4 pb-4"
            >
              <!-- Existing Images -->
              <div
                v-for="(image, index) in images"
                :key="`image-${index}`"
                class="flex-shrink-0 relative group"
              >
                <div class="w-64 h-48 bg-gray-100 rounded-lg border-2 border-gray-200 overflow-hidden">
                  <img
                    :src="image.preview"
                    :alt="`Image ${index + 1}`"
                    class="w-full h-full object-cover"
                  >
                </div>
                <!-- Remove Image Button -->
                <button
                  type="button"
                  class="absolute top-2 right-2 bg-red-600 text-white rounded-full p-1 opacity-0 group-hover:opacity-100 transition-opacity duration-200 hover:bg-red-700"
                  @click="removeImage(index)"
                >
                  <XMarkIcon class="h-4 w-4" />
                </button>
                <!-- Image Order -->
                <div class="absolute bottom-2 left-2 bg-black bg-opacity-50 text-white text-xs px-2 py-1 rounded">
                  {{ index + 1 }}
                </div>
              </div>

              <!-- Upload Area (Always Last) -->
              <div class="flex-shrink-0">
                <div
                  class="w-64 h-48 border-2 border-dashed border-gray-300 rounded-lg flex flex-col items-center justify-center cursor-pointer hover:border-blue-400 hover:bg-blue-50 transition-colors duration-200"
                  :class="{ 'border-blue-400 bg-blue-50': isDragging }"
                  @click="triggerFileInput"
                  @drop="handleDropEvent"
                  @dragover.prevent
                  @dragenter="handleDragEnter"
                  @dragleave="handleDragLeave"
                >
                  <CloudArrowUpIcon class="h-12 w-12 text-gray-400 mb-2" />
                  <p class="text-sm text-gray-500 text-center">
                    <span class="font-medium">Click to upload</span><br>
                    or drag and drop
                  </p>
                  <p class="text-xs text-gray-400 mt-1">PNG, JPG, GIF up to 5MB</p>
                </div>
              </div>
            </div>

            <!-- Hidden File Input -->
            <input
              ref="fileInput"
              type="file"
              accept="image/*"
              multiple
              class="hidden"
              @change="handleFileSelect"
            >
          </div>

          <!-- Images Count -->
          <div
            v-if="images.length > 0"
            class="mt-2 text-sm text-gray-500"
          >
            {{ images.length }} image{{ images.length !== 1 ? 's' : '' }} selected
          </div>
        </div>

        <!-- Content -->
        <div class="bg-white shadow rounded-lg p-6">
          <label
            for="content"
            class="block text-sm font-medium text-gray-700 mb-2"
          >
            Content
          </label>
          <textarea
            id="content"
            v-model="form.text"
            rows="20"
            placeholder="Write your post content here..."
            class="block w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-blue-500 focus:border-blue-500 resize-vertical"
          />
          <p class="mt-2 text-sm text-gray-500">
            Plain text content. You can format it later.
          </p>
        </div>

        <!-- Save Error -->
        <div
          v-if="saveError"
          class="bg-red-50 border border-red-200 rounded-lg p-4"
        >
          <div class="flex">
            <div class="flex-shrink-0">
              <svg
                class="h-5 w-5 text-red-400"
                viewBox="0 0 20 20"
                fill="currentColor"
              >
                <path
                  fill-rule="evenodd"
                  d="M10 18a8 8 0 100-16 8 8 0 000 16zM8.707 7.293a1 1 0 00-1.414 1.414L8.586 10l-1.293 1.293a1 1 0 101.414 1.414L10 11.414l1.293 1.293a1 1 0 001.414-1.414L11.414 10l1.293-1.293a1 1 0 00-1.414-1.414L10 8.586 8.707 7.293z"
                  clip-rule="evenodd"
                />
              </svg>
            </div>
            <div class="ml-3">
              <p class="text-sm text-red-800">{{ saveError }}</p>
            </div>
          </div>
        </div>

        <!-- Actions -->
        <div class="bg-white shadow rounded-lg p-6">
          <div class="flex items-center justify-between">
            <div class="flex space-x-3">
              <button
                type="button"
                :disabled="isSaving"
                class="inline-flex items-center px-4 py-2 border border-gray-300 rounded-md shadow-sm text-sm font-medium text-gray-700 bg-white hover:bg-gray-50 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-blue-500 disabled:opacity-50 disabled:cursor-not-allowed"
                @click="saveDraft"
              >
                <div
                  v-if="isSaving"
                  class="animate-spin rounded-full h-4 w-4 border-b-2 border-gray-600 mr-2"
                />
                <DocumentIcon
                  v-else
                  class="h-4 w-4 mr-2"
                />
                {{ isSaving ? 'Saving...' : 'Save as Draft' }}
              </button>
            </div>
            <div class="flex space-x-3">
              <button
                type="button"
                :disabled="isSaving"
                class="inline-flex items-center px-4 py-2 border border-gray-300 rounded-md shadow-sm text-sm font-medium text-gray-700 bg-white hover:bg-gray-50 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-blue-500 disabled:opacity-50 disabled:cursor-not-allowed"
                @click="goBack"
              >
                Cancel
              </button>
              <button
                type="submit"
                :disabled="isSaving"
                class="inline-flex items-center px-4 py-2 border border-transparent rounded-md shadow-sm text-sm font-medium text-white bg-blue-600 hover:bg-blue-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-blue-500 disabled:opacity-50 disabled:cursor-not-allowed"
              >
                <div
                  v-if="isSaving"
                  class="animate-spin rounded-full h-4 w-4 border-b-2 border-white mr-2"
                />
                <CheckIcon
                  v-else
                  class="h-4 w-4 mr-2"
                />
                {{ isSaving ? 'Publishing...' : (isEditing ? 'Update & Publish' : 'Create & Publish') }}
              </button>
            </div>
          </div>
        </div>
      </form>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted, onUnmounted } from "vue";
import { useRoute, useRouter } from "vue-router";
import {
  ArrowLeftIcon,
  CloudArrowUpIcon,
  XMarkIcon,
  DocumentIcon,
  CheckIcon
} from "@heroicons/vue/24/outline";
import { adminService, notificationService } from "@/services";
import type { AdminPost, CreatePostDto, UpdatePostDto } from "@/types";

// Types
interface ImageFile {
  file: File;
  preview: string;
  isExisting?: boolean;
  imageId?: number;
}

interface PostForm {
  title: string;
  subtitle: string;
  text: string;
  isPublished: boolean;
}

// Router
const route = useRoute();
const router = useRouter();

// Reactive data
const form = ref<PostForm>({
  title: "",
  subtitle: "",
  text: "",
  isPublished: false
});

const images = ref<ImageFile[]>([]);
const isDragging = ref(false);
const fileInput = ref<HTMLInputElement>();
const sliderContainer = ref<HTMLElement>();

// Loading and error states
const isLoading = ref(false);
const isSaving = ref(false);
const loadError = ref<string | null>(null);
const saveError = ref<string | null>(null);

// Computed
const isEditing = computed(() => route.params.id !== undefined);

// Methods
const goBack = () => {
  router.push("/admin");
};

const triggerFileInput = () => {
  fileInput.value?.click();
};

const handleFileSelect = (event: Event) => {
  const target = event.target as HTMLInputElement;
  if (target.files) {
    addImages(Array.from(target.files));
  }
};

const handleDrop = (event: DragEvent) => {
  event.preventDefault();
  if (event.dataTransfer?.files) {
    addImages(Array.from(event.dataTransfer.files));
  }
};

const handleDropEvent = (event: DragEvent) => {
  event.preventDefault();
  isDragging.value = false;
  if (event.dataTransfer?.files) {
    addImages(Array.from(event.dataTransfer.files));
  }
};

const handleDragEnter = (event: DragEvent) => {
  event.preventDefault();
  isDragging.value = true;
};

const handleDragLeave = (event: DragEvent) => {
  event.preventDefault();
  isDragging.value = false;
};

const addImages = (files: File[]) => {
  files.forEach(file => {
    if (file.type.startsWith("image/")) {
      const reader = new FileReader();
      reader.onload = (e) => {
        images.value.push({
          file,
          preview: e.target?.result as string
        });
      };
      reader.readAsDataURL(file);
    }
  });
};

const removeImage = async (index: number) => {
  const imageToRemove = images.value[index];
  
  // If it's an existing image (has imageId), delete it from server
  if (imageToRemove.isExisting && imageToRemove.imageId) {
    try {
      await adminService.deleteImage(imageToRemove.imageId);
      notificationService.success("Image Deleted", "Image has been removed from the post.");
    } catch (error) {
      console.error("Failed to delete image:", error);
      notificationService.error("Delete Failed", "Failed to delete image from server.");
      return; // Don't remove from UI if server deletion failed
    }
  }
  
  // Clean up blob URL if it's an existing image
  if (imageToRemove.isExisting && imageToRemove.preview.startsWith('blob:')) {
    URL.revokeObjectURL(imageToRemove.preview);
  }
  
  // Remove from UI
  images.value.splice(index, 1);
};

const saveNewImages = async (postId: number) => {
  const newImages = images.value.filter(img => !img.isExisting);
    if (newImages.length > 0) {
      for (const imageData of newImages) {
        try {
          await adminService.uploadImageForPost(postId, imageData.file, `Image ${images.value.indexOf(imageData) + 1}`);
        } catch (imageError) {
          console.error("Failed to upload image:", imageError);
          // Continue with other images but show warning
          notificationService.warning("Image Upload", "Some images failed to upload but post was saved.");
        }
      }
    }
};

const handleSubmit = async () => {
  if (!form.value.title.trim() || !form.value.subtitle.trim()) {
    saveError.value = "Please fill in all required fields (title and subtitle)";
    return;
  }

  isSaving.value = true;
  saveError.value = null;

  try {
    let postId: number;

    if (isEditing.value) {
      // Update existing post (keep current publish state or publish if saving)
      postId = Number(route.params.id);
      const updateData: UpdatePostDto = {
        title: form.value.title.trim(),
        subtitle: form.value.subtitle.trim(),
        text: form.value.text.trim(),
        isPublished: true // Publish when using main submit
      };

      await adminService.updatePost(postId, updateData);
    } else {
      // Create new post as published
      const createData: CreatePostDto = {
        title: form.value.title.trim(),
        subtitle: form.value.subtitle.trim(),
        text: form.value.text.trim(),
        isPublished: true // Publish when using main submit
      };

      const newPost = await adminService.createPost(createData);
      postId = newPost.id;
    }

    await saveNewImages(postId);
    
    // Show success message and navigate back
    const action = isEditing.value ? "updated" : "created";
    notificationService.success("Post Saved", `Post has been ${action} and published successfully!`);
    router.push("/admin");
  } catch (error: any) {
    console.error("Failed to save post:", error);
    const errorMessage = error.message || "Failed to save post. Please try again.";
    saveError.value = errorMessage;
    notificationService.error("Save Failed", errorMessage);
  } finally {
    isSaving.value = false;
  }
};

const saveDraft = async () => {
  if (!form.value.title.trim() || !form.value.subtitle.trim()) {
    saveError.value = "Please fill in at least the title and subtitle to save as draft";
    return;
  }

  isSaving.value = true;
  saveError.value = null;

  try {
    let postId: number;

    if (isEditing.value) {
      // Update existing post as draft
      postId = Number(route.params.id);
      const updateData: UpdatePostDto = {
        title: form.value.title.trim(),
        subtitle: form.value.subtitle.trim(),
        text: form.value.text.trim(),
        isPublished: false
      };

      await adminService.updatePost(postId, updateData);
      form.value.isPublished = false; // Update local state
    } else {
      // Create new post as draft
      const createData: CreatePostDto = {
        title: form.value.title.trim(),
        subtitle: form.value.subtitle.trim(),
        text: form.value.text.trim(),
        isPublished: false
      };

      const newPost = await adminService.createPost(createData);
      postId = newPost.id;
      
      await saveNewImages(postId);

      // Navigate to edit mode for the new draft
      router.push(`/admin/posts/edit/${postId}`);
      return;
    }

    // Upload any new images for existing draft
    if (images.value.length > 0) {
      for (const imageData of images.value) {
        try {
          await adminService.uploadImageForPost(postId, imageData.file, `Image ${images.value.indexOf(imageData) + 1}`);
        } catch (imageError) {
          console.error("Failed to upload image:", imageError);
          notificationService.warning("Image Upload", "Draft saved but some images failed to upload.");
        }
      }
    }

    notificationService.success("Draft Saved", "Post has been saved as draft successfully!");
  } catch (error: any) {
    console.error("Failed to save draft:", error);
    const errorMessage = error.message || "Failed to save draft. Please try again.";
    saveError.value = errorMessage;
    notificationService.error("Save Draft Failed", errorMessage);
  } finally {
    isSaving.value = false;
  }
};

// Load post data if editing
onMounted(async () => {
  if (isEditing.value) {
    const postId = Number(route.params.id);
    
    if (isNaN(postId)) {
      loadError.value = "Invalid post ID";
      return;
    }

    isLoading.value = true;
    loadError.value = null;
    
    try {
      const post = await adminService.getPostById(postId);
      
      form.value = {
        title: post.title || "",
        subtitle: post.subtitle || "",
        text: post.text || "",
        isPublished: post.isPublished || false
      };

      // Load existing images
      try {
        const existingImages = await adminService.getImagesByPost(postId);
        
        // Convert server images to preview format for display
        // We need to fetch the actual image data with auth headers to create blob URLs
        for (const img of existingImages) {
          try {
            const blob = await adminService.fetchImageBlob(img.id);
            const blobUrl = URL.createObjectURL(blob);
            
            images.value.push({
              file: new File([], img.fileName), // Placeholder file - for existing images
              preview: blobUrl,
              isExisting: true,
              imageId: img.id
            });
          } catch (fetchError) {
            console.error(`Failed to fetch image ${img.id}:`, fetchError);
            // Skip this image but continue with others
          }
        }
      } catch (imageError) {
        console.error("Failed to load images:", imageError);
        // Don't show error for images, just log it
      }
      
      console.log("Loaded post for editing:", post);
    } catch (error: any) {
      console.error("Failed to load post:", error);
      loadError.value = error.message || "Failed to load post data";
    } finally {
      isLoading.value = false;
    }
  }
});

// Cleanup blob URLs on component unmount to prevent memory leaks
onUnmounted(() => {
  images.value.forEach(img => {
    if (img.isExisting && img.preview.startsWith('blob:')) {
      URL.revokeObjectURL(img.preview);
    }
  });
});
</script>