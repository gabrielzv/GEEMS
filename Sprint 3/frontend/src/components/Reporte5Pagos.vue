<template>
  <div class="w-full flex justify-center">
    <div class="w-full max-w-screen-xl px-4">
      <!-- Encabezado del reporte -->
      <div class="mb-8 text-center">
        <h1 class="text-2xl font-bold uppercase">REPORTE 5</h1>
        <div
          class="flex flex-col md:flex-row justify-between items-center mt-4"
        >
          <div class="text-left">
            <h2 class="text-xl font-semibold">
              {{ nombreEmpresa || "Nombre de la empresa" }}
            </h2>
            
          </div>
          <button
            class="bg-green-500 hover:bg-green-600 text-white px-6 py-2 rounded mt-4 md:mt-0"
            @click="exportarAExcel"
          >
            Generar Excel
          </button>
        </div>
      </div>

      <!-- Filtros -->
      <div
        class="bg-gray-100 p-4 rounded-lg mb-6 grid grid-cols-1 md:grid-cols-4 gap-4"
      >
        <div>
          <label class="block text-sm font-medium text-gray-700 mb-1"
            >Fecha desde</label
          >
          <input
            type="date"
            class="w-full p-2 border rounded"
            v-model="fechaDesde"
            @change="aplicarFiltros"
          />
        </div>
        <div>
          <label class="block text-sm font-medium text-gray-700 mb-1"
            >Fecha hasta</label
          >
          <input
            type="date"
            class="w-full p-2 border rounded"
            v-model="fechaHasta"
            @change="aplicarFiltros"
          />
        </div>
        <div>
          <label class="block text-sm font-medium text-gray-700 mb-1"
            >Tipo de empleados</label
          >
          <select
            class="w-full p-2 border rounded"
            v-model="filtroTipo"
            @change="aplicarFiltros"
          >
            <option value="">Todos</option>
            <option value="Tiempo completo">Tiempo completo</option>
            <option value="Medio tiempo">Medio tiempo</option>
          </select>
        </div>
        <div>
          <label class="block text-sm font-medium text-gray-700 mb-1"
            >Cédula</label
          >
          <input
            type="text"
            class="w-full p-2 border rounded"
            placeholder="Buscar por cédula"
            v-model="filtroCedula"
            @input="aplicarFiltros"
          />
        </div>
      </div>

      <!-- Tabla de empleados -->
      <div v-if="loading" class="text-center py-8">
        <p class="text-gray-600">Cargando empleados...</p>
      </div>
      <div v-else class="bg-white rounded-lg shadow w-full">
        <table class="w-full table-auto divide-y divide-gray-200">
          <thead class="bg-gray-50">
            <tr>
              <th
                class="px-4 py-3 text-left text-xs font-medium text-gray-500 uppercase"
              >
                Nombre empleado
              </th>
              <th
                class="px-4 py-3 text-left text-xs font-medium text-gray-500 uppercase"
              >
                Cédula
              </th>
              <th
                class="px-4 py-3 text-left text-xs font-medium text-gray-500 uppercase"
              >
                Tipo de empleado
              </th>
              <th
                class="px-4 py-3 text-left text-xs font-medium text-gray-500 uppercase"
              >
                Período de pago
              </th>
              <th
                class="px-4 py-3 text-left text-xs font-medium text-gray-500 uppercase"
              >
                Fecha de pago
              </th>
              <th
                class="px-4 py-3 text-left text-xs font-medium text-gray-500 uppercase"
              >
                Salario Bruto
              </th>
              <th
                class="px-4 py-3 text-left text-xs font-medium text-gray-500 uppercase"
              >
                Cargas sociales empleador
              </th>
              <th
                class="px-4 py-3 text-left text-xs font-medium text-gray-500 uppercase"
              >
                Deducciones voluntarias
              </th>
              <th
                class="px-4 py-3 text-left text-xs font-medium text-gray-500 uppercase"
              >
                Costo empleador
              </th>
            </tr>
          </thead>
          <tbody class="bg-white divide-y divide-gray-200">
            <tr v-for="(registro, index) in pagosFiltrados" :key="index">
              <td class="px-4 py-4 whitespace-nowrap text-sm text-gray-900">
                {{ registro.empleado.nombre }}
              </td>
              <td class="px-4 py-4 whitespace-nowrap text-sm text-gray-500">
                {{ registro.empleado.cedula }}
              </td>
              <td class="px-4 py-4 whitespace-nowrap text-sm text-gray-500">
                {{ registro.empleado.tipo }}
              </td>
              <td
                class="px-4 py-4 whitespace-pre-wrap text-sm text-gray-500 break-words max-w-[200px]"
              >
                Del<br />{{ formatearFecha(registro.pago.fechaInicio)
                }}<br />al<br />{{ formatearFecha(registro.pago.fechaFin) }}
              </td>
              <td class="px-4 py-4 whitespace-nowrap text-sm text-gray-500">
                {{ formatearFecha(registro.pago.fechaPago) }}
              </td>
              <td class="px-4 py-4 whitespace-nowrap text-sm text-gray-500">
                {{ formatearColones(registro.pago.salarioBruto) }}
              </td>
              <td class="px-4 py-4 whitespace-nowrap text-sm text-gray-500">
                {{ formatearColones(registro.pago.cargasSociales) }}
              </td>
              <td class="px-4 py-4 whitespace-nowrap text-sm text-gray-500">
                {{
                  formatearColones(registro.pago.totalDeduccionesVoluntarias)
                }}
              </td>
              <td class="px-4 py-4 whitespace-nowrap text-sm text-gray-500">
                {{ formatearColones(registro.pago.costoEmpleador) }}
              </td>
            </tr>
            <tr class="bg-gray-100 font-semibold">
              <td colspan="5" class="px-4 py-4 text-right">Totales:</td>
              <td class="px-4 py-4 whitespace-nowrap text-sm text-gray-700">
                {{
                  formatearColones(
                    pagosFiltrados.reduce((s, r) => s + r.pago.salarioBruto, 0)
                  )
                }}
              </td>
              <td class="px-4 py-4 whitespace-nowrap text-sm text-gray-700">
                {{
                  formatearColones(
                    pagosFiltrados.reduce(
                      (s, r) => s + r.pago.cargasSociales,
                      0
                    )
                  )
                }}
              </td>
              <td class="px-4 py-4 whitespace-nowrap text-sm text-gray-700">
                {{
                  formatearColones(
                    pagosFiltrados.reduce(
                      (s, r) => s + r.pago.totalDeduccionesVoluntarias,
                      0
                    )
                  )
                }}
              </td>
              <td class="px-4 py-4 whitespace-nowrap text-sm text-gray-700">
                {{
                  formatearColones(
                    pagosFiltrados.reduce(
                      (s, r) => s + r.pago.costoEmpleador,
                      0
                    )
                  )
                }}
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>
  </div>
