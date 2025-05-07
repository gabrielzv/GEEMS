import { createApp } from "vue";
import App from "./components/App.vue";
import { createRouter, createWebHistory } from "vue-router";
import Login from "./components/LoginForm.vue";
import Home from "./components/HomePage.vue";
import Recuperar from "./components/RecuperarCont.vue";
import BenefitCreation from "./components/BenefitCreation.vue";
import UserView from "./components/UserView.vue";
import Register from "./components/RegisterForm.vue";
import Prueba from "./components/Prueba.vue";
import CompanyBenefits from "./components/CompanyBenefits.vue";
import EmployeeView from "./components/EmployeeView.vue";
import RegistroEmpresa from "./components/RegistroEmpresa.vue";
import VerEmpresaIndv from "./components/VerEmpresaIndv.vue";
import VerEmpresaIndvSuperAdmin from "./components/VerEmpresaIndvSuperAdmin.vue";
import ConsultaEmpresa from "./components/ConsultaEmpresas.vue";
import AnadirEmpleado from "./components/AnadirEmpleado.vue";
// import MatricularBeneficio from "./components/MatricularBeneficio.vue";
import EmployeeBenefits from "./components/EmployeeBenefits.vue";
import { createPinia } from "pinia";
import { useUserStore } from "./store/user";
import "./assets/tailwind.css";

const router = createRouter({
  history: createWebHistory(),
  routes: [
    { path: "/", redirect: "/login" }, // Redirige automáticamente a /login
    {path: "/login", name: "Login", component: Login},
    {
      path: '/anadirEmpleado',
      name: 'anadirEmpleado',
      component: AnadirEmpleado,
      meta: { requiresAuth: true }
    },
    { path: "/home", name: "Home", component: Home },
    { path: "/recuperar", name: "Recuperar", component: Recuperar },
    { path: "/user", name: "User", component: UserView },
    { path: "/register", name: "Register", component: Register },
    { path: "/prueba", name: "Prueba", component: Prueba },
    {
      path: "/benefitCreation",
      name: "BenefitCreation",
      component: BenefitCreation,
    },
    {
      path: "/companyBenefits",
      name: "CompanyBenefits",
      component: CompanyBenefits,
    },
    {
      path: "/registroEmpresa",
      name: "RegistroEmprea",
      component: RegistroEmpresa,
    },
    {
      path: "/verEmpresaIndv",
      name: "VerEmpresaIndv",
      component: VerEmpresaIndv,
    },
    { path: "/employee/:cedula", name: "Employee", component: EmployeeView },
    {
      path: "/ConsulEmpresa",
      name: "ConsultaEmpresa",
      component: ConsultaEmpresa,
    },
    {
      path: "/anadirEmpleado",
      name: "AnadirEmpleado",
      component: AnadirEmpleado,
    },
    {
      path: "/VerEmpresaIndvSuperAdmin/:empresaId",
      name: "VerEmpresaIndvSuperAdmin",
      component: VerEmpresaIndvSuperAdmin,
    },
    // { path: "/matricularBeneficio", name: "MatricularBeneficio", component: MatricularBeneficio },
    {
      path: "/employeeBenefits",
      name: "EmployeeBenefits",
      component: EmployeeBenefits,
    },
  ],
});

const app = createApp(App);
app.use(createPinia());
app.use(router);
const userStore = useUserStore();
userStore.cargarDesdeSessionStorage();
app.mount("#app");
