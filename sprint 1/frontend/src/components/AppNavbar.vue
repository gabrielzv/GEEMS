<template>
  <header class="bg-white shadow flex justify-between items-center px-6 py-4">
    <router-link to="/home">
      <img src="@/assets/GEEMSLogo.jpg" class="h-20 w-auto" alt="GEEMS Logo" />
    </router-link>

    <!-- Navegación -->
    <nav v-if="isAuthenticated" class="space-x-6 text-gray-700 hidden md:flex">
      <router-link
        to="/home"
        class="hover:text-blue-600"
        active-class="text-blue-600 font-medium"
        >Inicio</router-link
      >
      <router-link
        to="/services"
        class="hover:text-blue-600"
        active-class="text-blue-600 font-medium"
        >Servicios</router-link
      >
      <router-link
        to="/contact"
        class="hover:text-blue-600"
        active-class="text-blue-600 font-medium"
        >Contacto</router-link
      >
      <router-link
        to="/about"
        class="hover:text-blue-600"
        active-class="text-blue-600 font-medium"
        >Acerca</router-link
      >
    </nav>

    <!-- Usuario + Logout -->
    <div v-if="isAuthenticated" class="flex items-center gap-4">
      <!-- Contenedor de información de usuario -->
      <div class="flex items-center gap-3 min-w-0">
        <!-- Nombre de usuario (con truncado para textos largos) -->
        <span
          v-if="user?.nombreUsuario"
          class="hidden sm:inline text-gray-700 truncate max-w-[120px]"
          :title="user.nombreUsuario"
        >
          Hola, {{ user.nombreUsuario }}
        </span>

        <!-- Tipo de empleado -->
        <span
          v-if="empleado?.tipo"
          class="hidden sm:inline text-gray-500 text-sm"
        >
          {{ empleado.tipo }}
        </span>

        <!-- Avatar -->
        <div class="flex-shrink-0">
          <img
            :src="user?.avatar || 'https://i.pravatar.cc/40'"
            alt="Avatar"
            class="w-9 h-9 rounded-full cursor-pointer border border-gray-200"
            @click="goToUserPage"
          />
        </div>
      </div>

      <!-- Botón de logout con separación adecuada -->
      <div class="flex-shrink-0 ml-2">
        <button
          @click="handleLogout"
          class="px-2.5 py-1 text-xs text-gray-500 hover:text-red-500 hover:bg-gray-50 rounded-md transition-colors border border-gray-200"
        >
          Salir
        </button>
      </div>
    </div>
  </header>
</template>

<script>
import { useRouter } from "vue-router";
import { useUserStore } from "@/store/user";
import { computed } from "vue";

export default {
  name: "AppHeader",
  setup() {
    const router = useRouter();
    const userStore = useUserStore();

    const isAuthenticated = computed(() => !!userStore.usuario);
    const user = computed(() => userStore.usuario);
    const empleado = computed(() => userStore.empleado);

    const goToUserPage = () => {
      router.push("/user");
    };

    const handleLogout = () => {
      userStore.logout();
      router.push("/login");
    };

    return {
      isAuthenticated,
      user,
      empleado,
      goToUserPage,
      handleLogout,
    };
  },
};
</script>
