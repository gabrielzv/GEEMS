<template>
  <div class="min-h-screen p-6 w-full flex flex-col bg-gray-100">
    <div v-if="loading" class="text-center text-xl mt-10">
      Cargando datos del empleado...
    </div>

    <div v-else-if="error" class="text-center text-red-600 mt-10">
      <p class="text-xl">Error al cargar los datos.</p>
      <button
        @click="fetchEmployeeData"
        class="mt-4 px-6 py-3 bg-blue-600 text-white rounded"
      >
        Reintentar
      </button>
    </div>

    <div v-else class="bg-white shadow-lg rounded-lg p-10 w-full max-w-6xl mx-auto">
      <h1 class="text-3xl font-bold mb-6">Editar perfil de empleado</h1>
      
      <div class="grid grid-cols-1 md:grid-cols-2 gap-6">
        <!-- Columna Izquierda -->
        <div>
          <div class="form-group mb-4">
            <label class="block font-medium mb-2">Nombre completo</label>
            <input v-model="editableEmployee.fullName" class="w-full p-2 border rounded">
          </div>

          <div class="form-group mb-4">
            <label class="block font-medium mb-2">Cédula</label>
            <input v-model="editableEmployee.cedulaPersona" disabled class="w-full p-2 border rounded bg-gray-100">
          </div>

          <div class="form-group mb-4">
            <label class="block font-medium mb-2">Teléfono</label>
            <input v-model="editableEmployee.phone" class="w-full p-2 border rounded">
          </div>

          <div class="form-group mb-4">
            <label class="block font-medium mb-2">Correo electrónico</label>
            <input v-model="editableEmployee.email" type="email" class="w-full p-2 border rounded">
          </div>
        </div>

        <!-- Columna Derecha -->
        <div>
          <div class="form-group mb-4">
            <label class="block font-medium mb-2">Dirección</label>
            <input v-model="editableEmployee.address" class="w-full p-2 border rounded">
          </div>

          <div class="form-group mb-4">
            <label class="block font-medium mb-2">Fecha de ingreso</label>
            <input v-model="editableEmployee.dateIn" type="date" class="w-full p-2 border rounded">
          </div>

          <div class="form-group mb-4">
            <label class="block font-medium mb-2">Rol</label>
            <select v-model="editableEmployee.role" class="w-full p-2 border rounded">
              <option value="Colaborador">Colaborador</option>
              <option value="Gerente">Gerente</option>
              <option value="Administrador">Administrador</option>
            </select>
          </div>

          <div class="form-group mb-4">
            <label class="block font-medium mb-2">Salario</label>
            <input v-model="editableEmployee.salario" type="number" class="w-full p-2 border rounded">
          </div>
        </div>
      </div>

      <div class="mt-8 flex justify-end gap-x-4">
        <button
          @click="cancelEditing"
          class="px-6 py-3 bg-gray-500 text-white rounded hover:bg-gray-600"
        >
          Cancelar
        </button>
        <button
          @click="saveChanges"
          class="px-6 py-3 bg-blue-600 text-white rounded hover:bg-blue-700"
        >
          Guardar cambios
        </button>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from "vue";
import { useRoute, useRouter } from "vue-router";
import axios from "axios";
import { useUserStore } from "../store/user";

const route = useRoute();
const router = useRouter();
const userStore = useUserStore();

const cedula = route.params.cedula;
const loading = ref(true);
const error = ref(false);
const editableEmployee = ref({});

const fetchEmployeeData = async () => {
  loading.value = true;
  error.value = false;

  try {
    // Obtener datos de Persona
    const personaRes = await axios.get(
      `https://localhost:7014/api/Persona/${cedula}`
    );
    const personaData = personaRes.data;

    // Obtener datos de Empleado
    await userStore.fetchEmpleado(cedula);
    const empleadoData = userStore.empleado || {};

    // Combinar los datos
    editableEmployee.value = {
      fullName: personaData.fullName || "",
      cedulaPersona: cedula,
      phone: personaData.phone || "",
      email: personaData.email || "",
      address: personaData.address || "",
      role: empleadoData.tipo || "Colaborador",
      dateIn: empleadoData.fechaIngreso || "",
      salario: empleadoData.salarioBruto || "",
      contract: empleadoData.contrato || "",
      genre: empleadoData.genero === "F" ? "Femenino" : "Masculino",
      state: empleadoData.estadoLaboral || "",
      type: empleadoData.tipo || "",
      company: empleadoData.nombreEmpresa || ""
    };
  } catch (err) {
    console.error("Error al obtener los datos del empleado", err);
    error.value = true;
  } finally {
    loading.value = false;
  }
};

const saveChanges = async () => {
  try {
    loading.value = true;
    
    // Preparar datos para actualizar Persona
    const personaUpdate = {
      fullName: editableEmployee.value.fullName,
      phone: editableEmployee.value.phone,
      email: editableEmployee.value.email,
      address: editableEmployee.value.address
    };

    // Preparar datos para actualizar Empleado
    const empleadoUpdate = {
      tipo: editableEmployee.value.role,
      salarioBruto: editableEmployee.value.salario,
      fechaIngreso: editableEmployee.value.dateIn,
      contrato: editableEmployee.value.contract
    };

    // Enviar actualizaciones: PUT para Persona y POST para Empleado (endpoint actualizado)
    await axios.put(`https://localhost:7014/api/Persona/${cedula}`, personaUpdate);
    await axios.post(`https://localhost:7014/api/Empleado/actualizar/${cedula}`, empleadoUpdate);

    // Actualizar datos en el store
    await userStore.fetchEmpleado(cedula);

    alert("Cambios guardados exitosamente");
    router.push(`/employee/${cedula}`);
  } catch (err) {
    console.error("Error al guardar los cambios", err);
    alert("Error al guardar los cambios");
  } finally {
    loading.value = false;
  }
};


const cancelEditing = () => {
  if (confirm("¿Estás seguro de que deseas descartar los cambios?")) {
    router.go(-1); // Volver a la página anterior
  }
};

onMounted(() => {
  if (!userStore.usuario || !userStore.usuario.cedulaPersona) {
    window.location.href = "/";
  } else {
    fetchEmployeeData();
  }
});
</script>

<style scoped>
.form-group {
  margin-bottom: 1rem;
}

input, select {
  width: 100%;
  padding: 0.5rem;
  border: 1px solid #ccc;
  border-radius: 4px;
}

@media (max-width: 640px) {
  .grid-cols-2 {
    grid-template-columns: 1fr;
  }
}
</style>