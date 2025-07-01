<template>
  <div class="min-h-screen bg-gray-50 py-12 px-4">
    <div class="max-w-7xl mx-auto">

      <div v-if="loading" class="text-center text-gray-600">Cargando datos...</div>

      <div v-else>
        <template v-if="userStore.usuario?.tipo === 'DuenoEmpresa'">
          <div class="space-y-12">
            <div class="flex flex-col lg:flex-row gap-8">
              <div class="w-full lg:w-1/2">
                <div class="bg-white rounded-xl shadow-md p-6 h-full">
                  <h2 class="text-xl font-semibold mb-4 text-gray-800 text-center">Distribución por tipo de contrato</h2>
                  <div class="w-full h-[28rem]">
                    <Pie :data="chartData" :options="chartOptions" />
                  </div>
                  <div class="flex justify-center gap-4 mt-6 text-sm text-gray-600">
                    <div class="flex items-center gap-2"><span class="w-3 h-3 bg-blue-400 rounded-full"></span> Tiempo completo</div>
                    <div class="flex items-center gap-2"><span class="w-3 h-3 bg-yellow-300 rounded-full"></span> Medio tiempo</div>
                    <div class="flex items-center gap-2"><span class="w-3 h-3 bg-red-400 rounded-full"></span> Por horas</div>
                  </div>
                </div>
              </div>

              <div class="w-full lg:w-1/2 flex flex-col justify-between gap-8">
                <!-- Tabla de pagos -->
                <div class="bg-white rounded-xl shadow-md p-6">
                  <h2 class="text-xl font-semibold mb-4 text-gray-800">Últimos pagos</h2>
                  <div class="overflow-x-auto">
                    <table class="w-full text-left text-sm text-gray-700">
                      <thead class="bg-gray-100 text-gray-600 uppercase text-xs tracking-wider">
                        <tr>
                          <th class="px-6 py-3">Planillas</th>
                          <th class="px-6 py-3">Fecha</th>
                          <th class="px-6 py-3">Costo Total</th>
                        </tr>
                      </thead>
                      <tbody>
                        <tr
                          v-for="(resumen, index) in resumenesPorPlanilla"
                          :key="index"
                          @click="handleSeleccionarPlanilla(resumen)"
                          class="hover:bg-blue-50 hover:shadow-sm border-l-4 border-transparent hover:border-blue-500 transition-all cursor-pointer"
                        >
                          <td class="px-6 py-4 font-semibold text-blue-800">Planilla</td>
                          <td class="px-6 py-4">
                            Del {{ formatearFecha(resumen.fechaInicio) }} al {{ formatearFecha(resumen.fechaFinal) }}
                          </td>
                          <td class="px-6 py-4">{{ formatearColones(resumen.costoTotalEmpleador || 0) }}</td>
                        </tr>
                      </tbody>
                    </table>
                  </div>
                </div>

                <!-- Gráfico detalle planilla -->
                <div v-if="planillaSeleccionada" class="bg-white rounded-xl shadow-md p-6">
                  <h3 class="text-lg font-bold mb-4 text-gray-800 text-center">
                    Detalle de la planilla: Del {{ formatearFecha(planillaSeleccionada.fechaInicio) }} al {{ formatearFecha(planillaSeleccionada.fechaFinal) }}
                  </h3>
                  <div class="w-full h-64">
                    <Pie :data="chartDetalleResumen" :options="{ responsive: true, plugins: { legend: { position: 'bottom' } } }" />
                  </div>
                  <p class="mt-4 text-sm text-gray-700 font-semibold text-center">
                    Costo Total: {{ formatearColones(planillaSeleccionada.costoTotalEmpleador || 0) }}
                  </p>
                </div>
              </div>
            </div>
          </div>
        </template>
        
        <!-- Dashboard empleado -->

        <template v-else-if="userStore.usuario?.tipo === 'Empleado'">
          <section class="bg-white rounded-xl shadow-md p-6 space-y-6">
            <h2 class="text-2xl font-semibold text-center text-gray-800">Historial de pagos</h2>

           
            <div class="w-full h-96 flex items-center justify-center" v-if="loadingPagosEmpleado">
              <p class="text-gray-500">Cargando datos...</p>
            </div>

            <!-- Contenido -->
            <div v-else class="w-full h-full">
              <!-- Gráfico de barras -->
              <Bar
                :data="chartEmpleadoData"
                :options="chartEmpleadoOptions"
                :plugins="[ChartDataLabels]"
              />

              <!-- Detalle de deducciones -->
              <div
                class="mt-6 bg-gray-50 p-6 rounded-xl shadow-inner"
                v-if="pagoSeleccionadoId && deduccionesPorPago[pagoSeleccionadoId]"
              >
                <h3 class="text-md font-bold text-gray-700 mb-4 text-center">
                  Detalle de deducciones para el pago seleccionado
                </h3>

                <div class="flex flex-col md:flex-row gap-6">
                  <!-- Lista de deducciones dividida -->
                  <div class="md:w-1/2 space-y-4">
                    <div v-if="deduccionesPorPago[pagoSeleccionadoId].some(d => d.tipoDeduccion?.toLowerCase() === 'voluntaria')">
                      <h4 class="text-sm font-semibold text-blue-600 mb-1">Deducciones voluntarias</h4>
                      <ul class="list-disc list-inside text-sm text-gray-700">
                        <li
                          v-for="(deduccion, i) in deduccionesPorPago[pagoSeleccionadoId].filter(d => d.tipoDeduccion?.toLowerCase() === 'voluntaria')"
                          :key="'vol-' + i"
                        >
                          {{ deduccion.nombre }}: {{ formatearColones(deduccion.monto || 0) }}
                        </li>
                      </ul>
                    </div>

                    <div v-if="deduccionesPorPago[pagoSeleccionadoId].some(d => d.tipoDeduccion?.toLowerCase() !== 'voluntaria')">
                      <h4 class="text-sm font-semibold text-yellow-600 mt-4 mb-1">Deducciones obligatorias</h4>
                      <ul class="list-disc list-inside text-sm text-gray-700">
                        <li
                          v-for="(deduccion, i) in deduccionesPorPago[pagoSeleccionadoId].filter(d => d.tipoDeduccion?.toLowerCase() !== 'voluntaria')"
                          :key="'obl-' + i"
                        >
                          {{ deduccion.nombre }}: {{ formatearColones(deduccion.monto || 0) }}
                        </li>
                      </ul>
                    </div>
                  </div>

                  <!-- Gráfico de pastel de las deducciones -->
                  <div class="md:w-1/2 flex items-center justify-center">
                    <div class="w-full max-w-[200px] h-[200px]">
                      <Pie
                        v-if="chartDeduccionPorTipo"
                        :data="chartDeduccionPorTipo"
                        :options="{ responsive: true, plugins: { legend: { position: 'bottom' } } }"
                      />
                    </div>
                  </div>
                </div>

                
              </div>
            </div>
          </section>
        </template>


      </div>
    </div>
  </div>
