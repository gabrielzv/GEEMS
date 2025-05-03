import { defineStore } from "pinia";

export const useUserStore = defineStore("user", {
  state: () => ({
    usuario: null,
  }),
  actions: {
    setUsuario(data) {
      this.usuario = data;
    },
    clearUsuario() {
      this.usuario = null;
    },
  },
});
