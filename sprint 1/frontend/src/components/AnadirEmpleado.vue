<template>
    <div class="min-h-screen flex items-center justify-center bg-gray-100 p-4">
      <form @submit.prevent="registrarEmpleado" class="bg-white p-8 rounded-2xl shadow-md w-full max-w-2xl space-y-6">
        <h2 class="text-2xl font-bold text-center">Registro de Empleado</h2>
  
        <!-- Datos Persona -->
        <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
          <input v-model="cedula" required type="number" placeholder="Cédula" class="input" />
          <input v-model="nombre" required type="text" placeholder="Nombre" class="input" />
          <input v-model="apellido1" required type="text" placeholder="Primer Apellido" class="input" />
          <input v-model="apellido2" type="text" placeholder="Segundo Apellido" class="input" />
          <input v-model="direccion" type="text" placeholder="Dirección" class="input" />
          <input v-model="correo" required type="email" placeholder="Correo" class="input" />
          <input v-model="telefono" required type="text" placeholder="Teléfono" class="input" />
          <input v-model="fechaNacimiento" required type="date" placeholder="Fecha de nacimiento" class="input" />
        </div>
  
        <!-- Usuario -->
        <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
          <input v-model="username" required type="text" placeholder="Nombre de usuario" class="input" />
        </div>
  
        <!-- Género -->
        <div>
          <label class="block font-semibold mb-2">Género:</label>
          <div class="flex gap-4">
            <label>
              <input type="radio" name="genero" value="M" v-model="genero" /> Masculino
            </label>
            <label>
              <input type="radio" name="genero" value="F" v-model="genero" /> Femenino
            </label>
          </div>
        </div>
  
        <!-- Contrato -->
        <div>
          <label class="block font-semibold mb-2">Contrato:</label>
          <div class="flex flex-wrap gap-4">
            <label>
              <input type="radio" name="contrato" value="Por Horas" v-model="contrato" /> Por Horas
            </label>
            <label>
              <input type="radio" name="contrato" value="Tiempo Completo" v-model="contrato" /> Tiempo Completo
            </label>
            <label>
              <input type="radio" name="contrato" value="Medio Tiempo" v-model="contrato" /> Medio Tiempo
            </label>
            <label>
              <input type="radio" name="contrato" value="Servicios Profesionales" v-model="contrato" /> Servicios Profesionales
            </label>
          </div>
        </div>
  
        <!-- Rol -->
        <div>
          <label class="block font-semibold mb-2">Rol:</label>
          <div class="flex gap-4">
            <label>
              <input type="radio" name="rol" value="Colaborador" v-model="rol" /> Colaborador
            </label>
            <label>
              <input type="radio" name="rol" value="Supervisor" v-model="rol" /> Supervisor
            </label>
            <label>
              <input type="radio" name="rol" value="Payroll" v-model="rol" /> Payroll
            </label>
          </div>
        </div>
  
        <!-- Empresa y Salario -->
        <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
          <input v-model="salarioBruto" required type="number" placeholder="Salario Bruto" class="input" />
          <input v-model="nombreEmpresa" required type="text" placeholder="Nombre Empresa" class="input" />
        </div>
  
        <!-- Botón -->
        <button type="submit" class="bg-blue-600 hover:bg-blue-700 text-white font-semibold py-2 px-4 rounded-xl w-full">
          Registrar Empleado
        </button>
      </form>
    </div>
  </template>
  
  <script setup>
import { v4 as uuidv4 } from "uuid";
import { ref } from "vue";
import axios from "axios";
import { useRouter } from "vue-router";

const router= useRouter();

// Función para obtener la fecha y hora formateada
function getFormattedDateTime() {
  const now = new Date();
  const day = String(now.getDate()).padStart(2, "0"); // Día del mes
  const month = String(now.getMonth() + 1).padStart(2, "0"); // Mes (1-12)
  const year = now.getFullYear(); // Año

  return `${day}/${month}/${year}`; // Formato DD/MM/AAAA
}

// Variables reactivas para el formulario
const cedula = ref(null);
const direccion = ref("");
const nombre = ref("");
const apellido1 = ref("");
const apellido2 = ref("");
const correo = ref("");
const telefono = ref("");
const fechaNacimiento = ref(""); // No se guarda, solo para generar contraseña
const username = ref("");
const genero = ref("");
const contrato = ref("");
const salarioBruto = ref("");
const rol = ref("");
const nombreEmpresa = ref("");

