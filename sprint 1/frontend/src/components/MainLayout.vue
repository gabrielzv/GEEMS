<template>
  <div class="min-h-screen bg-gray-100">
    <AppHeader :user="user" :empleado="empleado" />

    <main class="p-6">
      <router-view />
    </main>
  </div>
</template>

<script>
import AppHeader from "@/components/Header.vue";
import { useUserStore } from "@/store/user";
import { computed } from "vue";

export default {
  name: "MainLayout",
  components: {
    AppHeader,
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
