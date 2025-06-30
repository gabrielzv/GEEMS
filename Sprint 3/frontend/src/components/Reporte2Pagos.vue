<template>
  <div class="p-8">
    <h1 class="text-2xl font-bold mb-4 text-blue-700">
      REPORTE 2 – EMPLEADO (HISTÓRICO PAGO PLANILLA)
    </h1>
    <div class="mb-4 flex gap-4 items-end">
      <div>
        <label class="block font-semibold">Fecha inicio</label>
        <input
          type="date"
          v-model="fechaInicio"
          class="border rounded px-2 py-1"
          @change="fetchPagos"
        />
      </div>
      <div>
        <label class="block font-semibold">Fecha final</label>
        <input
          type="date"
          v-model="fechaFinal"
          class="border rounded px-2 py-1"
          @change="fetchPagos"
        />
      </div>
    </div>

    <div class="mb-2">
      <span class="font-bold text-blue-700">Nombre de la empresa:</span>
      <span>{{ nombreEmpresa }}</span>
    </div>
    <div class="mb-4">
      <span class="font-bold text-blue-700">Nombre completo del empleado:</span>
      <span>{{ nombreEmpleado }}</span>
    </div>

    <div class="overflow-x-auto">
      <template v-if="pagos.length">
        <table class="min-w-full border text-center">
          <thead>
            <tr class="bg-blue-200">
              <th class="px-2 py-1 border">Tipo de contrato</th>
              <th class="px-2 py-1 border">Posición</th>
              <th class="px-2 py-1 border">Fecha de pago</th>
              <th class="px-2 py-1 border">Salario Bruto</th>
              <th class="px-2 py-1 border">
                Deducciones obligatorias empleado
              </th>
              <th class="px-2 py-1 border">Deducciones voluntarias</th>
              <th class="px-2 py-1 border">Salario neto</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="pago in pagos" :key="pago.fechaRealizada">
              <td class="border">{{ pago.tipoContrato }}</td>
              <td class="border">{{ pago.posicion }}</td>
              <td class="border">{{ formatFecha(pago.fechaRealizada) }}</td>
              <td class="border">₡{{ formatNumber(pago.montoBruto) }}</td>
              <td class="border">
                ₡{{ formatNumber(totalObligatorias(pago.deducciones)) }}
              </td>
              <td class="border">
                ₡{{ formatNumber(totalVoluntarias(pago.deducciones)) }}
              </td>
              <td class="border font-semibold">
                ₡{{ formatNumber(pago.montoPago) }}
              </td>
            </tr>
          </tbody>
          <tfoot>
            <tr class="bg-blue-100 font-bold">
              <td class="border" colspan="3">Totales</td>
              <td class="border">
                ₡{{ formatNumber(totalColumn("montoBruto")) }}
              </td>
              <td class="border">
                ₡{{ formatNumber(totalColumn("obligatorias")) }}
              </td>
              <td class="border">
                ₡{{ formatNumber(totalColumn("voluntarias")) }}
              </td>
              <td class="border">
                ₡{{ formatNumber(totalColumn("montoPago")) }}
              </td>
            </tr>
          </tfoot>
        </table>
      </template>
      <template v-else>
        <div class="text-center text-gray-500 py-8">
          No hay pagos para mostrar en el periodo seleccionado.
        </div>
      </template>
    </div>

    <div class="px-4 py-4 mb-4 flex gap-4 items-end">
      <template v-if="pagos.length">
        <button
          @click="enviarPDFPorCorreo"
          class="bg-red-600 text-white px-4 py-2 rounded hover:bg-red-700"
        >
          Enviar PDF por correo
        </button>
        <button
          @click="exportarPDF"
          class="bg-red-600 text-white px-4 py-2 rounded hover:bg-red-700"
        >
          Descargar PDF
        </button>
        <button
          @click="enviarExcelPorCorreo"
          class="bg-green-600 text-white px-4 py-2 rounded hover:bg-green-700"
        >
          Enviar Excel por correo
        </button>
        <button
          @click="exportarExcel"
          class="bg-green-600 text-white px-4 py-2 rounded hover:bg-green-700"
        >
          Descargar Excel
        </button>
      </template>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted, watch } from "vue";
import axios from "axios";
import * as XLSX from "xlsx";
import jsPDF from "jspdf";
import autoTable from "jspdf-autotable";
import { useUserStore } from "../store/user";
import { API_BASE_URL } from "@/config";

const pagos = ref([]);
const fechaFinal = ref(new Date().toISOString().slice(0, 10));
const fechaInicio = ref(
  new Date(Date.now() - 7 * 24 * 60 * 60 * 1000).toISOString().slice(0, 10)
);

const nombreEmpresa = ref("");
const nombreEmpleado = ref("");
const correoDestino = ref("");
const nombre = ref({});

