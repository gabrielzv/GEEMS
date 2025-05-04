import { defineStore } from "pinia";
import axios from "axios";

export const useUserStore = defineStore("user", {
  state: () => ({
    usuario: null,
    empleado: null,
  }),
  actions: {
    setUsuario(usuario) {
      this.usuario = usuario;
      sessionStorage.setItem("usuario", JSON.stringify(usuario));
    },
    async fetchEmpleado(cedulaPersona) {
      try {
        const empleadoRes = await axios.get(
          `https://localhost:7014/api/GetEmpleado/${cedulaPersona}`
        );
        this.empleado = empleadoRes.data;
        sessionStorage.setItem("empleado", JSON.stringify(this.empleado));
      } catch (error) {
        console.warn("Error al obtener los datos del empleado:", error);
        this.empleado = null;
      }
    },
    cargarDesdeSessionStorage() {
      const usuario = sessionStorage.getItem("usuario");
      if (usuario) {
        this.usuario = JSON.parse(usuario);
      }

      const empleado = sessionStorage.getItem("empleado");
      if (empleado) {
        this.empleado = JSON.parse(empleado);
      }
    },
  },
});