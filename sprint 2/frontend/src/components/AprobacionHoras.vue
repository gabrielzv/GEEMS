<template>
  <div class="min-h-screen bg-gray-100 p-6">
    <div class="max-w-5xl mx-auto bg-white p-6 rounded-xl shadow-md">
      <!-- Encabezado -->
      <div class="flex justify-between items-center mb-6">
        <h1 class="text-3xl font-bold text-gray-800">
          Registros de horas - {{ empresa?.nombre || "Empresa" }}
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
import { ref, onMounted, computed } from "vue";
import axios from "axios";
import { useRoute } from "vue-router";

export default {
  setup() {
    const route = useRoute();
    const empresa = ref(null);
    const registros = ref([]);
    const searchQuery = ref("");

    const filteredRegistros = computed(() => {
      if (!searchQuery.value) return registros.value;
      return registros.value.filter(registro => 
        registro.nombreEmpleado.toLowerCase().includes(searchQuery.value.toLowerCase()) ||
        registro.usuario.toLowerCase().includes(searchQuery.value.toLowerCase())
      );
    });

    const formatDate = (dateString) => {
      if (!dateString) return "No disponible";
      const date = new Date(dateString);
      return date.toLocaleDateString('es-ES', { 
        year: 'numeric', 
        month: 'long', 
        day: 'numeric',
        hour: '2-digit',
        minute: '2-digit'
      });
    };

    const updateEstado = async (registro) => {
      try {
        await axios.put(`https://localhost:7014/api/RegistroHoras/${registro.id}`, {
          estado: registro.estado
        });
        console.log(`Estado actualizado a ${registro.estado}`);
      } catch (error) {
        console.error("Error al actualizar el estado:", error);
      }
    };

     

    const fetchData = async () => {
  const empresaId = route.params.empresaId;
  try {
    // Obtener datos de la empresa
    const empresaResponse = await axios.get(
      `https://localhost:7014/api/Empresa/${empresaId}`
    );
    // Asegura que el objeto tenga la estructura correcta
    empresa.value = {
      nombre: empresaResponse.data.nombre,
      cedulaJuridica: empresaResponse.data.cedulaJuridica,
      telefono: empresaResponse.data.telefono,
      dueno: empresaResponse.data.dueno,
      correo: empresaResponse.data.correo,
    };
    console.log("Empresa cargada:", empresa.value);

    // Obtener registros de horas
    const registrosResponse = await axios.get(
      `https://localhost:7014/api/RegistroHoras/por-empresa/${empresaId}`
    );
    registros.value = registrosResponse.data;
  } catch (error) {
    console.error("Error al obtener los datos:", error);
  }
};

    onMounted(() => {
      fetchData();
    });

    return { 
      empresa, 
      registros, 
      searchQuery, 
      filteredRegistros,
      formatDate,
      updateEstado
    };
  },
};
</script>

<style scoped>
/* Estilos adicionales si son necesarios */
</style>