const userStore = useUserStore();
const empleado = userStore.empleado;
const usuario = userStore.usuario;


const cedulaEmpleado = String(empleado.cedulaPersona);
nombreEmpresa.value = empleado.nombreEmpresa;
nombreEmpleado.value = nombre.value.fullName || "Dato no disponible";
correoDestino.value = usuario.email || "Dato no disponible";
function formatFecha(fecha) {
  if (!fecha) return "";
  return new Date(fecha).toLocaleDateString("es-CR");
}
async function cargarDatosEmpleado() {
  console.log("Cargando datos del empleado");
  try {
    const url = `${API_BASE_URL}Persona/${usuario.cedulaPersona}`;
    const personaRes = await axios.get(url);
    const data = personaRes.data;

    nombre.value = {
      fullName: data.fullName || "Dato no disponible",
      email: data.email || "Dato no disponible",
    };

    nombreEmpleado.value = nombre.value.fullName;
    correoDestino.value = nombre.value.email || "Dato no disponible";
  } catch (err) {
    console.error("Error al obtener los datos de la persona", err);
    nombreEmpleado.value = "Dato no disponible";
  }
  console.log("Datos del empleado cargados:", nombre.value);
}
function formatNumber(numero) {
  return numero?.toLocaleString("es-CR", { maximumFractionDigits: 2 }) || "0";
}
function totalObligatorias(deducciones) {
  return deducciones
    .filter((deducciones) => deducciones.tipo === "Obligatoria")
    .reduce((all, deducciones) => all + deducciones.monto, 0);
}
function totalVoluntarias(deducciones) {
  return deducciones
    .filter((deducciones) => deducciones.tipo === "Voluntaria")
    .reduce((all, deducciones) => all + deducciones.monto, 0);
}
function totalColumn(tipo) {
  if (tipo === "montoBruto")
    return pagos.value.reduce((all, pago) => all + pago.montoBruto, 0);
  if (tipo === "montoPago")
    return pagos.value.reduce((all, pago) => all + pago.montoPago, 0);
  if (tipo === "obligatorias")
    return pagos.value.reduce(
      (all, pago) => all + totalObligatorias(pago.deducciones),
      0
    );
  if (tipo === "voluntarias")
    return pagos.value.reduce(
      (all, pago) => all + totalVoluntarias(pago.deducciones),
      0
    );
  return 0;
}

async function fetchPagos() {
  if (!cedulaEmpleado || !fechaInicio.value || !fechaFinal.value) {
    pagos.value = [];
    return;
  }
  // Convertir fechas a 'yyyy-MM-dd HH:mm:ss.SSS'
  function toFullDateTime(fecha, isStart) {
    // fecha es 'YYYY-MM-DD'
    const [yyyy, MM, dd] = fecha.split("-");
    if (isStart) {
      return `${yyyy}-${MM}-${dd} 00:00:00.000`;
    } else {
      return `${yyyy}-${MM}-${dd} 23:59:59.999`;
    }
  }

  try {
    const res = await axios.get(`${API_BASE_URL}Pagos/Periodo`, {
      params: {
        cedulaEmpleado,
        fechaInicio: toFullDateTime(fechaInicio.value, true),
        fechaFin: toFullDateTime(fechaFinal.value, false),
      },
    });
    pagos.value = res.data || [];
  } catch (e) {
    pagos.value = [];
  }
}

function exportarExcel() {
  const ws = XLSX.utils.json_to_sheet(
    pagos.value.map((pago) => ({
      "Tipo de contrato": pago.tipoContrato,
      Posición: pago.posicion,
      "Fecha de pago": formatFecha(pago.fechaRealizada),
      "Salario Bruto": "₡" + pago.montoBruto,
      "Deducciones obligatorias empleado":
        "₡" + totalObligatorias(pago.deducciones),
      "Deducciones voluntarias": "₡" + totalVoluntarias(pago.deducciones),
      "Salario neto": "₡" + pago.montoPago,
    }))
  );
  const wb = XLSX.utils.book_new();
  XLSX.utils.book_append_sheet(wb, ws, "Reporte2");
  XLSX.writeFile(wb, "Reporte2Pagos.xlsx");
}

