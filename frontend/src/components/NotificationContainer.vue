<template>
  <Teleport to="body">
    <div 
      class="fixed bottom-4 right-4 z-50 space-y-2"
      role="region"
      aria-label="Notifications"
    >
      <TransitionGroup
        name="notification"
        tag="div"
        class="space-y-2"
      >
        <div
          v-for="notification in notifications.slice().reverse()"
          :key="notification.id"
          class="flex items-start p-4 rounded-lg shadow-lg border max-w-sm"
          :class="getNotificationClasses(notification.type)"
          role="alert"
          :aria-live="notification.type === 'error' ? 'assertive' : 'polite'"
        >
          <!-- Icon -->
          <div class="flex-shrink-0 mr-3">
            <CheckCircleIcon
              v-if="notification.type === 'success'"
              class="h-5 w-5 text-green-500"
            />
            <ExclamationTriangleIcon
              v-else-if="notification.type === 'warning'"
              class="h-5 w-5 text-yellow-500"
            />
            <XCircleIcon
              v-else-if="notification.type === 'error'"
              class="h-5 w-5 text-red-500"
            />
            <InformationCircleIcon
              v-else
              class="h-5 w-5 text-blue-500"
            />
          </div>

          <!-- Content -->
          <div class="flex-1 min-w-0">
            <h4 class="text-sm font-medium text-gray-900 mb-1">
              {{ notification.title }}
            </h4>
            <p 
              v-if="notification.message"
              class="text-sm text-gray-600"
            >
              {{ notification.message }}
            </p>
          </div>

          <!-- Close button -->
          <button
            class="flex-shrink-0 ml-3 text-gray-400 hover:text-gray-600 transition-colors"
            @click="removeNotification(notification.id)"
            :aria-label="'Dismiss notification: ' + notification.title"
          >
            <XMarkIcon class="h-4 w-4" />
          </button>
        </div>
      </TransitionGroup>
    </div>
  </Teleport>
</template>

<script setup lang="ts">
import { computed } from 'vue'
import {
  CheckCircleIcon,
  ExclamationTriangleIcon,
  XCircleIcon,
  InformationCircleIcon,
  XMarkIcon
} from '@heroicons/vue/24/outline'
import { notificationService, type Notification } from '@/services/notificationService'

// Reactive notifications from service
const notifications = computed(() => notificationService.items)

// Methods
const removeNotification = (id: string) => {
  notificationService.remove(id)
}

const getNotificationClasses = (type: Notification['type']) => {
  const baseClasses = 'bg-white border-l-4'
  
  switch (type) {
    case 'success':
      return `${baseClasses} border-l-green-500 shadow-green-100`
    case 'warning':
      return `${baseClasses} border-l-yellow-500 shadow-yellow-100`
    case 'error':
      return `${baseClasses} border-l-red-500 shadow-red-100`
    case 'info':
    default:
      return `${baseClasses} border-l-blue-500 shadow-blue-100`
  }
}
</script>

<style scoped>
/* Transition animations */
.notification-enter-active {
  transition: all 0.3s ease-out;
}

.notification-leave-active {
  transition: all 0.2s ease-in;
}

.notification-enter-from {
  transform: translateX(100%) translateY(20px);
  opacity: 0;
}

.notification-leave-to {
  transform: translateX(100%) translateY(-20px);
  opacity: 0;
}

.notification-move {
  transition: transform 0.3s ease;
}
</style>