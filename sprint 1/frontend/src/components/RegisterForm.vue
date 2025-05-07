<template>
  <div class="flex justify-center items-center min-h-screen bg-gray-100">
    <div class="w-full max-w-lg bg-white p-8 rounded-2xl shadow-lg">
      <h2 class="text-3xl font-bold text-center text-gray-800 mb-6">Formulario de Registro</h2>
      <form @submit.prevent="register" class="space-y-5">
        <div>
          <label for="nombre" class="block text-sm font-medium text-gray-700 mb-1">Nombre</label>
          <input
            type="text"
            id="nombre"
            v-model="nombre"
            :class="inputClass(nombreError)"
            placeholder="Nombre"
          />
          <p v-if="nombreError" class="text-sm text-red-500 mt-1">{{ nombreError }}</p>
        </div>

        <div>
          <label for="apellido1" class="block text-sm font-medium text-gray-700 mb-1">Primer Apellido</label>
          <input
            type="text"
            id="apellido1"
            v-model="apellido1"
            :class="inputClass(apellido1Error)"
            placeholder="Primer Apellido"
          />
          <p v-if="apellido1Error" class="text-sm text-red-500 mt-1">{{ apellido1Error }}</p>
        </div>

        <div>
          <label for="apellido2" class="block text-sm font-medium text-gray-700 mb-1">Segundo Apellido</label>
          <input
            type="text"
            id="apellido2"
            v-model="apellido2"
            :class="inputClass(apellido2Error)"
            placeholder="Segundo Apellido"
          />
          <p v-if="apellido2Error" class="text-sm text-red-500 mt-1">{{ apellido2Error }}</p>
        </div>

        <div>
          <label for="nombreUsuario" class="block text-sm font-medium text-gray-700 mb-1">Nombre de Usuario</label>
          <input
            type="text"
            id="nombreUsuario"
            v-model="nombreUsuario"
            :class="inputClass(usernameError)"
            placeholder="Nombre de Usuario"
            @blur="checkUsername"
          />
          <p v-if="usernameError" class="text-sm text-red-500 mt-1">{{ usernameError }}</p>
        </div>

        <div>
          <label for="correo" class="block text-sm font-medium text-gray-700 mb-1">Correo Electrónico</label>
          <input
            type="email"
            id="correo"
            v-model="correo"
            :class="inputClass(emailError)"
            placeholder="Correo Electrónico"
            @blur="checkCorreo"
          />
          <p v-if="emailError" class="text-sm text-red-500 mt-1">{{ emailError }}</p>
        </div>

        <div>
          <label for="cedula" class="block text-sm font-medium text-gray-700 mb-1">Cédula</label>
          <input
            type="text"
            id="cedula"
            v-model="cedula"
            :class="inputClass(cedulaError)"
            placeholder="Cédula"
            @blur="checkCedula"
          />
          <p v-if="cedulaError" class="text-sm text-red-500 mt-1">{{ cedulaError }}</p>
        </div>

        <div>
          <label for="telefono" class="block text-sm font-medium text-gray-700 mb-1">Teléfono</label>
          <input
            type="text"
            id="telefono"
            v-model="telefono"
            :class="inputClass(telefonoError)"
            placeholder="Teléfono"
          />
          <p v-if="telefonoError" class="text-sm text-red-500 mt-1">{{ telefonoError }}</p>
        </div>

        <div>
          <label for="direccion" class="block text-sm font-medium text-gray-700 mb-1">Dirección</label>
          <input
            type="text"
            id="direccion"
            v-model="direccion"
            :class="inputClass(direccionError)"
            placeholder="Dirección"
          />
          <p v-if="direccionError" class="text-sm text-red-500 mt-1">{{ direccionError }}</p>
        </div>

        <div>
          <label for="contrasena" class="block text-sm font-medium text-gray-700 mb-1">Contraseña</label>
          <input
            type="password"
            id="contrasena"
            v-model="contrasena"
            :class="inputClass(passwordError)"
            placeholder="Contraseña"
          />
          <p v-if="passwordError" class="text-sm text-red-500 mt-1">{{ passwordError }}</p>
        </div>

        <div>
          <label for="confirmarContrasena" class="block text-sm font-medium text-gray-700 mb-1">Confirmar Contraseña</label>
          <input
            type="password"
            id="confirmarContrasena"
            v-model="confirmarContrasena"
            :class="inputClass(confirmPasswordError)"
            placeholder="Confirmar Contraseña"
          />
          <p v-if="confirmPasswordError" class="text-sm text-red-500 mt-1">{{ confirmPasswordError }}</p>
        </div>

        <div>
          <button
            :disabled="isSubmitting || hasErrors"
            class="w-full bg-blue-600 hover:bg-blue-700 transition-colors duration-200 text-white font-semibold py-2 px-4 rounded-lg disabled:opacity-50"
          >
            {{ isSubmitting ? "Registrando..." : "Registrar" }}
          </button>
        </div>
        <p v-if="mensaje" :class="['text-center text-sm', isSubmitting ? 'text-gray-500' : 'text-green-600']">{{ mensaje }}</p>
      </form>
    </div>
  </div>
</template>

<script>
import { v4 as uuidv4 } from "uuid";
import axios from "axios";