</template>



<script setup>
import { ref, computed, onMounted } from 'vue'
import { Pie } from 'vue-chartjs'
import {
  Chart as ChartJS,
  Title,
  Tooltip,
  Legend,
  ArcElement, 
  BarElement,
  CategoryScale,  
  LinearScale
} from 'chart.js'
import { useUserStore } from '@/store/user'
import axios from 'axios'
import { API_BASE_URL } from '@/config'
import { Bar } from 'vue-chartjs'
import ChartDataLabels from 'chartjs-plugin-datalabels'
ChartJS.register(ChartDataLabels)


ChartJS.register(Title, Tooltip, Legend, ArcElement, BarElement, CategoryScale, LinearScale)

const userStore = useUserStore()
const empleados = ref([])
const planillas = ref([])
const resumenesPorPlanilla = ref([])
const planillaSeleccionada = ref(null)
const loading = ref(true)
const salariosPorContrato = ref([])
const deduccionesEmpleador = ref([])
const deduccionesObligatorias = ref([])
const beneficios = ref([])
const chartEmpleadoData = ref(null)
const loadingPagosEmpleado = ref(true)
const deduccionesPorPago = ref({})
const pagosOrdenados = ref([])
const pagoSeleccionadoId = ref(null)



const chartEmpleadoOptions = {
  responsive: true,
  plugins: {
    legend: {
      position: 'top'
    },
    tooltip: {
      mode: 'index',
      intersect: false
    },
    datalabels: {
      anchor: 'end',
      align: 'top',
      formatter: (value) => new Intl.NumberFormat('es-CR').format(value),
      color: '#374151',
      font: {
        weight: 'bold'
      }
    }
  },
  onClick: (e, elements) => {
    if (elements.length > 0) {
      const index = elements[0].index
      const idPago = pagosOrdenados.value[index]?.id
      pagoSeleccionadoId.value = idPago
    }
  },
  scales: {
    y: {
      beginAtZero: true
    }
  }
}




