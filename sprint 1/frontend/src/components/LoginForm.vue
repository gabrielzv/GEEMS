<template>
  <div class="login">
    <h2>Iniciar sesión</h2>
    <form @submit.prevent="login">
      <input type="text" v-model="nombreUsuario" placeholder="Usuario" />
      <input type="password" v-model="contrasena" placeholder="Contraseña" />
      <button type="submit">Entrar</button>
      <p>{{ mensaje }}</p>
    </form>
  </div>
</template>

<script>
import axios from "axios";

export default {
  data() {
    return {
      nombreUsuario: "",
      contrasena: "",
      mensaje: "",
    };
  },
  methods: {
    async login() {
      try {
        const res = await axios
          .post("https://localhost:7014/api/Auth/login", {
            nombreUsuario: this.nombreUsuario,
            contrasena: this.contrasena,
          })
          .then(function (response) {
            console.log(response);
            window.location.href = "http://localhost:8080/home";
          });
        this.mensaje = res.data.message;
      } catch (err) {
        this.mensaje = err.response?.data?.message || "Error al iniciar sesión";
      }
    },
  },
};
</script>

<style scoped>
.login {
  max-width: 300px;
  margin: 50px auto;
  text-align: center;
}

input,
button {
  display: block;
  margin: 10px auto;
  padding: 10px;
  width: 90%;
}
</style>
