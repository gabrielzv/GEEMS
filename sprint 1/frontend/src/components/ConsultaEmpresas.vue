<template>
  <div class="container mx-auto px-4 py-8 max-w-[1200px]">
    <!-- Encabezado -->
    <div class="flex justify-between items-center mb-8 border-b pb-4">
      <h1 class="text-2xl font-bold">Empresas registradas</h1>
      <div class="text-right">
        <p class="font-medium">{{ userView.fullName }}</p>
        <p class="text-sm text-gray-600">Super Admin</p>
      </div>
    </div>

    <!-- Filtro de búsqueda simplificado -->
    <div class="bg-white p-4 rounded-lg shadow mb-6">
      <div class="mb-4">
        <label class="block text-sm font-medium mb-1">Buscar por nombre</label>
        <input
          v-model="searchName"
          type="text"
          placeholder="Escribe el nombre de la empresa"
          class="w-full px-3 py-2 border border-gray-300 rounded focus:outline-none focus:ring-1 focus:ring-gray-400 transition-all duration-200"
        />
      </div>
    </div>

    <!-- Tabla de empresas -->
    <div class="bg-white rounded-lg shadow overflow-hidden">
      <table class="min-w-full divide-y divide-gray-200">
        <thead class="bg-gray-50">
          <tr>
            <th
              class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider"
            >
              Nombre
            </th>
            <th
              class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider"
            >
              Cédula
            </th>
            <th
              class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider"
            >
              Teléfono
            </th>
            <th
              class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider"
            >
              Propietario
            </th>
            <th
              class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider"
            >
              Correo Electrónico
            </th>
            <th
              class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider"
            >
              Acciones
            </th>
          </tr>
        </thead>
        <tbody class="bg-white divide-y divide-gray-200">
          <tr
            v-for="company in filteredCompanies"
            :key="company.id"
            class="hover:bg-gray-50 transition-colors duration-150"
          >
            <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
              {{ company.name }}
            </td>
            <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
              {{ company.legalId }}
            </td>
            <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
              {{ company.phone }}
            </td>
            <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
              {{ company.owner }}
            </td>
            <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
              {{ company.email }}
            </td>
            <td class="px-6 py-4 whitespace-nowrap text-sm font-medium">
              <button
                @click="viewDetail(company.id)"
                class="text-gray-600 hover:text-gray-900 mr-3 transition-colors duration-200"
              >
                Ver detalle
              </button>
            </td>
          </tr>
        </tbody>
      </table>
    </div>
  </div>
</template>

<script>
import { ref, computed, onMounted } from "vue";
import axios from "axios";
import { useUserStore } from "../store/user";

export default {
  setup() {
    const userStore = useUserStore();
    const usuario = computed(() => userStore.usuario);
    const loading = ref(true);
    const error = ref(false);
    const userView = ref({});
    const searchName = ref("");

    const companies = ref([
      {
        id: 1,
        name: "TebTEK",
        legalId: "123-45678",
        phone: "8888-8888",
        owner: "EstebanCR",
        email: "contacto@tebtek.com",
      },
      {
        id: 2,
        name: "Fabes",
        legalId: "321-48372",
        phone: "2222-2222",
        owner: "Salpizar",
        email: "info@fabes.com",
      },
      {
        id: 3,
        name: "Activision",
        legalId: "456-78910",
        phone: "7777-7777",
        owner: "Alpha",
        email: "soporte@activision.com",
      },
    ]);

    const filteredCompanies = computed(() => {
      if (!searchName.value) return companies.value;
      return companies.value.filter((company) =>
        company.name.toLowerCase().includes(searchName.value.toLowerCase())
      );
    });

    const fetchUserView = async () => {
      loading.value = true;
      error.value = false;

      try {
        const personaRes = await axios.get(
          `https://localhost:7014/api/Persona/${usuario.value.cedulaPersona}`
        );
        const data = personaRes.data;

        await userStore.fetchEmpleado(usuario.value.cedulaPersona);

        const dataEmpleado = userStore.empleado || {};

        userView.value = {
          fullName: data.fullName || "Dato no disponible",
          email: data.email || "Dato no disponible",
          phone: data.phone || "Dato no disponible",
          role: usuario.value.tipo || "Dato no disponible",
          address: data.address || "Dato no disponible",
          contract: dataEmpleado.contrato || "Dato no disponible",
          genre: dataEmpleado.genero || "Dato no disponible",
          state: dataEmpleado.estadoLaboral || "Dato no disponible",
          type: dataEmpleado.tipo || "Dato no disponible",
          dateIn: dataEmpleado.fechaIngreso || "Dato no disponible",
          company: dataEmpleado.nombreEmpresa || "Dato no disponible",
          cedulaPersona: usuario.value.cedulaPersona || "Dato no disponible",
        };

        if (dataEmpleado.genero == "F") {
          userView.value.genre = "Femenino";
        } else {
          userView.value.genre = "Masculino";
        }
      } catch (err) {
        console.error("Error al obtener los datos de la persona", err);
        error.value = true;
      } finally {
        loading.value = false;
      }
    };

    const viewDetail = (companyId) => {
      console.log("Ver detalle de empresa:", companyId);
      // this.$router.push(`/empresas/${companyId}`) // Si usas vue-router
    };

    onMounted(() => {
      if (!usuario.value || !usuario.value.cedulaPersona) {
        window.location.href = "/";
      } else if (usuario.value.tipo != "SuperAdmin") {
        const errorMessage =
          "No tienes acceso a esta página. Serás redirigido al homepage.";
        alert(errorMessage);
        window.location.href = "/home"; // O la ruta que prefieras
      } else {
        fetchUserView();
      }
    });

    return {
      searchName,
      companies,
      filteredCompanies,
      userView,
      viewDetail,
    };
  },
};
</script>
