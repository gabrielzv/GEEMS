import { createApp } from 'vue'
import App from './App.vue'
import {createRouter, createWebHistory} from "vue-router";
import Login from "./components/LoginForm.vue";
import Home from "./components/HomePage.vue";
import Recuperar from './components/RecuperarCont.vue';

const router = createRouter({
    history: createWebHistory(),
    routes: [
        { path: '/',name: "Login", component: Login },
        { path: '/home', name : "Home", component: Home },
        { path: '/recuperar', name : "Recuperar", component: Recuperar },
    ]
});

createApp(App).use(router).mount('#app')
