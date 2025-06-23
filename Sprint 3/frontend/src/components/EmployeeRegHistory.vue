<template>
  <div class="min-h-screen bg-gray-100 p-6">
    <div class="max-w-4xl mx-auto bg-white p-6 rounded-xl shadow-md">
      <!-- Encabezado -->
      <div class="flex justify-between items-center mb-6">
        <h2 class="text-2xl font-bold text-black-700">Historial de Registros de Horas</h2>
      </div>

      <!-- Mensaje de error -->
      <div v-if="error" class="mb-4 p-4 bg-red-100 border border-red-400 text-red-700 rounded">
        {{ error }}
      </div>

      <!-- Indicador de carga -->
      <div v-if="isLoading" class="mb-4 p-4 bg-blue-100 border border-blue-400 text-blue-700 rounded">
        Cargando registros de horas...
      </div>

      <!-- Barra de búsqueda -->
      <div class="mb-6">
        <input
          type="text"
          placeholder="Buscar por fecha o estado..."
          class="w-full p-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
          v-model="searchQuery"
        />
      </div>

      <!-- Tabla de registros -->
      <div class="overflow-x-auto">
        <table class="min-w-full bg-white">
          <thead>
            <tr class="bg-gray-200 text-gray-700">
              <th class="py-3 px-4 text-left">Fecha</th>
              <th class="py-3 px-4 text-center">Horas</th>
              <th class="py-3 px-4 text-left">Estado</th>
              <th class="py-3 px-4 text-center"></th>
            </tr>
          </thead>
          <tbody class="text-gray-700">
            <tr v-for="registro in filteredRegistros" :key="registro.id" class="border-b border-gray-200 hover:bg-gray-50">
              <td class="py-3 px-4">{{ formatFecha(registro.fecha) }}</td>
              <td class="py-3 px-4 text-center">{{ registro.numHoras }}</td>
              <td class="py-3 px-4">
                <span
                  :class="{
                    'text-yellow-600': registro.estado === 'NoRevisado',
                    'text-green-600': registro.estado === 'Aprobado',
                    'text-red-600': registro.estado === 'Denegado'
                  }"
                >
                  {{ registro.estado === 'NoRevisado' ? 'No Revisado' : registro.estado }}
                </span>
              </td>
              <td class="py-3 px-4 text-center">
                <button
                  v-if="registro.estado === 'Denegado' || registro.estado === 'NoRevisado'"
                  class="px-3 py-1 text-xs bg-blue-100 text-blue-700 rounded hover:bg-blue-200 transition"
                  @click="editarRegistro(registro)"
                >
                  Editar
                </button>
              </td>
            </tr>
            <tr v-if="filteredRegistros.length === 0 && !isLoading">
              <td colspan="4" class="py-4 text-center text-gray-500">
                No hay registros de horas.
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>
  </div>
</template>

<script>
import axios from "axios";
import { useUserStore } from "../store/user";
import { ref, computed, onMounted } from "vue";
import { useRouter } from "vue-router";
import { API_BASE_URL } from "../config";
export default {
  setup() {
    const userStore = useUserStore();
    const router = useRouter();
    const registros = ref([]);
    const guidEmpleado = ref(null);
    const isLoading = ref(false);
    const error = ref(null);
    const searchQuery = ref("");

    const obtenerRegistros = async () => {
      if (!guidEmpleado.value) return;
      try {
        isLoading.value = true;
        error.value = null;
        const url = `${API_BASE_URL}Horas/getRegister/${guidEmpleado.value}`;
        const res = await axios.get(url);
        registros.value = res.data;
      } catch (e) {
        error.value = "Error al obtener los registros de horas.";
        registros.value = [];
      } finally {
        isLoading.value = false;
      }
    };

    const fetchEmpleadoId = async () => {
      if (userStore.usuario && userStore.usuario.cedulaPersona) {
        try {
          isLoading.value = true;
          const url = `${API_BASE_URL}GetEmpleado/${userStore.usuario.cedulaPersona}`;
          const res = await axios.get(url);
          guidEmpleado.value = res.data.id;
          await obtenerRegistros();
        } catch (e) {
          error.value = "No se pudo obtener el ID del empleado.";
        } finally {
          isLoading.value = false;
        }
      }
    };

    const formatFecha = (fecha) => {
      if (!fecha) return "";
      const d = new Date(fecha);
      return d.toLocaleDateString('es-ES', {
        year: 'numeric',
        month: '2-digit',
        day: '2-digit'
      });
    };

    const editarRegistro = (registro) => {
      router.push({ path: `/editarHoras/${registro.id}` });
    };

    const filteredRegistros = computed(() => {
      if (!searchQuery.value) return [...registros.value].sort((a, b) => new Date(b.fecha) - new Date(a.fecha));
      const query = searchQuery.value.toLowerCase();
      return [...registros.value]
        .filter(r =>
          formatFecha(r.fecha).includes(query) ||
          (r.estado && r.estado.toLowerCase().includes(query))
        )
        .sort((a, b) => new Date(b.fecha) - new Date(a.fecha));
    });

    onMounted(() => {
      if (!userStore.usuario || !userStore.usuario.cedulaPersona) {
        router.push("/");
      } else {
        fetchEmpleadoId();
      }
    });

    return {
      registros,
      isLoading,
      error,
      searchQuery,
      filteredRegistros,
      formatFecha,
      editarRegistro
    };
  }
};
</script>

<style scoped>
</style>