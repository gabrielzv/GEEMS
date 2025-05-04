<template>
  <div class="min-h-screen bg-gray-100">
    <!-- Header -->
    <header class="bg-white shadow flex justify-between items-center px-6 py-4">
  <img
    src="../assets/GEEMSLogo.jpg"
    class="h-20 w-auto" />

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
        <!-- <span class="text-gray-700 hidden sm:block">Hola, {{ empleado.tipo }}</span> -->
        <img
          src="https://i.pravatar.cc/40"
          alt="Avatar"
          class="w-10 h-10 rounded-full cursor-pointer"
          @click="goToUserPage"
        />
      </div>
    </header>

    <!-- Contenido Principal -->
    <main class="p-6 flex justify-center items-center">
      <!-- Cuadro central -->
      <div class="bg-white p-8 rounded-xl shadow-md w-full max-w-md space-y-4">
        <h2 class="text-xl font-semibold text-gray-800 text-center mb-4">
          Menú de opciones
        </h2>

        <!-- Opciones para SuperAdmin -->
        <div v-if="user.tipo === 'SuperAdmin'" class="space-y-2">
          <button class="btn-option">Gestionar usuarios</button>
          <button class="btn-option">Ver reportes generales</button>
          <button class="btn-option">Configurar sistema</button>
        </div>

        <!-- Opciones para DuenoEmpresa -->
        <div v-else-if="user.tipo === 'DuenoEmpresa'" class="space-y-2">
          <button class="btn-option">Editar empresa</button>
          <button class="btn-option">Añadir nuevo empleado</button>
        </div>

        <!-- Opciones para Empleado -->
        <div v-else-if="user.tipo === 'Empleado'" class="space-y-2">
          <button class="btn-option">Registrar horas</button>
          <button class="btn-option">Seleccionar beneficios</button>
          <button class="btn-option">Desglose de pagos anteriores</button>
          <button class="btn-option">Historial de registros</button>
        </div>

        <!-- Por si el tipo no es reconocido -->
        <div v-else class="text-center text-gray-500">
          Rol de usuario no reconocido.
        </div>
      </div>
    </main>
  </div>
</template>

<script>
import { useUserStore } from "../store/user";
import { useRouter } from "vue-router";

export default {
  setup() {
    const router = useRouter();
    const userStore = useUserStore();
    const user = userStore.usuario;
    //const empleado = userStore.empleado;

    const goToUserPage = () => {
      router.push("/user");
    };

    return { goToUserPage, user, /*empleado*/ };
  },
};
</script>

<style scoped>
.btn-option {
  @apply w-full px-4 py-2 bg-blue-600 text-white rounded hover:bg-blue-500 transition;
}
</style>


