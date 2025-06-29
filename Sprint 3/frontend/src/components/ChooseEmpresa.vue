<template>
  <div class="p-8">
    <h1 class="text-2xl font-bold mb-4">Elegir empresa</h1>
    <select v-model="empresaSeleccionada" class="border p-2 rounded mb-4">
      <option disabled value="">Seleccione una empresa</option>
      <option v-for="empresa in empresas" :key="empresa.cedulaJuridica" :value="empresa.nombre">
        {{ empresa.nombre }}
      </option>
    </select>
    <button
      class="bg-blue-600 text-white px-4 py-2 rounded"
      :disabled="!empresaSeleccionada"
      @click="irAReporte"
    >
      Ver reporte
    </button>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import axios from 'axios'
import { useRouter } from 'vue-router'
import { API_BASE_URL } from '@/config'

const empresas = ref([])
const empresaSeleccionada = ref('')
const router = useRouter()

onMounted(async () => {
  const res = await axios.get(`${API_BASE_URL}Empresas/todas`)
  empresas.value = res.data
})

function irAReporte() {
  if (empresaSeleccionada.value) {
    console.log('redirigiendo a reporte4 con', empresaSeleccionada.value)
    router.push({ name: 'Reporte4Desglose', query: { nombreEmpresa: empresaSeleccionada.value } })
  }
}
</script>