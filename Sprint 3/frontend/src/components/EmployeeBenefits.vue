<template>
  <div class="container mx-auto px-4 py-8 max-w-[1200px]">
    <!-- Encabezado -->
    <div class="flex justify-between items-center mb-8 border-b pb-4">
      <h1 class="text-2xl font-bold">Beneficios matriculados</h1>
    </div>

    <!-- Tabla -->
    <div
      v-if="beneficios.length"
      class="bg-white rounded-lg shadow overflow-hidden"
    >
      <table class="min-w-full divide-y divide-gray-200">
        <thead class="bg-gray-50">
          <tr>
            <th
              class="px-6 py-3 text-center text-xs font-medium text-gray-500 uppercase"
            >
              Nombre
            </th>
            <th
              class="px-6 py-3 text-center text-xs font-medium text-gray-500 uppercase"
            >
              Descripción
            </th>
            <th
              class="px-6 py-3 text-center text-xs font-medium text-gray-500 uppercase"
            >
              Costo
            </th>
            <th
              class="px-6 py-3 text-center text-xs font-medium text-gray-500 uppercase"
            >
              Frecuencia de cobro
            </th>
          </tr>
        </thead>
        <tbody class="bg-white divide-y divide-gray-200 text-center">
          <tr
            v-for="beneficio in beneficios"
            :key="beneficio.id"
            class="hover:bg-gray-50 transition"
          >
            <td class="px-6 py-4 text-sm text-gray-900">
              {{ beneficio.nombre }}
            </td>
            <td class="px-6 py-4 text-sm text-gray-500">
              {{ beneficio.descripcion }}
            </td>
            <td class="px-6 py-4 text-sm text-gray-500">
              {{ beneficio.costo }}
            </td>
            <td class="px-6 py-4 text-sm text-gray-500">
              {{ beneficio.frecuencia }}
            </td>
          </tr>
        </tbody>
      </table>
    </div>

    <!-- Mensaje si no hay beneficios matriculados -->
    <div v-else class="text-center text-red-500 mt-4">
      Todavía no se han matriculado beneficios.
      <br />
      <br />
      <p class="text-gray-500">
        Puedes matricular beneficios desde la sección de "Beneficios" en la
        barra superior de navegación, en la opción "Matricular Beneficios".
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

    // Método para obtener los beneficios del empleado
    const fetchBeneficiosEmpleado = async () => {
      try {
        // Se llama a fetchEmpleado para obtener la información del empleado
        await userStore.fetchEmpleado(userStore.usuario.cedulaPersona);

        if (userStore.empleado && userStore.empleado.id) {
          const idEmpleado = userStore.empleado.id;

          // Se hace el get para obtener los beneficios que tiene matriculados el empleado
          const response = await axios.get(
            `https://localhost:7014/api/GetEmployeeBenefits/${idEmpleado}`
          );
          beneficios.value = response.data;
        } else {
          error.value = "No se pudo obtener la información del empleado.";
        }
      } catch (err) {
        console.error("Error al obtener los beneficios matriculados:", err);
        error.value =
          err.response?.data?.message ||
          "Ocurrió un error al obtener los beneficios matriculados.";
      }
    };

    // Llama a la función al montar el componente
    onMounted(() => {
      if (!userStore.usuario || !userStore.usuario.cedulaPersona) {
        window.location.href = "/";
      } else {
        fetchBeneficiosEmpleado();
      }
    });

    return {
      beneficios,
      error,
    };
  },
};
</script>