const cargarPagosEmpleado = async () => {
  try {
    const empleadoId = userStore.usuario?.id
    if (!empleadoId) return

    const res = await axios.get(`${API_BASE_URL}Pagos/${empleadoId}`)

    pagosOrdenados.value = res.data
      .sort((a, b) => new Date(b.fechaFinal) - new Date(a.fechaFinal))
      .slice(0, 2)

    const labels = pagosOrdenados.value.map(p =>
      `${new Date(p.fechaInicio).toLocaleDateString()} - ${new Date(p.fechaFinal).toLocaleDateString()}`
    )

    const montoBruto = pagosOrdenados.value.map(p => p.montoBruto)
    const montoNeto = pagosOrdenados.value.map(p => p.montoPago)
    const deducciones = pagosOrdenados.value.map(p => p.montoBruto - p.montoPago)

    chartEmpleadoData.value = {
      labels,
      datasets: [
        {
          label: 'Salario bruto',
          data: montoBruto,
          backgroundColor: '#60A5FA'
        },
        {
          label: 'Pago neto',
          data: montoNeto,
          backgroundColor: '#10B981'
        },
        {
          label: 'Deducciones',
          data: deducciones,
          backgroundColor: '#F87171'
        }
      ]
    }

    // Obtener deducciones por pago
    for (const pago of pagosOrdenados.value) {
      try {
        const resDeducciones = await axios.get(`${API_BASE_URL}Deducciones/${pago.id}`)
        deduccionesPorPago.value[pago.id] = resDeducciones.data
      } catch (err) {
        console.warn(`No se pudieron obtener deducciones para el pago ${pago.id}`, err)
        deduccionesPorPago.value[pago.id] = []
      }
    }

  } catch (err) {
    console.error('Error al cargar pagos del empleado:', err)
  } finally {
    loadingPagosEmpleado.value = false
  }
}

const chartDeduccionPorTipo = computed(() => {
  const id = pagoSeleccionadoId.value
  const lista = deduccionesPorPago.value[id]
  if (!id || !lista) return null

  let totalObligatorias = 0
  let totalVoluntarias = 0

  lista.forEach(d => {
    if ((d.tipoDeduccion || '').toLowerCase() === 'voluntaria') {
      totalVoluntarias += d.monto || 0
    } else {
      totalObligatorias += d.monto || 0
    }
  })

  return {
    labels: ['Obligatorias', 'Voluntarias'],
    datasets: [{
      data: [totalObligatorias, totalVoluntarias],
      backgroundColor: ['#fbbf24', '#3b82f6']
    }]
  }
})


// Formato utilidades
const formatearFecha = (fechaStr) => {
  const fecha = new Date(fechaStr)
  return fecha.toLocaleDateString('es-CR', {
    day: '2-digit',
    month: '2-digit',
    year: 'numeric'
  })
}

const formatearColones = (valor) => {
  return new Intl.NumberFormat('es-CR', {
    style: 'currency',
    currency: 'CRC',
    minimumFractionDigits: 0
  }).format(valor)
}

