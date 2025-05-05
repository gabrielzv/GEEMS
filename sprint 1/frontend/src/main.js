import { createApp } from "vue";
import App from "./components/App.vue";
import { createRouter, createWebHistory } from "vue-router";
import Login from "./components/LoginForm.vue";
import Home from "./components/HomePage.vue";
import Recuperar from "./components/RecuperarCont.vue";
import UserView from "./components/UserView.vue";
import Register from "./components/RegisterForm.vue";
import Prueba from "./components/Prueba.vue";
import { createPinia } from "pinia";
import { useUserStore } from "./store/user";
import "./assets/tailwind.css";

const router = createRouter({
  history: createWebHistory(),
  routes: [
    { path: "/", redirect: "/login" }, // Redirige automáticamente a /login
    { path: "/login", name: "Login", component: Login },
    { path: "/home", name: "Home", component: Home },
    { path: "/recuperar", name: "Recuperar", component: Recuperar },
    { path: "/user", name: "User", component: UserView },
    { path: "/register", name: "Register", component: Register },
    { path: "/prueba", name: "Prueba", component: Prueba },
  ],
});

const app = createApp(App);
app.use(createPinia());
app.use(router);
const userStore = useUserStore();
userStore.cargarDesdeSessionStorage();
app.mount("#app");
