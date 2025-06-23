<template>
  <div class="max-w-md mx-auto mt-10 p-6 bg-white rounded shadow text-center">
    <h2 class="text-2xl font-bold mb-6 text-blue-700">
      {{ formatPlanilla(fechaInicio, fechaFin) }}
    </h2>
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
        <span class="font-semibold text-red-600">
          {{ currency(resumen.totalDeducciones) }}
        </span>
      </div>
    </div>
    <button
      class="mt-6 px-6 py-2 bg-blue-600 text-white rounded hover:bg-blue-700 transition"
      @click="pagarPlanilla"
    >
      Pagar planilla
    </button>
  </div>
</template>

<script>
import { API_BASE_URL } from "../config";
import axios from "axios";
import { useUserStore } from "../store/user";

export default {
  name: "payrollSummary",
  data() {
    return {
      resumen: {
        totalBruto: 0,
        totalNeto: 0,
        totalDeducciones: 0,
      },
      idPlanilla: "",
      fechaInicio: "",
      fechaFin: "",
      nombreEmpresa: "",
      cedulaPersona: "",
    };
  },
  async mounted() {
    // Recibe parámetros por query
    this.idPlanilla = this.$route.query.idPlanilla;
    this.fechaInicio = this.$route.query.fechaInicio;
    this.fechaFin = this.$route.query.fechaFinal;

    const userStore = useUserStore();
    // Asegúrate de cargar el usuario desde sessionStorage si es necesario
    if (!userStore.usuario) {
      userStore.cargarDesdeSessionStorage();
    }
    const usuario = userStore.usuario;

    this.cedulaPersona =
      usuario && usuario.cedulaPersona ? usuario.cedulaPersona : "";

    if (!this.cedulaPersona) {
      alert("No se encontró la cédula del usuario logueado.");
      return;
    }

    await userStore.fetchEmpleado(this.cedulaPersona);
    const empleado = userStore.empleado;

    this.nombreEmpresa = empleado?.nombreEmpresa || "";

    if (!this.nombreEmpresa) {
      alert("No se encontró el nombre de la empresa del empleado logueado.");
      return;
    }

    // Generar pagos y obtener resumen automáticamente
    await this.generarPagosYResumen();
  },
  methods: {
    async generarPagosYResumen() {
      const url = `${API_BASE_URL}Pagos/generarPagosEmpresa`;
      try {
        await axios.post(
          url,
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
        const msg = e.response?.data?.message || e.message;
        if (msg.includes("No hay empleados con horas registradas")) {
          alert(
            "No hay empleados a los que se les pueda pagar planilla. " + msg
          );
          this.$router.push({ name: "selectCreatePayroll" }); // Cambia el name si tu ruta es diferente
        } else {
          alert("Error al generar los pagos de la planilla: " + msg);
        }
        return;
      }
    },
    async obtenerResumen() {
      const url = `${API_BASE_URL}Pagos/resumenPlanilla`;
      try {
        const res = await axios.get(
          url,
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
      this.$router.push("/home");
    },
    currency(value) {
      return new Intl.NumberFormat("es-CR", {
        style: "currency",
        currency: "CRC",
      }).format(value);
    },
    formatPlanilla(fechaInicio, fechaFin) {
      const inicio = new Date(fechaInicio);
      const fin = new Date(fechaFin);
      return `${inicio.getDate()} de ${inicio.toLocaleString("es-ES", {
        month: "long",
      })} ${inicio.getFullYear()} a ${fin.getDate()} de ${fin.toLocaleString(
        "es-ES",
        { month: "long" }
      )} ${fin.getFullYear()}`;
    },
  },
};
</script>
