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
        Buscar Beneficios por Empresa
      </h1>

      <!-- Ingreso de cédula jurídica -->
      <input
        v-model="cedula"
        type="number"
        placeholder="Ingrese cédula jurídica"
        class="border border-gray-300 rounded px-4 py-2 w-full mb-4"
      />

      <!-- Botón -->
      <button
        @click="buscarBeneficios"
        class="bg-blue-500 text-white px-4 py-2 rounded w-full hover:bg-blue-600"
      >
        Buscar
      </button>

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
    </div>
  </div>
</template>

<script>
import axios from "axios";

export default {
  data() {
    return {
      cedula: "",
      beneficios: [],
      error: "",
    };
  },
  methods: {
    // Método para validar la cédula jurídica ingresada
    validarSolicitud() {
      if (!this.cedula || isNaN(this.cedula) || this.cedula.length < 11) {
        alert("Debe ingresar una cédula jurídica válida.");
        return false;
      }
      return true;
    },
    // Método para buscar los beneficios de la empresa
    async buscarBeneficios() {
      this.beneficios = [];
      if (!this.validarSolicitud()) {
        return;
      }
      try {
        const response = await axios.get(
          `https://localhost:7014/api/GetCompanyBenefits/${this.cedula}`
        );
        this.beneficios = response.data;
      } catch (err) {
        if (err.response && err.response.data) {
          alert(err.response.data.message);
        } else {
          alert("Error al obtener beneficios.");
        }
      }
    },
  },
};
</script>
