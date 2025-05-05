<template>
  <div class="min-h-screen flex items-center justify-center bg-gray-100 px-4">
    <form
      @submit.prevent="registrarEmpresa"
      novalidate
      class="bg-white p-6 rounded-xl shadow-md w-full max-w-4xl space-y-5"
    >
      <p class="text-3xl font-bold text-center text-gray-800">
        Registrar nueva empresa
      </p>

      <div class="grid grid-cols-2 gap-4">
        <!-- Nombre de la compañía -->
        <div>
          <label for="nombre" class="block text-sm font-medium text-gray-700">
            Nombre de la compañía
          </label>
          <input
            type="text"
            id="nombre"
            v-model="empresa.nombre"
            placeholder=""
            class="w-full px-4 py-2 rounded border border-gray-300 focus:outline-none focus:ring-2 focus:ring-blue-300"
          />
        </div>

        <!-- Teléfono -->
        <div>
          <label for="telefono" class="block text-sm font-medium text-gray-700">
            Teléfono
          </label>
          <input
            type="tel"
            id="telefono"
            v-model="empresa.telefono"
            placeholder="XXXX-XXXX"
            class="w-full px-4 py-2 rounded border border-gray-300 focus:outline-none focus:ring-2 focus:ring-blue-300"
          />
        </div>

        <!-- Cédula jurídica -->
        <div>
          <label for="cedula" class="block text-sm font-medium text-gray-700">
            Cédula jurídica
          </label>
          <input
            type="text"
            id="cedula"
            v-model="empresa.cedulaJuridica"
            placeholder="X-XXX-XXXXXXX"
            class="w-full px-4 py-2 rounded border border-gray-300 focus:outline-none focus:ring-2 focus:ring-blue-300"
          />
        </div>

        <!-- Correo -->
        <div>
          <label for="correo" class="block text-sm font-medium text-gray-700">
            Correo
          </label>
          <input
            type="email"
            id="correo"
            v-model="empresa.correo"
            placeholder="XXX@XXXX.XXX"
            class="w-full px-4 py-2 rounded border border-gray-300 focus:outline-none focus:ring-2 focus:ring-blue-300"
          />
        </div>

        <!-- Descripción -->
        <div class="col-span-2">
          <label
            for="descripcion"
            class="block text-sm font-medium text-gray-700"
          >
            Descripción
          </label>
          <textarea
            id="descripcion"
            v-model="empresa.descripcion"
            placeholder=""
            rows="3"
            class="w-full px-4 py-2 rounded border border-gray-300 focus:outline-none focus:ring-2 focus:ring-blue-300"
          ></textarea>
        </div>

        <!-- Dirección -->
        <div>
          <label for="provincia" class="block text-sm font-medium text-gray-700">
            Provincia
          </label>
          <input
            type="text"
            id="provincia"
            v-model="empresa.direccion.provincia"
            placeholder=""
            class="w-full px-4 py-2 rounded border border-gray-300 focus:outline-none focus:ring-2 focus:ring-blue-300"
          />
        </div>

        <div>
          <label for="canton" class="block text-sm font-medium text-gray-700">
            Cantón
          </label>
          <input
            type="text"
            id="canton"
            v-model="empresa.direccion.canton"
            placeholder=""
            class="w-full px-4 py-2 rounded border border-gray-300 focus:outline-none focus:ring-2 focus:ring-blue-300"
          />
        </div>

        <div>
          <label for="distrito" class="block text-sm font-medium text-gray-700">
            Distrito
          </label>
          <input
            type="text"
            id="distrito"
            v-model="empresa.direccion.distrito"
            placeholder=""
            class="w-full px-4 py-2 rounded border border-gray-300 focus:outline-none focus:ring-2 focus:ring-blue-300"
          />
        </div>

        <div>
          <label for="senas" class="block text-sm font-medium text-gray-700">
            Señas
          </label>
          <input
            type="text"
            id="senas"
            v-model="empresa.direccion.senas"
            placeholder=""
            class="w-full px-4 py-2 rounded border border-gray-300 focus:outline-none focus:ring-2 focus:ring-blue-300"
          />
        </div>
      </div>

      <button
        type="submit"
        class="w-full bg-blue-600 hover:bg-blue-700 text-white font-semibold py-2 px-4 rounded transition-colors"
      >
        Registrar
      </button>
    </form>
  </div>
</template>

<script>
import axios from "axios";

export default {
  data() {
    return {
      empresa: {
        nombre: "",
        telefono: "",
        cedulaJuridica: "",
        correo: "",
        descripcion: "",
        direccion: {
          provincia: "",
          canton: "",
          distrito: "",
          senas: "",
        },
      },
    };
  },
  methods: {
    async registrarEmpresa() {
      // Validar datos antes de enviarlos
      if (
        !this.empresa.nombre ||
        !this.empresa.telefono ||
        !this.empresa.cedulaJuridica ||
        !this.empresa.correo ||
        !this.empresa.descripcion ||
        !this.empresa.direccion.provincia ||
        !this.empresa.direccion.canton ||
        !this.empresa.direccion.distrito ||
        !this.empresa.direccion.senas
      ) {
        alert("Por favor, complete todos los campos.");
        return;
      }

      // Convertir cedulaJuridica a un número entero eliminando caracteres no numéricos
      const cedulaJuridica = parseInt(this.empresa.cedulaJuridica.replace(/[^0-9]/g, ""), 10);

      if (isNaN(cedulaJuridica)) {
        alert("La cédula jurídica debe ser un número válido.");
        return;
      }

      // Aplanar el objeto empresa para que coincida con el modelo del backend
      const empresaPayload = {
        cedulaJuridica: cedulaJuridica,
        nombre: this.empresa.nombre,
        descripcion: this.empresa.descripcion,
        telefono: this.empresa.telefono,
        correo: this.empresa.correo,
        provincia: this.empresa.direccion.provincia,
        canton: this.empresa.direccion.canton,
        distrito: this.empresa.direccion.distrito,
        senas: this.empresa.direccion.senas,
      };

      try {
        // Enviar datos al backend
        const response = await axios.post(
          "https://localhost:7014/api/SetEmpresa/crearEmpresa",
          empresaPayload
        );
        alert("Empresa registrada exitosamente.");
        console.log("Respuesta del servidor:", response.data);
      } catch (error) {
        console.error("Error al registrar la empresa:", error);
        alert("Ocurrió un error al registrar la empresa.");
      }
    },
  },
};
</script>

<style scoped>
/* Puedes agregar estilos personalizados aquí */
</style>