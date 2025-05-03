<template>
  <div class="p-6 max-w-2xl mx-auto">
    <div v-if="loading" class="text-center">Cargando perfil desde vista...</div>

    <div v-else-if="error" class="text-center text-red-600">
      <p>Error al cargar los datos.</p>
      <button
        @click="fetchUserView"
        class="mt-2 px-4 py-2 bg-blue-600 text-white rounded"
      >
        Reintentar
      </button>
    </div>

    <div
      v-else
      class="bg-white shadow-md rounded-lg p-6 space-y-4"
      aria-label="Vista del usuario"
    >
      <h2 class="text-xl font-bold">Vista de Usuario (UserView)</h2>

      <div class="grid grid-cols-1 sm:grid-cols-2 gap-4">
        <UserInfo label="Nombre completo" :value="userView.fullName" />
        <UserInfo label="Correo electrónico" :value="userView.email" />
        <UserInfo label="Teléfono" :value="userView.phone" />
        <UserInfo label="Rol" :value="userView.role" />
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from "vue";
import axios from "axios";
import UserInfo from "./UserInfo.vue";
import { useUserStore } from "../store/user"; // ajusta la ruta si es diferente

const userStore = useUserStore(); // acceder al store de usuario
const usuario = userStore.usuario; // acceder al usuario global almacenado

const loading = ref(true);
const error = ref(false);
const userView = ref({});

const fetchUserView = async () => {
  loading.value = true;
  error.value = false;

  try {
    const { data } = await axios.get(
      `https://localhost:7014/api/Persona/${usuario.cedulaPersona}`
    );

    userView.value = {
      fullName: data.fullName || "Dato no disponible",
      email: data.email || "Dato no disponible",
      phone: data.phone || "Dato no disponible",
      role: usuario.tipo || "Dato no disponible",
      address: data.address || "Dato no disponible",
    };
  } catch (err) {
    error.value = true;
  } finally {
    loading.value = false;
  }
};

onMounted(() => {
  if (!usuario || !usuario.cedulaPersona) {
    // Redirige si no hay usuario cargado
    window.location.href = "/";
  } else {
    fetchUserView();
  }
});
</script>

<style scoped>
@media (max-width: 640px) {
  .grid-cols-2 {
    grid-template-columns: repeat(1, minmax(0, 1fr));
  }
}
</style>
