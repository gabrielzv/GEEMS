<template>
    <div class="min-h-screen flex items-center justify-center bg-gray-100 px-4">
      <form
        @submit.prevent="registrarEmpresa"
        novalidate
        class="bg-white p-6 rounded-xl shadow-md w-full max-w-4xl space-y-5"
      >
        <p class="text-3xl font-bold text-center text-gray-800">
          Registrar nueva empresa
        </p>
  
        <div class="grid grid-cols-2 gap-4">
          <!-- Nombre de la compañía -->
          <div>
            <label for="nombre" class="block text-sm font-medium text-gray-700">
              Nombre de la compañía
            </label>
            <input
              type="text"
              id="nombre"
              v-model="empresa.nombre"
              placeholder=""
              class="w-full px-4 py-2 rounded border border-gray-300 focus:outline-none focus:ring-2 focus:ring-blue-300"
            />
          </div>
  
          <!-- Teléfono -->
          <div>
            <label for="telefono" class="block text-sm font-medium text-gray-700">
              Teléfono
            </label>
            <input
              type="tel"
              id="telefono"
              v-model="empresa.telefono"
              placeholder="XXXX-XXXX"
              class="w-full px-4 py-2 rounded border border-gray-300 focus:outline-none focus:ring-2 focus:ring-blue-300"
            />
          </div>
  
          <!-- Cédula jurídica -->
          <div>
            <label for="cedula" class="block text-sm font-medium text-gray-700">
              Cédula jurídica
            </label>
            <input
              type="text"
              id="cedula"
              v-model="empresa.cedula"
              placeholder="X-XXX-XXXXXXX"
              class="w-full px-4 py-2 rounded border border-gray-300 focus:outline-none focus:ring-2 focus:ring-blue-300"
            />
          </div>
  
          <!-- Correo -->
          <div>
            <label for="correo" class="block text-sm font-medium text-gray-700">
              Correo
            </label>
            <input
              type="email"
              id="correo"
              v-model="empresa.correo"
              placeholder="XXX@XXXX.XXX"
              class="w-full px-4 py-2 rounded border border-gray-300 focus:outline-none focus:ring-2 focus:ring-blue-300"
            />
          </div>
  
          <!-- Descripción -->
          <div class="col-span-2">
            <label
              for="descripcion"
              class="block text-sm font-medium text-gray-700"
            >
              Descripción
            </label>
            <textarea
              id="descripcion"
              v-model="empresa.descripcion"
              placeholder=""
              rows="3"
              class="w-full px-4 py-2 rounded border border-gray-300 focus:outline-none focus:ring-2 focus:ring-blue-300"
            ></textarea>
          </div>
  
          <!-- Dirección -->
          <div>
            <label for="provincia" class="block text-sm font-medium text-gray-700">
              Provincia
            </label>
            <input
              type="text"
              id="provincia"
              v-model="empresa.direccion.provincia"
              placeholder=""
              class="w-full px-4 py-2 rounded border border-gray-300 focus:outline-none focus:ring-2 focus:ring-blue-300"
            />
          </div>
  
          <div>
            <label for="canton" class="block text-sm font-medium text-gray-700">
              Cantón
            </label>
            <input
              type="text"
              id="canton"
              v-model="empresa.direccion.canton"
              placeholder=""
              class="w-full px-4 py-2 rounded border border-gray-300 focus:outline-none focus:ring-2 focus:ring-blue-300"
            />
          </div>
  
          <div>
            <label for="distrito" class="block text-sm font-medium text-gray-700">
              Distrito
            </label>
            <input
              type="text"
              id="distrito"
              v-model="empresa.direccion.distrito"
              placeholder=""
              class="w-full px-4 py-2 rounded border border-gray-300 focus:outline-none focus:ring-2 focus:ring-blue-300"
            />
          </div>
  
          <div>
            <label for="senas" class="block text-sm font-medium text-gray-700">
              Señas
            </label>
            <input
              type="text"
              id="senas"
              v-model="empresa.direccion.senas"
              placeholder=""
              class="w-full px-4 py-2 rounded border border-gray-300 focus:outline-none focus:ring-2 focus:ring-blue-300"
            />
          </div>
        </div>
  
        <button
          type="submit"
          class="w-full bg-blue-600 hover:bg-blue-700 text-white font-semibold py-2 px-4 rounded transition-colors"
        >
          Registrar
        </button>
      </form>
    </div>
  </template>
  
  <script>
  export default {
    data() {
      return {
        empresa: {
          nombre: "",
          telefono: "",
          cedula: "",
          correo: "",
          descripcion: "",
          direccion: {
            provincia: "",
            canton: "",
            distrito: "",
            senas: "",
          },
        },
      };
    },
    methods: {
      registrarEmpresa() {
        console.log("Datos de la empresa:", this.empresa);
        alert("Datos registrados localmente. Revisa la consola.");
      },
    },
  };
  </script>
  
  <style scoped>
  /* Puedes agregar estilos personalizados aquí */
  </style>