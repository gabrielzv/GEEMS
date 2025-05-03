<template>
  <div>
    <p>Esto será el Homepage</p>
    <button
      class="px-4 py-2 bg-blue-600 text-white rounded transition-all hover:bg-blue-500"
      @click="goToUserPage"
    >
      Ir a User
    </button>
  </div>
</template>

<script>
import { useRouter } from "vue-router"; // Importa el composable useRouter

export default {
  setup() {
    const router = useRouter(); // Usa el router dentro del setup

    const goToUserPage = () => {
      router.push("/user"); // Redirige a la página de usuario
    };

    return { goToUserPage };
  },
};
</script>

<style scoped>
/* Ya no es necesario SCSS ni CSS adicionales, Tailwind lo maneja */
</style>
