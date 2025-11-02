import { createRouter, createWebHistory } from "vue-router";
import Home from "@/views/Home.vue";
import About from "@/views/About.vue";
import PostDetail from "@/views/PostDetail.vue";
import PostGrid from "@/views/PostGrid.vue";
import Admin from "@/views/Admin.vue";
import PostEditor from "@/views/PostEditor.vue";
import { useAuth } from "@/composables/useAuth";

const routes = [
  {
    path: "/",
    name: "Home",
    component: Home
  },
  {
    path: "/posts",
    name: "PostGrid",
    component: PostGrid
  },
  {
    path: "/posts/:id",
    name: "PostDetail",
    component: PostDetail,
    props: true
  },
  {
    path: "/about",
    name: "About",
    component: About
  },
  {
    path: "/admin",
    name: "Admin",
    component: Admin,
    meta: { requiresAuth: true }
  },
  {
    path: "/admin/posts/new",
    name: "CreatePost",
    component: PostEditor,
    meta: { requiresAuth: true }
  },
  {
    path: "/admin/posts/edit/:id",
    name: "EditPost",
    component: PostEditor,
    meta: { requiresAuth: true }
  }
];

const router = createRouter({
  history: createWebHistory(),
  routes
});

// Navigation guards
router.beforeEach((to, from, next) => {
  const { isAuthenticated } = useAuth();
  
  if (to.meta.requiresAuth && !isAuthenticated.value) {
    // Redirect to home page, authentication dialog will be shown
    next("/");
    return;
  }
  
  next();
});

export default router;