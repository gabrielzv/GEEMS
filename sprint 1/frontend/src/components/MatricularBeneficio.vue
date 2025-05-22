<template>
  <div class="flex justify-center items-center min-h-screen bg-gray-100">
    <div class="bg-white shadow rounded p-6 max-w-md w-full">
      <h1 class="text-2xl font-bold mb-4 text-center">
        Beneficios de la Empresa
      </h1>

      <!-- Mensaje de error -->
      <p v-if="error" class="text-red-500 mt-4 text-center">{{ error }}</p>

      <!-- Lista de beneficios disponibles a matricular-->
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
            <button
              @click="matricularBeneficio(beneficio.id)"
              class="bg-blue-500 text-white py-2 my-2 px-4 rounded hover:bg-blue-600"
              type="button"
            >
              Matricular beneficio
            </button>
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

    // Método para obtener la empresa del usuario y después los beneficios
    const fetchBeneficios = async () => {
      try {
        await userStore.fetchEmpleado(userStore.usuario.cedulaPersona);

        if (userStore.empleado && userStore.empleado.nombreEmpresa) {
          const nombreEmpresa = userStore.empleado.nombreEmpresa;

          // Se obtiene la cédula jurídica de la empresa usando el nombre
          const cedulaResponse = await axios.get(
            `https://localhost:7014/api/Empresa/cedula-juridica/${nombreEmpresa}`
          );
          const cedulaJuridica = cedulaResponse.data.cedulaJuridica;

          // Se hace el get para obtener los beneficios creados de la empresa
          const beneficiosResponse = await axios.get(
            `https://localhost:7014/api/GetCompanyBenefits/${cedulaJuridica}`
          );
          beneficios.value = beneficiosResponse.data;
        } else {
          error.value =
            "No se pudo obtener el nombre de la empresa del empleado.";
        }
      } catch (err) {
        console.error("Error al obtener los beneficios:", err);
        error.value =
          err.response?.data?.message ||
          "Ocurrió un error al obtener los beneficios.";
      }
    };
    // Método para crear el beneficio
    const matricularBeneficio = async (beneficioId) => {
      try {
        const empleadoId = userStore.empleado?.id;

        if (!empleadoId) {
          alert("No se pudo obtener la información del empleado.");
          return;
        }

        // Se hace el post para poder matricular el beneficio
        const response = await axios.post(
          "https://localhost:7014/api/SetBeneficioPorEmpleado/matricularBeneficio",
          {
            IdEmpleado: empleadoId,
            IdBeneficio: beneficioId,
          }
        );
        alert(response.data);

        // Actualizar la lista de beneficios disponibles
        beneficios.value = beneficios.value.filter((b) => b.id !== beneficioId);
      } catch (error) {
        console.error("Error al matricular el beneficio:", error);
        alert(
          error.response?.data?.message ||
            "Ocurrió un error al intentar matricular el beneficio."
        );
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
      matricularBeneficio,
    };
  },
};
</script>
