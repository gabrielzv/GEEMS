<template>
  <div class="min-h-screen bg-white p-6 relative">
    <!-- Botón de regreso pequeño y flotante -->
    <button
      @click="goBack"
      class="absolute top-1 left-1 w-10 h-10 flex items-center justify-center rounded-full bg-gray-100 hover:bg-gray-200 text-gray-600 hover:text-black shadow transition"
      title="Volver"
    >
      <span class="text-xl">←</span>
    </button>

    <div class="flex gap-6 w-full mt-2">
      <!-- Dropdown de planillas -->
      <div class="w-1/3 p-4 border rounded-xl shadow-md">
        <h2 class="text-lg font-semibold mb-2">Planillas disponibles</h2>
        <select
          v-model="selectedPlanillaId"
          @change="seleccionarPlanilla"
          class="w-full p-2 border rounded focus:outline-none focus:ring"
        >
          <option disabled value="">Seleccione una planilla</option>
          <option
            v-for="planilla in planillas"
            :key="planilla.id"
            :value="planilla.id"
          >
            Planilla de {{ nombreMes(planilla.fechaInicio) }}
          </option>
        </select>
      </div>

      <!-- Detalles del pago -->
      <div class="flex-1 p-6 border rounded-xl shadow-md">
        <div v-if="selectedPlanilla">
          <!-- Detalles generales -->
          <div class="mb-4 space-y-1">
            <p><strong>Nombre de la empresa:</strong> {{ userStore.empleado?.nombreEmpresa }}</p>
            <p><strong>Empleador:</strong> {{  }}</p>
            <p><strong>Fecha de planilla: </strong>{{ formatFecha(selectedPlanilla?.fechaInicio) }} - {{ formatFecha(selectedPlanilla?.fechaFinal) }}</p>
          </div>

          <!-- Salarios por tipo de contrato + total -->
          <div class="mb-4 space-y-1">
            <p>Salario empleados tiempo completo: ₡{{ salariosPorContrato.find(s => s.TipoContrato === 'Tiempo Completo')?.TotalSalario.toLocaleString() || 0 }}</p>
            <p>Salario empleados medio tiempo: ₡{{ salariosPorContrato.find(s => s.TipoContrato === 'Medio Tiempo')?.TotalSalario.toLocaleString() || 0 }}</p>
            <p>Salario empleados servicios profesionales: ₡{{ salariosPorContrato.find(s => s.TipoContrato === 'Servicios Profesionales')?.TotalSalario.toLocaleString() || 0 }}</p>
            <p>Salario empleados por horas: ₡{{ salariosPorContrato.find(s => s.TipoContrato === 'Por Horas')?.TotalSalario.toLocaleString() || 0 }}</p>
            <br>
            <p class="text-xl font-bold">Total salarios: ₡{{ totalSalarios.toLocaleString() }}</p>
          </div>

          <!-- Deducciones obligatorias -->
          <div class="mb-4 space-y-1 mt-8">
            <p v-for="nombre in ['SEM', 'IVM', 'Banco Popular', 'Impuesto De Renta']" :key="nombre">
              {{ nombre }}: ₡{{ deduccionesObligatorias.find(d => d.nombre === nombre)?.total.toLocaleString() || 0 }}
            </p>
            <br>
            <p class="text-xl font-bold">Total pagos de ley: ₡{{ totalPagosLey.toLocaleString() }}</p>
          </div>

          <!-- Beneficios -->
          <div class="mb-4 space-y-1 mt-8">
            <p v-for="beneficio in beneficios" :key="beneficio.nombre">
              {{ beneficio.nombre }}: ₡{{ beneficio.total.toLocaleString() }}
            </p>
            <br>
            <p class="text-xl font-bold">Total beneficios: ₡{{ totalBeneficios.toLocaleString() }}</p>
          </div>

          <!-- Costo total empleador -->
          <div class="mb-4 mt-8">
            <p class="text-2xl font-bold">Costo total del empleador: ₡{{ costoTotalEmpleador.toLocaleString() }}</p>
          </div>

          <!-- Botones -->
          <div class="flex gap-4 mt-6">
            <button
              class="bg-teal-500 hover:bg-teal-600 text-white py-2 px-4 rounded"
              @click="descargarPDF"
            >
              Descargar
            </button>
            <button
              class="bg-blue-500 hover:bg-blue-600 text-white py-2 px-4 rounded"
              @click="enviarPorCorreo"
            >
              Enviar por correo
            </button>
          </div>
        </div>

        <!-- No hay pagos -->
        <div v-else class="text-gray-500 text-center mt-16 text-lg">
          No hay pagos disponibles para esta planilla
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, computed, onMounted, watch } from 'vue'
import { useUserStore } from '@/store/user'
import axios from 'axios'
import { API_BASE_URL } from '@/config'


