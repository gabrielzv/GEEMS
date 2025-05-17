<template>
  <div class="flex justify-center items-center min-h-screen bg-gray-100">
    <div class="bg-white shadow rounded p-6 max-w-md w-full">
      <h1 class="text-2xl font-bold mb-4 text-center">
        Beneficios de la Empresa
      </h1>

      <!-- Mensaje de error -->
      <p v-if="error" class="text-red-500 mt-4 text-center">{{ error }}</p>

      <!-- Lista de beneficios -->
      <div v-if="beneficios.length" class="mt-6">
        <h2 class="text-xl font-semibold mb-2 text-center">
          Beneficios encontrados:
        </h2>
        <ul class="space-y-4">
          <li
            v-for="beneficio in beneficios"
            :key="beneficio.id"
            class="p-4 border rounded shadow"
          >
            <p><strong>Nombre:</strong> {{ beneficio.nombre }}</p>
            <p><strong>Descripción:</strong> {{ beneficio.descripcion }}</p>
            <p><strong>Costo:</strong> ₡{{ beneficio.costo }}</p>
            <p>
              <strong>Tiempo mínimo en empresa:</strong>
              {{ beneficio.tiempoMinimoEnEmpresa }} meses
            </p>
          </li>
        </ul>
      </div>

      <!-- Mensaje si no hay beneficios -->
      <p v-else class="text-center text-gray-500 mt-4">
        No se encontraron beneficios para esta empresa.
      </p>
    </div>
  </div>
</template>

<script>
import axios from "axios";
import { useUserStore } from "../store/user";
import { onMounted, ref } from "vue";

export default {
  setup() {
    const userStore = useUserStore();
    const beneficios = ref([]);
    const error = ref("");

    // Método para obtener los beneficios de la empresa
    const fetchBeneficios = async () => {
      try {
        // Se llama a fetchEmpresa para obtener la cédula jurídica
        await userStore.fetchEmpresa(userStore.usuario.cedulaPersona);

        if (userStore.empresa) {
          const cedulaJuridica = userStore.empresa.cedulaJuridica;

          // Se hace el get para obtener los beneficios creados de la empresa
          const response = await axios.get(
            `https://localhost:7014/api/GetCompanyBenefits/${cedulaJuridica}`
          );
          beneficios.value = response.data;
        } else {
          error.value = "No se pudo obtener la información de la empresa.";
        }
      } catch (err) {
        console.error("Error al obtener los beneficios:", err);
        error.value =
          err.response?.data?.message ||
          "Ocurrió un error al obtener los beneficios.";
      }
    };

    // Se llama a la función al montar el componente
    onMounted(() => {
      if (!userStore.usuario || !userStore.usuario.cedulaPersona) {
        window.location.href = "/";
      } else {
        fetchBeneficios();
      }
    });

    return {
      beneficios,
      error,
    };
  },
};
</script>
