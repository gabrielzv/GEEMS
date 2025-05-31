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
          />
          <p v-if="usernameError" class="text-sm text-red-500 mt-1">{{ usernameError }}</p>
        </div>
      </div>

      <!-- Género -->
      <div>
        <label class="block font-semibold mb-2">Género:</label>
        <div class="flex gap-4">
          <label class="flex items-center">
            <input type="radio" name="genero" value="M" v-model="genero" class="mr-2" /> Masculino
          </label>
          <label class="flex items-center">
            <input type="radio" name="genero" value="F" v-model="genero" class="mr-2" /> Femenino
          </label>
        </div>
        <p v-if="generoError" class="text-sm text-red-500 mt-1">{{ generoError }}</p>
      </div>

      <!-- Contrato -->
      <div>
        <label class="block font-semibold mb-2">Contrato:</label>
        <div class="flex flex-wrap gap-4">
          <label class="flex items-center">
            <input type="radio" name="contrato" value="Por Horas" v-model="contrato" class="mr-2" /> Por Horas
          </label>
          <label class="flex items-center">
            <input type="radio" name="contrato" value="Tiempo Completo" v-model="contrato" class="mr-2" /> Tiempo Completo
          </label>
          <label class="flex items-center">
            <input type="radio" name="contrato" value="Medio Tiempo" v-model="contrato" class="mr-2" /> Medio Tiempo
          </label>
          <label class="flex items-center">
            <input type="radio" name="contrato" value="Servicios Profesionales" v-model="contrato" class="mr-2" /> Servicios Profesionales
          </label>
        </div>
        <p v-if="contratoError" class="text-sm text-red-500 mt-1">{{ contratoError }}</p>
      </div>

      <!-- Rol -->
      <div>
        <label class="block font-semibold mb-2">Rol:</label>
        <div class="flex gap-4">
          <label class="flex items-center">
            <input type="radio" name="rol" value="Colaborador" v-model="rol" class="mr-2" /> Colaborador
          </label>
          <label class="flex items-center">
            <input type="radio" name="rol" value="Supervisor" v-model="rol" class="mr-2" /> Supervisor
          </label>
          <label class="flex items-center">
            <input type="radio" name="rol" value="Payroll" v-model="rol" class="mr-2" /> Payroll
          </label>
        </div>
        <p v-if="rolError" class="text-sm text-red-500 mt-1">{{ rolError }}</p>
      </div>

      <!-- Empresa y Salario -->
      <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
        <div>
          <label class="block text-sm font-medium text-gray-700 mb-1">Salario Bruto</label>
          <input
            v-model="salarioBruto"
            type="number"
            placeholder="Salario Bruto"
            :class="inputClass(salarioBrutoError)"
          />
          <p v-if="salarioBrutoError" class="text-sm text-red-500 mt-1">{{ salarioBrutoError }}</p>
        </div>
      </div>

      <!-- Botón -->
      <button
        type="submit"
        class="bg-blue-600 hover:bg-blue-700 text-white font-semibold py-2 px-4 rounded-xl w-full"
        :disabled="isSubmitting"
      >
        {{ isSubmitting ? "Registrando..." : "Registrar Empleado" }}
      </button>
    </form>
  </div>
</template>

<script setup>
import { v4 as uuidv4 } from "uuid";
import { ref, onMounted } from "vue";
import axios from "axios";
import { useRouter, useRoute } from "vue-router";

const router = useRouter();
const route = useRoute();

const duenoCedula = ref(route.query.duenoCedula || null);

// Función para obtener la fecha y hora formateada
function getFormattedDateTime() {
  const now = new Date();
  const day = String(now.getDate()).padStart(2, "0");
  const month = String(now.getMonth() + 1).padStart(2, "0");
  const year = now.getFullYear();
  return `${day}/${month}/${year}`;
}

// Variables reactivas para el formulario
const cedula = ref(null);
const direccion = ref("");
const nombre = ref("");
const apellido1 = ref("");
const apellido2 = ref("");
const correo = ref("");
const telefono = ref("");
const fechaNacimiento = ref("");
const username = ref("");
const genero = ref("");
const contrato = ref("");
const salarioBruto = ref("");
const rol = ref("");
const nombreEmpresa = ref("");
const isSubmitting = ref(false);

// Variables de error
const cedulaError = ref("");
const direccionError = ref("");
const nombreError = ref("");
const apellido1Error = ref("");
const apellido2Error = ref("");
const correoError = ref("");
const telefonoError = ref("");
const fechaNacimientoError = ref("");
const usernameError = ref("");
const salarioBrutoError = ref("");
const nombreEmpresaError = ref("");
const generoError = ref("");
const contratoError = ref("");
const rolError = ref("");

// Función para estilos de input
function inputClass(error) {
  return [
    "w-full px-4 py-2 rounded border focus:outline-none focus:ring-2",
    error ? "border-red-500 focus:ring-red-300" : "border-gray-300 focus:ring-blue-300",
  ];
}

// Función para validar email
function validateEmail(email) {
  const regex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
  return regex.test(email);
}

