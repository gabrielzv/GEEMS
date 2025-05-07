<template>
  <div class="flex justify-center items-center min-h-screen bg-gray-100">
    <div class="bg-white shadow rounded p-6 max-w-md w-full">
      <h1 class="text-2xl font-bold mb-4 text-center">
        Beneficios matriculados:
      </h1>

      <!-- Mensaje de error -->
      <p v-if="error" class="text-red-500 mt-4 text-center">{{ error }}</p>

      <!-- Lista de beneficios -->
      <div v-if="beneficios.length" class="mt-6">
        <ul class="space-y-4">
          <li
            v-for="beneficio in beneficios"
            :key="beneficio.id"
            class="p-4 border rounded shadow"
          >
            <p><strong>Nombre:</strong> {{ beneficio.nombre }}</p>
            <p><strong>Descripción:</strong> {{ beneficio.descripcion }}</p>
            <p><strong>Costo:</strong> ₡{{ beneficio.costo }}</p>
            <!-- ATRIBUTOS POR AGREGAR A LA BASE DE DATOS EN BENEFICIO -->
            <!--<p><strong>Frecuencia de pago:</strong> {{ beneficio.frecuencia }}</p>
            <p><strong>Estado:</strong> {{ beneficio.estado }}</p> -->
          </li>
        </ul>
      </div>

      <!-- Mensaje si no hay beneficios matriculados -->
      <p v-else class="text-center text-gray-500 mt-4">
        No se han matriculado beneficios.
      </p>
    </div>
  </div>
</template>

<script>
import axios from "axios";
import { useUserStore } from "../store/user";
import { onMounted, ref } from "vue";

export default {
  setup() {
    const userStore = useUserStore();
    const beneficios = ref([]);
    const error = ref("");

    // Método para obtener los beneficios del empleado
    const fetchBeneficiosEmpleado = async () => {
      try {
        // Se llama a fetchEmpleado para obtener la información del empleado
        await userStore.fetchEmpleado(userStore.usuario.cedulaPersona);

        if (userStore.empleado && userStore.empleado.id) {
          const idEmpleado = userStore.empleado.id;

          // Se hace el get para obtener los beneficios que tiene matriculados el empleado
          const response = await axios.get(
            `https://localhost:7014/api/GetEmployeeBenefits/${idEmpleado}`
          );
          beneficios.value = response.data;
        } else {
          error.value = "No se pudo obtener la información del empleado.";
        }
      } catch (err) {
        console.error("Error al obtener los beneficios matriculados:", err);
        error.value =
          err.response?.data?.message ||
          "Ocurrió un error al obtener los beneficios matriculados.";
      }
    };

    // Llama a la función al montar el componente
    onMounted(() => {
      if (!userStore.usuario || !userStore.usuario.cedulaPersona) {
        window.location.href = "/";
      } else {
        fetchBeneficiosEmpleado();
      }
    });

    return {
      beneficios,
      error,
    };
  },
};
</script>
