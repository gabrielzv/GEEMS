<template>
  <div class="min-h-screen flex items-center justify-center bg-gray-100 p-4">
    <form @submit.prevent="registrarEmpleado" class="bg-white p-8 rounded-2xl shadow-md w-full max-w-2xl space-y-6">
      <h2 class="text-2xl font-bold text-center">Registro de Empleado</h2>

      <!-- Datos Persona -->
      <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
        <div>
          <label class="block text-sm font-medium text-gray-700 mb-1">Cédula</label>
          <input
            v-model="cedula"
            type="number"
            placeholder="Cédula"
            :class="inputClass(cedulaError)"
            @blur="checkCedula"
          />
          <p v-if="cedulaError" class="text-sm text-red-500 mt-1">{{ cedulaError }}</p>
        </div>

        <div>
          <label class="block text-sm font-medium text-gray-700 mb-1">Nombre</label>
          <input
            v-model="nombre"
            type="text"
            placeholder="Nombre"
            :class="inputClass(nombreError)"
          />
          <p v-if="nombreError" class="text-sm text-red-500 mt-1">{{ nombreError }}</p>
        </div>

        <div>
          <label class="block text-sm font-medium text-gray-700 mb-1">Primer Apellido</label>
          <input
            v-model="apellido1"
            type="text"
            placeholder="Primer Apellido"
            :class="inputClass(apellido1Error)"
          />
          <p v-if="apellido1Error" class="text-sm text-red-500 mt-1">{{ apellido1Error }}</p>
        </div>

        <div>
          <label class="block text-sm font-medium text-gray-700 mb-1">Segundo Apellido</label>
          <input
            v-model="apellido2"
            type="text"
            placeholder="Segundo Apellido"
            :class="inputClass(apellido2Error)"
          />
          <p v-if="apellido2Error" class="text-sm text-red-500 mt-1">{{ apellido2Error }}</p>
        </div>

        <div>
          <label class="block text-sm font-medium text-gray-700 mb-1">Dirección</label>
          <input
            v-model="direccion"
            type="text"
            placeholder="Dirección"
            :class="inputClass(direccionError)"
          />
          <p v-if="direccionError" class="text-sm text-red-500 mt-1">{{ direccionError }}</p>
        </div>

        <div>
          <label class="block text-sm font-medium text-gray-700 mb-1">Correo</label>
          <input
            v-model="correo"
            type="email"
            placeholder="Correo"
            :class="inputClass(correoError)"
            @blur="checkCorreo"
          />
          <p v-if="correoError" class="text-sm text-red-500 mt-1">{{ correoError }}</p>
        </div>

        <div>
          <label class="block text-sm font-medium text-gray-700 mb-1">Teléfono</label>
          <input
            v-model="telefono"
            type="text"
            placeholder="Teléfono"
            :class="inputClass(telefonoError)"
          />
          <p v-if="telefonoError" class="text-sm text-red-500 mt-1">{{ telefonoError }}</p>
        </div>

        <div>
          <label class="block text-sm font-medium text-gray-700 mb-1">Fecha de Nacimiento</label>
          <input
            v-model="fechaNacimiento"
            type="date"
            placeholder="Fecha de nacimiento"
            :class="inputClass(fechaNacimientoError)"
          />
          <p v-if="fechaNacimientoError" class="text-sm text-red-500 mt-1">{{ fechaNacimientoError }}</p>
        </div>
      </div>

      <!-- Usuario -->
      <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
        <div>
          <label class="block text-sm font-medium text-gray-700 mb-1">Nombre de usuario</label>
          <input
            v-model="username"
            type="text"
            placeholder="Nombre de usuario"
            :class="inputClass(usernameError)"
            @blur="checkUsername"
          />
          <p v-if="usernameError" class="text-sm text-red-500 mt-1">{{ usernameError }}</p>
        </div>
      </div>

      <!-- Botón -->
      <button
        type="submit"
        class="bg-blue-600 hover:bg-blue-700 text-white font-semibold py-2 px-4 rounded-xl w-full"
        :disabled="isSubmitting || hasErrors"
      >
        {{ isSubmitting ? "Registrando..." : "Registrar Empleado" }}
      </button>
    </form>
  </div>
</template>

<script setup>
import { ref, computed } from "vue";
import axios from "axios";

const cedula = ref(null);
const correo = ref("");
const username = ref("");
const isSubmitting = ref(false);

const cedulaError = ref("");
const correoError = ref("");
const usernameError = ref("");

const hasErrors = computed(() => {
  return !!cedulaError.value || !!correoError.value || !!usernameError.value;
});

function inputClass(error) {
  return [
    "w-full px-4 py-2 rounded border focus:outline-none focus:ring-2",
    error ? "border-red-500 focus:ring-red-300" : "border-gray-300 focus:ring-blue-300",
  ];
}

async function checkCedula() {
  if (!cedula.value) {
    cedulaError.value = "La cédula es obligatoria.";
    return false;
  }
  try {
    const response = await axios.get(`https://localhost:7014/api/CheckDupe/cedula/${cedula.value}`);
    cedulaError.value = response.data ? "La cédula ya está en uso." : "";
    return !response.data;
  } catch (error) {
    console.error("Error al verificar la cédula:", error);
    cedulaError.value = "Error al verificar la cédula.";
    return false;
  }
}

async function checkCorreo() {
  if (!correo.value) {
    correoError.value = "El correo es obligatorio.";
    return false;
  }
  try {
    const response = await axios.get(`https://localhost:7014/api/CheckDupe/correo/${correo.value}`);
    correoError.value = response.data ? "El correo electrónico ya está en uso." : "";
    return !response.data;
  } catch (error) {
    console.error("Error al verificar el correo electrónico:", error);
    correoError.value = "Error al verificar el correo.";
    return false;
  }
}

async function checkUsername() {
  if (!username.value) {
    usernameError.value = "El nombre de usuario es obligatorio.";
    return false;
  }
  try {
    const response = await axios.get(`https://localhost:7014/api/CheckDupe/username/${username.value}`);
    usernameError.value = response.data ? "El nombre de usuario ya está en uso." : "";
    return !response.data;
  } catch (error) {
    console.error("Error al verificar el nombre de usuario:", error);
    usernameError.value = "Error al verificar el nombre de usuario.";
    return false;
  }
}

async function registrarEmpleado() {
  isSubmitting.value = true;

  // Validar campos antes de enviar
  const isCedulaValid = await checkCedula();
  const isCorreoValid = await checkCorreo();
  const isUsernameValid = await checkUsername();

  if (!isCedulaValid || !isCorreoValid || !isUsernameValid) {
    isSubmitting.value = false;
    return;
  }

  try {
    // Lógica para registrar al empleado
    const empleadoData = {
      cedula: cedula.value,
      correo: correo.value,
      username: username.value,
      // Agrega aquí los demás campos necesarios
    };

    const response = await axios.post("https://localhost:7014/api/Register/empleado", empleadoData);
    console.log("Empleado registrado:", response.data);

    alert("Empleado registrado exitosamente.");
  } catch (error) {
    console.error("Error al registrar empleado:", error);
    alert("Error al registrar empleado.");
  } finally {
    isSubmitting.value = false;
  }
}
</script>