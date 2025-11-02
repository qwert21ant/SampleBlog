<template>
  <!-- Modal Overlay -->
  <div
    v-if="isOpen"
    class="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50 p-4"
    @click="closeDialog"
  >
    <!-- Modal Content -->
    <div
      class="bg-white rounded-lg shadow-xl w-full max-w-md transform transition-all"
      @click.stop
    >
      <!-- Modal Header -->
      <div class="px-6 py-4 border-b border-gray-200">
        <div class="flex items-center justify-between">
          <h2 class="text-xl font-semibold text-gray-900">
            {{ mode === 'login' ? $t('auth.signIn') : $t('auth.createAccount') }}
          </h2>
          <button
            class="text-gray-400 hover:text-gray-600 transition-colors"
            @click="closeDialog"
          >
            <XMarkIcon class="h-6 w-6" />
          </button>
        </div>
      </div>

      <!-- Modal Body -->
      <form
        class="px-6 py-4"
        @submit.prevent="handleSubmit"
      >
        <!-- Error Message -->
        <div
          v-if="error"
          class="mb-4 p-3 bg-red-50 border border-red-200 rounded-lg"
        >
          <div class="flex items-center">
            <ExclamationCircleIcon class="h-5 w-5 text-red-400 mr-2" />
            <p class="text-sm text-red-800">{{ error }}</p>
          </div>
        </div>

        <!-- Registration Fields -->
        <div
          v-if="mode === 'register'"
          class="space-y-4 mb-4"
        >
          <div>
            <label
              for="username"
              class="block text-sm font-medium text-gray-700 mb-1"
            >
              {{ $t('auth.username') }}
            </label>
            <input
              id="username"
              v-model="form.username"
              type="text"
              required
              class="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500 transition-colors"
              :class="{ 'border-red-500': errors.username }"
              placeholder="johndoe"
            >
            <p
              v-if="errors.username"
              class="mt-1 text-sm text-red-600"
            >
              {{ errors.username }}
            </p>
          </div>
        </div>

        <!-- Common Fields -->
        <div class="space-y-4">
          <div>
            <label
              for="email"
              class="block text-sm font-medium text-gray-700 mb-1"
            >
              {{ $t('auth.email') }}
            </label>
            <input
              id="email"
              v-model="form.email"
              type="email"
              required
              class="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500 transition-colors"
              :class="{ 'border-red-500': errors.email }"
              placeholder="john@example.com"
            >
            <p
              v-if="errors.email"
              class="mt-1 text-sm text-red-600"
            >
              {{ errors.email }}
            </p>
          </div>

          <div>
            <label
              for="password"
              class="block text-sm font-medium text-gray-700 mb-1"
            >
              {{ $t('auth.password') }}
            </label>
            <div class="relative">
              <input
                id="password"
                v-model="form.password"
                :type="showPassword ? 'text' : 'password'"
                required
                class="w-full px-3 py-2 pr-10 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500 transition-colors"
                :class="{ 'border-red-500': errors.password }"
                placeholder="••••••••"
              >
              <button
                type="button"
                class="absolute inset-y-0 right-0 pr-3 flex items-center"
                @click="showPassword = !showPassword"
              >
                <EyeIcon
                  v-if="!showPassword"
                  class="h-5 w-5 text-gray-400"
                />
                <EyeSlashIcon
                  v-else
                  class="h-5 w-5 text-gray-400"
                />
              </button>
            </div>
            <p
              v-if="errors.password"
              class="mt-1 text-sm text-red-600"
            >
              {{ errors.password }}
            </p>
          </div>

          <!-- Confirm Password for Registration -->
          <div v-if="mode === 'register'">
            <label
              for="confirmPassword"
              class="block text-sm font-medium text-gray-700 mb-1"
            >
              {{ $t('auth.confirmPassword') }}
            </label>
            <input
              id="confirmPassword"
              v-model="form.confirmPassword"
              type="password"
              required
              class="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500 transition-colors"
              :class="{ 'border-red-500': errors.confirmPassword }"
              placeholder="••••••••"
            >
            <p
              v-if="errors.confirmPassword"
              class="mt-1 text-sm text-red-600"
            >
              {{ errors.confirmPassword }}
            </p>
          </div>
        </div>
      </form>

      <!-- Modal Footer -->
      <div class="px-6 py-4 border-t border-gray-200">
        <div class="flex flex-col space-y-3">
          <!-- Submit Button -->
          <button
            :disabled="isLoading || !isFormValid"
            class="w-full bg-blue-600 text-white py-2 px-4 rounded-lg font-medium hover:bg-blue-700 focus:ring-2 focus:ring-blue-500 focus:ring-offset-2 transition-colors disabled:opacity-50 disabled:cursor-not-allowed flex items-center justify-center"
            @click="handleSubmit"
          >
            <div
              v-if="isLoading"
              class="animate-spin rounded-full h-4 w-4 border-b-2 border-white mr-2"
            />
            {{ mode === 'login' ? $t('auth.signIn') : $t('auth.createAccount') }}
          </button>

          <!-- Mode Switch -->
          <div class="text-center">
            <span class="text-sm text-gray-600">
              {{ mode === 'login' ? $t('auth.noAccount') : $t('auth.haveAccount') }}
            </span>
            <button
              type="button"
              class="text-sm text-blue-600 hover:text-blue-700 font-medium ml-1"
              @click="switchMode"
            >
              {{ mode === 'login' ? $t('auth.signUp') : $t('auth.signIn') }}
            </button>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, watch } from "vue";
