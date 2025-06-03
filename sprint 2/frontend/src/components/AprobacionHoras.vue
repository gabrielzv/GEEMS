<template>
  <div class="min-h-screen bg-gray-100 p-6">
    <div class="max-w-5xl mx-auto bg-white p-6 rounded-xl shadow-md">
      <!-- Encabezado -->
      <div class="flex justify-between items-center mb-6">
        <h1 class="text-3xl font-bold text-gray-800">
          Aprobar horas - {{ empresa?.nombre || "Nombre de empresa" }}
        </h1>
      </div>

      <!-- Barra de búsqueda -->
      <div class="mb-6">
        <input
          type="text"
          placeholder="Buscar empleado..."
          class="w-full p-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
          v-model="searchQuery"
        />
      </div>

      <!-- Tabla de registros -->
      <div class="overflow-x-auto">
        <table class="min-w-full bg-white">
          <thead>
            <tr class="bg-gray-200 text-gray-700">
              <th class="py-3 px-4 text-left">Nombre</th>
              <th class="py-3 px-4 text-left">Usuario</th>
              <th class="py-3 px-4 text-center">Horas</th>
              <th class="py-3 px-4 text-left">Fecha de solicitud</th>
              <th class="py-3 px-4 text-left">Estado</th>
            </tr>
          </thead>
          <tbody class="text-gray-700">
            <tr v-for="registro in filteredRegistros" :key="registro.id" class="border-b border-gray-200 hover:bg-gray-50">
              <td class="py-3 px-4">{{ registro.nombreEmpleado }}</td>
              <td class="py-3 px-4">{{ registro.usuario }}</td>
              <td class="py-3 px-4 text-center">{{ registro.horas || 0 }}</td>
              <td class="py-3 px-4">{{ formatDate(registro.fechaSolicitud) }}</td>
              <td class="py-3 px-4">
                <select 
                  v-model="registro.estado" 
                  :class="[
                    'border rounded-md p-1 focus:outline-none focus:ring-2 focus:ring-blue-500',
                    {
                      'bg-green-100 border-green-500 text-green-800': registro.estado === 'Aprobado',
                      'bg-red-100 border-red-500 text-red-800': registro.estado === 'Denegado',
                      'bg-gray-100 border-gray-300 text-gray-800': registro.estado === 'NoRevisado'
                    }
                  ]"
                  @change="updateEstado(registro)"
                >
                  <option value="NoRevisado" class="bg-gray-100">No revisado</option>
                  <option value="Aprobado" class="bg-green-100">Aprobado</option>
                  <option value="Denegado" class="bg-red-100">Denegado</option>
                </select>
              </td>
            </tr>
            <tr v-if="filteredRegistros.length === 0">
              <td colspan="5" class="py-4 text-center text-gray-500">
                No se encontraron registros de horas
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>
  </div>
</template>

<script>
import { useUserStore } from "../store/user";
import { onMounted, ref, computed } from "vue";
import { useRouter } from "vue-router";

export default {
  setup() {
    const router = useRouter();
    const userStore = useUserStore();
    const empresa = ref(null);
    const searchQuery = ref("");
    const registrosHoras = ref([]);
    const isLoading = ref(false);
    const error = ref(null);

    // Computed property para filtrar registros
    const filteredRegistros = computed(() => {
      if (!searchQuery.value) {
        return registrosHoras.value;
      }
      const query = searchQuery.value.toLowerCase();
      return registrosHoras.value.filter(registro =>
        registro.nombreEmpleado.toLowerCase().includes(query) ||
        registro.usuario.toLowerCase().includes(query)
      );
    });

    // Función para formatear fechas
    const formatDate = (fecha) => {
      if (!fecha) return "";
      const date = new Date(fecha);
      return date.toLocaleDateString('es-ES', {
        year: 'numeric',
        month: '2-digit',
        day: '2-digit',
        hour: '2-digit',
        minute: '2-digit'
      });
    };

    // Función para actualizar estado
    const updateEstado = async (registro) => {
      try {
        const response = await fetch(`/api/RegistroHoras/${registro.id}`, {
          method: 'PUT',
          headers: {
            'Content-Type': 'application/json',
            'Authorization': `Bearer ${userStore.token}` // Si usas autenticación
          },
          body: JSON.stringify({
            estado: registro.estado
          })
        });

        if (!response.ok) {
          const errorData = await response.json();
          throw new Error(errorData.message || 'Error al actualizar el estado');
        }

        // Actualizar el estado localmente para reflejar el cambio inmediato
        const index = registrosHoras.value.findIndex(r => r.id === registro.id);
        if (index !== -1) {
          registrosHoras.value[index].estado = registro.estado;
        }
      } catch (err) {
        console.error("Error al actualizar estado:", err);
        error.value = err.message;
        // Revertir el cambio en el frontend si falla
        registro.estado = registro.estado === 'Aprobado' ? 'Denegado' : 'Aprobado';
      }
    };

    // Función para obtener registros de horas desde el backend
    const fetchRegistrosHoras = async (cedulaJuridica) => {
  try {
    // Usa la URL completa como en Swagger
    const url = `https://localhost:7014/api/RegistroHoras/por-empresa/${cedulaJuridica}`;
    
    const response = await fetch(url, {
      headers: {
        'Accept': '*/*',
        'Content-Type': 'application/json'
      },
      credentials: 'include'
    });

    if (!response.ok) throw new Error(`Error HTTP: ${response.status}`);
    
    registrosHoras.value = await response.json();
  } catch (error) {
    console.error("Error al obtener registros:", error);
    error.value = `Error al cargar registros: ${error.message}`;
  }
};

    const fetchEmpresaData = async () => {
      try {
        await userStore.fetchEmpresa(userStore.usuario.cedulaPersona);
        empresa.value = userStore.empresa;
        
        if (empresa.value?.cedulaJuridica) {
          await fetchRegistrosHoras(empresa.value.cedulaJuridica);
        }
      } catch (err) {
        console.error("Error al obtener los datos de la empresa:", err);
        error.value = err.message;
      }
    };

    onMounted(() => {
      if (!userStore.usuario || !userStore.usuario.cedulaPersona) {
        router.push("/");
      } else {
        fetchEmpresaData();
      }
    });

    return { 
      empresa, 
      searchQuery,
      registrosHoras,
      filteredRegistros,
      isLoading,
      error,
      formatDate,
      updateEstado
    };
  },
};
</script>

<style scoped>
</style>