export default {
  data() {
    return {
      nombre: "",
      apellido1: "",
      apellido2: "",
      nombreUsuario: "",
      correo: "",
      cedula: null,
      telefono: "",
      direccion: "",
      contrasena: "",
      confirmarContrasena: "",
      mensaje: "",

      // Errores
      nombreError: "",
      apellido1Error: "",
      apellido2Error: "",
      usernameError: "",
      emailError: "",
      cedulaError: "",
      telefonoError: "",
      direccionError: "",
      passwordError: "",
      confirmPasswordError: "",

      isSubmitting: false,
    };
  },
  computed: {
    hasErrors() {
      return (
        !!this.usernameError ||
        !!this.emailError ||
        !!this.cedulaError ||
        !!this.nombreError ||
        !!this.apellido1Error ||
        !!this.apellido2Error ||
        !!this.telefonoError ||
        !!this.direccionError ||
        !!this.passwordError ||
        !!this.confirmPasswordError
      );
    },
  },
  methods: {
    inputClass(error) {
      return [
        "w-full px-4 py-2 rounded border focus:outline-none focus:ring-2",
        error ? "border-red-500 focus:ring-red-300" : "border-gray-300 focus:ring-blue-300",
      ];
    },

    async checkUsername() {
      if (!this.nombreUsuario) return;
      try {
        const response = await axios.get(`https://localhost:7014/api/CheckDupe/username/${this.nombreUsuario}`);
        if (response.data) {
          this.usernameError = "El nombre de usuario ya está en uso.";
        } else {
          this.usernameError = "";
        }
      } catch (error) {
        console.error("Error al verificar el nombre de usuario:", error);
      }
    },

    async checkCorreo() {
      if (!this.correo) return;
      try {
        const response = await axios.get(`https://localhost:7014/api/CheckDupe/correo/${this.correo}`);
        if (response.data) {
          this.emailError = "El correo electrónico ya está en uso.";
        } else {
          this.emailError = "";
        }
      } catch (error) {
        console.error("Error al verificar el correo electrónico:", error);
      }
    },

    async checkCedula() {
      if (!this.cedula) return;
      try {
        const response = await axios.get(`https://localhost:7014/api/CheckDupe/cedula/${this.cedula}`);
        if (response.data) {
          this.cedulaError = "La cédula ya está en uso.";
        } else {
          this.cedulaError = "";
        }
      } catch (error) {
        console.error("Error al verificar la cédula:", error);
      }
    },

    validateFields() {
      let isValid = true;

      // Validar campos requeridos
      if (!this.nombre) {
        this.nombreError = "El nombre es obligatorio.";
        isValid = false;
      } else {
        this.nombreError = "";
      }

      if (!this.apellido1) {
        this.apellido1Error = "El primer apellido es obligatorio.";
        isValid = false;
      } else {
        this.apellido1Error = "";
      }

      if (!this.apellido2) {
        this.apellido2Error = "El segundo apellido es obligatorio.";
        isValid = false;
      } else {
        this.apellido2Error = "";
      }

      if (!this.nombreUsuario) {
        this.usernameError = "El nombre de usuario es obligatorio.";
        isValid = false;
      }

      if (!this.correo) {
        this.emailError = "El correo electrónico es obligatorio.";
        isValid = false;
      }

      if (!this.cedula) {
        this.cedulaError = "La cédula es obligatoria.";
        isValid = false;
      }

      if (!this.telefono) {
        this.telefonoError = "El teléfono es obligatorio.";
        isValid = false;
      }

      if (!this.direccion) {
        this.direccionError = "La dirección es obligatoria.";
        isValid = false;
      }

      if (!this.contrasena) {
        this.passwordError = "La contraseña es obligatoria.";
        isValid = false;
      }

      if (this.contrasena !== this.confirmarContrasena) {
        this.confirmPasswordError = "Las contraseñas no coinciden.";
        isValid = false;
      } else {
        this.confirmPasswordError = "";
      }

      return isValid;
    },

    async register() {
      this.isSubmitting = true;

      // Validar campos antes de registrar
      if (!this.validateFields()) {
        this.isSubmitting = false;
        return;
      }

      // Generar UUID único
      const uniqueId = uuidv4();

      try {
        // Hacer las solicitudes POST
        const responsePersona = await axios.post("https://localhost:7014/api/Register/persona", {
          cedula: this.cedula,
          direccion: this.direccion,
          telefono: this.telefono,
          nombrePila: this.nombre,
          apellido1: this.apellido1,
          apellido2: this.apellido2,
          correo: this.correo,
        });

        console.log("Respuesta de Persona:", responsePersona.data);

        const responseUsuario = await axios.post("https://localhost:7014/api/Register/usuario", {
          id: uniqueId,
          username: this.nombreUsuario,
          contrasena: this.contrasena,
          tipo: "DuenoEmpresa",
          cedulaPersona: this.cedula,
          correoPersona: this.correo,
        });

        console.log("Respuesta de Usuario:", responseUsuario.data);

        this.mensaje = "Registro exitoso.";
        this.$router.push({
          path: "/registroEmpresa",
          query: {
            id: uniqueId,
            cedulaPersona: this.cedula,
          },
        });
      } catch (error) {
        console.error("Error durante el registro:", error);
        this.mensaje = "Error al registrarse.";
      } finally {
        this.isSubmitting = false;
      }
    },
  },
};
</script>

<style scoped>
</style>