function exportarPDF() {
  const doc = new jsPDF();
  doc.setFontSize(14);
  doc.text("REPORTE 2 – EMPLEADO (HISTÓRICO PAGO PLANILLA)", 10, 10);
  doc.setFontSize(10);
  doc.text(`Empresa: ${nombreEmpresa.value}`, 10, 18);
  doc.text(`Empleado: ${nombreEmpleado.value}`, 10, 24);
  autoTable(doc, {
    startY: 30,
    head: [
      [
        "Tipo de contrato",
        "Posición",
        "Fecha de pago",
        "Salario Bruto",
        "Deducciones obligatorias empleado",
        "Deducciones voluntarias",
        "Salario neto",
      ],
    ],
    body: pagos.value.map((pago) => [
      pago.tipoContrato,
      pago.posicion,
      formatFecha(pago.fechaRealizada),
      "CRC" + formatNumber(pago.montoBruto),
      "CRC" + formatNumber(totalObligatorias(pago.deducciones)),
      "CRC" + formatNumber(totalVoluntarias(pago.deducciones)),
      "CRC" + formatNumber(pago.montoPago),
    ]),
    foot: [
      [
        { content: "Totales", colSpan: 3 },
        "CRC" + formatNumber(totalColumn("montoBruto")),
        "CRC" + formatNumber(totalColumn("obligatorias")),
        "CRC" + formatNumber(totalColumn("voluntarias")),
        "CRC" + formatNumber(totalColumn("montoPago")),
      ],
    ],
  });
  doc.save("Reporte2Pagos.pdf");
}

async function enviarPDFPorCorreo() {
  try {
    // Generar PDF como Blob
    const doc = new jsPDF();
    doc.setFontSize(14);
    doc.text("REPORTE 2 – EMPLEADO (HISTÓRICO PAGO PLANILLA)", 10, 10);
    doc.setFontSize(10);
    doc.text(`Empresa: ${nombreEmpresa.value}`, 10, 18);
    doc.text(`Empleado: ${nombreEmpleado.value}`, 10, 24);
    autoTable(doc, {
      startY: 30,
      head: [
        [
          "Tipo de contrato",
          "Posición",
          "Fecha de pago",
          "Salario Bruto",
          "Deducciones obligatorias empleado",
          "Deducciones voluntarias",
          "Salario neto",
        ],
      ],
      body: pagos.value.map((pago) => [
        pago.tipoContrato,
        pago.posicion,
        formatFecha(pago.fechaRealizada),
        "CRC" + formatNumber(pago.montoBruto),
        "CRC" + formatNumber(totalObligatorias(pago.deducciones)),
        "CRC" + formatNumber(totalVoluntarias(pago.deducciones)),
        "CRC" + formatNumber(pago.montoPago),
      ]),
      foot: [
        [
          { content: "Totales", colSpan: 3 },
          "CRC" + formatNumber(totalColumn("montoBruto")),
          "CRC" + formatNumber(totalColumn("obligatorias")),
          "CRC" + formatNumber(totalColumn("voluntarias")),
          "CRC" + formatNumber(totalColumn("montoPago")),
        ],
      ],
    });
    const pdfBlob = doc.output("blob");

    const formData = new FormData();
    formData.append("Archivo", pdfBlob, "Reporte2Pagos.pdf");
    formData.append("Correo", correoDestino.value || usuario.email || "");
    formData.append("NombreUsuario", nombreEmpleado.value || "Usuario");

    await axios.post(`${API_BASE_URL}Reporte/Reporte`, formData, {
      headers: { "Content-Type": "multipart/form-data" },
    });
    alert("PDF enviado correctamente.");
  } catch (err) {
    alert("Error al enviar el PDF.");
  }
}

async function enviarExcelPorCorreo() {
  try {
    // Generar Excel como Blob
    const ws = XLSX.utils.json_to_sheet(
      pagos.value.map((pago) => ({
        "Tipo de contrato": pago.tipoContrato,
        Posición: pago.posicion,
        "Fecha de pago": formatFecha(pago.fechaRealizada),
        "Salario Bruto": "₡" + pago.montoBruto,
        "Deducciones obligatorias empleado":
          "₡" + totalObligatorias(pago.deducciones),
        "Deducciones voluntarias": "₡" + totalVoluntarias(pago.deducciones),
        "Salario neto": "₡" + pago.montoPago,
      }))
    );
    const wb = XLSX.utils.book_new();
    XLSX.utils.book_append_sheet(wb, ws, "Reporte2");
    const excelBuffer = XLSX.write(wb, { bookType: "xlsx", type: "array" });
    const excelBlob = new Blob([excelBuffer], {
      type: "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
    });

    const formData = new FormData();
    formData.append("Archivo", excelBlob, "Reporte2Pagos.xlsx");
    formData.append("Correo", correoDestino.value || usuario.email || "");
    formData.append("NombreUsuario", nombreEmpleado.value || "Usuario");

    await axios.post(`${API_BASE_URL}Reporte/Reporte`, formData, {
      headers: { "Content-Type": "multipart/form-data" },
    });
    alert("Excel enviado correctamente.");
  } catch (err) {
    alert("Error al enviar el Excel." + err);
  }
}
onMounted(async () => {
  await cargarDatosEmpleado();
  await fetchPagos();
});
watch([fechaInicio, fechaFinal], fetchPagos);
</script>

<style scoped>
table th,
table td {
  min-width: 120px;
}
</style>
