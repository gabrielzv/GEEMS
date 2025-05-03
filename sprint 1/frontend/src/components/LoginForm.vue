<template>
  <div class="min-h-screen flex items-center justify-center bg-gray-100 px-4">
    <form
      @submit.prevent="login"
      novalidate
      class="bg-white p-6 rounded-xl shadow-md w-full max-w-sm space-y-5"
    >
      <p class="text-2xl font-bold text-center text-gray-800">Iniciar sesión</p>

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
        <span v-if="emailError" id="emailError" class="text-red-500 text-sm">{{
          emailError
        }}</span>
      </div>

      <div>
        <input
          type="password"
          v-model="contrasena"
          placeholder="Contraseña"
          :class="[
            'w-full px-4 py-2 rounded border focus:outline-none focus:ring-2',
            passwordError
              ? 'border-red-500 focus:ring-red-300'
              : 'border-gray-300 focus:ring-blue-300',
          ]"
          required
          aria-describedby="passwordError"
        />
        <span
          v-if="passwordError"
          id="passwordError"
          class="text-red-500 text-sm"
        >
          {{ passwordError }}
        </span>
      </div>

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
    </form>
  </div>
</template>

<script>
import axios from "axios";
//import { mapActions } from "vuex";
//import { useStore } from "vuex";
import "../styles/login.css"; // Importar el archivo CSS
import { useUserStore } from "../store/user";

//const store = useStore();
export default {
  data() {
    return {
      Id: 0,
      tipo: "",
      cedulaPersona: 0,
      nombreUsuario: "",
      contrasena: "",
      mensaje: "",
      emailError: "",
      passwordError: "",
      isSubmitting: false,
    };
  },
  methods: {
    validateFields() {
      let isValid = true;
      this.emailError = "";
      this.passwordError = "";

      // Validación del correo
      if (!this.nombreUsuario) {
        this.emailError = "El correo o usuario es obligatorio.";
        isValid = false;
      } /*else if (!this.validateEmail(this.nombreUsuario)) {
        this.emailError = "Formato de correo inválido.";
        isValid = false;
      }*/

      // Validación de la contraseña
      if (!this.contrasena) {
        this.passwordError = "La contraseña es obligatoria.";
        isValid = false;
      }

      return isValid;
    },

    validateEmail(email) {
      const regex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
      return regex.test(email);
    },

    async login() {
      if (!this.validateFields()) return;

      this.isSubmitting = true;
      this.mensaje = "";

      try {
        const res = await axios.post("https://localhost:7014/api/Auth/login", {
          Id: this.Id || "00000000-0000-0000-0000-000000000000", // Asignar un Guid vacío
          Username: this.nombreUsuario,
          Contrasena: this.contrasena,
          Tipo: this.tipo || "", // Valor vacío por defecto si no tienes "Tipo"
          CedulaPersona: this.cedulaPersona || 0, // Valor por defecto si no tienes "CedulaPersona"
        });

        console.log(res.data);
        const user = await axios.get(
          `https://localhost:7014/api/GetUser/getUser/${this.nombreUsuario}`
        );
        console.log(user.data);

        const userStore = useUserStore();
        await userStore.setUsuario({
          id: user.data.id,
          tipo: user.data.tipo,
          cedulaPersona: user.data.cedulaPersona,
          nombreUsuario: this.nombreUsuario,
          contrasena: user.data.contrasena,
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
