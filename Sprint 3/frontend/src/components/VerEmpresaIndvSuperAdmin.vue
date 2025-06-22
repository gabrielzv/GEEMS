<template>
    <div class="min-h-screen bg-gray-100 p-6">
      <div class="max-w-5xl mx-auto bg-white p-6 rounded-xl shadow-md">
        <!-- Encabezado -->
        <div class="flex justify-between items-center mb-6">
          <h1 class="text-3xl font-bold text-gray-800">
            {{ empresa?.nombre || "Nombre de empresa" }}
          </h1>
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
  
  
        <!-- Botones de acción -->
        <div class="flex justify-between mt-6">
          <button
            class="bg-blue-600 text-white font-semibold py-2 px-4 rounded hover:bg-blue-700"
          >
            Modificar
          </button>
          <button
            class="bg-red-600 text-white font-semibold py-2 px-4 rounded hover:bg-red-700"
          >
            Eliminar
          </button>
        </div>
      </div>
    </div>
  </template>
  
  <script>
  import { ref, onMounted } from "vue";
  import axios from "axios";
  import { useRoute } from "vue-router";
  import { API_BASE_URL } from "../config";
  export default {
    setup() {
      const route = useRoute();
      const empresa = ref(null);
      const empleadosEmpresa = ref([]);
  
      const fetchEmpresaData = async () => {
        const empresaId = route.params.empresaId; // Obtener el ID de la empresa desde el routing
        console.log("ID de la empresa:", empresaId);
        const url = `${API_BASE_URL}Empresa/por-cedula-juridica/${empresaId}`;
        try {
          const response = await axios.get(
            url
          );
  
          // Asignar los datos de la empresa y empleados
          empresa.value = response.data.empresa;
          empleadosEmpresa.value = response.data.empleados;
        } catch (error) {
          console.error("Error al obtener los datos de la empresa:", error);
        }
      };
  
      onMounted(() => {
        fetchEmpresaData();
      });
  
      return { empresa, empleadosEmpresa };
    },
  };
  </script>
  
  <style scoped>
  </style>