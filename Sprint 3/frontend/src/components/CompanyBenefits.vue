<template>
  <div class="container mx-auto px-4 py-8 max-w-[1200px]">
    <!-- Encabezado -->
    <div class="flex justify-between items-center mb-8 border-b pb-4">
      <h1 class="text-2xl font-bold">Beneficios de la empresa</h1>
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
              Costo / Porcentaje
            </th>
            <th
              class="px-6 py-3 text-center text-xs font-medium text-gray-500 uppercase"
            >
              Tiempo mínimo en empresa (meses)
            </th>
            <th
              colspan="2"
              class="px-6 py-3 text-center text-xs font-medium text-gray-500 uppercase"
            >
              Acciones
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
              {{ beneficio.tiempoMinimo }}
            </td>
            <td class="px-6 py-4 text-sm font-medium">
              <button
                @click="EditarBeneficio(beneficio.id)"
                class="text-blue-600 hover:text-blue-800"
              >
                Editar
              </button>
            </td>
            <td class="px-6 py-4 text-sm font-medium">
              <button
                @click="EliminarBeneficio(beneficio.id)"
                class="text-red-600 hover:text-red-800"
              >
                Eliminar
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
import { onMounted, ref } from "vue";
import { useRouter } from "vue-router";
import { API_BASE_URL } from "../config";

export default {
  setup() {
    const userStore = useUserStore();
    const beneficios = ref([]);
    const error = ref("");
    const router = useRouter();

    // Método para obtener los beneficios de la empresa
    const fetchBeneficios = async () => {
      try {
        // Se llama a fetchEmpresa para obtener la cédula jurídica
        await userStore.fetchEmpresa(userStore.usuario.cedulaPersona);

        if (userStore.empresa) {
          const cedulaJuridica = userStore.empresa.cedulaJuridica;

          // Se hace el get para obtener los beneficios creados de la empresa
          const url = `${API_BASE_URL}Beneficio/Company/${cedulaJuridica}`;
          const response = await axios.get(url);
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

    const EditarBeneficio = (id) => {
      router.push({ name: "EditarBeneficio", params: { id } });
    };

    const EliminarBeneficio = (id) => {
      const confirmacion = window.confirm(
        `¿Estás seguro que deseas eliminar el beneficio ${id}?`
      );
      if (confirmacion) {
        try {
          // Se hace el delete para borrar el beneficio en la base de datos
          const url = `${API_BASE_URL}Beneficio/${id}`;
          const response = axios.delete(url);
          beneficios.value = response.data;
          // Se actualiza la lista de beneficios después de una eliminación
          fetchBeneficios();
        } catch (err) {
          console.error("Error al eliminar el beneficio:", err);
          error.value =
            err.response?.data?.message ||
            "Ocurrió un error al intentar eliminar el beneficio.";
        }
        alert(`El beneficio ${id} se ha eliminado con éxito.`);
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
      EditarBeneficio,
      EliminarBeneficio,
    };
  },
};
</script>