</template>

<script>
import { useUserStore } from "@/store/user";
import { ref, computed, onMounted } from "vue";
import axios from "axios";
import { API_BASE_URL } from "@/config";

export default {
  setup() {
    const userStore = useUserStore();
    const empleados = ref([]);
    const loading = ref(true);

    // Variables para los filtros
    const fechaDesde = ref("");
    const fechaHasta = ref("");
    const filtroTipo = ref("");
    const filtroCedula = ref("");
    const pagos = ref([]);
    const pagosFiltrados = ref([]);

    const nombreEmpresa = computed(() => {
      return (
        userStore.empleado?.nombreEmpresa ||
        userStore.empresa?.nombre ||
        "Nombre de empresa"
      );
    });

    const formatearNombre = (empleado) => {
      if (empleado.nombreCompleto) return empleado.nombreCompleto;
      return `${empleado.apellido1 || ""} ${empleado.apellido2 || ""}, ${
        empleado.nombre || ""
      }`.trim();
    };

    const formatearFecha = (fechaStr) => {
      const fecha = new Date(fechaStr);
      if (isNaN(fecha)) return "";
      return fecha.toLocaleDateString("es-CR", {
        day: "2-digit",
        month: "2-digit",
        year: "numeric",
      });
    };

    const fetchPagosGlobal = async () => {
      loading.value = true;
      try {
        const resultados = await Promise.all(
          userStore.empleadosEmpresa.map(async (e) => {
            try {
              const resEmpleado = await axios.get(
                `${API_BASE_URL}GetEmpleado/${e.cedula}`
              );
              const empleadoData = resEmpleado.data;

              const resPagos = await axios.get(
                `${API_BASE_URL}Pagos/${empleadoData.id}`
              );
              const pagosEmpleado = resPagos.data;

              return await Promise.all(
                pagosEmpleado.map(async (p) => {
                  const resDeducciones = await axios.get(
                    `${API_BASE_URL}Deducciones/${p.id}`
                  );
                  const deducciones = resDeducciones.data;

                  const resDeduccionesDetalladas = await axios.get(
                    `${API_BASE_URL}Deduccion/DeduccionesDetalladas/${p.montoBruto}`
                  );
                  const deduccionesDetalladas = resDeduccionesDetalladas.data;

                  const totalVoluntarias = deducciones
                    .filter((d) => d.tipoDeduccion === "Voluntaria")
                    .reduce((suma, d) => suma + d.monto, 0);

                  const salarioBruto = p.montoBruto;
                  const cargasSociales = deduccionesDetalladas.totalDeducciones;
                  const costoEmpleador =
                    salarioBruto + cargasSociales + totalVoluntarias;

                  return {
                    empleado: {
                      nombre: formatearNombre(e),
                      cedula: empleadoData.cedulaPersona,
                      tipo: empleadoData.contrato || "No especificado",
                      empresa: empleadoData.nombreEmpresa || "-",
                    },
                    pago: {
                      idPago: p.id,
                      fechaPago: p.fechaRealizada,
                      fechaInicio: p.fechaInicio,
                      fechaFin: p.fechaFinal,
                      salarioBruto: p.montoBruto,
                      totalDeduccionesVoluntarias: totalVoluntarias,
                      cargasSociales: deduccionesDetalladas.totalDeducciones,
                      costoEmpleador,
                    },
                  };
                })
              );
            } catch (err) {
              console.warn(`No se pudo procesar al empleado ${e.cedula}`, err);
              return [];
            }
          })
        );

        pagos.value = resultados.flat();
        pagosFiltrados.value = [...pagos.value];
      } catch (error) {
        console.error("Error general al obtener pagos:", error);
        pagos.value = [];
        pagosFiltrados.value = [];
      } finally {
        loading.value = false;
      }
    };

    const formatearColones = (valor) => {
      return new Intl.NumberFormat("es-CR", {
        style: "currency",
        currency: "CRC",
        minimumFractionDigits: 0,
      }).format(valor);
    };

    const aplicarFiltros = () => {
      let filtrados = [...pagos.value];

      if (fechaDesde.value || fechaHasta.value) {
        const desde = fechaDesde.value ? new Date(fechaDesde.value) : null;
        const hasta = fechaHasta.value ? new Date(fechaHasta.value) : null;

        filtrados = filtrados.filter((registro) => {
          const inicioPeriodo = new Date(registro.pago.fechaInicio);
          const finPeriodo = new Date(registro.pago.fechaFin);

          if (desde && !hasta) {
            return finPeriodo >= desde;
          } else if (!desde && hasta) {
            return inicioPeriodo <= hasta;
          } else if (desde && hasta) {
            return inicioPeriodo <= hasta && finPeriodo >= desde;
          }
          return true;
        });
      }

      if (filtroTipo.value) {
        filtrados = filtrados.filter((registro) =>
          registro.empleado.tipo
            .toLowerCase()
            .includes(filtroTipo.value.toLowerCase())
        );
      }

      if (filtroCedula.value) {
        filtrados = filtrados.filter(
          (registro) =>
            registro.empleado.cedula &&
            String(registro.empleado.cedula).includes(
              String(filtroCedula.value)
            )
        );
      }

      pagosFiltrados.value = filtrados;
    };

    const exportarAExcel = () => {
      // Crear el contenido CSV
      let csvContent = "data:text/csv;charset=utf-8,";

      // Encabezados
      const headers = [
        "Nombre empleado",
        "Cédula",
        "Tipo de empleado",
        "Fecha inicio período",
        "Fecha fin período",
        "Fecha de pago",
        "Salario Bruto (₡)",
        "Cargas sociales empleador (₡)",
        "Deducciones voluntarias (₡)",
        "Costo empleador (₡)",
      ];
      csvContent += headers.join(",") + "\n";

      // Datos
      pagosFiltrados.value.forEach((registro) => {
        const row = [
          `"${registro.empleado.nombre}"`,
          `"${registro.empleado.cedula}"`,
          `"${registro.empleado.tipo}"`,
          `"${formatearFecha(registro.pago.fechaInicio)}"`,
          `"${formatearFecha(registro.pago.fechaFin)}"`,
          `"${formatearFecha(registro.pago.fechaPago)}"`,
          registro.pago.salarioBruto,
          registro.pago.cargasSociales,
          registro.pago.totalDeduccionesVoluntarias,
          registro.pago.costoEmpleador,
        ];
        csvContent += row.join(",") + "\n";
      });

      // Crear enlace de descarga
      const encodedUri = encodeURI(csvContent);
      const link = document.createElement("a");
      link.setAttribute("href", encodedUri);
      link.setAttribute(
        "download",
        `reporte_pagos_${new Date().toISOString().slice(0, 10)}.csv`
      );
      document.body.appendChild(link);
      link.click();
      document.body.removeChild(link);
    };

    onMounted(async () => {
      if (!userStore.empresa && userStore.usuario?.cedulaPersona) {
        await userStore.fetchEmpresa(userStore.usuario.cedulaPersona);
      }
      await fetchPagosGlobal();
    });

    return {
      nombreEmpresa,
      empleados,
      loading,
      pagos,
      pagosFiltrados,
      fechaDesde,
      fechaHasta,
      filtroTipo,
      filtroCedula,
      aplicarFiltros,
      exportarAExcel,
      formatearFecha,
      formatearColones,
    };
  },
};
</script>

<style scoped>
.container {
  max-width: 1200px;
}
</style>