// refs
const userStore = useUserStore()
const planillas = ref([])
const selectedPlanillaId = ref('')
const selectedPlanilla = computed(() =>
  planillas.value.find(p => p.id === selectedPlanillaId.value)
)
const pagos = ref([])
const selectedPagoId = ref('')
const deducciones = ref([])
const deduccionesPorPlanilla = ref([])
const persona = ref(null)
const nombreEmpresa = ref('')

// Computed
const salariosPorContrato = ref([
  { TipoContrato: "Tiempo Completo", TotalSalario: 0 },
  { TipoContrato: "Medio Tiempo", TotalSalario: 0 },
  { TipoContrato: "Servicios Profesionales", TotalSalario: 0 },
  { TipoContrato: "Por Horas", TotalSalario: 0 }
])

const totalSalarios = computed(() =>
  salariosPorContrato.value.reduce((sum, s) => sum + (s.TotalSalario || 0), 0)
)

const deduccionesObligatorias = computed(() => {
  // Agrupa por nombre y suma total
  const map = {}
  deduccionesPorPlanilla.value.forEach(d => {
    if (!d.esBeneficio) {
      if (!map[d.nombre]) {
        map[d.nombre] = { ...d }
      } else {
        map[d.nombre].total += d.total
      }
    }
  })
  return Object.values(map)
})

const beneficios = computed(() => {
  // Agrupa por nombre y suma total
  const map = {}
  deduccionesPorPlanilla.value.forEach(d => {
    if (d.esBeneficio) {
      if (!map[d.nombre]) {
        map[d.nombre] = { ...d }
      } else {
        map[d.nombre].total += d.total
      }
    }
  })
  return Object.values(map)
})

const totalPagosLey = computed(() =>
  deduccionesObligatorias.value.reduce((sum, d) => sum + (d.total || 0), 0)
)
const totalBeneficios = computed(() =>
  beneficios.value.reduce((sum, d) => sum + (d.total || 0), 0)
)
const costoTotalEmpleador = computed(() =>
  totalSalarios.value + totalPagosLey.value + totalBeneficios.value
)

// Funciones
function formatFecha(fecha) {
  if (!fecha) return ''
  const d = new Date(fecha)
  const day = String(d.getDate()).padStart(2, '0')
  const month = String(d.getMonth() + 1).padStart(2, '0')
  const year = d.getFullYear()
  return `${day}/${month}/${year}`
}

function nombreMes(fecha) {
  if (!fecha) return ''
  const d = new Date(fecha)
  return d.toLocaleString('es-ES', { month: 'long', year: 'numeric' })
}

function goBack() {
  window.history.back()
}

// Fetchers
async function fetchPlanillas(nombreEmpresa) {
  try {
    const res = await axios.get(`${API_BASE_URL}Planilla/listar`, {
      params: { nombreEmpresa }
    })
    planillas.value = res.data
    if (planillas.value.length > 0) {
      selectedPlanillaId.value = planillas.value[0].id
      await seleccionarPlanilla()
    }
  } catch (e) {
    planillas.value = []
  }
}

async function fetchPagosPorPlanilla(planillaId) {
  try {
    const res = await axios.get(`${API_BASE_URL}Planilla/PagosPorPlanilla/${planillaId}`, {
      params: { idPlanilla: planillaId }
    })
    pagos.value = res.data
    if (pagos.value.length > 0) {
      selectedPagoId.value = pagos.value[0].id
      await seleccionarPago()
    } else {
      selectedPagoId.value = ''
      deducciones.value = []
    }
    // Cargar deducciones por planilla después de cargar pagos
    await fetchDeduccionesPorPlanilla(planillaId)
  } catch (e) {
    pagos.value = []
    selectedPagoId.value = ''
    deducciones.value = []
    deduccionesPorPlanilla.value = []
  }
}

