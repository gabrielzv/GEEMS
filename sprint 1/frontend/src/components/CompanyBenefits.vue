<!-- <template>
  <div class="flex justify-center items-center min-h-screen bg-gray-100">
    <form class="bg-white shadow rounded p-6 w-full max-w-md space-y-4">
      <h1 class="text-2xl font-bold mb-4 text-center">Lista de beneficios</h1>
      Ingresar cédula jurídica
      <div>
        <label class="block text-sm font-medium text-gray-700 mb-1"
          >Cédula Jurídica de la Empresa</label
        >
        <input
          v-model="form.cedulaJuridica"
          type="number"
          min="0"
          placeholder="Ej: 684"
          class="w-full border border-gray-300 rounded px-2 py-1 text-sm focus:outline-none focus:ring-2 focus:ring-blue-400"
        />
      </div>

      Botón
      <div class="flex justify-center pt-2">
        <button
          @click="verListaBeneficios"
          class="bg-blue-600 hover:bg-blue-700 text-white text-sm font-semibold py-1.5 px-4 rounded transition duration-200"
          type="button"
        >
          Ver lista de beneficios
        </button>
      </div>
    </form>
  </div>
</template>

<script>
import axios from "axios";
export default {
  data() {
    return {
      form: {
        cedulaJuridica: "",
      },
    };
  },
  methods: {
    // Método para validar el formulario, verifica que todos los campos estén llenos
    validarFormulario() {
      const { cedulaJuridica } = this.form;

      if (!cedulaJuridica) {
        alert(
          "Complete todos los espacios para la creación correcta del beneficio."
        );
        return false;
      }

      return true;
    },
    async verListaBeneficios() {
      if (!this.validarFormulario()) return;

      try {
        const response = await axios.post(
          "https://localhost:7014/api/GetCompanyBenefits/crearBeneficio",
          this.form
        );
        alert(response.data);
      } catch (error) {
        console.error("Error al crear el beneficio:", error);
        alert(
          error.response?.data ||
            "Ocurrió un error al intentar crear el beneficio."
        );
      }
    },
  },
};
</script> -->

<!-- <template>
  <div class="flex justify-center items-center min-h-screen bg-gray-100">
    <div class="w-full max-w-2xl">
      Formulario para ingresar la cédula jurídica 
      <form
        v-if="!mostrarBeneficios"
        class="bg-white shadow rounded p-6 space-y-4"
      >
        <h1 class="text-2xl font-bold mb-4 text-center">Lista de beneficios</h1>
        <div>
          <label class="block text-sm font-medium text-gray-700 mb-1"
            >Cédula Jurídica de la Empresa</label
          >
          <input
            v-model="form.cedulaJuridica"
            type="number"
            min="0"
            placeholder="Ej: 684"
            class="w-full border border-gray-300 rounded px-2 py-1 text-sm focus:outline-none focus:ring-2 focus:ring-blue-400"
          />
        </div>
        <div class="flex justify-center pt-2">
          <button
            @click="verListaBeneficios"
            class="bg-blue-600 hover:bg-blue-700 text-white text-sm font-semibold py-1.5 px-4 rounded transition duration-200"
            type="button"
          >
            Ver lista de beneficios
          </button>
        </div>
      </form>

      Componente para mostrar la lista de beneficios
      <BenefitsList
        v-if="mostrarBeneficios"
        :beneficios="beneficios"
      />
    </div>
  </div>
</template>

<script>
import axios from "axios";
import BenefitsList from "./BenefitsList.vue";

export default {
  components: {
    BenefitsList,
  },
  data() {
    return {
      form: {
        cedulaJuridica: "",
      },
      beneficios: [],
      mostrarBeneficios: false,
    };
  },
  methods: {
    validarFormulario() {
      const { cedulaJuridica } = this.form;

      if (!cedulaJuridica) {
        alert("Complete todos los espacios para la creación correcta del beneficio.");
        return false;
      }

      return true;
    },
    async verListaBeneficios() {
      if (!this.validarFormulario()) return;

      try {
        const response = await axios.get(
          `https://localhost:7014/api/GetCompanyBenefits/${this.form.cedulaJuridica}`
        );
        this.beneficios = response.data;
        this.mostrarBeneficios = true;
      } catch (error) {
        console.error("Error al obtener los beneficios:", error);
        alert(
          error.response?.data?.message || "Ocurrió un error al intentar obtener los beneficios."
        );
      }
    },
  },
};
</script> -->

<template>
  <div class="flex justify-center items-center min-h-screen bg-gray-100">
    <div class="bg-white shadow rounded p-6 max-w-md w-full">
      <h1 class="text-2xl font-bold mb-4 text-center">
        Beneficios de la Empresa
      </h1>

      <!-- Mensaje de error -->
      <p v-if="error" class="text-red-500 mt-4 text-center">{{ error }}</p>

      <!-- Lista de beneficios -->
      <div v-if="beneficios.length" class="mt-6">
        <h2 class="text-xl font-semibold mb-2 text-center">
          Beneficios encontrados:
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
          </li>
        </ul>
      </div>

      <!-- Mensaje si no hay beneficios -->
      <p v-else class="text-center text-gray-500 mt-4">
        No se encontraron beneficios para esta empresa.
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

    // Método para obtener los beneficios de la empresa
    const fetchBeneficios = async () => {
      try {
        // Se llama a fetchEmpresa para obtener la cédula jurídica
        await userStore.fetchEmpresa(userStore.usuario.cedulaPersona);

        if (userStore.empresa) {
          const cedulaJuridica = userStore.empresa.cedulaJuridica;

          // Se hace el get para obtener los beneficios creados de la empresa
          const response = await axios.get(
            `https://localhost:7014/api/GetCompanyBenefits/${cedulaJuridica}`
          );
          beneficios.value = response.data;
        } else {
          error.value = "No se pudo obtener la información de la empresa.";
        }
      } catch (err) {
        console.error("Error al obtener los beneficios:", err);
        error.value =
          err.response?.data?.message ||
          "Ocurrió un error al obtener los beneficios.";
      }
    };

    // Se llama a la función al montar el componente
    onMounted(() => {
      if (!userStore.usuario || !userStore.usuario.cedulaPersona) {
        window.location.href = "/";
      } else {
        fetchBeneficios();
      }
    });

    return {
      beneficios,
      error,
    };
  },
};
</script>