// Cargar empleados
const cargarEmpleados = async () => {
  try {
    const resultados = await Promise.all(
      userStore.empleadosEmpresa.map(async (e) => {
        try {
          const res = await axios.get(`${API_BASE_URL}GetEmpleado/${e.cedula}`)
          return res.data
        } catch (err) {
          console.warn(`No se pudo obtener el empleado ${e.cedula}`, err)
          return null
        }
      })
    )
    empleados.value = resultados.filter(e => e !== null)
  } catch (err) {
    console.error('Error al cargar empleados:', err)
    empleados.value = []
  }
}

// Cargar planillas
const cargarPlanillas = async () => {
  try {
    const nombreEmpresa = userStore.empresa?.nombre
    const res = await axios.get(`${API_BASE_URL}Planilla/listar`, {
      params: { nombreEmpresa }
    })
    planillas.value = res.data
  } catch (error) {
    console.error('Error al cargar planillas:', error)
    planillas.value = []
  }
}


// Cargar resumen por planilla
const cargarResumenesPorPlanilla = async () => {
  const nombreEmpresa = userStore.empresa?.nombre
  const resultados = []

  for (const planilla of planillas.value) {
    try {
      const resResumen = await axios.get(`${API_BASE_URL}Pagos/resumenPlanilla`, {
        params: {
          nombreEmpresa,
          fechaInicio: planilla.fechaInicio,
          fechaFin: planilla.fechaFinal
        }
      })

      const resSalarios = await axios.get(`${API_BASE_URL}Reporte/salariosPorContrato/${planilla.id}`)
      const salarios = resSalarios.data.map(s => s.totalSalario || s.TotalSalario || 0)
      const totalSalarios = salarios.reduce((acc, cur) => acc + cur, 0)

      const resDedEmpleador = await axios.get(`${API_BASE_URL}Deduccion/DeduccionesDetalladas/${totalSalarios}`)
      const totalDeduccionesEmpleador = resDedEmpleador.data.deducciones.reduce((sum, d) => sum + (d.monto || 0), 0)

      const resDeducciones = await axios.get(`${API_BASE_URL}Reporte/deduccionesPorPlanilla/${planilla.id}`)
      
      const beneficios = resDeducciones.data.filter(d => d.esBeneficio)
      const totalBeneficios = beneficios.reduce((sum, d) => sum + (d.total || 0), 0)

      const resDeducciones2 = await axios.get(`${API_BASE_URL}Reporte/deduccionesPorPlanilla/${planilla.id}`)
      const deducciones = resDeducciones2.data
      
      const mapObligatorias = {}
      deducciones.forEach(d => {
        if (!d.esBeneficio) {
          if (!mapObligatorias[d.nombre]) {
            mapObligatorias[d.nombre] = { ...d }
          } else {
            mapObligatorias[d.nombre].total += d.total
          }
        }
      })
      deduccionesObligatorias.value = Object.values(mapObligatorias)

      const totalPagosLey = deduccionesObligatorias.value.reduce((sum, d) => sum + (d.total || 0), 0)


      const totalCosto = totalSalarios + totalDeduccionesEmpleador + totalBeneficios + totalPagosLey

      resultados.push({
        idPlanilla: planilla.id,
        fechaInicio: planilla.fechaInicio,
        fechaFinal: planilla.fechaFinal,
        resumen: resResumen.data,
        costoTotalEmpleador: totalCosto
      })
    } catch (error) {
      console.error(`Error al obtener datos para planilla ${planilla.id}:`, error)
    }
  }

  resumenesPorPlanilla.value = resultados
}

