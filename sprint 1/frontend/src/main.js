import { createApp } from "vue";
import App from "./components/App.vue";
import { createRouter, createWebHistory } from "vue-router";
import Login from "./components/LoginForm.vue";
import Home from "./components/HomePage.vue";
import Recuperar from "./components/RecuperarCont.vue";
import BenefitCreation from "./components/BenefitCreation.vue";
import UserView from "./components/UserView.vue";
import { createPinia } from "pinia";
import { useUserStore } from "./store/user";
import "./assets/tailwind.css";

const router = createRouter({
  history: createWebHistory(),
  routes: [
    { path: "/", name: "Login", component: Login },
    { path: "/home", name: "Home", component: Home },
    { path: "/recuperar", name: "Recuperar", component: Recuperar },
    { path: "/user", name: "User", component: UserView },
    { path: "/benefitCreation", name: "BenefitCreation", component: BenefitCreation},
  ],
});

const app = createApp(App);
app.use(createPinia());
app.use(router);
const userStore = useUserStore();
userStore.cargarDesdeSessionStorage();
app.mount("#app");
