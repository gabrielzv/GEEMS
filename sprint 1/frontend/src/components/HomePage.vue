<template>
  <div class="min-h-screen bg-gray-100">
    <!-- Header -->
    <header class="bg-white shadow flex justify-between items-center px-6 py-4">
      <img src="../assets/GEEMSLogo.jpg" class="h-20 w-auto" />

      <!-- Navegación -->
      <nav class="space-x-6 text-gray-700 hidden md:flex">
        <a href="#" class="hover:text-blue-600">Inicio</a>
        <a href="#" class="hover:text-blue-600">Servicios</a>
        <a href="#" class="hover:text-blue-600">Contacto</a>
        <a href="#" class="hover:text-blue-600">Acerca</a>
      </nav>

      <!-- Usuario -->
      <div class="flex items-center space-x-3">
        <span class="text-gray-700 hidden sm:block">Hola, {{ user.nombreUsuario }}</span>
        <span v-if="empleado" class="text-gray-700 hidden sm:block">Tipo: {{ empleado.tipo }}</span>
        <img
          src="https://i.pravatar.cc/40"
          alt="Avatar"
          class="w-10 h-10 rounded-full cursor-pointer"
          @click="goToUserPage"
        />
      </div>
    </header>

    <!-- Contenido principal -->
    <main class="p-6 flex justify-center items-center">
      <div class="bg-white p-8 rounded-xl shadow-md w-full max-w-md space-y-4">
        <template v-if="loading">
          <p class="text-center text-gray-500">Cargando opciones...</p>
        </template>

        <template v-else-if="error">
          <p class="text-center text-red-600">Error al cargar los datos del empleado.</p>
        </template>

        <template v-else>
          <h2 class="text-xl font-semibold text-gray-800 text-center mb-4">
            Menú de opciones
          </h2>

          <!-- Opciones para SuperAdmin -->
          <div v-if="user.tipo === 'SuperAdmin'" class="space-y-2">
            <button class="btn-option">Gestionar usuarios</button>
            <button class="btn-option">Ver reportes generales</button>
            <button class="btn-option">Configurar sistema</button>
            <button class="btn-option" @click="goToVerEmpresasRegistradas">Ver empresas registradas</button>
          </div>

          <!-- Opciones para DuenoEmpresa -->
          <div v-else-if="user.tipo === 'DuenoEmpresa'" class="space-y-2">
            <button class="btn-option" @click="goToEditarEmpresa">Editar empresa</button>
            <button class="btn-option">Añadir nuevo empleado</button>
            <button class="btn-option" @click="goToVerEmpresaIndv">Ver información empresa</button>
          </div>

          <!-- Opciones para Empleado -->
          <div v-else-if="user.tipo === 'Empleado'" class="space-y-2">
            <template v-if="empleado.tipo === 'Supervisor'">
              <button class="btn-option">Registrar horas</button>
              <button class="btn-option">Seleccionar beneficios</button>
              <button class="btn-option">Desglose de pagos anteriores</button>
              <button class="btn-option">Historial de registros</button>
            </template>

            <template v-else-if="empleado?.tipo === 'Colaborador'">
              <button class="btn-option">Registrar horas</button>
              <button class="btn-option">Historial de registros</button>
            </template>

            <template v-else-if="empleado?.tipo === 'Payroll'">
              <button class="btn-option">Registrar horas</button>
              <button class="btn-option">Historial de registros</button>
            </template>

            <template v-else>
              <p class="text-center text-gray-500">Tipo de empleado no reconocido.</p>
            </template>
          </div>

          <!-- Rol no reconocido -->
          <div v-else class="text-center text-gray-500">
            Rol de usuario no reconocido.
          </div>
        </template>
      </div>
    </main>
  </div>
</template>

<script>
import { ref, onMounted } from "vue";
import { useUserStore } from "../store/user";
import { useRouter } from "vue-router";

export default {
  setup() {
    const router = useRouter();
    const userStore = useUserStore();
    const user = userStore.usuario;
    const empleado = ref(null);
    const loading = ref(true);
    const error = ref(false);

    const fetchEmpleado = async () => {
      try {
        loading.value = true;
        error.value = false;

        // Obtener datos desde el store
        await userStore.fetchEmpleado(user.cedulaPersona);
        empleado.value = userStore.empleado;
      } catch (err) {
        console.error("Error al obtener los datos del empleado:", err);
        error.value = true;
      } finally {
        loading.value = false;
      }
    };
    
    onMounted(() => {
      if (!user || !user.cedulaPersona) {
        router.push("/"); // Redirige si no hay sesión
      } else {
        fetchEmpleado();
      }
    });

    const goToUserPage = () => {
      router.push("/user");
    };

    const goToVerEmpresaIndv = () => {
      router.push("/verEmpresaIndv");
    };

    const goToVerEmpresasRegistradas = () => {
      router.push("/ConsulEmpresa");
    };

    return { goToUserPage, goToVerEmpresaIndv, goToVerEmpresasRegistradas, user, empleado, loading, error };
  },
};
</script>

<style scoped>
/* Ya no es necesario SCSS ni CSS adicionales, Tailwind lo maneja */
</style>