// Cargar datos detallados de la planilla
const cargarDetallePlanilla = async (idPlanilla) => {
  try {
    const resSalarios = await axios.get(`${API_BASE_URL}Reporte/salariosPorContrato/${idPlanilla}`)
    const tipos = ["Tiempo Completo", "Medio Tiempo", "Servicios Profesionales", "Por Horas"]
    salariosPorContrato.value = tipos.map(tipo => {
      const found = resSalarios.data.find(s => s.tipoContrato === tipo || s.TipoContrato === tipo)
      return {
        TipoContrato: tipo,
        TotalSalario: found ? found.totalSalario || found.TotalSalario : 0
      }
    })

    // Cargar deducciones empleador
    const totalSalarios = salariosPorContrato.value.reduce((sum, s) => sum + (s.TotalSalario || 0), 0)
    const resDedEmpleador = await axios.get(`${API_BASE_URL}Deduccion/DeduccionesDetalladas/${totalSalarios}`)
    

    // Cargar deducciones obligatorias y beneficios
    const resDeducciones = await axios.get(`${API_BASE_URL}Reporte/deduccionesPorPlanilla/${idPlanilla}`)
    const deducciones = resDeducciones.data
    
    // Agrupar deducciones obligatorias
    const mapObligatorias = {}
    deducciones.forEach(d => {
      if (!d.esBeneficio) {
        if (!mapObligatorias[d.nombre]) {
          mapObligatorias[d.nombre] = { ...d }
        } else {
          mapObligatorias[d.nombre].total += d.total
        }
      }
    })
    deduccionesObligatorias.value = Object.values(mapObligatorias)

    // Agrupar beneficios
    const mapBeneficios = {}
    deducciones.forEach(d => {
      if (d.esBeneficio) {
        if (!mapBeneficios[d.nombre]) {
          mapBeneficios[d.nombre] = { ...d }
        } else {
          mapBeneficios[d.nombre].total += d.total
        }
      }
    })
    beneficios.value = Object.values(mapBeneficios)

  } catch (error) {
    console.error('Error al cargar detalle de planilla:', error)
    salariosPorContrato.value = []
    deduccionesEmpleador.value = []
    deduccionesObligatorias.value = []
    beneficios.value = []
  }
}

// Seleccionar fila
const handleSeleccionarPlanilla = async (resumen) => {
  planillaSeleccionada.value = resumen
  await cargarDetallePlanilla(resumen.idPlanilla)
}

// Datos para el gráfico del detalle
const chartDetalleResumen = computed(() => {
  if (!planillaSeleccionada.value) return null

  const totalSalarios = salariosPorContrato.value.reduce((sum, s) => sum + (s.TotalSalario || 0), 0)
  const totalPagosLey = deduccionesObligatorias.value.reduce((sum, d) => sum + (d.total || 0), 0)
  const totalPagosLeyConEmpleador = totalPagosLey +
    deduccionesEmpleador.value.reduce((sum, d) => sum + (d.monto || 0), 0)
  const totalBeneficios = beneficios.value.reduce((sum, d) => sum + (d.total || 0), 0)

  return {
    labels: ['Salarios', 'Pagos de Ley', 'Beneficios'],
    datasets: [{
      data: [totalSalarios, totalPagosLeyConEmpleador, totalBeneficios],
      backgroundColor: ['#60a5fa', '#f87171', '#34d399']
    }]
  }
})

// Datos del gráfico general
const chartData = computed(() => {
  const conteo = {
    'Tiempo completo': 0,
    'Medio tiempo': 0,
    'Por horas': 0
  }

  empleados.value.forEach(e => {
    const tipo = (e.contrato || '').toLowerCase()
    if (tipo.includes('tiempo completo')) conteo['Tiempo completo']++
    else if (tipo.includes('medio tiempo')) conteo['Medio tiempo']++
    else if (tipo.includes('hora') || tipo.includes('por horas')) conteo['Por horas']++
  })

  return {
    labels: Object.keys(conteo),
    datasets: [{
      data: Object.values(conteo),
      backgroundColor: ['#60a5fa', '#fcd34d', '#f87171']
    }]
  }
})

const chartOptions = {
  responsive: true,
  plugins: {
    legend: {
      position: 'bottom'
    }
  }
}

// Carga inicial
onMounted(async () => {
  if (!userStore.empresa && userStore.usuario?.cedulaPersona) {
    await userStore.fetchEmpresa(userStore.usuario.cedulaPersona)
  }

  if (userStore.usuario?.tipo === 'Empleado') {
    await cargarPagosEmpleado()
  }

  await Promise.all([cargarEmpleados(), cargarPlanillas()])
  await cargarResumenesPorPlanilla()
  loading.value = false
})
</script>