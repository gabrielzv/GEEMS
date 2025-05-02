<template>
  <div class="p-6 max-w-2xl mx-auto">
    <div v-if="loading" class="text-center">Cargando perfil...</div>

    <div v-else-if="error" class="text-center text-red-600">
      <p>Error al cargar el perfil.</p>
      <button
        @click="fetchProfile"
        class="mt-2 px-4 py-2 bg-blue-600 text-white rounded"
      >
        Reintentar
      </button>
    </div>

    <div
      v-else
      class="bg-white shadow-md rounded-lg p-6 space-y-4"
      aria-label="Perfil de usuario"
    >
      <h2 class="text-xl font-bold">Perfil de Usuario</h2>

      <div class="grid grid-cols-1 sm:grid-cols-2 gap-4">
        <UserInfo label="Nombre completo" :value="profile.fullName" />
        <UserInfo label="Correo electrónico" :value="profile.email" />
        <UserInfo label="Número de teléfono" :value="profile.phone" />
        <UserInfo label="Rol de usuario" :value="profile.role" />
        <UserInfo label="Empresa asociada" :value="profile.company" />
        <UserInfo label="Fecha de registro" :value="profile.registeredAt" />
      </div>
    </div>
  </div>
</template>

<script setup>
import { onMounted, ref } from "vue";
import axios from "axios";
import UserInfo from "./UserInfo.vue";

const loading = ref(true);
const error = ref(false);
const profile = ref({});

const dummyUser = {
  fullName: "Juan Pérez Rodríguez",
  email: "juan.perez@example.com",
  phone: null, // Este campo mostrará "Dato no disponible"
  role: "Empleado",
  company: "EjemploCorp S.A.",
  registeredAt: "2024-03-15",
};

const fetchProfile = async () => {
  loading.value = true;
  error.value = false;
  try {
    const { data } = await axios.get("/api/profile", { withCredentials: true });
    profile.value = {
      fullName: data.fullName || "Dato no disponible",
      email: data.email || "Dato no disponible",
      phone: data.phone || "Dato no disponible",
      role: data.role || "Dato no disponible",
      company: data.company || "Dato no disponible",
      registeredAt: data.registeredAt || "Dato no disponible",
    };
  } catch (err) {
    error.value = true;
  } finally {
    loading.value = false;
  }
};

const fetchProfileDummy = async () => {
  loading.value = true;
  error.value = false;
  try {
    await new Promise((resolve) => setTimeout(resolve, 800)); // Simula el tiempo de carga
    profile.value = {
      fullName: dummyUser.fullName || "Dato no disponible",
      email: dummyUser.email || "Dato no disponible",
      phone: dummyUser.phone || "Dato no disponible",
      role: dummyUser.role || "Dato no disponible",
      company: dummyUser.company || "Dato no disponible",
      registeredAt: dummyUser.registeredAt || "Dato no disponible",
    };
  } catch {
    error.value = true;
  } finally {
    loading.value = false;
  }
};

onMounted(fetchProfileDummy);
</script>

<script>
export default {
  name: "UserProfile",
};
</script>

<style scoped>
@media (max-width: 640px) {
  .grid-cols-2 {
    grid-template-columns: repeat(1, minmax(0, 1fr));
  }
}
</style>
