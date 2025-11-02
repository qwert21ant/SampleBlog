import { ref } from "vue";

export interface Notification {
  id: string
  type: "success" | "error" | "warning" | "info"
  title: string
  message?: string
  duration?: number
  persistent?: boolean
}

export interface NotificationOptions {
  type?: Notification["type"]
  title: string
  message?: string
  duration?: number
  persistent?: boolean
}

class NotificationService {
  private notifications = ref<Notification[]>([]);
  private nextId = 1;

  // Reactive state for components to access
  get items() {
    return this.notifications.value;
  }

  // Add a notification
  add(options: NotificationOptions): string {
    const id = `notification-${this.nextId++}`;
    const notification: Notification = {
      id,
      type: options.type || "info",
      title: options.title,
      message: options.message,
      duration: options.duration || 5000,
      persistent: options.persistent || false
    };

    this.notifications.value.push(notification);

    // Auto-remove after duration (unless persistent)
    if (!notification.persistent) {
      setTimeout(() => {
        this.remove(id);
      }, notification.duration);
    }

    return id;
  }

  // Remove a notification
  remove(id: string): void {
    const index = this.notifications.value.findIndex(n => n.id === id);
    if (index > -1) {
      this.notifications.value.splice(index, 1);
    }
  }

  // Clear all notifications
  clear(): void {
    this.notifications.value = [];
  }

  // Convenience methods for different types
  success(title: string, message?: string, options?: Partial<NotificationOptions>): string {
    return this.add({
      type: "success",
      title,
      message,
      ...options
    });
  }

  error(title: string, message?: string, options?: Partial<NotificationOptions>): string {
    return this.add({
      type: "error",
      title,
      message,
      duration: 8000, // Errors stay longer
      ...options
    });
  }

  warning(title: string, message?: string, options?: Partial<NotificationOptions>): string {
    return this.add({
      type: "warning",
      title,
      message,
      ...options
    });
  }

  info(title: string, message?: string, options?: Partial<NotificationOptions>): string {
    return this.add({
      type: "info",
      title,
      message,
      ...options
    });
  }
}

// Export singleton instance
export const notificationService = new NotificationService();
export default notificationService;