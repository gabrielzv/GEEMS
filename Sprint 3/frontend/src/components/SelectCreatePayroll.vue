<template>
  <div class="max-w-md mx-auto mt-10 p-6 bg-white rounded shadow text-center">
    <h2 class="text-2xl font-bold mb-6 text-blue-700">Gestión de Planillas</h2>
    <div class="mb-6">
      <label class="block mb-2 font-semibold">Seleccione una planilla existente:</label>
      <select v-model="planillaSeleccionada" class="mb-4 p-2 border rounded w-full">
        <option disabled value="">Seleccione una planilla</option>
        <option
          v-for="planilla in planillas"
          :key="planilla.id"
          :value="planilla"
        >
          {{ formatPlanilla(planilla) }}
        </option>
      </select>
      <button
        class="mb-4 px-4 py-2 bg-blue-600 text-white rounded hover:bg-blue-700"
        :disabled="!planillaSeleccionada"
        @click="irAResumen(planillaSeleccionada)"
      >
        Ver resumen de planilla
      </button>
    </div>
    <div class="mb-6 border-t pt-6">
      <h3 class="text-lg font-semibold mb-2">Crear nueva planilla</h3>
      <div class="flex flex-col gap-2 mb-4">
        <input type="date" v-model="nuevaFechaInicio" class="p-2 border rounded" />
        <input type="date" v-model="nuevaFechaFin" class="p-2 border rounded" />
      </div>
      <button
        class="px-4 py-2 bg-green-600 text-white rounded hover:bg-green-700"
        :disabled="!nuevaFechaInicio || !nuevaFechaFin"
        @click="crearPlanilla"
      >
        Crear planilla
      </button>
    </div>
  </div>
</template>

<script>
import axios from "axios";
import { useUserStore } from "../store/user";
import { ref, onMounted } from "vue";
import { useRouter } from "vue-router";
import { API_BASE_URL } from "../config";
export default {
  setup() {
    const userStore = useUserStore();
    const router = useRouter();
    const planillas = ref([]);
    const planillaSeleccionada = ref("");
    const nuevaFechaInicio = ref("");
    const nuevaFechaFin = ref("");
    const nombreEmpresa = ref("");

    const cargarPlanillas = async () => {
      if (!nombreEmpresa.value) return;

      const url = `${API_BASE_URL}Planilla/listar`;
      const res = await axios.get(url, {
        params: { nombreEmpresa: nombreEmpresa.value },
      });
      planillas.value = res.data;
    };

    const formatPlanilla = (planilla) => {
    const inicio = new Date(planilla.fechaInicio);
    const fin = new Date(planilla.fechaFinal);
    return `${inicio.getDate()} de ${inicio.toLocaleString('es-ES', { month: 'long' })} ${inicio.getFullYear()} a ${fin.getDate()} de ${fin.toLocaleString('es-ES', { month: 'long' })} ${fin.getFullYear()}`;
    };

    const irAResumen = (planilla) => {
      router.push({
        name: "payrollSummary",
        query: {
          idPlanilla: planilla.id,
          fechaInicio: planilla.fechaInicio,
          fechaFinal: planilla.fechaFinal,
        },
      });
    };

    const crearPlanilla = async () => {
    if (!nuevaFechaInicio.value || !nuevaFechaFin.value) {
        alert("Debe ingresar ambas fechas.");
        return;
    }

    const regexFecha = /^\d{4}-\d{2}-\d{2}$/;
    if (!regexFecha.test(nuevaFechaInicio.value) || !regexFecha.test(nuevaFechaFin.value)) {
        alert("Formato de fecha inválido.");
        return;
    }

    const payload = {
        IdPayroll: userStore.usuario.id,
        fechaInicio: nuevaFechaInicio.value,
        fechaFinal: nuevaFechaFin.value,
    };
    console.log("Payload enviado a backend:", payload);

    try {
        const url = `${API_BASE_URL}Planilla/crear`;
        await axios.post(url, payload);
        await cargarPlanillas();
        alert("Planilla creada correctamente.");
    } catch (e) {
        console.error("Error al crear la planilla:", e);
        alert("Error al crear la planilla: " + (e.response?.data?.message || e.message));
    }
    };

    onMounted(async () => {
      // Obtener nombreEmpresa desde el empleado
      let usuario = userStore.usuario;
      if (!usuario) {
        userStore.cargarDesdeSessionStorage();
        usuario = userStore.usuario;
      }
      let cedula = usuario?.cedulaPersona;
      if (cedula) {
        await userStore.fetchEmpleado(cedula);
        const empleado = userStore.empleado;
        nombreEmpresa.value = empleado?.nombreEmpresa || "";
      }
      await cargarPlanillas();
    });

    return {
      planillas,
      planillaSeleccionada,
      nuevaFechaInicio,
      nuevaFechaFin,
      formatPlanilla,
      irAResumen,
      crearPlanilla,
    };
  },
};
</script>