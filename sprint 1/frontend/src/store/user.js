import { defineStore } from "pinia";

export const useUserStore = defineStore("user", {
  state: () => ({
    usuario: null,
    empleado: null,
  }),
  actions: {
    setUsuario(usuario) {
      this.usuario = usuario;
      // Guardar en sessionStorage en lugar de localStorage
      sessionStorage.setItem("usuario", JSON.stringify(usuario));
    },
    // setEmpleado(empleado) {
    //   this.empleado = empleado;
    //   // Guardar en sessionStorage en lugar de localStorage
    //   sessionStorage.setItem("empleado", JSON.stringify(empleado));
    // },
    cargarDesdeSessionStorage() {
      const usuario = sessionStorage.getItem("usuario");
      if (usuario) {
        this.usuario = JSON.parse(usuario);
      }
    
      // const empleado = sessionStorage.getItem("empleado");
      // if (empleado) {
      //   this.empleado = JSON.parse(empleado);
      // }
    },
    logout() {
      this.usuario = null;
      sessionStorage.removeItem("usuario");
    },
  },
});
