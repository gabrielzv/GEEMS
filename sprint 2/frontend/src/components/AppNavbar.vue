<template>
  <header
    class="bg-header shadow flex justify-between items-center px-6 py-4"
    style="background-color: #fbf7ec"
  >
    <router-link to="/home">
      <img src="@/assets/GEEMSLogo.jpg" class="h-20 w-auto" alt="GEEMS Logo" />
    </router-link>

    <!-- Navegación principal -->
    <nav
      v-if="isAuthenticated"
      class="flex items-center space-x-6 text-gray-700"
    >
      <!-- Opciones para SuperAdmin -->
      <template v-if="user?.tipo === 'SuperAdmin'">
        <router-link
          to="/gestionarUsuarios"
          class="hover:text-blue-600 text-blue-600"
          active-class="text-blue-600 font-medium"
          >Gestionar usuarios</router-link
        >
        <router-link
          to="/reportesGenerales"
          class="hover:text-blue-600 text-blue-600"
          active-class="text-blue-600 font-medium"
          >Reportes</router-link
        >
        <router-link
          to="/configuracion"
          class="hover:text-blue-600 text-blue-600"
          active-class="text-blue-600 font-medium"
          >Configuración</router-link
        >
        <router-link
          to="/ConsulEmpresa"
          class="hover:text-blue-600 text-blue-600"
          active-class="text-blue-600 font-medium"
          >Empresas</router-link
        >
      </template>

      <!-- Opciones para DuenoEmpresa -->
      <template v-else-if="user?.tipo === 'DuenoEmpresa'">
        <!-- Dropdown para DueñoEmpresa (Empresa) -->
        <div class="relative group">
          <button class="hover:text-blue-600 text-blue-600 flex items-center">
            Empresa
            <svg
              class="w-4 h-4 ml-1"
              fill="none"
              stroke="currentColor"
              viewBox="0 0 24 24"
            >
              <path
                stroke-linecap="round"
                stroke-linejoin="round"
                stroke-width="2"
                d="M19 9l-7 7-7-7"
              />
            </svg>
          </button>
          <div
            class="absolute hidden group-hover:block rounded-md top-full py-1 w-48 z-10"
            style="background-color: #fbf7ec"
          >
            <router-link
              to="/editarEmpresa"
              class="block px-4 py-2 hover:bg-gray-50 hover:text-blue-600"
              active-class="text-blue-600 font-medium"
              >Editar empresa</router-link
            >
            <router-link
              to="/verEmpresaIndv"
              class="block px-4 py-2 hover:bg-gray-50 hover:text-blue-600"
              active-class="text-blue-600 font-medium"
              >Ver mi empresa</router-link
            >
          </div>
        </div>

        <router-link
          to="/anadirEmpleado"
          class="hover:text-blue-600 text-blue-600"
          active-class="text-blue-600 font-medium"
          >Añadir empleado</router-link
        >
      </template>

      <!-- Opciones para Empleado -->
      <template v-else-if="user?.tipo === 'Empleado'">
        <!-- Dropdowns para Empleado (Registros y Beneficios) -->
        <div class="relative group">
          <button class="hover:text-blue-600 text-blue-600 flex items-center">
            Registros
            <svg
              class="w-4 h-4 ml-1"
              fill="none"
              stroke="currentColor"
              viewBox="0 0 24 24"
            >
              <path
                stroke-linecap="round"
                stroke-linejoin="round"
                stroke-width="2"
                d="M19 9l-7 7-7-7"
              />
            </svg>
          </button>
          <div
            class="absolute hidden group-hover:block rounded-md top-full py-1 w-48 z-10"
            style="background-color: #fbf7ec"
          >
            <router-link
              to="/registrarHoras"
              class="block px-4 py-2 hover:bg-gray-50 hover:text-blue-600"
              active-class="text-blue-600 font-medium"
              >Registrar horas</router-link
            >
            <router-link
              to="/historialRegistros"
              class="block px-4 py-2 hover:bg-gray-50 hover:text-blue-600"
              active-class="text-blue-600 font-medium"
              >Historial</router-link
            >
          </div>
        </div>
        <div class="relative group">
          <button class="hover:text-blue-600 text-blue-600 flex items-center">
            Beneficios
            <svg
              class="w-4 h-4 ml-1"
              fill="none"
              stroke="currentColor"
              viewBox="0 0 24 24"
            >
              <path
                stroke-linecap="round"
                stroke-linejoin="round"
                stroke-width="2"
                d="M19 9l-7 7-7-7"
              />
            </svg>
          </button>
          <div
            class="absolute hidden group-hover:block rounded-md top-full py-1 w-48 z-10"
            style="background-color: #fbf7ec"
          >
            <router-link
              to="/matricularBeneficio"
              class="block px-4 py-2 hover:bg-gray-50 hover:text-blue-600"
              active-class="text-blue-600 font-medium"
              >Matricular Beneficios</router-link
            >
            <router-link
              to="/employeeBenefits"
              class="block px-4 py-2 hover:bg-gray-50 hover:text-blue-600"
              active-class="text-blue-600 font-medium"
              >Mis beneficios</router-link
            >
          </div>
        </div>

        <!-- Opciones adicionales para Supervisor -->
        <template v-if="empleado?.tipo === 'Supervisor'">
          <router-link
            to="/desglosePagos"
            class="hover:text-blue-600 text-blue-600"
            active-class="text-blue-600 font-medium"
            >Desglose de pagos</router-link
          >
        </template>

        <!-- Opciones adicionales para Payroll -->
        <template v-if="empleado?.tipo === 'Payroll'">
          <router-link
            to="/realizarPago"
            class="hover:text-blue-600 text-blue-600"
            active-class="text-blue-600 font-medium"
            >Realizar Pago</router-link
          >
        </template>
      </template>
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
          v-if="user?.tipo"
          class="hidden sm:inline text-gray-500 text-sm"
          :title="user.tipo"
        >
          {{ user.tipo }}
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
import { computed, ref, onMounted } from "vue";

export default {
  name: "AppHeader",
  setup() {
    const router = useRouter();
    const userStore = useUserStore();

    const isAuthenticated = computed(() => !!userStore.usuario);
    const user = computed(() => userStore.usuario);
    const empleado = ref(null);

    const fetchEmpleado = async () => {
      if (user.value?.tipo === "Empleado" && user.value?.cedulaPersona) {
        try {
          await userStore.fetchEmpleado(user.value.cedulaPersona);
          empleado.value = userStore.empleado;
        } catch (err) {
          console.error("Error al obtener datos del empleado:", err);
        }
      }
    };

    onMounted(() => {
      fetchEmpleado();
    });

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
