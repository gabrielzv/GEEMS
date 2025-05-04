import { defineStore } from "pinia";

export const useUserStore = defineStore("user", {
  state: () => ({
    usuario: null,
  }),
  actions: {
    setUsuario(usuario) {
      this.usuario = usuario;
      // Guardar en sessionStorage en lugar de localStorage
      sessionStorage.setItem("usuario", JSON.stringify(usuario));
    },

    cargarDesdeSessionStorage() {
      const usuario = sessionStorage.getItem("usuario");
      if (usuario) {
        this.usuario = JSON.parse(usuario);
      }
    },
    logout() {
      this.usuario = null;
      sessionStorage.removeItem("usuario");
    },
  },
});
