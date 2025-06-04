<template>
  <div class="max-w-md mx-auto mt-10 p-6 bg-white rounded shadow text-center">
    <h2 class="text-2xl font-bold mb-6 text-blue-700">Resumen de Planilla</h2>
    <div class="mb-4">
      <label class="block mb-2 font-semibold">Seleccione la planilla:</label>
      <select
        v-model="planillaSeleccionada"
        @change="onPlanillaChange"
        class="mb-4 p-2 border rounded w-full"
      >
        <option disabled value="">Seleccione una planilla</option>
        <option
          v-for="planilla in planillas"
          :key="planilla.id"
          :value="planilla"
        >
          {{ planilla.fechaInicio }} a {{ planilla.fechaFinal }}
        </option>
      </select>
    </div>
    <button
      class="mb-4 px-4 py-2 bg-green-600 text-white rounded hover:bg-green-700"
      @click="generarPagosYResumen"
      :disabled="!planillaSeleccionada"
    >
      Generar pagos de planilla
    </button>
    <div class="mb-4">
      <div class="text-lg">
        Total Salarios Brutos:
        <span class="font-semibold">{{ currency(resumen.totalBruto) }}</span>
      </div>
      <div class="text-lg">
        Total Salarios Netos:
        <span class="font-semibold">{{ currency(resumen.totalNeto) }}</span>
      </div>
      <div class="text-lg">
        Total Deducciones:
        <span class="font-semibold text-red-600">{{
          currency(resumen.totalDeducciones)
        }}</span>
      </div>
    </div>
    <button
      class="mt-6 px-6 py-2 bg-blue-600 text-white rounded hover:bg-blue-700 transition"
      @click="pagarPlanilla"
      :disabled="!planillaSeleccionada"
    >
      Pagar planilla
    </button>
  </div>
</template>

<script>
import axios from "axios";
import { useUserStore } from "../store/user";

export default {
  name: "PayrollSummary",
  data() {
    return {
      resumen: {
        totalBruto: 0,
        totalNeto: 0,
        totalDeducciones: 0,
      },
      planillas: [],
      planillaSeleccionada: "",
      fechaInicio: "",
      fechaFin: "",
      idPlanilla: "",
      nombreEmpresa: "",
      cedulaPersona: "",
    };
  },
  async mounted() {
    const userStore = useUserStore();
    const usuario = userStore.usuario;
    this.cedulaPersona =
      usuario && usuario.cedulaPersona ? usuario.cedulaPersona : "";

    if (!this.cedulaPersona) {
      alert("No se encontró la cédula del usuario logueado.");
      return;
    }

    // 1. Obtener el nombre de la empresa del empleado Payroll usando la cédula
    this.nombreEmpresa = await this.obtenerNombreEmpresaDeEmpleado(
      this.cedulaPersona
    );

    // 2. Cargar planillas de la empresa
    await this.cargarPlanillas();
  },
  methods: {
    async obtenerNombreEmpresaDeEmpleado(cedulaPersona) {
      try {
        const res = await axios.get(
          `https://localhost:7014/api/GetEmpleado/${cedulaPersona}`
        );
        return res.data.nombreEmpresa;
      } catch (e) {
        alert("No se pudo obtener el nombre de la empresa del empleado.");
        return "";
      }
    },
    async cargarPlanillas() {
      try {
        const res = await axios.get(
          "https://localhost:7014/api/Planilla/listar",
          {
            params: { nombreEmpresa: this.nombreEmpresa },
          }
        );
        this.planillas = res.data;
      } catch (e) {
        alert("Error al cargar las planillas");
      }
    },
    onPlanillaChange() {
      if (this.planillaSeleccionada) {
        this.fechaInicio = this.planillaSeleccionada.fechaInicio;
        this.fechaFin = this.planillaSeleccionada.fechaFinal;
        this.idPlanilla = this.planillaSeleccionada.id;
      }
    },
    async generarPagosYResumen() {
      try {
        await axios.post(
          "https://localhost:7014/api/Pagos/generarPagosEmpresa",
          null,
          {
            params: {
              nombreEmpresa: this.nombreEmpresa,
              idPlanilla: this.idPlanilla,
              fechaInicio: this.fechaInicio,
              fechaFinal: this.fechaFin,
            },
          }
        );
        await this.obtenerResumen();
      } catch (e) {
        alert(
          "Error al generar los pagos de la planilla: " +
            e.response?.data?.message
        );
        console.log(e);
        return;
      }
    },
    async obtenerResumen() {
      try {
        const res = await axios.get(
          `https://localhost:7014/api/Pagos/resumenPlanilla`,
          {
            params: {
              nombreEmpresa: this.nombreEmpresa,
              fechaInicio: this.fechaInicio,
              fechaFin: this.fechaFin,
            },
          }
        );
        this.resumen = res.data;
      } catch (e) {
        alert("Error al obtener el resumen de planilla");
      }
    },
    pagarPlanilla() {
      alert("¡Planilla pagada!");
      // redirigir a la pagina de inicio
      this.$router.push("/home");
    },
    currency(value) {
      return new Intl.NumberFormat("es-CR", {
        style: "currency",
        currency: "CRC",
      }).format(value);
    },
  },
};
</script>
