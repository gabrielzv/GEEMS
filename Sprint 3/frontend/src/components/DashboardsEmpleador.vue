<template>
  <div class="w-full max-w-md mx-auto">
    <h2 class="text-xl font-bold mb-4 text-center">Distribución por tipo de empleado</h2>
    <Pie :data="chartData" :options="chartOptions" />
  </div>
</template>

<script setup>
import { computed } from 'vue'
import { Pie } from 'vue-chartjs'
import {
  Chart as ChartJS,
  Title,
  Tooltip,
  Legend,
  ArcElement
} from 'chart.js'

ChartJS.register(Title, Tooltip, Legend, ArcElement)

const props = defineProps({
  empleados: {
    type: Array,
    required: true
  }
})

const chartData = computed(() => {
  const conteo = {
    'Tiempo completo': 0,
    'Medio tiempo': 0,
    'Por horas': 0
  }

  props.empleados.forEach(e => {
    const tipo = (e.empleado.tipo || '').toLowerCase()
    if (tipo.includes('tiempo completo')) conteo['Tiempo completo']++
    else if (tipo.includes('medio tiempo')) conteo['Medio tiempo']++
    else if (tipo.includes('hora') || tipo.includes('por horas')) conteo['Por horas']++
  })

  return {
    labels: ['Tiempo completo', 'Medio tiempo', 'Por horas'],
    datasets: [
      {
        label: 'Tipo de empleado',
        data: Object.values(conteo),
        backgroundColor: ['#60a5fa', '#fcd34d', '#f87171']
      }
    ]
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
</script>