import { 
  XMarkIcon, 
  ExclamationCircleIcon, 
  EyeIcon, 
  EyeSlashIcon 
} from "@heroicons/vue/24/outline";
import { useAuth } from "@/composables/useAuth";
import type { LoginCredentials, RegisterCredentials, ValidationErrors } from "@/types";

// Props
interface Props {
  isOpen: boolean
  initialMode?: "login" | "register"
}

const props = withDefaults(defineProps<Props>(), {
  initialMode: "login"
});

// Emits
const emit = defineEmits<{
  close: []
  success: []
}>();

// Composables
const { login, register, isLoading, error, clearError, validateLoginCredentials, validateRegisterCredentials } = useAuth();

// Reactive state
const mode = ref<"login" | "register">(props.initialMode);
const showPassword = ref(false);
const errors = ref<ValidationErrors>({});

const form = ref<RegisterCredentials>({
  email: "",
  password: "",
  confirmPassword: "",
  username: ""
});

// Computed
const isFormValid = computed(() => {
  if (mode.value === "login") {
    return form.value.email && form.value.password;
  } else {
    return form.value.email && form.value.password && form.value.confirmPassword && form.value.username;
  }
});

// Methods
const closeDialog = () => {
  clearError();
  resetForm();
  emit("close");
};

const resetForm = () => {
  form.value = {
    email: "",
    password: "",
    confirmPassword: "",
    username: ""
  };
  errors.value = {};
  showPassword.value = false;
};

const switchMode = () => {
  mode.value = mode.value === "login" ? "register" : "login";
  clearError();
  errors.value = {};
};

const handleSubmit = async () => {
  clearError();
  errors.value = {};

  let success = false;

  if (mode.value === "login") {
    const credentials: LoginCredentials = {
      email: form.value.email,
      password: form.value.password
    };
    
    const validationErrors = validateLoginCredentials(credentials);
    if (Object.keys(validationErrors).length > 0) {
      errors.value = validationErrors;
      return;
    }

    success = await login(credentials);
  } else {
    const validationErrors = validateRegisterCredentials(form.value);
    if (Object.keys(validationErrors).length > 0) {
      errors.value = validationErrors;
      return;
    }

    success = await register(form.value);
  }

  if (success) {
    resetForm();
    emit("success");
    emit("close");
  }
};

// Watchers
watch(() => props.isOpen, (isOpen) => {
  if (isOpen) {
    mode.value = props.initialMode;
    clearError();
  } else {
    resetForm();
  }
});

watch(() => props.initialMode, (newMode) => {
  mode.value = newMode;
});

// Clear error when form changes
watch(form, () => {
  if (error.value) {
    clearError();
  }
  errors.value = {};
}, { deep: true });
</script>