<template>
  <div class="min-h-screen bg-gray-100 p-6">
    <div class="max-w-5xl mx-auto bg-white p-6 rounded-xl shadow-md">
      <!-- Encabezado -->
      <div class="flex justify-between items-center mb-6">
        <h1 class="text-3xl font-bold text-gray-800">
          Empleados de {{ empresa?.nombre || "Empresa" }}
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

      <!-- Tabla de empleados -->
      <div class="overflow-x-auto">
        <table class="min-w-full bg-white">
          <thead>
            <tr class="bg-gray-200 text-gray-700">
              <th class="py-3 px-4 text-left">Nombre</th>
              <th class="py-3 px-4 text-left">Usuario</th>
              <th class="py-3 px-4 text-center">Horas solicitadas</th>
              <th class="py-3 px-4 text-left">Fecha de solicitud</th>
              <th class="py-3 px-4 text-left">Estado</th>
            </tr>
          </thead>
          <tbody class="text-gray-700">
            <tr v-for="empleado in filteredEmpleados" :key="empleado.id" class="border-b border-gray-200 hover:bg-gray-50">
              <td class="py-3 px-4">{{ empleado.nombre }}</td>
              <td class="py-3 px-4">{{ empleado.usuario }}</td>
              <td class="py-3 px-4 text-center">{{ empleado.horasSolicitadas }}</td>
              <td class="py-3 px-4">{{ formatDate(empleado.fechaSolicitud) }}</td>
              <td class="py-3 px-4">
                <select 
                  v-model="empleado.estado" 
                  :class="[
                    'border rounded-md p-1 focus:outline-none focus:ring-2 focus:ring-blue-500',
                    {
                      'bg-green-100 border-green-500 text-green-800': empleado.estado === 'Aprobado',
                      'bg-red-100 border-red-500 text-red-800': empleado.estado === 'Denegado',
                      'bg-gray-100 border-gray-300 text-gray-800': empleado.estado === 'No revisado'
                    }
                  ]"
                  @change="updateEstado(empleado)"
                >
                  <option value="No revisado" class="bg-gray-100">No revisado</option>
                  <option value="Aprobado" class="bg-green-100">Aprobado</option>
                  <option value="Denegado" class="bg-red-100">Denegado</option>
                </select>
              </td>
            </tr>
            <tr v-if="filteredEmpleados.length === 0">
              <td colspan="5" class="py-4 text-center text-gray-500">
                No se encontraron empleados
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

export default {
  setup() {
    const empresa = ref({
      nombre: "Tech Solutions S.A.",
      cedulaJuridica: "3-101-205045"
    });
    const empleadosEmpresa = ref([]);
    const searchQuery = ref("");

    // Datos de prueba en JSON
    const datosPrueba = [
      {
        id: 1,
        nombre: "Juan Pérez Rojas",
        usuario: "jperez",
        horasSolicitadas: 8,
        fechaSolicitud: "2023-05-15",
        estado: "No revisado"
      },
      {
        id: 2,
        nombre: "María González López",
        usuario: "mgonzalez",
        horasSolicitadas: 4,
        fechaSolicitud: "2023-05-16",
        estado: "Aprobado"
      },
      {
        id: 3,
        nombre: "Carlos Rodríguez Vargas",
        usuario: "crodriguez",
        horasSolicitadas: 12,
        fechaSolicitud: "2023-05-14",
        estado: "Denegado"
      },
      {
        id: 4,
        nombre: "Ana Méndez Soto",
        usuario: "amendez",
        horasSolicitadas: 6,
        fechaSolicitud: "2023-05-17",
        estado: "No revisado"
      }
    ];

    const filteredEmpleados = computed(() => {
      if (!searchQuery.value) return empleadosEmpresa.value;
      return empleadosEmpresa.value.filter(empleado => 
        empleado.nombre.toLowerCase().includes(searchQuery.value.toLowerCase()) ||
        empleado.usuario.toLowerCase().includes(searchQuery.value.toLowerCase())
      );
    });

    const formatDate = (dateString) => {
      if (!dateString) return "No disponible";
      const options = { year: 'numeric', month: 'long', day: 'numeric' };
      return new Date(dateString).toLocaleDateString('es-ES', options);
    };

    const updateEstado = async (empleado) => {
      try {
        // Simulación de actualización
        console.log(`Actualizando estado de ${empleado.nombre} a ${empleado.estado}`);
        // En una implementación real, aquí iría la llamada a la API
      } catch (error) {
        console.error("Error al actualizar el estado:", error);
      }
    };

    onMounted(() => {
      // Cargamos los datos de prueba al montar el componente
      empleadosEmpresa.value = datosPrueba;
    });

    return { 
      empresa, 
      empleadosEmpresa, 
      searchQuery, 
      filteredEmpleados,
      formatDate,
      updateEstado
    };
  },
};
</script>

<style scoped>
/* Estilos adicionales si son necesarios */
</style>