async function fetchDeducciones(pagoId) {
  try {
    const res = await axios.get(`${API_BASE_URL}Deducciones/${pagoId}`)
    deducciones.value = res.data
  } catch (e) {
    deducciones.value = []
  }
}

async function fetchDeduccionesPorPlanilla(idPlanilla) {
  if (!idPlanilla) return
  try {
    const res = await axios.get(`${API_BASE_URL}Reporte/deduccionesPorPlanilla/${idPlanilla}`)
    deduccionesPorPlanilla.value = res.data
  } catch (e) {
    deduccionesPorPlanilla.value = []
  }
}

async function fetchSalariosPorContrato(idPlanilla) {
  if (!idPlanilla) return
  try {
    const res = await axios.get(`${API_BASE_URL}Reporte/salariosPorContrato/${idPlanilla}`)
    const tipos = [
      "Tiempo Completo",
      "Medio Tiempo",
      "Servicios Profesionales",
      "Por Horas"
    ]
    salariosPorContrato.value = tipos.map(tipo => {
      const found = res.data.find(s => s.tipoContrato === tipo || s.TipoContrato === tipo)
      return {
        TipoContrato: tipo,
        TotalSalario: found ? found.totalSalario || found.TotalSalario : 0
      }
    })
  } catch (e) {
    salariosPorContrato.value = [
      { TipoContrato: "Tiempo Completo", TotalSalario: 0 },
      { TipoContrato: "Medio Tiempo", TotalSalario: 0 },
      { TipoContrato: "Servicios Profesionales", TotalSalario: 0 },
      { TipoContrato: "Por Horas", TotalSalario: 0 }
    ]
  }
}

async function fetchPersona(cedula) {
  try {
    if (!cedula) return
    const res = await axios.get(`${API_BASE_URL}Persona/${cedula}`)
    persona.value = res.data
  } catch (e) {
    persona.value = null
  }
}

// Selecciones
async function seleccionarPlanilla() {
  if (selectedPlanillaId.value) {
    await fetchPagosPorPlanilla(selectedPlanillaId.value)
    await fetchSalariosPorContrato(selectedPlanillaId.value)
  }
}

async function seleccionarPago() {
  if (selectedPagoId.value) {
    await fetchDeducciones(selectedPagoId.value)
  }
}

// Watchers / Ciclo de vida
onMounted(async () => {
  if (userStore.usuario.tipo === 'Empleado') {
    nombreEmpresa.value = userStore.empleado?.nombreEmpresa
    await fetchPlanillas(nombreEmpresa.value)
    await fetchPersona(userStore.empleado?.cedulaPersona)
  } else if (userStore.usuario.tipo === 'DuenoEmpresa') {
    // 1. Obtener dueño de empresa
    const cedula = userStore.usuario?.cedulaPersona
    const duenoRes = await axios.get(`${API_BASE_URL}DuenoEmpresa/${cedula}`)
    const cedulaJuridica = duenoRes.data?.cedulaEmpresa
    if (cedulaJuridica) {
      // 2. Obtener empresa por cédula jurídica
      const empresaRes = await axios.get(`${API_BASE_URL}Empresa/por-cedula-juridica/${cedulaJuridica}`)
      console.log(empresaRes.data)
      nombreEmpresa.value = empresaRes.data?.empresa.nombre
      // 3. Usar el nombre para fetchPlanillas
      await fetchPlanillas(nombreEmpresa.value)
      await fetchPersona(userStore.usuario?.cedula)
    }
  }
  // Si es super admin, hay que redirigir a otra vista para elegir empresa
  
})

watch(selectedPlanillaId, async () => {
  await seleccionarPlanilla()
})

// PDF / Correo
function descargarPDF() {
  if (!selectedPlanilla.value) return
  // ...implementación PDF...
}

async function enviarPorCorreo() {
  // ...implementación correo...
}
</script>