<template>
    <div class="p-4">
      <button
        @click="probarPost"
        class="bg-blue-500 hover:bg-blue-600 text-white font-bold py-2 px-4 rounded"
      >
        Probar POST Persona
      </button>
  
      <div v-if="mensaje" class="mt-4 text-green-600">{{ mensaje }}</div>
      <div v-if="error" class="mt-4 text-red-600">{{ error }}</div>
      <div v-if="respuesta" class="mt-4">
        <h3 class="font-semibold">Respuesta del servidor:</h3>
        <pre>{{ respuesta }}</pre>
      </div>
    </div>
  </template>
  
  <script>
  import axios from "axios";
  
  export default {
    name: "PruebaPost", // Cambia el nombre aquí
    data() {
      return {
        mensaje: "",
        error: "",
        respuesta: null,
      };
    },
    methods: {
      async probarPost() {
        this.mensaje = "";
        this.error = "";
        this.respuesta = null;
  
        try {
          const response = await axios.post("https://localhost:7014/api/Register/persona", {
            Cedula: 200,
            NombrePila: "kyrian",
            Apellido1: "Pérez",
            Apellido2: "Gómez",
            Correo: "kyrian.perez@example.com",
            Telefono: "8888-8888",
            Direccion: "San José, Costa Rica"
          });
  
          this.mensaje = "POST enviado correctamente.";
          this.respuesta = response.data;
        } catch (err) {
          this.error = "Error al enviar POST.";
          console.error(err);
        }
      },
    },
  };
  </script>