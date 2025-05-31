<template>
  <div class="max-w-2xl mx-auto mt-8 p-6 bg-white rounded shadow">
    <h2 class="text-2xl font-bold mb-4 text-blue-700">Historial de Registros de Horas</h2>
    <table class="min-w-full bg-white border">
      <thead>
        <tr>
          <th class="py-2 px-4 border-b">Fecha</th>
          <th class="py-2 px-4 border-b">Horas</th>
          <th class="py-2 px-4 border-b">Estado</th>
        </tr>
      </thead>
      <tbody>
        <tr v-for="registro in registrosOrdenados" :key="registro.id">
            <td class="py-2 px-4 border-b">{{ formatFecha(registro.fecha) }}</td>
            <td class="py-2 px-4 border-b">{{ registro.numHoras }}</td>
            <td class="py-2 px-4 border-b">
            <div class="flex justify-center items-center gap-2">
                <span
                :class="{
                    'text-yellow-600': registro.estado === 'NoRevisado',
                    'text-green-600': registro.estado === 'Aprobado',
                    'text-red-600': registro.estado === 'Denegado'
                }"
                >
                {{ registro.estado === 'NoRevisado' ? 'No Revisado' : registro.estado }}
                </span>
                <button
                v-if="registro.estado === 'Denegado' || registro.estado === 'NoRevisado'"
                class="px-1 py-0.5 text-xs bg-blue-100 text-blue-700 rounded hover:bg-blue-200 transition min-w-0"
                style="width:auto; min-width:0;"
                @click="editarRegistro(registro)"
                >
                Editar
                </button>
            </div>
            </td>
        </tr>
        <tr v-if="registros.length === 0">
            <td colspan="3" class="py-4 text-center text-gray-500">No hay registros de horas.</td>
        </tr>
    </tbody>
    </table>
  </div>
</template>

<script>
import axios from "axios";
import { useUserStore } from "../store/user";

export default {
  data() {
    return {
      registros: [],
      guidEmpleado: null,
    };
  },
  computed: {
    registrosOrdenados() {
      // Ordenar por fecha descendente
      return [...this.registros].sort((a, b) => new Date(b.fecha) - new Date(a.fecha));
    },
  },
  async created() {
    const userStore = useUserStore();
    if (userStore.usuario && userStore.usuario.cedulaPersona) {
      // Obtener el ID del empleado
      try {
        const res = await axios.get(`https://localhost:7014/api/GetEmpleado/${userStore.usuario.cedulaPersona}`);
        this.guidEmpleado = res.data.id;
        this.obtenerRegistros();
      } catch (e) {
        console.error("No se pudo obtener el ID del empleado.", e);
      }
    }
  },
  methods: {
    async obtenerRegistros() {
      if (!this.guidEmpleado) return;
      try {
        const res = await axios.get(`https://localhost:7014/api/Horas/getRegister/${this.guidEmpleado}`);
        this.registros = res.data;
      } catch (e) {
        console.error("Error al obtener los registros de horas:", e);
      }
    },
    formatFecha(fecha) {
      if (!fecha) return "";
      const d = new Date(fecha);
      return d.toLocaleDateString();
    },
    editarRegistro(registro) {
    // Aquí puedes abrir un modal, navegar a otra vista, etc.
    alert(`Editar registro con ID: ${registro.id}`);
    }
  },
};
</script>

<style scoped>
/* Puedes agregar estilos personalizados aquí */
</style>