import { createApp } from 'vue'
import App from './App.vue'
import {createRouter, createWebHistory} from "vue-router";
import Login from "./components/LoginForm.vue";
import Home from "./components/HomePage.vue";

const router = createRouter({
    history: createWebHistory(),
    routes: [
        { path: '/',name: "Login", component: Login },
        { path: '/home', name : "Home", component: Home },
    ]
});

createApp(App).use(router).mount('#app')
