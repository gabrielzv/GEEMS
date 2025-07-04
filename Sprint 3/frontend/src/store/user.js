import { defineStore } from "pinia";
import axios from "axios";
import { API_BASE_URL } from "../config";

export const useUserStore = defineStore("user", {
  state: () => ({
    usuario: null,
    empleado: null,
    empresa: null,
    empleadosEmpresa: [],
  }),
  actions: {
    setUsuario(usuario) {
      this.usuario = usuario;
      sessionStorage.setItem("usuario", JSON.stringify(usuario));
    },
    async fetchEmpleado(cedulaPersona) {
      try {
        const empleadoRes = await axios.get(
          `${API_BASE_URL}GetEmpleado/${cedulaPersona}`
        );
        this.empleado = empleadoRes.data;
        sessionStorage.setItem("empleado", JSON.stringify(this.empleado));
      } catch (error) {
        console.warn("Error al obtener los datos del empleado:", error);
        this.empleado = null;
      }
    },
    async fetchEmpresa(cedulaPersona) {
      try {
        const empresaRes = await axios.get(
          `${API_BASE_URL}Empresa/${cedulaPersona}`
        );

        const { empresa, empleados } = empresaRes.data;

        this.empresa = empresa;
        sessionStorage.setItem("empresa", JSON.stringify(this.empresa));

        this.empleadosEmpresa = empleados || [];
        sessionStorage.setItem(
          "empleadosEmpresa",
          JSON.stringify(this.empleadosEmpresa)
        );
      } catch (error) {
        console.warn("Error al obtener los datos de la empresa:", error);
        this.empresa = null;
        this.empleadosEmpresa = [];
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

      const empresa = sessionStorage.getItem("empresa");
      if (empresa) {
        this.empresa = JSON.parse(empresa);
      }

      const empleadosEmpresa = sessionStorage.getItem("empleadosEmpresa");
      if (empleadosEmpresa) {
        this.empleadosEmpresa = JSON.parse(empleadosEmpresa);
      }
    },
    logout() {
      this.usuario = null;
      this.empleado = null;
      this.empresa = null;
      this.empleadosEmpresa = [];

      sessionStorage.removeItem("usuario");
      sessionStorage.removeItem("empleado");
      sessionStorage.removeItem("empresa");
      sessionStorage.removeItem("empleadosEmpresa");
    },
  },
});
