<template>
  <div class="container mx-auto mt-8 p-4">
    <h2 class="text-2xl font-semibold mb-4">Formulario de Registro</h2>
    <form @submit.prevent="register">
      <div class="mb-4">
        <label for="nombreCompleto" class="block text-sm font-medium text-gray-700">Nombre Completo</label>
        <input
          type="text"
          id="nombreCompleto"
          v-model="nombreCompleto"
          :class="inputClass(nombreCompletoError)"
          placeholder="Nombre Completo"
        />
        <p v-if="nombreCompletoError" class="text-sm text-red-500">{{ nombreCompletoError }}</p>
      </div>

      <div class="mb-4">
        <label for="nombreUsuario" class="block text-sm font-medium text-gray-700">Nombre de Usuario</label>
        <input
          type="text"
          id="nombreUsuario"
          v-model="nombreUsuario"
          :class="inputClass(usernameError)"
          placeholder="Nombre de Usuario"
        />
        <p v-if="usernameError" class="text-sm text-red-500">{{ usernameError }}</p>
      </div>

      <div class="mb-4">
        <label for="correo" class="block text-sm font-medium text-gray-700">Correo Electrónico</label>
        <input
          type="email"
          id="correo"
          v-model="correo"
          :class="inputClass(emailError)"
          placeholder="Correo Electrónico"
        />
        <p v-if="emailError" class="text-sm text-red-500">{{ emailError }}</p>
      </div>

      <div class="mb-4">
        <label for="cedula" class="block text-sm font-medium text-gray-700">Cédula</label>
        <input
          type="text"
          id="cedula"
          v-model="cedula"
          :class="inputClass(cedulaError)"
          placeholder="Cédula"
        />
        <p v-if="cedulaError" class="text-sm text-red-500">{{ cedulaError }}</p>
      </div>

      <div class="mb-4">
        <label for="telefono" class="block text-sm font-medium text-gray-700">Teléfono</label>
        <input
          type="text"
          id="telefono"
          v-model="telefono"
          :class="inputClass(telefonoError)"
          placeholder="Teléfono"
        />
        <p v-if="telefonoError" class="text-sm text-red-500">{{ telefonoError }}</p>
      </div>

      <div class="mb-4">
        <label for="direccion" class="block text-sm font-medium text-gray-700">Dirección</label>
        <input
          type="text"
          id="direccion"
          v-model="direccion"
          :class="inputClass(direccionError)"
          placeholder="Dirección"
        />
        <p v-if="direccionError" class="text-sm text-red-500">{{ direccionError }}</p>
      </div>

      <div class="mb-4">
        <label for="contrasena" class="block text-sm font-medium text-gray-700">Contraseña</label>
        <input
          type="password"
          id="contrasena"
          v-model="contrasena"
          :class="inputClass(passwordError)"
          placeholder="Contraseña"
        />
        <p v-if="passwordError" class="text-sm text-red-500">{{ passwordError }}</p>
      </div>

      <div class="mb-4">
        <label for="confirmarContrasena" class="block text-sm font-medium text-gray-700">Confirmar Contraseña</label>
        <input
          type="password"
          id="confirmarContrasena"
          v-model="confirmarContrasena"
          :class="inputClass(confirmPasswordError)"
          placeholder="Confirmar Contraseña"
        />
        <p v-if="confirmPasswordError" class="text-sm text-red-500">{{ confirmPasswordError }}</p>
      </div>

      <div class="mb-4">
        <button
          :disabled="isSubmitting"
          class="w-full bg-blue-500 text-white py-2 px-4 rounded-lg"
        >
          {{ isSubmitting ? "Registrando..." : "Registrar" }}
        </button>
      </div>
      <p v-if="mensaje" :class="isSubmitting ? '' : 'text-green-500'">{{ mensaje }}</p>
    </form>
  </div>
</template>

<script>
import { v4 as uuidv4 } from "uuid";
import axios from "axios";

export default {
  data() {
    return {
      nombreCompleto: "",
      nombreUsuario: "",
      correo: "",
      cedula: "",
      telefono: "",
      direccion: "",
      contrasena: "",
      confirmarContrasena: "",
      mensaje: "",

      // Errores
      nombreCompletoError: "",
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
  methods: {
    inputClass(error) {
      return [
        "w-full px-4 py-2 rounded border focus:outline-none focus:ring-2",
        error ? "border-red-500 focus:ring-red-300" : "border-gray-300 focus:ring-blue-300",
      ];
    },

    validateEmail(email) {
      const regex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
      return regex.test(email);
    },

    validateFields() {
      let valid = true;

      // Reset errores
      this.nombreCompletoError = "";
      this.usernameError = "";
      this.emailError = "";
      this.cedulaError = "";
      this.telefonoError = "";
      this.direccionError = "";
      this.passwordError = "";
      this.confirmPasswordError = "";

      if (!this.nombreCompleto) {
        this.nombreCompletoError = "Nombre completo es obligatorio.";
        valid = false;
      }

      if (!this.nombreUsuario || this.nombreUsuario.length > 30) {
        this.usernameError = "Nombre de usuario requerido y máximo 30 caracteres.";
        valid = false;
      }

      if (!this.correo || !this.validateEmail(this.correo)) {
        this.emailError = "Correo inválido.";
        valid = false;
      }

      // if (!/^\d{9}$/.test(this.cedula)) {
      //   this.cedulaError = "Cédula inválida.";
      //   valid = false;
      // }

      if (!this.telefono || !/^\d{4}-\d{4}$/.test(this.telefono)) {
        this.telefonoError = "Teléfono inválido.";
        valid = false;
      }

      if (!this.direccion) {
        this.direccionError = "Dirección requerida.";
        valid = false;
      }

      if (!this.contrasena) {
        this.passwordError = "Contraseña requerida.";
        valid = false;
      }

      if (this.contrasena !== this.confirmarContrasena) {
        this.confirmPasswordError = "Las contraseñas no coinciden.";
        valid = false;
      }

      return valid;
    },

    async register() {
      this.isSubmitting = true;

      // Validar campos antes de registrar
      if (!this.validateFields()) {
        this.isSubmitting = false;
        return;
      }

      // Generar UUID único
      const uniqueId = uuidv4(); // Este ID será el mismo para Persona y DuenoEmpresa

      try {
        // Hacer las tres solicitudes POST por separado
        const responsePersona = await axios.post("https://localhost:7014/api/Register/persona", {
          cedula: this.cedula, 
          direccion: this.direccion,
          telefono: this.telefono,
          nombrePila: this.nombreCompleto.split(" ")[0], 
          apellido1: this.nombreCompleto.split(" ")[1], 
          apellido2: this.nombreCompleto.split(" ")[2],
          correo: this.correo,
          id: uniqueId,
        });

        // imprimir respuesta de persona
        console.log("Respuesta de Persona:", responsePersona.data);

        const responseUsuario = await axios.post("https://localhost:7014/api/Register/usuario", {
          id: uniqueId,
          username: this.nombreUsuario,
          contrasena: this.contrasena,
          tipo: "DuenoEmpresa", 
          cedulaPersona: this.cedula,
        });

        // imprimir respuesta de usuario
        console.log("Respuesta de Usuario:", responseUsuario.data);

        // Manejo de la respuesta final
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
