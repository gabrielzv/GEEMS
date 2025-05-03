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
      <h2 class="text-xl font-bold">Vista de Usuario</h2>

      <div class="grid grid-cols-1 sm:grid-cols-2 gap-4">
        <UserInfo label="Nombre completo" :value="userView.fullName" />
        <UserInfo label="Correo electrónico" :value="userView.email" />
        <UserInfo label="Teléfono" :value="userView.phone" />
        <UserInfo label="Rol" :value="userView.role" />
        <UserInfo label="Dirección" :value="userView.address" />
        <UserInfo label="Contrato" :value="userView.contract" />
        <UserInfo label="Género" :value="userView.genre" />
        <UserInfo label="Estado" :value="userView.state" />
        <UserInfo label="Tipo" :value="userView.type" />
        <UserInfo label="Fecha de ingreso" :value="userView.dateIn" />
        <UserInfo label="Compañía" :value="userView.company" />
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from "vue";
import axios from "axios";
import UserInfo from "./UserInfo.vue";
import { useUserStore } from "../store/user";

const userStore = useUserStore();
const usuario = userStore.usuario;

const loading = ref(true);
const error = ref(false);
const userView = ref({});

const fetchUserView = async () => {
  loading.value = true;
  error.value = false;

  try {
    const personaRes = await axios.get(
      `https://localhost:7014/api/Persona/${usuario.cedulaPersona}`
    );
    const empleadoRes = await axios.get(
      `https://localhost:7014/api/GetEmpleado/${usuario.cedulaPersona}`
    );

    const data = personaRes.data;
    const dataEmpleado = empleadoRes.data;
    console.log(data);
    console.log(dataEmpleado);
    userView.value = {
      fullName: data.fullName || "Dato no disponible",
      email: data.email || "Dato no disponible",
      phone: data.phone || "Dato no disponible",
      role: usuario.tipo || "Dato no disponible",
      address: data.address || "Dato no disponible",
      contract: dataEmpleado.contrato || "Dato no disponible",
      genre: dataEmpleado.genero || "Dato no disponible",
      state: dataEmpleado.estadoLaboral || "Dato no disponible",
      type: dataEmpleado.tipo || "Dato no disponible",
      dateIn: dataEmpleado.fechaIngreso || "Dato no disponible",
      company: dataEmpleado.nombreEmpresa || "Dato no disponible",
    };
  } catch (err) {
    error.value = true;
  } finally {
    loading.value = false;
  }
};

onMounted(() => {
  if (!usuario || !usuario.cedulaPersona) {
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
