<template>
  <div class="max-w-6xl mx-auto">
    <div class="space-y-8">
      <h1 class="text-center text-slate-800 text-4xl md:text-5xl font-bold mb-8">
        About SampleBlog
      </h1>
      
      <div class="bg-white rounded-lg p-6 md:p-8 shadow-lg border border-gray-200">
        <h2 class="text-slate-800 text-2xl font-semibold mb-6 pb-2 border-b-2 border-blue-500 flex items-center space-x-2">
          <CodeBracketIcon class="h-6 w-6 text-blue-500" />
          <span>Frontend Technology Stack</span>
        </h2>
        <div class="grid grid-cols-1 md:grid-cols-2 gap-6">
          <div class="bg-gray-50 p-6 rounded-lg border-l-4 border-blue-500">
            <h3 class="text-slate-800 text-xl font-medium mb-2 flex items-center space-x-2">
              <span class="text-2xl">⚡</span>
              <span>Vue 3</span>
            </h3>
            <p class="text-slate-600 leading-relaxed">Progressive JavaScript framework with Composition API</p>
          </div>
          <div class="bg-gray-50 p-6 rounded-lg border-l-4 border-blue-500">
            <h3 class="text-slate-800 text-xl font-medium mb-2 flex items-center space-x-2">
              <span class="text-2xl">🔷</span>
              <span>TypeScript</span>
            </h3>
            <p class="text-slate-600 leading-relaxed">Typed superset of JavaScript for better development experience</p>
          </div>
          <div class="bg-gray-50 p-6 rounded-lg border-l-4 border-blue-500">
            <h3 class="text-slate-800 text-xl font-medium mb-2 flex items-center space-x-2">
              <span class="text-2xl">⚡</span>
              <span>Vite</span>
            </h3>
            <p class="text-slate-600 leading-relaxed">Fast build tool and development server</p>
          </div>
          <div class="bg-gray-50 p-6 rounded-lg border-l-4 border-blue-500">
            <h3 class="text-slate-800 text-xl font-medium mb-2 flex items-center space-x-2">
              <span class="text-2xl">🛣️</span>
              <span>Vue Router</span>
            </h3>
            <p class="text-slate-600 leading-relaxed">Official router for Vue.js applications</p>
          </div>
        </div>
      </div>

      <div class="bg-white rounded-lg p-6 md:p-8 shadow-lg border border-gray-200">
        <h2 class="text-slate-800 text-2xl font-semibold mb-6 pb-2 border-b-2 border-green-500 flex items-center space-x-2">
          <ServerIcon class="h-6 w-6 text-green-500" />
          <span>Backend Technology Stack</span>
        </h2>
        <div class="grid grid-cols-1 md:grid-cols-2 gap-6">
          <div class="bg-gray-50 p-6 rounded-lg border-l-4 border-green-500">
            <h3 class="text-slate-800 text-xl font-medium mb-2 flex items-center space-x-2">
              <span class="text-2xl">🏗️</span>
              <span>ASP.NET Core 9.0</span>
            </h3>
            <p class="text-slate-600 leading-relaxed">Modern web API framework</p>
          </div>
          <div class="bg-gray-50 p-6 rounded-lg border-l-4 border-green-500">
            <h3 class="text-slate-800 text-xl font-medium mb-2 flex items-center space-x-2">
              <span class="text-2xl">🐘</span>
              <span>PostgreSQL</span>
            </h3>
            <p class="text-slate-600 leading-relaxed">Powerful, open source object-relational database system</p>
          </div>
          <div class="bg-gray-50 p-6 rounded-lg border-l-4 border-green-500">
            <h3 class="text-slate-800 text-xl font-medium mb-2 flex items-center space-x-2">
              <span class="text-2xl">🔗</span>
              <span>Entity Framework Core</span>
            </h3>
            <p class="text-slate-600 leading-relaxed">Object-relational mapper (ORM) for .NET</p>
          </div>
          <div class="bg-gray-50 p-6 rounded-lg border-l-4 border-green-500">
            <h3 class="text-slate-800 text-xl font-medium mb-2 flex items-center space-x-2">
              <span class="text-2xl">🔐</span>
              <span>JWT Authentication</span>
            </h3>
            <p class="text-slate-600 leading-relaxed">Secure token-based authentication</p>
          </div>
        </div>
      </div>

      <div class="bg-white rounded-lg p-6 md:p-8 shadow-lg border border-gray-200">
        <h2 class="text-slate-800 text-2xl font-semibold mb-6 pb-2 border-b-2 border-purple-500 flex items-center space-x-2">
          <SparklesIcon class="h-6 w-6 text-purple-500" />
          <span>Project Features</span>
        </h2>
        <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
          <div class="bg-gray-50 p-4 rounded-lg border-l-4 border-purple-500 flex items-center space-x-3">
            <PencilIcon class="h-5 w-5 text-purple-500 flex-shrink-0" />
            <span class="text-slate-700">Blog post management with CRUD operations</span>
          </div>
          <div class="bg-gray-50 p-4 rounded-lg border-l-4 border-purple-500 flex items-center space-x-3">
            <PhotoIcon class="h-5 w-5 text-purple-500 flex-shrink-0" />
            <span class="text-slate-700">Multiple image support per post</span>
          </div>
          <div class="bg-gray-50 p-4 rounded-lg border-l-4 border-purple-500 flex items-center space-x-3">
            <UserIcon class="h-5 w-5 text-purple-500 flex-shrink-0" />
            <span class="text-slate-700">User authentication and authorization</span>
          </div>
          <div class="bg-gray-50 p-4 rounded-lg border-l-4 border-purple-500 flex items-center space-x-3">
            <ChartBarIcon class="h-5 w-5 text-purple-500 flex-shrink-0" />
            <span class="text-slate-700">Admin panel for content management</span>
          </div>
          <div class="bg-gray-50 p-4 rounded-lg border-l-4 border-purple-500 flex items-center space-x-3">
            <CubeIcon class="h-5 w-5 text-purple-500 flex-shrink-0" />
            <span class="text-slate-700">Docker containerization</span>
          </div>
          <div class="bg-gray-50 p-4 rounded-lg border-l-4 border-purple-500 flex items-center space-x-3">
            <DevicePhoneMobileIcon class="h-5 w-5 text-purple-500 flex-shrink-0" />
            <span class="text-slate-700">Responsive web design</span>
          </div>
          <div class="bg-gray-50 p-4 rounded-lg border-l-4 border-purple-500 flex items-center space-x-3 md:col-span-2">
            <MagnifyingGlassIcon class="h-5 w-5 text-purple-500 flex-shrink-0" />
            <span class="text-slate-700">REST API with OpenAPI documentation</span>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { 
  CodeBracketIcon, 
  ServerIcon, 
  SparklesIcon,
  PencilIcon,
  PhotoIcon,
  UserIcon,
  ChartBarIcon,
  CubeIcon,
  DevicePhoneMobileIcon,
  MagnifyingGlassIcon
} from "@heroicons/vue/24/outline";
</script>