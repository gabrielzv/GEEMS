<template>
  <div class="min-h-screen flex items-center justify-center bg-gray-100 px-4">
    <div class="flex flex-col items-center space-y-6">
      <img src="../assets/GEEMSLogo.png" class="h-20 w-auto mx-auto" />
      <form
        @submit.prevent="login"
        novalidate
        class="bg-white p-6 rounded-xl shadow-md w-full max-w-sm space-y-5"
      >
        <p class="text-2xl font-bold text-center text-gray-800">
          Iniciar sesión
        </p>

        <div>
          <input
            type="text"
            v-model="nombreUsuario"
            placeholder="Correo o Usuario"
            :class="[
              'w-full px-4 py-2 rounded border focus:outline-none focus:ring-2',
              emailError
                ? 'border-red-500 focus:ring-red-300'
                : 'border-gray-300 focus:ring-blue-300',
            ]"
            required
            aria-describedby="emailError"
          />
          <span v-if="emailError" id="emailError" class="text-red-500 text-sm">
            {{ emailError }}
          </span>
        </div>

        <div class="relative">
          <input
            :type="showPassword ? 'text' : 'password'"
            v-model="contrasena"
            placeholder="Contraseña"
            class="w-full px-3 py-2 border rounded focus:outline-none focus:ring-1 focus:ring-blue-300 pr-14"
            required
          />

          <p v-if="passwordError" class="text-red-500 text-sm mt-1">
            {{ passwordError }}
          </p>
        </div>
        <button
          type="button"
          @mousedown="showPassword = true"
          @mouseup="showPassword = false"
          @mouseleave="showPassword = false"
          class="w-full bg-blue-600 hover:bg-blue-700 text-white font-semibold py-2 px-4 rounded transition-colors disabled:opacity-50"
          :aria-label="
            showPassword ? 'Ocultar contraseña' : 'Mostrar contraseña'
          "
        >
          Ver Contraseña
        </button>
        <button
          type="submit"
          :disabled="isSubmitting"
          class="w-full bg-blue-600 hover:bg-blue-700 text-white font-semibold py-2 px-4 rounded transition-colors disabled:opacity-50"
        >
          Entrar
        </button>

        <p v-if="mensaje" class="text-center text-red-600">{{ mensaje }}</p>

        <router-link
          to="/recuperar"
          class="block text-center text-sm text-blue-600 hover:underline"
        >
          ¿Olvidaste tu contraseña?
        </router-link>

        <!-- Nuevo botón de registro -->
        <router-link
          to="/register"
          class="block text-center text-sm text-blue-600 hover:underline"
        >
          ¿No tiene una cuenta aún?
          <span class="font-semibold">Regístrese</span>
        </router-link>
      </form>
    </div>
  </div>
</template>

<script>
import axios from "axios";
import "../styles/login.css";
import { useUserStore } from "../store/user";
import { API_BASE_URL } from "../config";
export default {
  data() {
    return {
      Id: 0,
      tipo: "",
      cedulaPersona: 0,
      nombreUsuario: "",
      CorreoPersona: "",
      contrasena: "",
      mensaje: "",
      emailError: "",
      passwordError: "",
      isSubmitting: false,
      showPassword: false,
    };
  },
  methods: {
    validateFields() {
      let isValid = true;
      this.emailError = "";
      this.passwordError = "";

      if (!this.nombreUsuario) {
        this.emailError = "El correo o usuario es obligatorio.";
        isValid = false;
      }

      if (!this.contrasena) {
        this.passwordError = "La contraseña es obligatoria.";
        isValid = false;
      }

      return isValid;
    },
    async hashPassword(password) {
      const encoder = new TextEncoder();
      const data = encoder.encode(password);
      const hashBuffer = await crypto.subtle.digest("SHA-256", data);
      const hashArray = Array.from(new Uint8Array(hashBuffer));
      const hashHex = hashArray
        .map((b) => b.toString(16).padStart(2, "0"))
        .join("");
      return hashHex;
    },

    async login() {
      if (!this.validateFields()) return;

      this.isSubmitting = true;
      this.mensaje = "";
      //const hashedPassword = await this.hashPassword(this.contrasena);
      const url = `${API_BASE_URL}Auth/login`;
      try {
        const res = await axios.post(url, {
          Id: this.Id || "00000000-0000-0000-0000-000000000000",
          Username: this.nombreUsuario,
          Contrasena: this.contrasena,
          Tipo: this.tipo || "",
          CedulaPersona: this.cedulaPersona || 0,
          CorreoPersona: this.CorreoPersona || "",
        });
        
        const usuario = res.data.usuario;

        const userStore = useUserStore();
        await userStore.setUsuario({
          id: usuario.id,
          tipo: usuario.tipo,
          cedulaPersona: usuario.cedulaPersona,
          nombreUsuario: usuario.nombreUsuario,
          contrasena: usuario.contrasena,
        });

        this.mensaje = "Inicio de sesión exitoso.";
        this.$router.push("/home");
      } catch (err) {
        console.error(err);
        this.mensaje =
          err.response?.data?.message || "Error al iniciar sesión.";
      } finally {
        this.isSubmitting = false;
      }
    },
  },
};
</script>
