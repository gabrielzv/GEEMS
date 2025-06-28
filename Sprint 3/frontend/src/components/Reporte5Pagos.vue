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
            <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Nombre empleado</th>
            <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Cédula</th>
            <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Tipo de empleado</th>
            <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Período de pago</th>
            <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Fecha de pago</th>
            <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Salario Bruto</th>
            <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Cargas sociales empleador</th>
            <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Deducciones voluntarias</th>
            <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Costo empleador</th>
          </tr>
        </thead>
        <tbody class="bg-white divide-y divide-gray-200">
          <tr v-for="(registro, index) in pagos" :key="index">
            <td class="px-6 py-4 whitespace-nowrap">
              <div class="text-sm font-medium text-gray-900">{{ registro.empleado.nombre }}</div>
            </td>
            <td class="px-6 py-4 whitespace-nowrap">
              <div class="text-sm text-gray-500">{{ registro.empleado.cedula }}</div>
            </td>
            <td class="px-6 py-4 whitespace-nowrap">
              <div class="text-sm text-gray-500">{{ registro.empleado.tipo }}</div>
            </td>
            <td class="px-6 py-4 whitespace-pre-wrap">
              <div class="text-sm text-gray-500">
                Del<br>{{ formatearFecha(registro.pago.fechaInicio) }}<br>al<br>{{ formatearFecha(registro.pago.fechaFin) }}
              </div>
            </td>
            <td class="px-6 py-4 whitespace-nowrap">
              <div class="text-sm text-gray-500">{{ formatearFecha(registro.pago.fechaPago) }}</div>
            </td>
            <td class="px-6 py-4 whitespace-nowrap">
              <div class="text-sm text-gray-500">{{ registro.pago.salarioBruto }}</div>
            </td>
            <!-- Puedes rellenar las siguientes celdas con datos reales cuando estén disponibles -->
            <td class="px-6 py-4 whitespace-nowrap"><div class="text-sm text-gray-500">-</div></td>
            <td class="px-6 py-4 whitespace-nowrap"><div class="text-sm text-gray-500">-</div></td>
            <td class="px-6 py-4 whitespace-nowrap"><div class="text-sm text-gray-500">-</div></td>
          </tr>
        </tbody>
      </table>
    </div>
  </div>
</template>

<script>
import { useUserStore } from '@/store/user'
import { ref, computed, onMounted } from 'vue'
import axios from 'axios'

const pagos = ref([])

export default {
  setup() {
    const userStore = useUserStore()
    const empleados = ref([])
    const loading = ref(true)
    const baseUrl = 'https://localhost:7014'

    const nombreEmpresa = computed(() => {
      return userStore.empleado?.nombreEmpresa || 
             userStore.empresa?.nombre || 
             "Nombre de empresa"
    })

    const formatearNombre = (empleado) => {
      if (empleado.nombreCompleto) return empleado.nombreCompleto
      return `${empleado.apellido1 || ''} ${empleado.apellido2 || ''}, ${empleado.nombre || ''}`.trim()
    }

    const formatearFecha = (fechaStr) => {
      const fecha = new Date(fechaStr)
      if (isNaN(fecha)) return ''
      return fecha.toLocaleDateString('es-CR', {
        day: '2-digit',
        month: '2-digit',
        year: 'numeric'
      })
    }

    const fetchPagosGlobal = async () => {
      loading.value = true
      try {
        const resultados = await Promise.all(
          userStore.empleadosEmpresa.map(async (e) => {
            try {
              const resEmpleado = await axios.get(`${baseUrl}/api/GetEmpleado/${e.cedula}`)
              const empleadoData = resEmpleado.data

              const resPagos = await axios.get(`${baseUrl}/api/Pagos/${empleadoData.id}`)
              const pagosEmpleado = resPagos.data

              return pagosEmpleado.map(p => ({
                empleado: {
                  nombre: formatearNombre(e),
                  cedula: empleadoData.cedulaPersona,
                  tipo: empleadoData.contrato || 'No especificado',
                  empresa: empleadoData.nombreEmpresa || '-'
                },
                pago: {
                  idPago: p.id,
                  fechaPago: p.fechaRealizada,
                  fechaInicio: p.fechaInicio,
                  fechaFin: p.fechaFinal,
                  salarioBruto: p.montoBruto
                }
              }))
            } catch (err) {
              console.warn(`No se pudo procesar al empleado ${e.cedula}`, err)
              return []
            }
          })
        )

        pagos.value = resultados.flat()
      } catch (error) {
        console.error("Error general al obtener pagos:", error)
        pagos.value = []
      } finally {
        loading.value = false
      }
    }

    onMounted(async () => {
      if (!userStore.empresa && userStore.usuario?.cedulaPersona) {
        await userStore.fetchEmpresa(userStore.usuario.cedulaPersona)
      }
      await fetchPagosGlobal()
    })

    return {
      nombreEmpresa,
      empleados,
      loading,
      pagos,
      formatearFecha
    }
  }
}
</script>

<style scoped>
.container {
  max-width: 1200px;
}
</style>
