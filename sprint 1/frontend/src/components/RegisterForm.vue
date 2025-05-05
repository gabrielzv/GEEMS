<template>
    <div class="container mx-auto mt-8 p-4">
      <h2 class="text-2xl font-semibold mb-4">Formulario de Registro</h2>
      <form @submit.prevent="register">
        <div class="mb-4">
          <label for="nombreCompleto" class="block text-sm font-medium text-gray-700">Nombre Completo</label>
          <input
            type="text"
            id="nombreCompleto"
            v-model="nombreCompleto"
            :class="inputClass(nombreCompletoError)"
            placeholder="Nombre Completo"
          />
          <p v-if="nombreCompletoError" class="text-sm text-red-500">{{ nombreCompletoError }}</p>
        </div>
  
        <div class="mb-4">
          <label for="nombreUsuario" class="block text-sm font-medium text-gray-700">Nombre de Usuario</label>
          <input
            type="text"
            id="nombreUsuario"
            v-model="nombreUsuario"
            :class="inputClass(usernameError)"
            placeholder="Nombre de Usuario"
          />
          <p v-if="usernameError" class="text-sm text-red-500">{{ usernameError }}</p>
        </div>
  
        <div class="mb-4">
          <label for="correo" class="block text-sm font-medium text-gray-700">Correo Electrónico</label>
          <input
            type="email"
            id="correo"
            v-model="correo"
            :class="inputClass(emailError)"
            placeholder="Correo Electrónico"
          />
          <p v-if="emailError" class="text-sm text-red-500">{{ emailError }}</p>
        </div>
  
        <div class="mb-4">
          <label for="cedula" class="block text-sm font-medium text-gray-700">Cédula</label>
          <input
            type="text"
            id="cedula"
            v-model="cedula"
            :class="inputClass(cedulaError)"
            placeholder="Cédula"
          />
          <p v-if="cedulaError" class="text-sm text-red-500">{{ cedulaError }}</p>
        </div>
  
        <div class="mb-4">
          <label for="telefono" class="block text-sm font-medium text-gray-700">Teléfono</label>
          <input
            type="text"
            id="telefono"
            v-model="telefono"
            :class="inputClass(telefonoError)"
            placeholder="Teléfono"
          />
          <p v-if="telefonoError" class="text-sm text-red-500">{{ telefonoError }}</p>
        </div>
  
        <div class="mb-4">
          <label for="direccion" class="block text-sm font-medium text-gray-700">Dirección</label>
          <input
            type="text"
            id="direccion"
            v-model="direccion"
            :class="inputClass(direccionError)"
            placeholder="Dirección"
          />
          <p v-if="direccionError" class="text-sm text-red-500">{{ direccionError }}</p>
        </div>
  
        <div class="mb-4">
          <label for="contrasena" class="block text-sm font-medium text-gray-700">Contraseña</label>
          <input
            type="password"
            id="contrasena"
            v-model="contrasena"
            :class="inputClass(passwordError)"
            placeholder="Contraseña"
          />
          <p v-if="passwordError" class="text-sm text-red-500">{{ passwordError }}</p>
        </div>
  
        <div class="mb-4">
          <label for="confirmarContrasena" class="block text-sm font-medium text-gray-700">Confirmar Contraseña</label>
          <input
            type="password"
            id="confirmarContrasena"
            v-model="confirmarContrasena"
            :class="inputClass(confirmPasswordError)"
            placeholder="Confirmar Contraseña"
          />
          <p v-if="confirmPasswordError" class="text-sm text-red-500">{{ confirmPasswordError }}</p>
        </div>
  
        <div class="mb-4">
          <button
            :disabled="isSubmitting"
            class="w-full bg-blue-500 text-white py-2 px-4 rounded-lg"
          >
            {{ isSubmitting ? "Registrando..." : "Registrar" }}
          </button>
        </div>
        <p v-if="mensaje" :class="isSubmitting ? '' : 'text-green-500'">{{ mensaje }}</p>
      </form>
    </div>
  </template>
  
  <script>
  import { v4 as uuidv4 } from "uuid";
  import axios from "axios";
  
  export default {
    data() {
      return {
        nombreCompleto: "",
        nombreUsuario: "",
        correo: "",
        cedula: "",
        telefono: "",
        direccion: "",
        contrasena: "",
        confirmarContrasena: "",
        mensaje: "",
  
        // Variables divididas para el nombre
        nombrePila: "",
        apellido1: "",
        apellido2: "",
  
        // Errores
        nombreCompletoError: "",
        usernameError: "",
        emailError: "",
        cedulaError: "",
        telefonoError: "",
        direccionError: "",
        passwordError: "",
        confirmPasswordError: "",
  
        isSubmitting: false,
      };
    },
    methods: {
      inputClass(error) {
        return [
          "w-full px-4 py-2 rounded border focus:outline-none focus:ring-2",
          error ? "border-red-500 focus:ring-red-300" : "border-gray-300 focus:ring-blue-300",
        ];
      },
  
      validateEmail(email) {
        const regex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
        return regex.test(email);
      },
  
      validateFields() {
        let valid = true;
  
        // Reset errores
        this.nombreCompletoError = "";
        this.usernameError = "";
        this.emailError = "";
        this.cedulaError = "";
        this.telefonoError = "";
        this.direccionError = "";
        this.passwordError = "";
        this.confirmPasswordError = "";
  
        const partesNombre = this.nombreCompleto.trim().split(" ").filter(Boolean);
        if (partesNombre.length < 3) {
          this.nombreCompletoError = "Debe incluir nombre y dos apellidos.";
          valid = false;
        }
  
        if (!this.nombreUsuario || this.nombreUsuario.length > 30) {
          this.usernameError = "Nombre de usuario requerido y máximo 30 caracteres.";
          valid = false;
        }
  
        if (!this.correo || this.correo.length > 100 || !this.validateEmail(this.correo)) {
          this.emailError = "Correo requerido, válido y menor a 100 caracteres.";
          valid = false;
        }
  
        if (!/^\d{3}$/.test(this.cedula)) {
          this.cedulaError = "Cédula debe tener exactamente 3 dígitos.";
          valid = false;
        }
  
        if (!/^\d{4}-\d{4}$/.test(this.telefono)) {
          this.telefonoError = "Teléfono debe tener el formato XXXX-XXXX.";
          valid = false;
        }
  
        if (!this.direccion) {
          this.direccionError = "La dirección es obligatoria.";
          valid = false;
        }
  
        if (!this.contrasena) {
          this.passwordError = "La contraseña es obligatoria.";
          valid = false;
        }
  
        if (this.contrasena !== this.confirmarContrasena) {
          this.confirmPasswordError = "Las contraseñas no coinciden.";
          valid = false;
        }
  
        return valid;
      },
  
      dividirNombreCompleto() {
        const partes = this.nombreCompleto.trim().split(" ").filter(Boolean);
        this.nombrePila = partes.slice(0, -2).join(" ");
        this.apellido1 = partes[partes.length - 2] || "";
        this.apellido2 = partes[partes.length - 1] || "";
      },
  
      async register() {
        this.isSubmitting = true;
  
        // Validar campos antes de registrar
        if (!this.validateFields()) {
          this.isSubmitting = false;
          return;
        }
  
        // Generar UUID único
        const uniqueId = uuidv4(); // Este ID será el mismo para Persona y DuenoEmpresa
  
        // Llamamos a dividir el nombre completo
        this.dividirNombreCompleto();
  
        try {
          // Hacer las tres solicitudes POST por separado
          const responsePersona = await axios.post("https://localhost:7014/api/Register/persona", {
            cedula: this.cedula, // Convertimos la cédula a un número
            direccion: this.direccion,
            telefono: this.telefono,
            nombrePila: this.nombrePila,
            apellido1: this.apellido1,
            apellido2: this.apellido2,
            correo: this.correo,
            id: uniqueId, // Usamos el mismo UUID aquí
          });
          console.log("Respuesta de persona:", responsePersona.data);
  
          const responseUsuario = await axios.post("https://localhost:7014/api/Register/usuario", {
            id: uniqueId, // Usamos el mismo UUID aquí
            username: this.nombreUsuario,
            contrasena: this.contrasena,
            tipo: "DuenoEmpresa", // Define tipoUsuario según corresponda
            cedulaPersona: this.cedula, // Asegúrate de que también sea un número
          });
          console.log("Respuesta de usuario:", responseUsuario.data);
  
          const responseDuenoEmpresa = await axios.post("https://localhost:7014/api/Register/duenoempresa", {
            id: uniqueId, // Usamos el mismo UUID aquí
            cedulaEmpresa: 556, // Asegúrate de que sea un número
            cedulaPersona: this.cedula, // Asegúrate de que sea un número
          });
          console.log("Respuesta de dueño de empresa:", responseDuenoEmpresa.data);
  
          // Manejo de la respuesta final
          this.mensaje = "Registro exitoso."; // O lo que desees mostrar
          this.$router.push("/login"); // Redirige al login si es necesario
        } catch (error) {
          console.error("Error durante el registro:", error);
          this.mensaje = "Error al registrarse.";
        } finally {
          this.isSubmitting = false;
        }
      },
    },
  };
  </script>
  
  <style scoped>
  /* Puedes agregar estilos adicionales aquí */
  </style>
  