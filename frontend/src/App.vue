<template>
  <div
    id="app"
    class="min-h-screen flex flex-col bg-gray-50"
  >
    <header class="bg-slate-800 text-white shadow-lg">
      <nav class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <div class="flex justify-between items-center h-16">
          <div class="flex items-center space-x-2">
            <NewspaperIcon class="h-8 w-8 text-primary-400" />
            <h1 class="text-xl font-bold">SampleBlog</h1>
          </div>
          <div class="flex items-center space-x-4">
            <!-- Navigation Links -->
            <div class="hidden md:flex space-x-6">
              <router-link
                to="/"
                class="flex items-center px-3 py-2 rounded-md text-sm font-medium transition-colors hover:bg-slate-700 hover:text-white"
                :class="{ 'bg-slate-900 text-white': $route.path === '/' }"
              >
                <HomeIcon class="h-4 w-4 mr-1" />
                Home
              </router-link>
              <router-link
                to="/about"
                class="flex items-center px-3 py-2 rounded-md text-sm font-medium transition-colors hover:bg-slate-700 hover:text-white"
                :class="{ 'bg-slate-900 text-white': $route.path === '/about' }"
              >
                <InformationCircleIcon class="h-4 w-4 mr-1" />
                About
              </router-link>
            </div>

            <!-- Authentication Section -->
            <div
              v-if="!isAuthenticated"
              class="flex items-center space-x-3"
            >
              <button
                class="text-sm font-medium text-slate-300 hover:text-white transition-colors"
                @click="openAuthDialog('login')"
              >
                Sign In
              </button>
              <button
                class="bg-blue-600 hover:bg-blue-700 text-white px-4 py-2 rounded-lg text-sm font-medium transition-colors"
                @click="openAuthDialog('register')"
              >
                Sign Up
              </button>
            </div>

            <!-- User Menu (when authenticated) -->
            <div
              v-else
              class="relative"
            >
              <button
                class="flex items-center space-x-2 px-3 py-2 rounded-md text-sm font-medium hover:bg-slate-700 transition-colors"
                @click="toggleUserMenu"
              >
                <UserCircleIcon class="h-6 w-6" />
                <span class="hidden sm:block">{{ user?.username || user?.email }}</span>
                <ChevronDownIcon class="h-4 w-4" />
              </button>

              <!-- Dropdown Menu -->
              <div
                v-show="showUserMenu"
                class="absolute right-0 mt-2 w-56 bg-white rounded-lg shadow-lg border border-gray-200 py-1 z-50"
              >
                <div class="px-4 py-2 border-b border-gray-200">
                  <p class="text-sm font-medium text-gray-900">{{ user?.username || 'User' }}</p>
                  <p class="text-sm text-gray-500">{{ user?.email }}</p>
                </div>
                
                <router-link
                  to="/admin"
                  class="flex items-center px-4 py-2 text-sm text-gray-700 hover:bg-gray-100"
                  @click="closeUserMenu"
                >
                  <CogIcon class="h-4 w-4 mr-2" />
                  Admin Panel
                </router-link>
                
                <button
                  class="w-full flex items-center px-4 py-2 text-sm text-red-600 hover:bg-red-50"
                  @click="handleLogout"
                >
                  <ArrowRightOnRectangleIcon class="h-4 w-4 mr-2" />
                  Sign Out
                </button>
              </div>
            </div>
          </div>
        </div>
      </nav>
    </header>

    <main class="flex-1 max-w-7xl mx-auto w-full px-4 sm:px-6 lg:px-8 py-8">
      <router-view />
    </main>

    <footer class="bg-slate-700 text-white">
      <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-4">
        <p class="text-center text-sm text-slate-300">
          &copy; 2025 SampleBlog. Built with Vue 3 + TypeScript + Tailwind CSS
        </p>
      </div>
    </footer>

    <!-- Global Components -->
    <AuthDialog
      :is-open="authDialog.isOpen"
      :initial-mode="authDialog.mode"
      @close="closeAuthDialog"
      @success="handleAuthSuccess"
    />
    
    <NotificationContainer />
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, onUnmounted } from "vue";
import { 
  NewspaperIcon, 
  HomeIcon, 
  InformationCircleIcon,
  UserCircleIcon,
  ChevronDownIcon,
  CogIcon,
  ArrowRightOnRectangleIcon
} from "@heroicons/vue/24/outline";
import AuthDialog from "@/components/AuthDialog.vue";
import NotificationContainer from "@/components/NotificationContainer.vue";
import { useAuth } from "@/composables/useAuth";

// Composables
const { isAuthenticated, user, logout, initializeAuth } = useAuth();

// Reactive state
const showUserMenu = ref(false);
const authDialog = ref({
  isOpen: false,
  mode: "login" as "login" | "register"
});

// Methods
const openAuthDialog = (mode: "login" | "register") => {
  authDialog.value.mode = mode;
  authDialog.value.isOpen = true;
};

const closeAuthDialog = () => {
  authDialog.value.isOpen = false;
};

const handleAuthSuccess = () => {
  console.log("Authentication successful");
};

const toggleUserMenu = () => {
  showUserMenu.value = !showUserMenu.value;
};

const closeUserMenu = () => {
  showUserMenu.value = false;
};

const handleLogout = async () => {
  closeUserMenu();
  await logout();
};

// Lifecycle
onMounted(() => {
  initializeAuth();
});
</script>