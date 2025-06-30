<!-- Dashboard de empleador -->
<template>
  <div class="w-full flex justify-center py-10">
    <div class="w-full max-w-2xl px-4">
      <h1 class="text-2xl font-bold text-center mb-8">Dashboard de empleados</h1>
      <div v-if="loading" class="text-center text-gray-600">Cargando datos...</div>
      <div v-else>
        <Pie :data="chartData" :options="chartOptions" />
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
  ArcElement
} from 'chart.js'
import { useUserStore } from '@/store/user'
import axios from 'axios'
import { API_BASE_URL } from '@/config'

ChartJS.register(Title, Tooltip, Legend, ArcElement)

const userStore = useUserStore()
const empleados = ref([])
const loading = ref(true)

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
  } finally {
    loading.value = false
  }
}

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

onMounted(async () => {
  if (!userStore.empresa && userStore.usuario?.cedulaPersona) {
    await userStore.fetchEmpresa(userStore.usuario.cedulaPersona)
  }
  await cargarEmpleados()
})
</script>
