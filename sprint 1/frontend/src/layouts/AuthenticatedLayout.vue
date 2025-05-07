<!-- src/layouts/AuthenticatedLayout.vue -->
<template>
  <div class="min-h-screen bg-gray-100">
    <NavBar :user="user" :empleado="empleado" />
    <main class="p-6">
      <router-view />
    </main>
  </div>
</template>

<script>
import NavBar from "../components/AppNavbar.vue";
import { useUserStore } from "@/store/user";
import { computed } from "vue";

export default {
  name: "AuthenticatedLayout",
  components: {
    NavBar,
  },
  setup() {
    const userStore = useUserStore();

    return {
      user: computed(() => userStore.usuario),
      empleado: computed(() => userStore.empleado),
    };
  },
};
</script>