// const cedulaError = ref("");
// const direccionError = ref("");
// const nombreError = ref("");
// const apellido1Error = ref("");
// const apellido2Error = ref("");
// const correoError = ref("");
// const telefonoError = ref("");
// const fechaNacimientoError = ref("");
// const usernameError = ref("");
// const salarioBrutoError = ref("");
// const nombreEmpresaError = ref("");

// //Función para validar campos
// function validateFields() {
//   let valid = true;

//   // Reset errores
//   cedulaError.value = "";
//   direccionError.value = "";
//   nombreError.value = "";
//   apellido1Error.value = "";
//   apellido2Error.value = "";
//   correoError.value = "";
//   telefonoError.value = "";
//   fechaNacimientoError.value = "";
//   usernameError.value = "";
//   salarioBrutoError.value = "";
//   nombreEmpresaError.value = "";

//   if (!cedula.value || !/^\d{9}$/.test(cedula.value)) {
//     cedulaError.value = "Cédula inválida, debe tener 9 dígitos.";
//     valid = false;
//   }

//   if (!nombre.value) {
//     nombreError.value = "Nombre es obligatorio.";
//     valid = false;
//   }

//   if (!apellido1.value) {
//     apellido1Error.value = "Primer apellido es obligatorio.";
//     valid = false;
//   }

//   if (!correo.value || !/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(correo.value)) {
//     correoError.value = "Correo inválido.";
//     valid = false;
//   }

//   if (!telefono.value || !/^\d{4}-\d{4}$/.test(telefono.value)) {
//     telefonoError.value = "Teléfono inválido. El formato debe ser 1234-5678.";
//     valid = false;
//   }

//   if (!fechaNacimiento.value) {
//     fechaNacimientoError.value = "Fecha de nacimiento es obligatoria.";
//     valid = false;
//   }

//   if (!username.value || username.value.length > 30) {
//     usernameError.value = "Nombre de usuario requerido y máximo 30 caracteres.";
//     valid = false;
//   }

//   if (!salarioBruto.value || salarioBruto.value <= 0) {
//     salarioBrutoError.value = "Salario bruto debe ser mayor a 0.";
//     valid = false;
//   }

//   if (!nombreEmpresa.value) {
//     nombreEmpresaError.value = "Nombre de la empresa es obligatorio.";
//     valid = false;
//   }

//   return valid;
// }

// Función para registrar empleado
async function registrarEmpleado() {
//   if (!validateFields()) {
//     return;
//   }

  const uniqueId = uuidv4();
  const contraseña = `${apellido1.value.toLowerCase()}${new Date(fechaNacimiento.value).getFullYear()}`;
  const fechaIngreso = getFormattedDateTime();

  try {
    console.log("Enviando datos de Persona:");
    const requestPersona = {
        cedula: parseInt(cedula.value, 10),
        direccion: direccion.value.trim(),
        nombrePila: nombre.value.trim(),
        apellido1: apellido1.value.trim(),
        apellido2: apellido2.value.trim(),
        correo: correo.value.trim(),
        telefono: telefono.value.trim(),
};
    console.log("Request Persona:", requestPersona);
    try {
        const responsePersona = await axios.post("https://localhost:7014/api/Register/persona", requestPersona);
        console.log("Respuesta de Persona:", responsePersona.data);
    } catch (error) {
        console.error("Error al registrar persona:", error.response?.data || error.message);
    }

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

    const requestEmpleado = {
      id: uniqueId,
      cedulaPersona: cedula.value,
      contrato: contrato.value,
      numHorasTrabajadas: 0,
      genero: genero.value,
      estadoLaboral: "Activo",
      salarioBruto: salarioBruto.value,
      tipo: rol.value,
      fechaIngreso: fechaIngreso,
      nombreEmpresa: nombreEmpresa.value,
    };
    console.log("Request Empleado:", requestEmpleado);
    const responseEmpleado = await axios.post("https://localhost:7014/api/Register/empleado", requestEmpleado);
    console.log("Respuesta de Empleado:", responseEmpleado.data);

    alert("Empleado registrado exitosamente");
    router.push("/home");
  } catch (error) {
    console.error(error);
    alert("Error al registrar empleado");
  }
}
</script>
  
  <style scoped>
  .input {
    @apply p-2 border border-gray-300 rounded-xl w-full;
  }
  </style>
  