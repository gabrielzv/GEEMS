import { createApp } from "vue";
import App from "./components/App.vue";
import { createRouter, createWebHistory } from "vue-router";
// Importa todos tus componentes...
import Login from "./components/LoginForm.vue";
import Home from "./components/HomePage.vue";
import Recuperar from "./components/RecuperarCont.vue";
import BenefitCreation from "./components/BenefitCreation.vue";
import UserView from "./components/UserView.vue";
import Register from "./components/RegisterForm.vue";
import CompanyBenefits from "./components/CompanyBenefits.vue";
import EmployeeView from "./components/EmployeeView.vue";
import RegistroEmpresa from "./components/RegistroEmpresa.vue";
import VerEmpresaIndv from "./components/VerEmpresaIndv.vue";
import VerEmpresaIndvSuperAdmin from "./components/VerEmpresaIndvSuperAdmin.vue";
import ConsultaEmpresa from "./components/ConsultaEmpresas.vue";
import AnadirEmpleado from "./components/AnadirEmpleado.vue";
import MatricularBeneficio from "./components/MatricularBeneficio.vue";
import EmployeeBenefits from "./components/EmployeeBenefits.vue";
import AuthenticatedLayout from "./layouts/AuthenticatedLayout.vue";
import RegistroHoras from "./components/RegistroHoras.vue";
import EmployeeRegHistory from "./components/EmployeeRegHistory.vue";
import EditarHoras from "./components/EditarHoras.vue";
import { createPinia } from "pinia";
import { useUserStore } from "./store/user";
import "./assets/tailwind.css";

const router = createRouter({
  history: createWebHistory(),
  routes: [
    { path: "/", redirect: "/login" },
    {path: "/login", name: "Login", component: Login},
    { path: "/registroEmpresa", name: "RegistroEmprea", component: RegistroEmpresa },
    { path: "/register", name: "Register", component: Register },
    { path: "/recuperar", name: "Recuperar", component: Recuperar },
    
    // Rutas que requieren autenticación (con header)
    {
      path: "/",
      component: AuthenticatedLayout,
      children: [
        { path: "/home", name: "Home", component: Home },
        { path: "/user", name: "User", component: UserView },
        { path: "/benefitCreation", name: "BenefitCreation", component: BenefitCreation },
        { path: "/companyBenefits", name: "CompanyBenefits", component: CompanyBenefits },
        { path: "/verEmpresaIndv", name: "VerEmpresaIndv", component: VerEmpresaIndv },
        { path: "/employee/:cedula", name: "Employee", component: EmployeeView },
        { path: "/ConsulEmpresa", name: "ConsultaEmpresa", component: ConsultaEmpresa },
        { path: "/VerEmpresaIndvSuperAdmin/:empresaId", name: "VerEmpresaIndvSuperAdmin", component: VerEmpresaIndvSuperAdmin },
        { path: "/matricularBeneficio", name: "MatricularBeneficio", component: MatricularBeneficio },
        { path: "/employeeBenefits", name: "EmployeeBenefits", component: EmployeeBenefits },
        { path: '/anadirEmpleado', name: 'anadirEmpleado', component: AnadirEmpleado, meta: { requiresAuth: true }},
        { path: '/registrarHoras', name: 'registrarHoras', component: RegistroHoras, meta: {requiresAuth: true}},
        { path: '/employeeRegHistory', name: 'employeeRegHistory', component: EmployeeRegHistory, meta: {requiresAuth: true} },
        { path: '/editarHoras/:registroId', name: 'editarHoras', component: EditarHoras, meta: {requiresAuth: true}},
      ]
    }
  ],
});

const app = createApp(App);
app.use(createPinia());
app.use(router);
router.beforeEach((to, from, next) => {
  const userStore = useUserStore();
  const isAuthenticated = userStore.usuario !== null;

  // Rutas que no requieren autenticación
  const publicPages = ['/login', '/register', '/recuperar', '/prueba','/registroEmpresa'];
  const isPublicPage = publicPages.includes(to.path);

  if (!isPublicPage && !isAuthenticated) {
    return next('/login');
  }

  // Redirigir al home si ya está autenticado y trata de acceder a login/register
  if ((to.path === '/login' || to.path === '/register') && isAuthenticated) {
    return next('/home');
  }

  next();
});
// Cargar el estado del usuario al iniciar
const userStore = useUserStore();
userStore.cargarDesdeSessionStorage();

app.mount("#app");