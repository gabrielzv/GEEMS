<template>
  <div class="min-h-screen flex items-center justify-center bg-gray-100 px-4">
    <div class="flex flex-col items-center space-y-6">
      <img src="../assets/GEEMSLogo.png" class="h-20 w-auto mx-auto" />
      <form
        @submit.prevent="login"
        novalidate
        class="bg-white p-6 rounded-xl shadow-md w-full max-w-sm space-y-5"
      >
        <p class="text-2xl font-bold text-center text-gray-800">
            Empresa eliminada
        </p>

        <small class="block text-center text-gray-500 text-sm">
            La empresa a la que pertenece su cuenta fue eliminada. <br>
            En una futura actualización podrá crear una nueva empresa.
        </small>

        
        <button
          @click="handleLogout"
          class="w-full bg-blue-600 hover:bg-blue-700 text-white font-semibold py-2 px-4 rounded transition-colors disabled:opacity-50"
        >
          Salir
        </button>

      </form>
    </div>
  </div>
</template>

<script setup lang="jsx">
import { useRouter } from "vue-router";
import { useUserStore } from "@/store/user";
const router = useRouter();
const userStore = useUserStore();
const handleLogout = () => {
    userStore.logout();
    router.push("/login");
}
</script>
