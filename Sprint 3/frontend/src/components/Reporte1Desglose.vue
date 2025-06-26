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

    <!-- Contenedor principal -->
    <div class="flex gap-6 w-full mt-2">
      <!-- Dropdown -->
      <div class="w-1/3 p-4 border rounded-xl shadow-md">
        <h2 class="text-lg font-semibold mb-2">Pagos disponibles</h2>
        <select
          v-model="selectedPagoId"
          @change="seleccionarPago"
          class="w-full p-2 border rounded focus:outline-none focus:ring"
        >
          <option disabled value="">Seleccione un pago</option>
          <option
            v-for="pago in pagos"
            :key="pago.id"
            :value="pago.id"
          >
            Pago de {{ nombreMes(pago.planillaFechaInicio) }}
          </option>
        </select>
      </div>

      <!-- Detalles del pago -->
      <div class="flex-1 p-6 border rounded-xl shadow-md">
        <div v-if="selectedPago">
          <!-- Detalles generales -->
          <div class="mb-4 space-y-1">
            <p><strong>Nombre de la empresa:</strong> {{ userStore.empleado.nombreEmpresa }}</p>
            <p><strong>Empleado:</strong> {{ persona?.fullName }}</p>
            <p><strong>Tipo de contrato:</strong> {{ userStore.empleado.contrato }}</p>
            <p><strong>Fecha de pago:</strong> {{ formatFecha(selectedPago.fechaRealizada) }}</p>
          </div>

          <!-- Deducciones -->
          <div class="mb-4 space-y-1">
            <p class="font-bold text-lg">Salario Bruto: ₡{{ selectedPago.montoBruto?.toLocaleString() }}</p>
            <div v-for="deduccion in deducciones" :key="deduccion.id">
              <p>
                {{ deduccion.nombre }}: -₡{{ deduccion.monto?.toLocaleString() }}
              </p>
            </div>
            <p class="font-semibold mt-2">
              Total deducciones: -₡{{ totalDeducciones?.toLocaleString() }}
            </p>
          </div>

          <!-- Pago neto -->
          <p class="text-xl font-bold">Salario Neto: ₡{{ selectedPago.montoPago?.toLocaleString() }}</p>

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
              disabled
            >
              Enviar por correo
            </button>
          </div>
        </div>

        <!-- No hay pagos -->
        <div v-else class="text-gray-500 text-center mt-16 text-lg">
          No hay pagos disponibles
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue'
import { useUserStore } from '@/store/user'
import axios from 'axios'
import { API_BASE_URL } from '@/config'
import jsPDF from 'jspdf'

const userStore = useUserStore()
const pagos = ref([])
const deducciones = ref([])
const selectedPagoId = ref('')
const persona = ref(null)

const selectedPago = computed(() =>
  pagos.value.find(p => p.id === selectedPagoId.value)
)
const totalDeducciones = computed(() =>
  deducciones.value.reduce((sum, d) => sum + (d.monto || 0), 0)
)

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

onMounted(async () => {
  const empleadoId = userStore.empleado?.id || userStore.usuario?.cedulaPersona
  if (empleadoId) {
    await fetchPagos(empleadoId)
    if (pagos.value.length > 0) {
      selectedPagoId.value = pagos.value[0].id
      await fetchDeducciones(selectedPagoId.value)
    }
  }
  const cedula = userStore.empleado?.cedula || userStore.usuario?.cedulaPersona
  if (cedula) {
    await fetchPersona(cedula)
  } else {
    console.error('Empleado o usuario no encontrado')
  }
})

async function fetchPagos(empleadoId) {
  try {
    const res = await axios.get(`${API_BASE_URL}Pagos/${empleadoId}`)
    // Para cada pago, obtener la fecha de inicio de la planilla asociada
    const pagosConPlanilla = await Promise.all(
      res.data.map(async pago => {
        try {
          const planillaRes = await axios.get(`${API_BASE_URL}Planilla/${pago.idPlanilla}`)
          return {
            ...pago,
            planillaFechaInicio: planillaRes.data.fechaInicio
          }
        } catch {
          return {
            ...pago,
            planillaFechaInicio: null
          }
        }
      })
    )
    pagos.value = pagosConPlanilla
  } catch (e) {
    pagos.value = []
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

async function fetchPersona(cedula) {
  try {
    const res = await axios.get(`${API_BASE_URL}Persona/${cedula}`)
    persona.value = res.data
  } catch (e) {
    console.error('Error fetching persona:', e)
  }
}

async function seleccionarPago() {
  if (selectedPagoId.value) {
    await fetchDeducciones(selectedPagoId.value)
  }
}

function goBack() {
  window.history.back()
}


function descargarPDF() {
  if (!selectedPago.value) return

  const doc = new jsPDF()
  doc.setFontSize(18)
  doc.text('Recibo de Pago', 105, 18, { align: 'center' })

  doc.setFontSize(12)
  let y = 35

  // Nombre de la empresa (negrita)
  doc.setFont(undefined, 'bold')
  doc.text('Nombre de la empresa:', 20, y)
  doc.setFont(undefined, 'normal')
  doc.text(`${userStore.empleado.nombreEmpresa}`, 70, y)
  y += 10

  // Empleado (negrita)
  doc.setFont(undefined, 'bold')
  doc.text('Empleado:', 20, y)
  doc.setFont(undefined, 'normal')
  doc.text(`${persona.value?.fullName || ''}`, 70, y)
  y += 10

  // Tipo de contrato (negrita)
  doc.setFont(undefined, 'bold')
  doc.text('Tipo de contrato:', 20, y)
  doc.setFont(undefined, 'normal')
  doc.text(`${userStore.empleado.contrato}`, 70, y)
  y += 10

  // Fecha de pago (negrita)
  doc.setFont(undefined, 'bold')
  doc.text('Fecha de pago:', 20, y)
  doc.setFont(undefined, 'normal')
  doc.text(`${formatFecha(selectedPago.value.fechaRealizada)}`, 70, y)
  y += 15

  // Resumen de Pago (sección)
  doc.setFontSize(14)
  doc.setFont(undefined, 'bold')
  doc.text('Resumen de Pago', 20, y)
  y += 10

  // Salario Bruto (negrita)
  doc.setFontSize(12)
  doc.setFont(undefined, 'bold')
  doc.text('Salario Bruto:', 20, y)
  doc.setFont(undefined, 'normal')
  doc.text(`${selectedPago.value.montoBruto?.toLocaleString('en-US')}`, 60, y)
  y += 10

  // Deducciones
  doc.setFont(undefined, 'bold')
  doc.text('Deducciones:', 20, y)
  y += 8
  deducciones.value.forEach(d => {
    doc.setFont(undefined, 'normal')
    doc.text(`${d.nombre}: -${d.monto?.toLocaleString('en-US')}`, 25, y)
    y += 8
  })
  y += 2

  // Total deducciones (negrita)
  doc.setFont(undefined, 'bold')
  doc.text('Total deducciones:', 20, y)
  doc.setFont(undefined, 'normal')
  doc.text(`-${totalDeducciones.value?.toLocaleString('en-US')}`, 65, y)
  y += 12

  // Salario Neto (negrita)
  doc.setFontSize(14)
  doc.setFont(undefined, 'bold')
  doc.text('Salario Neto:', 20, y)
  doc.setFont(undefined, 'normal')
  doc.text(`${selectedPago.value.montoPago?.toLocaleString('en-US')}`, 60, y)

  // Abrir en nueva pestaña
  const pdfBlob = doc.output('blob')
  const url = URL.createObjectURL(pdfBlob)
  window.open(url, '_blank')
}
</script>