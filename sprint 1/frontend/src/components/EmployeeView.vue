<template>
  <div class="min-h-screen p-6 w-full flex flex-col bg-gray-100">
    <div v-if="loading" class="text-center text-xl mt-10">
      Cargando perfil desde vista...
    </div>

    <div v-else-if="error" class="text-center text-red-600 mt-10">
      <p class="text-xl">Error al cargar los datos.</p>
      <button
        @click="fetchUserView"
        class="mt-4 px-6 py-3 bg-blue-600 text-white rounded"
      >
        Reintentar
      </button>
    </div>

    <div
      v-else
      class="bg-white shadow-lg rounded-lg p-10 w-full max-w-6xl space-y-6 text-lg mx-auto"
      aria-label="Vista del usuario"
    >
      <h2 class="text-3xl font-bold">
        {{ userView.fullName }}
      </h2>

      <div class="grid grid-cols-1 md:grid-cols-2 gap-6">
        <UserInfo label="Cedula" :value="userView.cedulaPersona" />
        <UserInfo label="Teléfono" :value="userView.phone" />
        <UserInfo label="Dirección" :value="userView.address" />
        <UserInfo label="Fecha de ingreso" :value="userView.dateIn" />
        <UserInfo label="Correo electrónico" :value="userView.email" />
        <UserInfo label="Rol" :value="userView.role" />
        <UserInfo label="Contrato" :value="userView.contract" />
        <UserInfo label="Género" :value="userView.genre" />
        <UserInfo label="Estado" :value="userView.state" />
        <UserInfo label="Tipo" :value="userView.type" />
        <UserInfo label="Salario" :value="userView.salario" />
        <UserInfo label="Compañía" :value="userView.company" />
      </div>

      <div class="mt-6 flex justify-end gap-x-4">
        <button
          @click="$router.back()"
          class="px-6 py-3 bg-gray-500 text-white rounded hover:bg-gray-600"
        >
          Volver
        </button>
        <router-link
          to="/actualizar-perfil"
          class="px-6 py-3 bg-blue-600 text-white rounded hover:bg-blue-700"
        >
          Editar perfil de Empleado
        </router-link>
        <router-link
          to="/eliminar-perfil"
          class="px-6 py-3 bg-red-600 text-white rounded hover:bg-red-700"
        >
          Eliminar Empleado
        </router-link>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from "vue";
import { useRoute } from "vue-router";
import axios from "axios";
import UserInfo from "./UserInfo.vue";
import { useUserStore } from "../store/user";

const route = useRoute();
const cedula = route.params.cedula;
console.log(cedula);
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
      `https://localhost:7014/api/Persona/${cedula}`
    );
    const data = personaRes.data;

    await userStore.fetchEmpleado(cedula);
    const dataEmpleado = userStore.empleado || {};

    userView.value = {
      fullName: data.fullName || "Dato no disponible",
      email: data.email || "Dato no disponible",
      phone: data.phone || "Dato no disponible",
      role: dataEmpleado.tipo || "Dato no disponible",
      address: data.address || "Dato no disponible",
      contract: dataEmpleado.contrato || "Dato no disponible",
      genre: dataEmpleado.genero === "F" ? "Femenino" : "Masculino",
      state: dataEmpleado.estadoLaboral || "Dato no disponible",
      type: dataEmpleado.tipo || "Dato no disponible",
      dateIn: dataEmpleado.fechaIngreso || "Dato no disponible",
      company: dataEmpleado.nombreEmpresa || "Dato no disponible",
      cedulaPersona: cedula,
      salario: dataEmpleado.salarioBruto || "Dato no disponible",
    };
  } catch (err) {
    console.error("Error al obtener los datos de la persona", err);
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
