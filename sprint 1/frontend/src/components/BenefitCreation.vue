<template>
  <div class="flex justify-center items-center min-h-screen bg-gray-100">
    <form class="bg-white shadow rounded p-6 w-full max-w-md space-y-4">
      <h1 class="text-2xl font-bold mb-4 text-center">
        Creación de Beneficios
      </h1>
      <!-- Nombre -->
      <div>
        <label class="block text-sm font-medium text-gray-700 mb-1"
          >Nombre</label
        >
        <input
          v-model="form.nombre"
          type="text"
          placeholder="Ej: Gimnasio de la empresa"
          class="w-full border border-gray-300 rounded px-2 py-1 text-sm focus:outline-none focus:ring-2 focus:ring-blue-400"
        />
      </div>
      <!-- Descripción -->
      <div>
        <label class="block text-sm font-medium text-gray-700 mb-1"
          >Descripción</label
        >
        <textarea
          v-model="form.descripcion"
          rows="2"
          placeholder="Ej: Este beneficio incluye acceso a un gimnasio local, clases de yoga y pilates."
          class="w-full border border-gray-300 rounded px-2 py-1 text-sm focus:outline-none focus:ring-2 focus:ring-blue-400"
        ></textarea>
      </div>
      <!-- Tipo de Beneficio -->
      <!-- <div>
        <label class="block text-sm font-medium text-gray-700 mb-1"
          >Tipo de Beneficio</label
        >
        <select
          v-model="form.tipoBeneficio"
          class="w-full border border-gray-300 rounded px-2 py-1 text-sm focus:outline-none focus:ring-2 focus:ring-blue-400"
        >
          <option disabled value="">Selecciona una opción</option>
          <option value="Salud">Salud</option>
          <option value="Financiero">Financiero</option>
          <option value="Nutricional">Nutricional</option>
          <option value="Maternidad/Paternidad">Maternidad/Paternidad</option>
          <option value="Legal">Legal</option>
        </select>
      </div> -->
      <!-- Costo -->
      <div>
        <label class="block text-sm font-medium text-gray-700 mb-1"
          >Costo</label
        >
        <input
          v-model="form.costo"
          type="number"
          min="0"
          placeholder="Ej: 12000"
          class="w-full border border-gray-300 rounded px-2 py-1 text-sm focus:outline-none focus:ring-2 focus:ring-blue-400"
        />
      </div>
      <!-- Tipo de Empleado Elegible -->
      <!-- <div>
        <label class="block text-sm font-medium text-gray-700 mb-2"
          >Tipo de Empleado Elegible</label
        >
        <ul
          class="w-full text-sm font-medium text-gray-900 bg-white border border-gray-200 rounded-lg"
        >
          <li
            v-for="tipo in tiposDeEmpleado"
            :key="tipo"
            class="w-full border-b border-gray-200 last:border-b-0"
          >
            <div class="flex items-center ps-3">
              <input
                :id="'checkbox-' + tipo"
                type="checkbox"
                :value="tipo"
                v-model="form.tipoEmpleado"
                class="w-4 h-4 text-blue-600 bg-gray-100 border-gray-300 rounded-sm focus:ring-blue-500 focus:ring-2"
              />
              <label
                :for="'checkbox-' + tipo"
                class="w-full py-3 ms-2 text-sm font-medium text-gray-900"
              >
                {{ tipo }}
              </label>
            </div>
          </li>
        </ul>
      </div> -->
      <!-- Tiempo Mínimo en Empresa -->
      <div>
        <label class="block text-sm font-medium text-gray-700 mb-1"
          >Tiempo Mínimo en Empresa (meses)</label
        >
        <input
          v-model="form.tiempoMinimo"
          type="number"
          min="0"
          placeholder="Ej: 6"
          class="w-full border border-gray-300 rounded px-2 py-1 text-sm focus:outline-none focus:ring-2 focus:ring-blue-400"
        />
      </div>
      <!-- Cédula Jurídica -->
      <div>
        <label class="block text-sm font-medium text-gray-700 mb-1"
          >Cédula Jurídica de la Empresa</label
        >
        <input
          v-model="form.cedulaJuridica"
          type="number"
          min="0"
          placeholder="Ej: 987123"
          class="w-full border border-gray-300 rounded px-2 py-1 text-sm focus:outline-none focus:ring-2 focus:ring-blue-400"
          readonly
        />
      </div>

      <!-- Botón -->
      <div class="flex justify-center pt-2">
        <button
          @click="crearBeneficio"
          class="bg-blue-600 hover:bg-blue-700 text-white text-sm font-semibold py-1.5 px-4 rounded transition duration-200"
          type="button"
        >
          Crear beneficio
        </button>
      </div>
    </form>
  </div>
</template>

<script>
import axios from "axios";
import { useUserStore } from "../store/user";
import { onMounted, ref } from "vue";

export default {
  setup() {
    const userStore = useUserStore();
    const form = ref({
      nombre: "",
      descripcion: "",
      costo: "",
      tiempoMinimo: "",
      cedulaJuridica: "", // Se llena automáticamente por el fetch de la empresa
    });

    // Método para validar el formulario
    const validarFormulario = () => {
      const { nombre, descripcion, costo, tiempoMinimo, cedulaJuridica } =
        form.value;

      if (
        !nombre.trim() ||
        !descripcion.trim() ||
        !costo ||
        !tiempoMinimo ||
        !cedulaJuridica
      ) {
        alert(
          "Complete todos los espacios para la creación correcta del beneficio."
        );
        return false;
      }

      return true;
    };

    // Método para crear el beneficio
    const crearBeneficio = async () => {
      if (!validarFormulario()) return;

      try {
        const response = await axios.post(
          "https://localhost:7014/api/Beneficio/crearBeneficio",
          form.value
        );
        alert(response.data);
      } catch (error) {
        console.error("Error al crear el beneficio:", error);
        alert(
          error.response?.data ||
            "Ocurrió un error al intentar crear el beneficio."
        );
      }
    };

    // Obtener la empresa automáticamente al montar el componente
    const fetchEmpresaData = async () => {
      try {
        await userStore.fetchEmpresa(userStore.usuario.cedulaPersona);
        if (userStore.empresa) {
          // Asigna la cédula jurídica automáticamente
          form.value.cedulaJuridica = userStore.empresa.cedulaJuridica;
        } else {
          alert("No se pudo obtener la información de la empresa.");
        }
      } catch (error) {
        console.error("Error al obtener los datos de la empresa:", error);
        alert("Ocurrió un error al cargar la información de la empresa.");
      }
    };
    // Se llama a la función al montar el componente
    onMounted(() => {
      if (!userStore.usuario || !userStore.usuario.cedulaPersona) {
        window.location.href = "/";
      } else {
        fetchEmpresaData();
      }
    });

    return {
      form,
      crearBeneficio,
    };
  },
};
</script>
