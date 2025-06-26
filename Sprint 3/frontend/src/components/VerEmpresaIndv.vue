<template>
  <div class="min-h-screen bg-gray-100 p-6">
    <div class="max-w-5xl mx-auto bg-white p-6 rounded-xl shadow-md">
      <!-- Encabezado -->
      <div class="flex justify-between items-center mb-6">
        <h1 class="text-3xl font-bold text-gray-800">
          {{ empresa?.nombre || "Nombre de empresa" }}
        </h1>
        <div class="text-center bg-white px-4 py-2 rounded-full shadow">
          <p class="text-lg font-semibold text-gray-800">
            {{ pagosPendientes }}
          </p>
          <p class="text-sm text-gray-600">Pagos Pendientes</p>
        </div>
      </div>

      <!-- Información de la empresa -->
      <div class="grid grid-cols-2 gap-6">
        <!-- Columna izquierda -->
        <div>
          <p class="text-lg font-bold text-gray-800">Nombre:</p>
          <p class="text-gray-700 mb-4">
            {{ empresa?.nombre || "Dato no disponible" }}
          </p>

          <p class="text-lg font-bold text-gray-800">Cédula jurídica:</p>
          <p class="text-gray-700 mb-4">
            {{ empresa?.cedulaJuridica || "Dato no disponible" }}
          </p>

          <p class="text-lg font-bold text-gray-800">Dirección:</p>
          <p class="text-gray-700">
            <span class="block"
              >Provincia: {{ empresa?.provincia || "Dato no disponible" }}</span
            >
            <span class="block"
              >Cantón: {{ empresa?.canton || "Dato no disponible" }}</span
            >
            <span class="block"
              >Distrito: {{ empresa?.distrito || "Dato no disponible" }}</span
            >
            <span class="block"
              >Señas: {{ empresa?.senas || "Dato no disponible" }}</span
            >
          </p>
        </div>

        <!-- Columna derecha -->
        <div>
          <p class="text-lg font-bold text-gray-800">Teléfono:</p>
          <p class="text-gray-700 mb-4">
            {{ empresa?.telefono || "Dato no disponible" }}
          </p>

          <p class="text-lg font-bold text-gray-800">Correo:</p>
          <p class="text-gray-700 mb-4">
            {{ empresa?.correo || "Dato no disponible" }}
          </p>

          <p class="text-lg font-bold text-gray-800">Modalidad de pago:</p>
          <p class="text-gray-700 mb-4">
            {{ empresa?.modalidadPago || "Dato no disponible" }}
          </p>
        </div>
      </div>

      <!-- Lista de empleados -->
      <div class="mt-6">
        <p class="text-lg font-bold text-gray-800 mb-2">Empleados:</p>
        <div class="bg-white p-4 rounded-xl shadow-md max-h-40 overflow-y-auto">
          <ul>
            <li
              v-for="empleado in empleadosEmpresa"
              :key="empleado.id"
              class="flex justify-between items-center mb-4 border-b pb-2"
            >
              <span class="text-gray-700">{{ empleado.nombre }}</span>
              <div class="flex space-x-2">
                <router-link
                  :to="`/employee/${empleado.cedula}`"
                  class="px-6 py-3 bg-blue-600 text-white rounded hover:bg-blue-700"
                >
                  Detalles
                </router-link>
                <button
                  class="bg-red-500 text-white px-3 py-1 rounded hover:bg-red-600"
                >
                  Eliminar
                </button>
              </div>
            </li>
          </ul>
        </div>
      </div>

      <!-- Botones de acción -->
      <div class="flex justify-between mt-6">
        <button
          @click="goToCrearEditarEmpresa"
          class="bg-blue-600 text-white font-semibold py-2 px-4 rounded hover:bg-blue-700"
        >
          Modificar
        </button>
        <button
          @click="goToEmpresaEliminada"
          class="bg-red-600 text-white font-semibold py-2 px-4 rounded hover:bg-red-700"
        >
          Eliminar
        </button>
      </div>
    </div>
  </div>
</template>

<script>
import { useUserStore } from "../store/user";
import { onMounted, ref } from "vue";
import { useRouter } from "vue-router";
import axios from "axios";
import { API_BASE_URL } from "../config";
export default {
  setup() {
    const router = useRouter();
    const userStore = useUserStore();
    const empresa = ref(null);
    const empleadosEmpresa = ref([]);
    const pagosPendientes = ref(0);

    const fetchEmpresaData = async () => {
      try {
        // Llama al método fetchEmpresa del store
        await userStore.fetchEmpresa(userStore.usuario.cedulaPersona);

        // Asigna los datos obtenidos al estado local
        empresa.value = userStore.empresa;
        empleadosEmpresa.value = userStore.empleadosEmpresa;
      } catch (error) {
        console.error("Error al obtener los datos de la empresa:", error);
      }
    };

    onMounted(() => {
      if (!userStore.usuario || !userStore.usuario.cedulaPersona) {
        window.location.href = "/";
      } else {
        fetchEmpresaData();
      }
    });

    const goToCrearBeneficios = () => {
      router.push("/benefitCreation");
    };

    const goToVerListaBeneficios = () => {
      router.push("/companyBenefits");
    };

    const goToCrearEditarEmpresa = () => {
      router.push("/editarEmpresa/" + userStore.usuario.cedulaPersona);
    };

    const goToEmpresaEliminada = async () => {
      const cedula = empresa.value?.cedulaJuridica;
      const url = `${API_BASE_URL}Empresas/borrar?cedula=${cedula}`;
      try {
        await axios.delete(url);
        router.push("/empresaEliminada");
      } catch (error) {
        console.error("Error al eliminar la empresa:", error);
      }
    };

    return {
      empresa,
      empleadosEmpresa,
      pagosPendientes,
      goToCrearBeneficios,
      goToVerListaBeneficios,
      goToCrearEditarEmpresa,
      goToEmpresaEliminada,
    };
  },
};
</script>

<style scoped></style>
