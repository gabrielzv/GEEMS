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
          @click="register"
          :disabled="isSubmitting"
          class="w-full bg-blue-500 text-white py-2 px-4 rounded hover:bg-blue-600 transition disabled:bg-gray-400 disabled:cursor-not-allowed"
        >
          {{ isSubmitting ? 'Registrando...' : 'Registrar' }}
        </button>
        </div>
        <p v-if="mensaje" :class="['text-center text-sm', isSubmitting ? 'text-gray-500' : 'text-red-600']">{{ mensaje }}</p>
      </form>
    </div>
  </div>
</template>

<script>
import { v4 as uuidv4 } from "uuid";
import axios from "axios";
import { API_BASE_URL } from "../config";
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

    validateFields() {
      let valid = true;

      // Validar nombre
      this.nombreError = this.nombre.trim() ? "" : "El nombre es obligatorio.";
      if (this.nombreError) valid = false;

      // Validar primer apellido
      this.apellido1Error = this.apellido1.trim() ? "" : "El primer apellido es obligatorio.";
      if (this.apellido1Error) valid = false;

      // Validar segundo apellido
      this.apellido2Error = this.apellido2.trim() ? "" : "El segundo apellido es obligatorio.";
      if (this.apellido2Error) valid = false;

      // Validar nombre de usuario
      this.usernameError =
        this.nombreUsuario.trim() && this.nombreUsuario.length <= 30
          ? ""
          : "El nombre de usuario es obligatorio y debe tener máximo 30 caracteres.";
      if (this.usernameError) valid = false;

      // Validar correo
      const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
      this.emailError = emailRegex.test(this.correo)
        ? ""
        : "Formato de correo inválido.";
      if (this.emailError) valid = false;

      // Validar cédula
      const cedulaRegex = /^\d{9}$/;
      this.cedulaError = cedulaRegex.test(this.cedula)
        ? ""
        : "Cédula inválida, debe tener 9 dígitos.";
      if (this.cedulaError) valid = false;

      // Validar teléfono
      const telefonoRegex = /^\d{4}-\d{4}$/;
      this.telefonoError = telefonoRegex.test(this.telefono)
        ? ""
        : "Teléfono inválido. El formato debe ser 1234-5678.";
      if (this.telefonoError) valid = false;

      // Validar dirección
      this.direccionError = this.direccion.trim() ? "" : "La dirección es obligatoria.";
      if (this.direccionError) valid = false;

      // Validar contraseña
      this.passwordError = this.contrasena.trim() ? "" : "La contraseña es obligatoria.";
      if (this.passwordError) valid = false;

      // Validar confirmación de contraseña
      this.confirmPasswordError =
        this.contrasena === this.confirmarContrasena
          ? ""
          : "Las contraseñas no coinciden.";
      if (this.confirmPasswordError) valid = false;

      return valid;
    },

    async register() {
      this.isSubmitting = true;
      this.mensaje = ""; // Limpiar mensajes anteriores

      // Validar campos antes de registrar
      if (!this.validateFields()) {
        this.isSubmitting = false; 
        this.mensaje = "Por favor, corrija los errores en el formulario.";
        return;
      }

      try {
        // Generar UUID único
        const uniqueId = uuidv4();

        // Hacer las solicitudes POST
        const urlPersona = `${API_BASE_URL}Register/persona`;
        const responsePersona = await axios.post(urlPersona, {
          cedula: this.cedula,
          direccion: this.direccion,
          telefono: this.telefono,
          nombrePila: this.nombre,
          apellido1: this.apellido1,
          apellido2: this.apellido2,
          correo: this.correo,
        });

        console.log("Respuesta de Persona:", responsePersona.data);
        const urlUsuario = `${API_BASE_URL}Register/usuario`;
        const responseUsuario = await axios.post(urlUsuario, {
          id: uniqueId,
          username: this.nombreUsuario,
          contrasena: this.contrasena,
          tipo: "DuenoEmpresa",
          cedulaPersona: this.cedula,
          correoPersona: this.correo,
        });

        console.log("Respuesta de Usuario:", responseUsuario.data);

        this.$router.push({
          path: "/registroEmpresa",
          query: {
            id: uniqueId,
            cedulaPersona: this.cedula,
          },
        });
      } catch (error) {
        console.error("Error durante el registro:", error);
        this.mensaje = "Ocurrió un error al registrar el usuario.";
      } finally {
        this.isSubmitting = false; // Asegurarse de que siempre se restablezca
      }
    },
  },
};
</script>

<style scoped>
</style>