// Función para validar campos
function validateFields() {
  let valid = true;

  // Reset errores
  cedulaError.value = "";
  direccionError.value = "";
  nombreError.value = "";
  apellido1Error.value = "";
  apellido2Error.value = "";
  correoError.value = "";
  telefonoError.value = "";
  fechaNacimientoError.value = "";
  usernameError.value = "";
  salarioBrutoError.value = "";
  nombreEmpresaError.value = "";
  generoError.value = "";
  contratoError.value = "";
  rolError.value = "";

  if (!cedula.value || !/^\d{9}$/.test(cedula.value)) {
    cedulaError.value = "Cédula inválida, debe tener 9 dígitos.";
    valid = false;
  }

  if (!nombre.value) {
    nombreError.value = "Nombre es obligatorio.";
    valid = false;
  }

  if (!apellido1.value) {
    apellido1Error.value = "Primer apellido es obligatorio.";
    valid = false;
  }

  if (!apellido2.value) {
    apellido2Error.value = "Segundo apellido es obligatorio.";
    valid = false;
  }

  if (!direccion.value) {
    direccionError.value = "Dirección es obligatoria.";
    valid = false;
  }

  if (!correo.value || !validateEmail(correo.value)) {
    correoError.value = "Formato de correo inválido.";
    valid = false;
  }

  if (!telefono.value || !/^\d{4}-\d{4}$/.test(telefono.value)) {
    telefonoError.value = "Teléfono inválido. El formato debe ser 1234-5678.";
    valid = false;
  }

  if (!fechaNacimiento.value) {
    fechaNacimientoError.value = "Fecha de nacimiento es obligatoria.";
    valid = false;
  }

  if (!username.value || username.value.length > 30) {
    usernameError.value = "Nombre de usuario requerido y máximo 30 caracteres.";
    valid = false;
  }

  if (!salarioBruto.value || salarioBruto.value <= 0) {
    salarioBrutoError.value = "Salario bruto debe ser mayor a 0.";
    valid = false;
  }

  // Validaciones para radio buttons
  if (!genero.value) {
    generoError.value = "Seleccione un género.";
    valid = false;
  }

  if (!contrato.value) {
    contratoError.value = "Seleccione un tipo de contrato.";
    valid = false;
  }

  if (!rol.value) {
    rolError.value = "Seleccione un rol.";
    valid = false;
  }

  return valid;
}
  // Función para obtener el nombre de la empresa
async function fetchNombreEmpresa(cedulaPersona) {
  console.log("Se entra al fetchNombreEmoresa");
  try {
    const response = await axios.get(`https://localhost:7014/api/GetDuenoEmpresa/${cedulaPersona}`);
    if (response.data && response.data.nombreEmpresa) {
      nombreEmpresa.value = response.data.nombreEmpresa;
      console.log("Se retorna el nombre de la empresa");
      return response.data.nombreEmpresa; // Retornamos el nombre para usarlo después
    }
    console.log("Se retorna el nombre de la empresa como nulo");
    return null;
  } catch (error) {
    console.error("Error al obtener nombre de empresa:", error);
    return null;
  }
}

// Al montar el componente, obtener el nombre de la empresa
onMounted(async () => {
  if (duenoCedula.value) {
    await fetchNombreEmpresa(duenoCedula.value);
  }
  else{
    console.log("No se tiene cedula para el dueno");
  }
});
// Función para registrar empleado
async function registrarEmpleado() {
  isSubmitting.value = true;

  if (!validateFields()) {
    isSubmitting.value = false;
    return;
  }

  const uniqueId = uuidv4();
  const contraseña = `${apellido1.value.toLowerCase()}${new Date(fechaNacimiento.value).getFullYear()}`;
  const fechaIngreso = getFormattedDateTime();

  try {
    console.log("Enviando datos de Persona:");
    const requestPersona = {
      Cedula: parseInt(cedula.value, 10),
      Direccion: direccion.value.trim(),
      NombrePila: nombre.value.trim(),
      Apellido1: apellido1.value.trim(),
      Apellido2: apellido2.value.trim(),
      Correo: correo.value.trim(),
      Telefono: telefono.value.trim(),
    };
    
    console.log("Request Persona:", requestPersona);
    const responsePersona = await axios.post("https://localhost:7014/api/Register/persona", requestPersona);
    console.log("Respuesta de Persona:", responsePersona.data);

    console.log("ID de Persona:", uniqueId);
    console.log("Contraseña generada:", contraseña);

    const requestUsuario = {
      id: uniqueId,
      username: username.value,
      contrasena: contraseña,
      tipo: "Empleado",
      cedulaPersona: cedula.value,
      correoPersona: correo.value,
    };
    console.log("Request Usuario:", requestUsuario);
    const responseUsuario = await axios.post("https://localhost:7014/api/Register/usuario", requestUsuario);
    console.log("Respuesta de Usuario:", responseUsuario.data);
    console.log("Se tiene el nombre de empresa en: ", nombreEmpresa.value);
    const requestEmpleado = {
      Id: uniqueId,
      CedulaPersona: cedula.value,
      Contrato: contrato.value,
      NumHorasTrabajadas: 0,
      Genero: genero.value,
      EstadoLaboral: "Activo",
      SalarioBruto: salarioBruto.value,
      Tipo: rol.value,
      FechaIngreso: fechaIngreso,
      NombreEmpresa: nombreEmpresa.value,
    };
    console.log("Request Empleado:", requestEmpleado);
    const responseEmpleado = await axios.post("https://localhost:7014/api/Register/empleado", requestEmpleado);
    console.log("Respuesta de Empleado:", responseEmpleado.data);

    alert("Empleado registrado exitosamente");
    router.push("/home");
  } catch (error) {
    console.error(error);
    alert("Error al registrar empleado");
  } finally {
    isSubmitting.value = false;
  }
}
</script>

<style scoped>
/* Estilos adicionales si son necesarios */
</style>