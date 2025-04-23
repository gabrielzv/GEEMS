<template>
  <div class="login">
    <form @submit.prevent="login" novalidate>
      <p class="title">Iniciar sesión</p>
      <div>
        <input
          type="text"
          v-model="nombreUsuario"
          placeholder="Correo o Usuario"
          :class="{ 'input-error': emailError }"
          required
          aria-describedby="emailError"
        />
        <span v-if="emailError" id="emailError" class="error">{{
          emailError
        }}</span>
      </div>

      <div>
        <input
          type="password"
          v-model="contrasena"
          placeholder="Contraseña"
          :class="{ 'input-error': passwordError }"
          required
          aria-describedby="passwordError"
        />
        <span v-if="passwordError" id="passwordError" class="error">{{
          passwordError
        }}</span>
      </div>

      <button type="submit" :disabled="isSubmitting">Entrar</button>

      <p v-if="mensaje" class="error">{{ mensaje }}</p>
      <router-link to="/recuperar">¿Olvidaste tu contraseña?</router-link>
    </form>
  </div>
</template>

<script>
import axios from "axios";
import "../styles/login.css"; // Importar el archivo CSS

export default {
  data() {
    return {
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
      } else if (!this.validateEmail(this.nombreUsuario)) {
        this.emailError = "Formato de correo inválido.";
        isValid = false;
      }

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
      if (!this.validateFields()) {
        return;
      }

      this.isSubmitting = true;
      this.mensaje = "";

      try {
        const res = await axios
          .post("https://localhost:7014/api/Auth/login", {
            nombreUsuario: this.nombreUsuario,
            contrasena: this.contrasena,
          })
          .then(function (response) {
            console.log(response);
            window.location.href = "http://localhost:8080/home";
          })
          .catch(function (error) {
            console.log(error);
            this.mensaje = res.data.message || "Error al iniciar sesión.";
          });
      } catch (err) {
        this.mensaje =
          err.response?.data?.message || "Error al conectar con el servidor.";
      } finally {
        this.isSubmitting = false;
      }
    },
  },
};
</script>
