<template>
  <div class="min-h-screen bg" style="background-color: #5fc176"></div>
  <!-- <div class="min-h-screen bg-gray-100">
    //Header

    //Contenido principal
    <main class="p-6 flex justify-center items-center">
      <div class="bg-white p-8 rounded-xl shadow-md w-full max-w-md space-y-4">
        <template v-if="loading">
          <p class="text-center text-gray-500">Cargando opciones...</p>
        </template>

        <template v-else-if="error">
          <p class="text-center text-red-600">
            Error al cargar los datos del empleado.
          </p>
        </template>

        <template v-else>
          <h2 class="text-xl font-semibold text-gray-800 text-center mb-4">
            Menú de opciones
          </h2>

          //Opciones para SuperAdmin
          <div v-if="user.tipo === 'SuperAdmin'" class="space-y-2">
            <button class="btn-option bg-blue-500 text-white py-2 my-2 px-4 rounded hover:bg-blue-600">Gestionar usuarios</button>
            <button class="btn-option bg-blue-500 text-white py-2 my-2 px-4 rounded hover:bg-blue-600">Ver reportes generales</button>
            <button class="btn-option bg-blue-500 text-white py-2 my-2 px-4 rounded hover:bg-blue-600">Configurar sistema</button>
            <button class="btn-option bg-blue-500 text-white py-2 my-2 px-4 rounded hover:bg-blue-600" @click="goToVerEmpresasRegistradas">Ver empresas registradas</button>
          </div>

          //Opciones para DuenoEmpresa
          <div v-else-if="user.tipo === 'DuenoEmpresa'" class="space-y-2">
            <button class="btn-option bg-blue-500 text-white py-2 my-2 px-4 rounded hover:bg-blue-600" @click="goToEditarEmpresa" >Editar empresa</button>
            <button class="btn-option bg-blue-500 text-white py-2 my-2 px-4 rounded hover:bg-blue-600" @click="goToAnadirEmpleado">Añadir nuevo empleado</button>
            <button class="btn-option bg-blue-500 text-white py-2 my-2 px-4 rounded hover:bg-blue-600" @click="goToVerEmpresaIndv">Ver información empresa</button>
          </div>

          //Opciones para Empleado
          <div v-else-if="user.tipo === 'Empleado'" class="space-y-2">
            <template v-if="empleado.tipo === 'Supervisor'">
              <button class="btn-option bg-blue-500 text-white py-2 my-2 px-4 rounded hover:bg-blue-600">Registrar horas</button>
              //<button class="btn-option bg-blue-500 text-white py-2 my-2 px-4 rounded hover:bg-blue-600">Seleccionar beneficios</button>
              <button class="btn-option bg-blue-500 text-white py-2 my-2 px-4 rounded hover:bg-blue-600">Desglose de pagos anteriores</button>
              <button class="btn-option bg-blue-500 text-white py-2 my-2 px-4 rounded hover:bg-blue-600">Historial de registros</button>
              <button class="btn-option bg-blue-500 text-white py-2 my-2 px-4 rounded hover:bg-blue-600" @click="goToMatricularBeneficios">
                Matricular Beneficios
              </button>
              <button class="btn-option bg-blue-500 text-white py-2 my-2 px-4 rounded hover:bg-blue-600" @click="goToVerBeneficiosMatriculados">
                Ver Beneficios Matriculados
              </button>
            </template>

            <template v-else-if="empleado?.tipo === 'Colaborador'">
              <button class="btn-option bg-blue-500 text-white py-2 my-2 px-4 rounded hover:bg-blue-600">Registrar horas</button>
              <button class="btn-option bg-blue-500 text-white py-2 my-2 px-4 rounded hover:bg-blue-600">Historial de registros</button>
              <button class="btn-option bg-blue-500 text-white py-2 my-2 px-4 rounded hover:bg-blue-600" @click="goToMatricularBeneficios">
                Matricular Beneficios
              </button>
              <button class="btn-option bg-blue-500 text-white py-2 my-2 px-4 rounded hover:bg-blue-600" @click="goToVerBeneficiosMatriculados">
                Ver Beneficios Matriculados
              </button>
            </template>

            <template v-else-if="empleado?.tipo === 'Payroll'">
              <button class="btn-option bg-blue-500 text-white py-2 my-2 px-4 rounded hover:bg-blue-600">Registrar horas</button>
              <button class="btn-option bg-blue-500 text-white py-2 my-2 px-4 rounded hover:bg-blue-600">Historial de registros</button>
              <button class="btn-option bg-blue-500 text-white py-2 my-2 px-4 rounded hover:bg-blue-600" @click="goToMatricularBeneficios">
                Matricular Beneficios
              </button>
              <button class="btn-option bg-blue-500 text-white py-2 my-2 px-4 rounded hover:bg-blue-600" @click="goToVerBeneficiosMatriculados">
                Ver Beneficios Matriculados
              </button>
            </template>

            <template v-else>
              <p class="text-center text-gray-500">
                Tipo de empleado no reconocido.
              </p>
            </template>
          </div>

          //Rol no reconocido 
          <div v-else class="text-center text-gray-500">
            Rol de usuario no reconocido.
          </div>
        </template>
      </div>
    </main>
  </div> -->
</template>

<script>
// import { ref, onMounted } from "vue";
// import { useUserStore } from "../store/user";
// import { useRouter } from "vue-router";

// export default {
//   setup() {
//     const router = useRouter();
//     const userStore = useUserStore();
//     const user = userStore.usuario;
//     const empleado = ref(null);
//     const loading = ref(true);
//     const error = ref(false);

//     const fetchEmpleado = async () => {
//       try {
//         loading.value = true;
//         error.value = false;

//         Obtener datos desde el store
//         await userStore.fetchEmpleado(user.cedulaPersona);
//         empleado.value = userStore.empleado;
//       } catch (err) {
//         console.error("Error al obtener los datos del empleado:", err);
//         error.value = true;
//       } finally {
//         loading.value = false;
//       }
//     };

//     onMounted(() => {
//       if (!user || !user.cedulaPersona) {
//         router.push("/"); // Redirige si no hay sesión
//       } else {
//         fetchEmpleado();
//       }
//     });

//     const goToUserPage = () => {
//       router.push("/user");
//     };

//     const goToVerEmpresaIndv = () => {
//       router.push("/verEmpresaIndv");
//     };

//     const goToAnadirEmpleado = () => {
//     Asumiendo que user.cedulaPersona es la cédula del dueño
//     router.push({
//       path: '/anadirEmpleado',
//       query: { duenoCedula: user.cedulaPersona }
//     });
//   };

//     const goToVerEmpresasRegistradas = () => {
//       router.push("/ConsulEmpresa");
//     };

//     const goToMatricularBeneficios = () => {
//       router.push("/matricularBeneficio");
//     };

//     const goToVerBeneficiosMatriculados = () => {
//       router.push("/employeeBenefits");
//     };

//     return {
//       goToUserPage,
//       goToVerEmpresaIndv,
//       goToVerEmpresasRegistradas,
//       goToAnadirEmpleado,
//       goToMatricularBeneficios,
//       goToVerBeneficiosMatriculados,
//       user,
//       empleado,
//       loading,
//       error,
//     };
//   },
// };
</script>