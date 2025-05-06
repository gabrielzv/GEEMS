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
            maxlength="90"
            placeholder=""
            class="w-full px-4 py-2 rounded border border-gray-300 focus:outline-none focus:ring-2 focus:ring-blue-300"
          />
        </div>

        <!-- Teléfono -->
        <div>
          <label class="block text-sm font-medium text-gray-700">
            Teléfono
          </label>
          <div class="flex space-x-2">
            <input
              type="text"
              v-model="telefonoParte1"
              maxlength="4"
              placeholder=""
              class="w-1/2 px-4 py-2 rounded border border-gray-300 focus:outline-none focus:ring-2 focus:ring-blue-300"
            />
            <input
              type="text"
              v-model="telefonoParte2"
              maxlength="4"
              placeholder=""
              class="w-1/2 px-4 py-2 rounded border border-gray-300 focus:outline-none focus:ring-2 focus:ring-blue-300"
            />
          </div>
        </div>

        <!-- Cédula jurídica -->
        <div>
          <label class="block text-sm font-medium text-gray-700">
            Cédula jurídica
          </label>
          <div class="flex space-x-2">
            <input
              type="text"
              v-model="cedulaParte1"
              maxlength="1"
              placeholder=""
              class="w-1/4 px-4 py-2 rounded border border-gray-300 focus:outline-none focus:ring-2 focus:ring-blue-300"
            />
            <input
              type="text"
              v-model="cedulaParte2"
              maxlength="3"
              placeholder=""
              class="w-1/4 px-4 py-2 rounded border border-gray-300 focus:outline-none focus:ring-2 focus:ring-blue-300"
            />
            <input
              type="text"
              v-model="cedulaParte3"
              maxlength="7"
              placeholder=""
              class="w-1/2 px-4 py-2 rounded border border-gray-300 focus:outline-none focus:ring-2 focus:ring-blue-300"
            />
          </div>
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
            placeholder=""
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
        correo: "",
        descripcion: "",
        direccion: {
          provincia: "",
          canton: "",
          distrito: "",
          senas: "",
        },
      },
      cedulaParte1: "",
      cedulaParte2: "",
      cedulaParte3: "",
      telefonoParte1: "",
      telefonoParte2: "",
      idUsuario: null, // NUEVO
      cedulaPersona: null, // NUEVO
    };
  },
  created() {
  const { id, cedulaPersona } = this.$route.query;

  if (!id || !cedulaPersona) {
    alert("Faltan datos necesarios para registrar al dueno.");
    return;
  }

  this.idUsuario = id;
  this.cedulaPersona = cedulaPersona;
},
  methods: {
    async registrarEmpresa() {
      if (
        !this.empresa.nombre ||
        !this.telefonoParte1 ||
        !this.telefonoParte2 ||
        !this.cedulaParte1 ||
        !this.cedulaParte2 ||
        !this.cedulaParte3 ||
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

      const cedulaJuridica = `${this.cedulaParte1}-${this.cedulaParte2}-${this.cedulaParte3}`;
      const telefono = `${this.telefonoParte1}${this.telefonoParte2}`;

      if (!/^\d{1}-\d{3}-\d{7}$/.test(cedulaJuridica)) {
        alert("La cédula jurídica debe tener el formato X-XXX-XXXXXXX.");
        console.error("Error: La cédula jurídica no tiene el formato correcto:", cedulaJuridica);
        return;
      }

      if (telefono.length !== 8 || isNaN(parseInt(telefono, 10))) {
        alert("El teléfono debe tener exactamente 8 dígitos.");
        console.error("Error: El teléfono no es válido:", telefono);
        return;
      }

      const empresaPayload = {
        cedulaJuridica: cedulaJuridica,
        nombre: this.empresa.nombre,
        descripcion: this.empresa.descripcion,
        telefono: telefono,
        correo: this.empresa.correo,
        provincia: this.empresa.direccion.provincia,
        canton: this.empresa.direccion.canton,
        distrito: this.empresa.direccion.distrito,
        senas: this.empresa.direccion.senas,
      };

      console.log("Datos enviados al backend:", empresaPayload);
      // imprimir los datos que llegaron por query
      console.log("Datos del dueno:", {
        id: this.idUsuario,
        cedulaPersona: this.cedulaPersona,
        cedulaEmpresa: cedulaJuridica,
      });

      try {
        const response = await axios.post(
          "https://localhost:7014/api/SetEmpresa/crearEmpresa",
          empresaPayload
        );
        alert("Empresa registrada exitosamente.");
        console.log("Respuesta del servidor:", response.data);

        try {
          const responseDuenoEmpresa = await axios.post("https://localhost:7014/api/Register/duenoempresa", {
            id: this.idUsuario,
            cedulaPersona: this.cedulaPersona,
            cedulaEmpresa: cedulaJuridica,
          });
          console.log("Respuesta de dueño de empresa:", responseDuenoEmpresa.data);
          alert("Dueño de empresa registrado exitosamente.");

          this.$router.push("/login");
        } catch (error) {
          console.error("Error al registrar al dueño de la empresa:", error.response?.data || error.message);
          alert("Ocurrió un error al registrar al dueño de la empresa.");
        }
      } catch (error) {
        console.error("Error al registrar la empresa:", error.response?.data || error.message);
        alert("Ocurrió un error al registrar la empresa.");
      }
    },
  },
};
</script>

<style scoped>
/* Puedes agregar estilos personalizados aquí */
</style>
