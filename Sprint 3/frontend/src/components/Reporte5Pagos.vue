<template>
  <div class="container mx-auto p-4">
    <!-- Encabezado del reporte -->
    <div class="mb-8">
      <h1 class="text-2xl font-bold text-center uppercase">REPORTE 5</h1>
      <div class="flex justify-between items-center mt-4">
        <div>
          <h2 class="text-xl font-semibold">{{ nombreEmpresa || "Nombre de la empresa" }}</h2>
          <p class="text-gray-600">Nombre del empleador</p>
        </div>
        <button class="bg-green-500 hover:bg-green-600 text-white px-4 py-2 rounded">
          Generar Excel
        </button>
      </div>
    </div>

    <!-- Filtros -->
    <div class="bg-gray-100 p-4 rounded-lg mb-6 grid grid-cols-1 md:grid-cols-4 gap-4">
      <div>
        <label class="block text-sm font-medium text-gray-700 mb-1">Por fecha</label>
        <input type="date" class="w-full p-2 border rounded">
      </div>
      <div>
        <label class="block text-sm font-medium text-gray-700 mb-1">Tipo de empleados</label>
        <select class="w-full p-2 border rounded">
          <option>Todos</option>
          <option>Tiempo completo</option>
          <option>Medio tiempo</option>
        </select>
      </div>
      <div>
        <label class="block text-sm font-medium text-gray-700 mb-1">Por proyecto</label>
        <select class="w-full p-2 border rounded">
          <option>Todos</option>
        </select>
      </div>
      <div>
        <label class="block text-sm font-medium text-gray-700 mb-1">Cédula</label>
        <input type="text" class="w-full p-2 border rounded" placeholder="Buscar por cédula">
      </div>
    </div>

    <!-- Tabla de empleados -->
    <div v-if="loading" class="text-center py-8">
      <p class="text-gray-600">Cargando empleados...</p>
    </div>
    <div v-else class="bg-white rounded-lg shadow overflow-x-auto">
      <table class="min-w-full divide-y divide-gray-200">
        <thead class="bg-gray-50">
          <tr>
            <th scope="col" class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Nombre empleado</th>
            <th scope="col" class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Cédula</th>
            <th scope="col" class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Tipo de empleado</th>
            <th scope="col" class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Período de pago</th>
            <th scope="col" class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Fecha de pago</th>
            <th scope="col" class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Salario Bruto</th>
            <th scope="col" class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Cargas sociales empleador</th>
            <th scope="col" class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Deducciones voluntarias</th>
            <th scope="col" class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Costo empleador</th>
          </tr>
        </thead>
        <tbody class="bg-white divide-y divide-gray-200">
          <tr v-for="empleado in empleados" :key="empleado.cedula">
            <td class="px-6 py-4 whitespace-nowrap">
              <div class="text-sm font-medium text-gray-900">{{ empleado.nombre }}</div>
            </td>
            <td class="px-6 py-4 whitespace-nowrap">
              <div class="text-sm text-gray-500">{{ empleado.cedula }}</div>
            </td>
            <td class="px-6 py-4 whitespace-nowrap">
              <div class="text-sm text-gray-500">{{ empleado.tipo }}</div>
            </td>
            <td class="px-6 py-4 whitespace-nowrap">
              <div class="text-sm text-gray-500">{{ empleado.contrato || JSON.stringify(empleado) }}</div>
            </td>
            <td class="px-6 py-4 whitespace-nowrap">
              <div class="text-sm text-gray-500"></div>
            </td>
            <td class="px-6 py-4 whitespace-nowrap">
              <div class="text-sm text-gray-500"></div>
            </td>
            <td class="px-6 py-4 whitespace-nowrap">
              <div class="text-sm text-gray-500"></div>
            </td>
            <td class="px-6 py-4 whitespace-nowrap">
              <div class="text-sm text-gray-500"></div>
            </td>
            <td class="px-6 py-4 whitespace-nowrap">
              <div class="text-sm text-gray-500"></div>
            </td>
            <td class="px-6 py-4 whitespace-nowrap">
              <div class="text-sm text-gray-500"></div>
            </td>
          </tr>
        </tbody>
      </table>
      
      <!-- Mensaje cuando no hay empleados -->
      <div 
        v-if="empleados.length === 0" 
        class="px-6 py-4 text-center text-gray-500"
      >
        No se encontraron empleados
      </div>
    </div>
  </div>
</template>

<script>
import { useUserStore } from '@/store/user'
import { ref, computed, onMounted } from 'vue'

export default {
  setup() {
    const userStore = useUserStore()
    const empleados = ref([])
    const loading = ref(true)

    // Obtener nombre de la empresa desde el store
    const nombreEmpresa = computed(() => {
      return userStore.empleado?.nombreEmpresa || 
             userStore.empresa?.nombre || 
             "Nombre de empresa"
    })

    // Cargar empleados del store
    const fetchEmpleados = () => {
      try {
        loading.value = true
        
        
        if (userStore.empleadosEmpresa?.length > 0) {
          empleados.value = userStore.empleadosEmpresa.map(e => ({
          nombre: formatearNombre(e),
          cedula: e.cedula || 'N/A',
          tipo: e.contrato || 'No especificado'
        }))
        } else {
          empleados.value = []
        }
      } catch (error) {
        console.error("Error al cargar empleados:", error)
        empleados.value = []
      } finally {
        loading.value = false
      }
    }

    // Formatear nombre completo
    const formatearNombre = (empleado) => {
      if (empleado.nombreCompleto) return empleado.nombreCompleto
      return `${empleado.apellido1 || ''} ${empleado.apellido2 || ''}, ${empleado.nombre || ''}`.trim()
    }

    // Cargar datos al montar el componente
    onMounted(async () => {
      // Si no hay datos de empresa en el store, cargarlos
      if (!userStore.empresa && userStore.usuario?.cedulaPersona) {
        await userStore.fetchEmpresa(userStore.usuario.cedulaPersona)
      }
      
      // Cargar empleados desde el store
      fetchEmpleados()
    })

    return {
      nombreEmpresa,
      empleados,
      loading
    }
  }
}
</script>

<style scoped>
.container {
  max-width: 1200px;
}
</style>