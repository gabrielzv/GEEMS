<template>
  <div class="container mx-auto px-4 py-8 max-w-[1200px]">
    <!-- Encabezado -->
    <div class="flex justify-between items-center mb-8 border-b pb-4">
      <h1 class="text-2xl font-bold">Beneficios de la empresa</h1>
    </div>

    <!-- Filtro -->
    <div class="bg-white p-4 rounded-lg shadow mb-6">
      <label class="block text-sm font-medium mb-1"
        >Buscar por nombre de beneficio</label
      >
      <input
        v-model="searchName"
        type="text"
        placeholder="Escribe el nombre del beneficio"
        class="w-full px-3 py-2 border border-gray-300 rounded focus:outline-none focus:ring-1 focus:ring-gray-400"
      />
    </div>

    <!-- Tabla -->
    <div class="bg-white rounded-lg shadow overflow-hidden">
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
              Tiempo mínimo en empresa (meses)
            </th>
            <th
              class="px-6 py-3 text-center text-xs font-medium text-gray-500 uppercase"
            >
              Acciones
            </th>
          </tr>
        </thead>
        <tbody class="bg-white divide-y divide-gray-200 text-center">
          <tr
            v-for="beneficio in filteredBenefits"
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
              {{ beneficio.tiempoMinimoEnEmpresa }}
            </td>
            <td class="px-6 py-4 text-sm font-medium">
              <button
                v-if="!isMatriculado(beneficio.nombre)"
                @click="matricularBeneficio(beneficio.id)"
                class="text-blue-600 hover:text-blue-800"
              >
                Matricular beneficio
              </button>
              <button
                v-else
                @click="eliminarBeneficio(beneficio.id)"
                class="text-red-600 hover:text-red-800"
              >
                Eliminar beneficio
              </button>
            </td>
          </tr>
        </tbody>
      </table>
    </div>
  </div>
</template>

<script>
import axios from "axios";
import { useUserStore } from "../store/user";
import { onMounted, ref, computed } from "vue";

export default {
  setup() {
    const userStore = useUserStore();
    const beneficios = ref([]);
    const error = ref("");
    const searchName = ref("");
    const beneficiosPorEmpleado = ref([]);

    // Se revisa si el beneficio ya fue matriculado por el empleado
    const isMatriculado = (nombre) => {
      return beneficiosPorEmpleado.value.some(
        (beneficio) => beneficio.nombre === nombre
      );
    };

    // Filtro para buscar beneficios por nombre
    const filteredBenefits = computed(() => {
      if (!searchName.value) return beneficios.value;
      return beneficios.value.filter((beneficio) =>
        beneficio.nombre.toLowerCase().includes(searchName.value.toLowerCase())
      );
    });

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

          // Se hace el get para obtener los beneficios matriculados por empleado
          const idEmpleado = userStore.empleado.id;
          const beneficiosPorEmpleadoResponse = await axios.get(
            `https://localhost:7014/api/GetEmployeeBenefits/${idEmpleado}`
          );
          beneficiosPorEmpleado.value = beneficiosPorEmpleadoResponse.data;

          console.log("Beneficios cargados:", beneficios.value);
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
        // Se actualiza la lista despues de matricular el beneficio
        await fetchBeneficios();
      } catch (error) {
        console.error("Error al matricular el beneficio:", error);
        alert(
          error.response?.data?.message ||
            "Ocurrió un error al intentar matricular el beneficio."
        );
      }
    };

    // Método para eliminar el beneficio de manera simulada
    const eliminarBeneficio = async (beneficioId) => {
      alert("Simulación de eliminar el beneficio con el ID: " + beneficioId);
      // Se actualiza la lista después de la eliminación
      await fetchBeneficios();
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
      filteredBenefits,
      searchName,
      beneficiosPorEmpleado,
      isMatriculado,
      eliminarBeneficio,
    };
  },
};
</script>
