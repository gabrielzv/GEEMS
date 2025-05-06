<template>
  <div class="container mx-auto px-4 py-8 max-w-[1200px]">
    <!-- Encabezado -->
    <div class="flex justify-between items-center mb-8 border-b pb-4">
      <h1 class="text-2xl font-bold">Empresas registradas</h1>
      <div class="text-right">
        <p class="font-medium">{{ usuario?.nombre || "Administrador" }}</p>
        <p class="text-sm text-gray-600">
          {{ usuario?.tipo || "Super Admin" }}
        </p>
      </div>
    </div>

    <!-- Filtro -->
    <div class="bg-white p-4 rounded-lg shadow mb-6">
      <label class="block text-sm font-medium mb-1">Buscar por nombre</label>
      <input
        v-model="searchName"
        type="text"
        placeholder="Escribe el nombre de la empresa"
        class="w-full px-3 py-2 border border-gray-300 rounded focus:outline-none focus:ring-1 focus:ring-gray-400"
      />
    </div>

    <!-- Tabla -->
    <div class="bg-white rounded-lg shadow overflow-hidden">
      <table class="min-w-full divide-y divide-gray-200">
        <thead class="bg-gray-50">
          <tr>
            <th
              class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase"
            >
              Nombre
            </th>
            <th
              class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase"
            >
              Cédula
            </th>
            <th
              class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase"
            >
              Teléfono
            </th>
            <th
              class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase"
            >
              Propietario
            </th>
            <th
              class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase"
            >
              Correo
            </th>
            <th
              class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase"
            >
              Acciones
            </th>
          </tr>
        </thead>
        <tbody class="bg-white divide-y divide-gray-200">
          <tr
            v-for="company in filteredCompanies"
            :key="company.id"
            class="hover:bg-gray-50 transition"
          >
            <td class="px-6 py-4 text-sm text-gray-900">{{ company.name }}</td>
            <td class="px-6 py-4 text-sm text-gray-500">
              {{ company.legalId }}
            </td>
            <td class="px-6 py-4 text-sm text-gray-500">{{ company.phone }}</td>
            <td class="px-6 py-4 text-sm text-gray-500">{{ company.owner }}</td>
            <td class="px-6 py-4 text-sm text-gray-500">{{ company.email }}</td>
            <td class="px-6 py-4 text-sm font-medium">
              <button
                @click="viewDetail(company.id)"
                class="text-blue-600 hover:text-blue-800"
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
    const usuario = userStore.usuario;
    const companies = ref([]);
    const searchName = ref("");
    const error = ref(false);
    const loading = ref(true);

    const filteredCompanies = computed(() => {
      if (!searchName.value) return companies.value;
      return companies.value.filter((company) =>
        company.name.toLowerCase().includes(searchName.value.toLowerCase())
      );
    });

    const fetchEmpresas = async () => {
      try {
        const res = await axios.get("https://localhost:7014/api/Empresa");
        companies.value = res.data.map((empresa) => ({
          id: empresa.cedulaJuridica,
          name: empresa.nombre,
          legalId: empresa.cedulaJuridica,
          phone: empresa.telefono,
          owner: empresa.dueno || "Sin asignar",
          email: empresa.correo,
        }));
      } catch (err) {
        console.error("Error al obtener las empresas:", err);
        error.value = true;
      } finally {
        loading.value = false;
      }
    };

    const viewDetail = (companyId) => {
      console.log("Ver detalle de empresa:", companyId);
    };

    onMounted(() => {
      if (!usuario?.cedulaPersona) {
        window.location.href = "/";
      } else {
        fetchEmpresas();
      }
    });

    return {
      searchName,
      companies,
      filteredCompanies,
      viewDetail,
      usuario,
    };
  },
};
</script>
