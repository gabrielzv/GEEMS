<template>
  <div class="flex justify-center items-center min-h-screen bg-gray-100">
    <div class="bg-white shadow rounded p-6 w-full max-w-2xl space-y-4">
      <h1 class="text-2xl font-bold mb-4 text-center">
        Beneficios Disponibles
      </h1>

      <!-- Mensaje de carga -->
      <div v-if="loading" class="text-center">Cargando beneficios...</div>

      <!-- Mensaje de error -->
      <div v-if="error" class="text-red-500 text-center">
        {{ error }}
      </div>

      <!-- Lista de beneficios disponibles -->
      <div v-if="!loading && beneficios.length" class="mt-6">
        <ul class="space-y-4">
          <li
            v-for="beneficio in beneficios"
            :key="beneficio.Id"
            class="p-4 border rounded shadow"
          >
            <p><strong>Nombre:</strong> {{ beneficio.Nombre }}</p>
            <p><strong>Descripción:</strong> {{ beneficio.Descripcion }}</p>
            <p><strong>Costo:</strong> ₡{{ beneficio.Costo }}</p>
            <p>
              <strong>Tiempo mínimo en empresa:</strong>
              {{ beneficio.TiempoMinimoEnEmpresa }} meses
            </p>
            <button
              @click="matricularBeneficio(beneficio.Id)"
              class="mt-5 mb-2 bg-blue-500 text-white px-4 py-2 rounded w-full hover:bg-blue-600"
            >
              Matricular beneficio
            </button>
          </li>
        </ul>
      </div>

      <!-- Mensaje si no hay beneficios disponibles -->
      <p
        v-if="!loading && !beneficios.length && !error"
        class="text-center text-gray-500 mt-4"
      >
        No se encontraron beneficios disponibles para esta empresa.
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
    const loading = ref(false);
    const matriculaLoading = ref(false);

    // Cargar datos iniciales del sessionStorage
    userStore.cargarDesdeSessionStorage();

    // Método para matricular un beneficio
    const matricularBeneficio = async (beneficioId) => {
      matriculaLoading.value = true;
      try {
        await axios.post(
          `https://localhost:7014/api/SetBeneficioPorEmpleado/matricularBeneficio`,
          {
            IdEmpleado: userStore.usuario.cedulaPersona,
            IdBeneficio: beneficioId,
          }
        );
        alert("Beneficio matriculado exitosamente");
      } catch (err) {
        console.error("Error al matricular el beneficio:", err);
        error.value =
          err.response?.data?.message ||
          err.response?.data ||
          "Error al matricular el beneficio";
      } finally {
        matriculaLoading.value = false;
      }
    };

    // Método para obtener los beneficios de la empresa
    const fetchBeneficios = async () => {
      loading.value = true;
      error.value = "";

      try {
        // Verificar si ya tenemos los datos necesarios
        if (
          !userStore.empleado ||
          userStore.empleado.cedulaPersona !== userStore.usuario.cedulaPersona
        ) {
          await userStore.fetchEmpleado(userStore.usuario.cedulaPersona);
        }

        if (!userStore.empleado?.cedulaEmpresa) {
          throw new Error(
            "No se pudo obtener la información del empleado o la empresa asociada."
          );
        }

        // Verificar si ya tenemos los datos de la empresa
        if (
          !userStore.empresa ||
          userStore.empresa.cedulaJuridica !== userStore.empleado.cedulaEmpresa
        ) {
          await userStore.fetchEmpresa(userStore.empleado.cedulaEmpresa);
        }

        if (!userStore.empresa?.cedulaJuridica) {
          throw new Error("No se pudo obtener la información de la empresa.");
        }

        // Obtener los beneficios
        const { data } = await axios.get(
          `https://localhost:7014/api/GetCompanyBenefits/${userStore.empresa.cedulaJuridica}`
        );

        beneficios.value = data || [];
      } catch (err) {
        console.error("Error al obtener los beneficios:", err);
        error.value =
          err.message ||
          err.response?.data?.message ||
          err.response?.data ||
          "Ocurrió un error al obtener los beneficios.";
        beneficios.value = [];
      } finally {
        loading.value = false;
      }
    };

    // Se llama a la función al montar el componente
    onMounted(async () => {
      if (!userStore.usuario || !userStore.usuario.cedulaPersona) {
        window.location.href = "/";
      } else {
        await fetchBeneficios();
      }
    });

    return {
      beneficios,
      error,
      loading,
      matriculaLoading,
      matricularBeneficio,
    };
  },
};
</script>
