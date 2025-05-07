<template>
  <!-- <div class="flex justify-center items-center min-h-screen bg-gray-100">
    <div class="bg-white shadow rounded p-6 w-full max-w-2xl space-y-4">
      <h1 class="text-2xl font-bold mb-4 text-center">Matricular Beneficio</h1>

      Paso 1: Solicitar cédula jurídica
      <div v-if="!mostrarBeneficios">
        <label class="block text-sm font-medium text-gray-700 mb-1">Cédula Jurídica</label>
        <input
          v-model="cedulaJuridica"
          type="number"
          placeholder="Ingrese la cédula jurídica"
          class="w-full border border-gray-300 rounded px-2 py-1 text-sm focus:outline-none focus:ring-2 focus:ring-blue-400"
        />
        <button
          @click="obtenerBeneficios"
          class="bg-blue-600 hover:bg-blue-700 text-white text-sm font-semibold py-1.5 px-4 rounded mt-4"
        >
          Ver Beneficios Disponibles
        </button>
      </div>

      Paso 2: Mostrar beneficios disponibles
      <div v-if="beneficios.length" class="mt-6">
        <h2 class="text-xl font-semibold mb-2 text-center">
          Beneficios disponibles:
        </h2>
        <ul class="space-y-4">
          <li
            v-for="beneficio in beneficios"
            :key="beneficio.id"
            class="p-4 border rounded shadow"
          >
            <p><strong>Nombre:</strong> {{ beneficio.nombre }}</p>
            <p><strong>Descripción:</strong> {{ beneficio.descripcion }}</p>
            <p><strong>Costo:</strong> ₡{{ beneficio.costo }}</p>
            <p>
              <strong>Tiempo mínimo en empresa:</strong>
              {{ beneficio.tiempoMinimoEnEmpresa }} meses
            </p>
            Botón
            <button
              @click="matricularBeneficio(beneficio.id)"
              class=" mt-5 mb-2 bg-blue-500 text-white px-4 py-2 rounded w-full hover:bg-blue-600"
            >
              Matricular beneficio
            </button>
          </li>
        </ul>
      </div>
    </div>
  </div> -->
  <div>Aqui van los beneficios que tiene el empleado para que pueda matricularlos</div>
</template>

<script>
// import axios from "axios";

// export default {
//   data() {
//     return {
//       cedulaJuridica: "", // Cédula jurídica ingresada por el usuario
//       beneficios: [], // Lista de beneficios disponibles
//       mostrarBeneficios: false, // Controla si se muestran los beneficios
//     };
//   },
//   methods: {
//     async obtenerBeneficios() {
//       if (!this.cedulaJuridica) {
//         alert("Por favor, ingrese la cédula jurídica.");
//         return;
//       }

//       try {
//         const response = await axios.get(
//           `https://localhost:7014/api/GetCompanyBenefits/${this.cedulaJuridica}`
//         );
//         this.beneficios = response.data;
//         this.mostrarBeneficios = true;
//       } catch (error) {
//         console.error("Error al obtener los beneficios:", error);
//         alert(
//           error.response?.data?.message ||
//             "Ocurrió un error al intentar obtener los beneficios."
//         );
//       }
//     },
//     async matricularBeneficio(beneficioId) {
//       const empleadoId = "26257399-F0F1-4E8A-9939-2C79D89C0E28"; // Reemplaza con el ID del empleado autenticado

//       try {
//         const response = await axios.post(
//           "https://localhost:7014/api/SetBeneficioPorEmpleado/matricularBeneficio",
//           {
//             BeneficioId: beneficioId,
//             EmpleadoId: empleadoId,
//           }
//         );
//         alert(response.data);
//       } catch (error) {
//         console.error("Error al matricular el beneficio:", error);
//         alert(
//           error.response?.data?.message ||
//             "Ocurrió un error al intentar matricular el beneficio."
//         );
//       }
//     },
//   },
// };
</script>