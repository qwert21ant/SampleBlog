<template>
  <Teleport to="body">
    <Transition
      name="modal"
      appear
    >
      <div
        v-if="isOpen"
        class="fixed inset-0 z-50 flex items-center justify-center p-4"
        role="dialog"
        aria-modal="true"
        :aria-labelledby="titleId"
        :aria-describedby="messageId"
      >
        <!-- Backdrop -->
        <div 
          class="absolute inset-0 bg-black bg-opacity-50 transition-opacity"
          @click="handleCancel"
        />

        <!-- Dialog -->
        <div class="relative bg-white rounded-lg shadow-xl max-w-md w-full p-6">
          <!-- Icon -->
          <div class="flex items-center justify-center w-12 h-12 mx-auto mb-4 bg-red-100 rounded-full">
            <ExclamationTriangleIcon class="w-6 h-6 text-red-600" />
          </div>

          <!-- Content -->
          <div class="text-center mb-6">
            <h3 
              :id="titleId"
              class="text-lg font-semibold text-gray-900 mb-2"
            >
              {{ displayTitle }}
            </h3>
            <p 
              :id="messageId"
              class="text-gray-600"
            >
              {{ displayMessage }}
            </p>
          </div>

          <!-- Actions -->
          <div class="flex space-x-3">
            <button
              class="flex-1 px-4 py-2 bg-gray-100 text-gray-700 rounded-lg hover:bg-gray-200 transition-colors font-medium"
              :disabled="loading"
              @click="handleCancel"
            >
              {{ displayCancelText }}
            </button>
            <button
              class="flex-1 px-4 py-2 bg-red-600 text-white rounded-lg hover:bg-red-700 disabled:opacity-50 disabled:cursor-not-allowed transition-colors font-medium"
              :disabled="loading"
              @click="handleConfirm"
            >
              <span v-if="!loading">{{ displayConfirmText }}</span>
              <span 
                v-else
                class="flex items-center justify-center"
              >
                <div class="animate-spin rounded-full h-4 w-4 border-2 border-white border-t-transparent mr-2" />
                {{ displayLoadingText }}
              </span>
            </button>
          </div>
        </div>
      </div>
    </Transition>
  </Teleport>
</template>

<script setup lang="ts">
import { computed } from "vue";
import { useI18n } from "vue-i18n";
import { ExclamationTriangleIcon } from "@heroicons/vue/24/outline";

// Props
interface Props {
  isOpen: boolean
  title?: string
  message?: string
  confirmText?: string
  cancelText?: string
  loadingText?: string
  loading?: boolean
}

const { t } = useI18n();

const props = withDefaults(defineProps<Props>(), {
  loading: false
});

// Computed properties for translated defaults
const displayTitle = computed(() => props.title || t("common.confirmAction"));
const displayMessage = computed(() => props.message || t("common.confirmMessage"));
const displayConfirmText = computed(() => props.confirmText || t("common.confirm"));
const displayCancelText = computed(() => props.cancelText || t("common.cancel"));
const displayLoadingText = computed(() => props.loadingText || t("common.processing"));

// Emits
const emit = defineEmits<{
  confirm: []
  cancel: []
  close: []
}>();

// Computed
const titleId = computed(() => `confirm-title-${Math.random().toString(36).substr(2, 9)}`);
const messageId = computed(() => `confirm-message-${Math.random().toString(36).substr(2, 9)}`);

// Methods
const handleConfirm = () => {
  if (!props.loading) {
    emit("confirm");
  }
};

const handleCancel = () => {
  if (!props.loading) {
    emit("cancel");
    emit("close");
  }
};

// Handle escape key
const handleKeydown = (event: KeyboardEvent) => {
  if (event.key === "Escape" && !props.loading) {
    handleCancel();
  }
};

// Add/remove event listeners when dialog opens/closes
watch(() => props.isOpen, (isOpen) => {
  if (isOpen) {
    document.addEventListener("keydown", handleKeydown);
    document.body.style.overflow = "hidden";
  } else {
    document.removeEventListener("keydown", handleKeydown);
    document.body.style.overflow = "";
  }
}, { immediate: true });

// Cleanup on unmount
onUnmounted(() => {
  document.removeEventListener("keydown", handleKeydown);
  document.body.style.overflow = "";
});
</script>

<script lang="ts">
import { watch, onUnmounted } from "vue";

export default {
  name: "ConfirmDialog"
};
</script>

<style scoped>
/* Modal transition animations */
.modal-enter-active {
  transition: opacity 0.2s ease-out;
}

.modal-leave-active {
  transition: opacity 0.15s ease-in;
}

.modal-enter-from,
.modal-leave-to {
  opacity: 0;
}

.modal-enter-active .relative {
  transition: all 0.2s ease-out;
  transition-delay: 0.05s;
}

.modal-leave-active .relative {
  transition: all 0.15s ease-in;
}

.modal-enter-from .relative {
  transform: scale(0.9) translateY(-10px);
  opacity: 0;
}

.modal-leave-to .relative {
  transform: scale(0.9) translateY(-10px);
  opacity: 0;
}
</style>