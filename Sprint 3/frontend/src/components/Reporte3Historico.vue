<template>
  <div class="w-full flex justify-center">
    <div class="w-full max-w-screen-xl px-4">
      <!-- Encabezado del reporte -->
      <div class="mb-8 text-center">
        <h1 class="text-2xl font-bold uppercase">
          REPORTE 3 Pago histórico de planillas
        </h1>
        <div
          class="flex flex-col md:flex-row justify-between items-center mt-4"
        ></div>
      </div>

      <!-- Filtro por fecha (Inicio-Final) -->
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
            @change="aplicarFiltroFecha"
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
            @change="aplicarFiltroFecha"
          />
        </div>
        <button
          class="bg-green-500 hover:bg-green-600 text-white px-6 py-2 rounded mt-4 md:mt-0 col-span-1 md:col-span-2"
          @click="exportarAExcel"
        >
          Generar Excel
        </button>
      </div>

      <!-- Tabla con las planillas -->
      <div v-if="loading" class="text-center py-8">
        <p class="text-gray-600">Cargando planillas...</p>
      </div>
      <div v-else class="bg-white rounded-lg shadow w-full">
        <table class="w-full table-auto divide-y divide-gray-200">
          <thead class="bg-gray-50">
            <!-- Header de los nombres de las columnas de las planillas -->
            <tr>
              <th
                v-if="esSuperAdmin"
                class="px-4 py-3 text-left text-xs font-medium text-gray-500 uppercase"
              >
                Nombre de la empresa
              </th>
              <th
                class="px-4 py-3 text-left text-xs font-medium text-gray-500 uppercase"
              >
                Frecuencia de pago
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
            <tr v-if="planillasFiltradas && planillasFiltradas.length === 0">
              <td colspan="8" class="text-center py-4 text-gray-500">
                No hay planillas en el rango seleccionado.
              </td>
            </tr>
            <!-- Valores de cada unas de los campos de las planillas -->
            <tr v-for="(planilla, index) in planillasFiltradas" :key="index">
              <td
                v-if="esSuperAdmin"
                class="px-4 py-4 whitespace-nowrap text-sm text-gray-500"
              >
                {{ planilla.nombreEmpresa }}
              </td>
              <td class="px-4 py-4 whitespace-nowrap text-sm text-gray-500">
                {{ planilla.frecuenciaPago }}
              </td>
              <td
                class="px-4 py-4 whitespace-pre-wrap text-sm text-gray-500 break-words max-w-[200px]"
              >
                Del<br />{{ formatearFecha(planilla.periodoPago.fechaInicio)
                }}<br />al<br />{{
                  formatearFecha(planilla.periodoPago.fechaFin)
                }}
              </td>
              <td class="px-4 py-4 whitespace-nowrap text-sm text-gray-500">
                {{ formatearFecha(planilla.fechaPago) }}
              </td>
              <td class="px-4 py-4 whitespace-nowrap text-sm text-gray-500">
                {{ formatearColones(planilla.salarioBruto) }}
              </td>
              <td class="px-4 py-4 whitespace-nowrap text-sm text-gray-500">
                {{ formatearColones(planilla.cargasSocialesEmpleador) }}
              </td>
              <td class="px-4 py-4 whitespace-nowrap text-sm text-gray-500">
                {{ formatearColones(planilla.deduccionesVoluntarias) }}
              </td>
              <td class="px-4 py-4 whitespace-nowrap text-sm text-gray-500">
                {{ formatearColones(planilla.costoEmpleador) }}
              </td>
            </tr>

            <!-- TOTALES -->
            <tr
              class="bg-gray-100 font-semibold"
              v-if="planillasFiltradas && planillasFiltradas.length > 0"
            >
              <td v-if="esSuperAdmin" colspan="4" class="px-4 py-4 text-right">
                Totales:
              </td>
              <td v-else colspan="3" class="px-4 py-4 text-right">Totales:</td>
              <td class="px-4 py-4 whitespace-nowrap text-sm text-gray-700">
                {{
                  formatearColones(
                    planillasFiltradas.reduce((s, r) => s + r.salarioBruto, 0)
                  )
                }}
              </td>
              <td class="px-4 py-4 whitespace-nowrap text-sm text-gray-700">
                {{
                  formatearColones(
                    planillasFiltradas.reduce(
                      (s, r) => s + r.cargasSocialesEmpleador,
                      0
                    )
                  )
                }}
              </td>
              <td class="px-4 py-4 whitespace-nowrap text-sm text-gray-700">
                {{
                  formatearColones(
                    planillasFiltradas.reduce(
                      (s, r) => s + r.deduccionesVoluntarias,
                      0
                    )
                  )
                }}
              </td>
              <td class="px-4 py-4 whitespace-nowrap text-sm text-gray-700">
                {{
                  formatearColones(
                    planillasFiltradas.reduce((s, r) => s + r.costoEmpleador, 0)
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
import { ref, onMounted, computed } from "vue";
import axios from "axios";
import { API_BASE_URL } from "../config";

export default {
  setup() {
    const userStore = useUserStore();
    const loading = ref(true);

    // Variables para los filtros
    const fechaDesde = ref("");
    const fechaHasta = ref("");
    const planillas = ref([]);
    const planillasFiltradas = ref([]);

    const esSuperAdmin = computed(() => !userStore.empresa);

    const ponerNombreEmpresa = () => {
      if (userStore.empresa) {
        return userStore.empresa.nombre;
      }
      return "Nombre de la empresa";
    };

    // Método para llamar al API y obtener cada una de las planillas
    const fetchPlanillas = async () => {
      loading.value = true;
      try {
        if (esSuperAdmin.value) {
          // Si es super admin
          const empresasRes = await axios.get(`${API_BASE_URL}Empresas/todas`);
          const empresas = empresasRes.data || [];
          let todasPlanillas = [];
          for (const empresa of empresas) {
            const res = await axios.get(`${API_BASE_URL}Planilla/historico`, {
              params: { cedulaJuridica: empresa.cedulaJuridica },
            });
            if (Array.isArray(res.data)) {
              todasPlanillas = todasPlanillas.concat(res.data);
            }
          }
          planillas.value = todasPlanillas;
          planillasFiltradas.value = [...todasPlanillas];
        } else {
          // Si es empleador
          const respuestaPlanillaHistorico = await axios.get(
            `${API_BASE_URL}Planilla/historico`,
            {
              params: { cedulaJuridica: userStore.empresa.cedulaJuridica },
            }
          );
          planillas.value = respuestaPlanillaHistorico.data || [];
          planillasFiltradas.value = [...planillas.value];
        }
      } catch (error) {
        console.error("Error al obtener el histórico de planillas:", error);
        planillas.value = [];
        planillasFiltradas.value = [];
      } finally {
        loading.value = false;
      }
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

    const formatearColones = (valor) => {
      return new Intl.NumberFormat("es-CR", {
        style: "currency",
        currency: "CRC",
        minimumFractionDigits: 0,
      }).format(valor);
    };

    const aplicarFiltroFecha = () => {
      let filtradas = [...planillas.value];

      if (fechaDesde.value || fechaHasta.value) {
        const desde = fechaDesde.value ? new Date(fechaDesde.value) : null;
        const hasta = fechaHasta.value ? new Date(fechaHasta.value) : null;

        filtradas = filtradas.filter((planilla) => {
          const inicioPeriodo = new Date(planilla.periodoPago.fechaInicio);
          const finPeriodo = new Date(planilla.periodoPago.fechaFin);

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
      planillasFiltradas.value = filtradas;
    };

    const exportarAExcel = () => {
      let csvContent = "data:text/csv;charset=utf-8,";

      // Encabezados
      const headers = [
        "Nombre de la empresa",
        "Frecuencia de pago",
        "Periodo de pago",
        "Fecha de pago",
        "Salario Bruto (₡)",
        "Cargas sociales empleador (₡)",
        "Deducciones voluntarias (₡)",
        "Costo empleador (₡)",
      ];
      csvContent += headers.join(",") + "\n";

      // Datos
      planillasFiltradas.value.forEach((registro) => {
        const row = [
          `"${registro.nombreEmpresa}"`,
          `"${registro.frecuenciaPago}"`,
          `"Del ${formatearFecha(
            registro.periodoPago.fechaInicio
          )} al ${formatearFecha(registro.periodoPago.fechaFin)}"`,
          `"${formatearFecha(registro.fechaPago)}"`,
          registro.salarioBruto,
          registro.cargasSocialesEmpleador,
          registro.deduccionesVoluntarias,
          registro.costoEmpleador,
        ];
        csvContent += row.join(",") + "\n";
      });

      // Crear enlace de descarga
      const encodedUri = encodeURI(csvContent);
      const link = document.createElement("a");
      link.setAttribute("href", encodedUri);
      link.setAttribute(
        "download",
        `reporte_historico_planilla_${new Date()
          .toISOString()
          .slice(0, 10)}.csv`
      );
      document.body.appendChild(link);
      link.click();
      document.body.removeChild(link);
    };

    // Se llama a la función al montar el componente
    onMounted(async () => {
      if (!userStore.empresa && userStore.usuario?.cedulaPersona) {
        await userStore.fetchEmpresa(userStore.usuario.cedulaPersona);
      }
      await fetchPlanillas();
    });

    return {
      planillas,
      planillasFiltradas,
      fetchPlanillas,
      formatearFecha,
      formatearColones,
      aplicarFiltroFecha,
      exportarAExcel,
      ponerNombreEmpresa,
      fechaDesde,
      fechaHasta,
      esSuperAdmin,
    };
  },
};
</script>

<style scoped>
.container {
  max-width: 1200px;
}
</style>
