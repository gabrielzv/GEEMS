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
        <div>
          <label for="nombre" class="block text-sm font-medium text-gray-700">
            Nombre de la compañía
          </label>
          <input
            type="text"
            id="nombre"
            v-model="empresa.nombre"
            maxlength="90"
            placeholder="GEEMS Solutions"
            :class="inputClass(errores.nombreVacio || errores.nombreDuplicado)"
          />
          <p v-if="errores.nombreVacio" class="text-sm text-red-500 mt-1">
            {{ errores.nombreVacio }}
          </p>
          <p v-if="errores.nombreDuplicado" class="text-sm text-red-500 mt-1">
            {{ errores.nombreDuplicado }}
          </p>
        </div>
        <div>
          <label class="block text-sm font-medium text-gray-700">
            Teléfono
          </label>
          <input
            type="text"
            v-model="telefono"
            maxlength="9"
            placeholder="8888-8888"
            :class="
              inputClass(errores.telefonoFormatoInvalido || errores.telefono)
            "
          />
          <p v-if="errores.telefonoVacio" class="text-sm text-red-500 mt-1">
            {{ errores.telefonoVacio }}
          </p>
          <p
            v-if="errores.telefonoFormatoInvalido && !errores.telefonoVacio"
            class="text-sm text-red-500 mt-1"
          >
            {{ errores.telefonoFormatoInvalido }}
          </p>
        </div>
        <div>
          <label class="block text-sm font-medium text-gray-700">
            Cédula jurídica
          </label>
          <input
            type="text"
            v-model="cedula"
            maxlength="12"
            placeholder="1-333-666666"
            :class="
              inputClass(
                errores.cedulaFormatoInvalido ||
                  errores.cedulaVacio ||
                  errores.cedulaDuplicada
              )
            "
          />
          <p v-if="errores.cedulaVacio" class="text-sm text-red-500 mt-1">
            {{ errores.cedulaVacio }}
          </p>
          <p
            v-if="errores.cedulaFormatoInvalido && !errores.cedulaVacio"
            class="text-sm text-red-500 mt-1"
          >
            {{ errores.cedulaFormatoInvalido }}
          </p>
          <p v-if="errores.cedulaDuplicada" class="text-sm text-red-500 mt-1">
            {{ errores.cedulaDuplicada }}
          </p>
        </div>
        <div>
          <label for="correo" class="block text-sm font-medium text-gray-700">
            Correo
          </label>
          <input
            type="email"
            id="correo"
            :class="
              inputClass(errores.correoVacio || errores.correoFormatoInvalido)
            "
            v-model="empresa.correo"
            placeholder="empresario@empresa.com"
          />
          <p v-if="errores.correoVacio" class="text-sm text-red-500 mt-1">
            {{ errores.correoVacio }}
          </p>
          <p
            v-if="errores.correoFormatoInvalido && !errores.correoVacio"
            class="text-sm text-red-500 mt-1"
          >
            {{ errores.correoFormatoInvalido }}
          </p>
        </div>
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
            placeholder="La empresa GEEMS Solutions se dedica a..."
            rows="3"
            maxlength="297"
            class="w-full px-4 py-2 rounded border border-gray-300 focus:outline-none focus:ring-2 focus:ring-blue-300"
          ></textarea>
        </div>

        <!-- Modalidad de pago -->
        <div class>
          <label class="block text-sm font-medium text-gray-700"
            >Modalidad de pago:</label
          >
          <div class="flex gap-4 text-sm">
            <label class="flex items-center">
              <input
                type="radio"
                id="modalidadPago"
                value="Mensual"
                v-model="empresa.modalidadPago"
                class="mr-2"
                @change="validateFrecuencia"
              />
              Mensual
            </label>
            <label class="flex items-center">
              <input
                type="radio"
                id="modalidadPago"
                value="Quincenal"
                v-model="empresa.modalidadPago"
                class="mr-2"
              />
              Quincenal
            </label>
            <label class="flex items-center">
              <input
                type="radio"
                id="modalidadPago"
                value="Semanal"
                v-model="empresa.modalidadPago"
                class="mr-2"
              />
              Semanal
            </label>
          </div>
          <p
            v-if="errores.modalidadPagoError"
            class="text-sm text-red-500 mt-1"
          >
            {{ errores.modalidadPagoError }}
          </p>
        </div>

        <!-- Máximo de beneficios por empleado -->
        <div>
          <label class="block text-sm font-medium text-gray-700">
            Máximo de beneficios por empleado
          </label>
          <input
            type="text"
            v-model="empresa.maxBeneficiosXEmpleado"
            maxlength="2"
            placeholder="3"
            :class="
              inputClass(
                errores.maxBeneficiosXEmpleadoFormatoInvalido ||
                  errores.maxBeneficiosXEmpleadoVacio ||
                  errores.maxBeneficiosXEmpleadoError
              )
            "
          />
          <p
            v-if="errores.maxBeneficiosXEmpleadoVacio"
            class="text-sm text-red-500 mt-1"
          >
            {{ errores.maxBeneficiosXEmpleadoVacio }}
          </p>
          <p
            v-if="
              errores.maxBeneficiosXEmpleadoFormatoInvalido &&
              !errores.maxBeneficiosXEmpleadoVacio
            "
            class="text-sm text-red-500 mt-1"
          >
            {{ errores.maxBeneficiosXEmpleadoFormatoInvalido }}
          </p>
        </div>

        <div>
          <label
            for="provincia"
            class="block text-sm font-medium text-gray-700"
          >
            Provincia
          </label>
          <input
            type="text"
            id="provincia"
            maxlength="30"
            v-model="empresa.direccion.provincia"
            placeholder="San José"
            :class="inputClass(errores.provinciaVacio)"
          />
          <p v-if="errores.provinciaVacio" class="text-sm text-red-500 mt-1">
            {{ errores.provinciaVacio }}
          </p>
        </div>

        <div>
          <label for="canton" class="block text-sm font-medium text-gray-700">
            Cantón
          </label>
          <input
            type="text"
            id="canton"
            maxlength="30"
            v-model="empresa.direccion.canton"
            placeholder="Montes de Oca"
            :class="inputClass(errores.cantonVacio)"
          />
          <p v-if="errores.cantonVacio" class="text-sm text-red-500 mt-1">
            {{ errores.cantonVacio }}
          </p>
        </div>

        <div>
          <label for="distrito" class="block text-sm font-medium text-gray-700">
            Distrito
          </label>
          <input
            type="text"
            id="distrito"
            maxlength="30"
            v-model="empresa.direccion.distrito"
            placeholder="San Pedro"
            :class="inputClass(errores.distritoVacio)"
          />
          <p v-if="errores.distritoVacio" class="text-sm text-red-500 mt-1">
            {{ errores.distritoVacio }}
          </p>
        </div>

        <div>
          <label for="senas" class="block text-sm font-medium text-gray-700">
            Señas
          </label>
          <input
            type="text"
            id="senas"
            maxlength="30"
            v-model="empresa.direccion.senas"
            placeholder="Frente a Universidad de Costa Rica"
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
        modalidadPago: "",
        maxBeneficiosXEmpleado: "",
      },
      cedula: "",
      telefono: "",
      idUsuario: null,
      cedulaPersona: null,
      errores: {
        nombreVacio: "",
        telefonoVacio: "",
        cedulaVacio: "",
        correoVacio: "",
        provinciaVacio: "",
        cantonVacio: "",
        distritoVacio: "",
        nombreDuplicado: "",
        cedulaDuplicada: "",
        telefonoFormatoInvalido: "",
        cedulaFormatoInvalido: "",
        correoFormatoInvalido: "",
        modalidadPagoError: "",
        maxBeneficiosXEmpleadoError: "",
        maxBeneficiosXEmpleadoVacio: "",
        maxBeneficiosXEmpleadoFormatoInvalido: "",
      },
    };
  },
  created() {
    const { id, cedulaPersona } = this.$route.query;

    if (!id || !cedulaPersona) {
      alert("Faltan datos necesarios para registrar al dueño.");
      return;
    }

    this.idUsuario = id;
    this.cedulaPersona = cedulaPersona;
  },
  methods: {
    async validarDuplicados(nombre, cedulaJuridica) {
      try {
        this.errores.nombreDuplicado = "";
        this.errores.cedulaDuplicada = "";

        const response = await axios.get("https://localhost:7014/api/Empresa");
        const empresas = response.data;

        const nombreDuplicado = empresas.some(
          (empresa) => empresa.nombre.toLowerCase() === nombre.toLowerCase()
        );

        const cedulaDuplicada = empresas.some(
          (empresa) => empresa.cedulaJuridica === cedulaJuridica
        );

        if (nombreDuplicado) {
          this.errores.nombreDuplicado =
            "El nombre de la empresa ya está registrado.";
        }

        if (cedulaDuplicada) {
          this.errores.cedulaDuplicada =
            "La cédula jurídica ya está registrada.";
        }
      } catch (error) {
        alert("Error al validar duplicados.");
      }
    },

    inputClass(error) {
      return [
        "w-full px-4 py-2 rounded border focus:outline-none focus:ring-2",
        error
          ? "border-red-500 focus:ring-red-300"
          : "border-gray-300 focus:ring-blue-300",
      ];
    },

    async registrarEmpresa() {
      this.errores.nombreVacio = this.empresa.nombre.trim()
        ? ""
        : "El nombre es obligatorio.";
      this.errores.telefonoVacio = this.telefono.trim()
        ? ""
        : "El teléfono es obligatorio.";
      this.errores.cedulaVacio = this.cedula.trim()
        ? ""
        : "La cédula es obligatoria.";
      this.errores.correoVacio = this.empresa.correo.trim()
        ? ""
        : "El correo es obligatorio.";
      this.errores.provinciaVacio = this.empresa.direccion.provincia.trim()
        ? ""
        : "La provincia es obligatoria.";
      this.errores.cantonVacio = this.empresa.direccion.canton.trim()
        ? ""
        : "El cantón es obligatorio.";
      this.errores.distritoVacio = this.empresa.direccion.distrito.trim()
        ? ""
        : "El distrito es obligatorio.";
      this.errores.modalidadPagoError = this.empresa.modalidadPago
        ? ""
        : "La modalidad de pago es obligatoria.";



      this.errores.cedulaFormatoInvalido = "";
      try {
        // Elimina los guiones de la cédula antes de enviarla al API
        const cedulaSinGuiones = this.cedula.replace(/-/g, "");
        const response = await axios.post(
          "https://localhost:7014/api/national-register/validate/" + cedulaSinGuiones
        );
        const apiResult = response.data;
        if (apiResult.error || apiResult.formatoInvalido) {
          this.errores.cedulaFormatoInvalido =
            apiResult.error || apiResult.formatoInvalido || "Cédula jurídica inválida.";
        }
      } catch (error) {
        if (error.response && error.response.data) {
          this.errores.cedulaFormatoInvalido =
            error.response.data.message || "Error validando la cédula jurídica.";
        } else {
          this.errores.cedulaFormatoInvalido = "Error validando la cédula jurídica.";
        }
      }

      this.errores.telefonoFormatoInvalido = "";
      if (!/^\d{4}-\d{4}$/.test(this.telefono)) {
        this.errores.telefonoFormatoInvalido =
          "El teléfono debe tener el formato XXXX-XXXX y contener solo números.";
      }

      this.errores.correoFormatoInvalido = "";
      if (!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(this.empresa.correo)) {
        this.errores.correoFormatoInvalido =
          "Por favor, ingrese un correo electrónico válido como ejemplo@correo.com";
      }

      this.errores.maxBeneficiosXEmpleadoVacio =
        this.empresa.maxBeneficiosXEmpleado.trim()
          ? ""
          : "El máximo de beneficios por empleado es obligatorio.";

      this.errores.maxBeneficiosXEmpleadoFormatoInvalido = "";
      if (this.empresa.maxBeneficiosXEmpleado.trim()) {
        const num = Number(this.empresa.maxBeneficiosXEmpleado);
        if (
          isNaN(num) ||
          !Number.isInteger(num) ||
          num < 0 ||
          num > 99 ||
          this.empresa.maxBeneficiosXEmpleado.length > 2
        ) {
          this.errores.maxBeneficiosXEmpleadoFormatoInvalido =
            "Debe ser un número de hasta 2 dígitos (0 a 99).";
        }
      }

      await this.validarDuplicados(this.empresa.nombre, this.cedula);

      let not_valid =
        this.errores.nombreVacio ||
        this.errores.telefonoVacio ||
        this.errores.cedulaVacio ||
        this.errores.correoVacio ||
        this.errores.provinciaVacio ||
        this.errores.cantonVacio ||
        this.errores.distritoVacio ||
        this.errores.nombreDuplicado ||
        this.errores.cedulaDuplicada ||
        this.errores.telefonoFormatoInvalido ||
        this.errores.cedulaFormatoInvalido ||
        this.errores.correoFormatoInvalido ||
        this.errores.modalidadPagoError ||
        this.errores.maxBeneficiosXEmpleadoVacio ||
        this.errores.maxBeneficiosXEmpleadoFormatoInvalido ||
        this.errores.maxBeneficiosXEmpleadoError;

      if (not_valid) {
        return;
      }

      const empresaPayload = {
        cedulaJuridica: this.cedula,
        nombre: this.empresa.nombre,
        descripcion: this.empresa.descripcion,
        telefono: this.telefono,
        correo: this.empresa.correo,
        provincia: this.empresa.direccion.provincia,
        canton: this.empresa.direccion.canton,
        distrito: this.empresa.direccion.distrito,
        senas: this.empresa.direccion.senas,
        modalidadPago: this.empresa.modalidadPago,
        maxBeneficiosXEmpleado: this.empresa.maxBeneficiosXEmpleado,
      };

      try {
        await axios.post(
          "https://localhost:7014/api/SetEmpresa/crearEmpresa",
          empresaPayload
        );
        try {
          await axios.post("https://localhost:7014/api/Register/duenoempresa", {
            id: this.idUsuario,
            cedulaPersona: this.cedulaPersona,
            cedulaEmpresa: this.cedula,
          });
          alert("Dueño de empresa y empresa registrados exitosamente.");

          this.$router.push("/login");
        } catch (error) {
          alert("Ocurrió un error al registrar al dueño de la empresa.");
        }
      } catch (error) {
        alert("Ocurrió un error al registrar la empresa.");
      }
    },
  },
};
</script>

<style scoped>
/* Quita el resize de la descripción */
textarea {
  resize: none;
}
</style>
