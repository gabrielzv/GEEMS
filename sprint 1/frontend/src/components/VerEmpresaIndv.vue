<template>
    <div class="min-h-screen bg-gray-100 p-6">
      <div class="max-w-5xl mx-auto bg-white p-6 rounded-xl shadow-md">
        <!-- Encabezado -->
        <div class="flex justify-between items-center mb-6">
          <h1 class="text-3xl font-bold text-gray-800">Nombre de empresa</h1>
          <div class="text-center bg-white px-4 py-2 rounded-full shadow">
            <p class="text-lg font-semibold text-gray-800">21</p>
            <p class="text-sm text-gray-600">Pagos Pendientes</p>
          </div>
        </div>
  
        <!-- Información de la empresa -->
        <div class="grid grid-cols-2 gap-6">
          <!-- Columna izquierda -->
          <div>
            <p class="text-lg font-bold text-gray-800">Nombre:</p>
            <p class="text-gray-700 mb-4">Nombre de empresa</p>
  
            <p class="text-lg font-bold text-gray-800">Cédula jurídica:</p>
            <p class="text-gray-700 mb-4">X-XXXX-XXXX</p>
  
            <p class="text-lg font-bold text-gray-800">Dirección:</p>
            <p class="text-gray-700">
              <span class="block">Provincia: San José</span>
              <span class="block">Cantón: San Pedro</span>
              <span class="block">Distrito: Montes de Oca</span>
              <span class="block">Señas: 50mts sureste entrada Derecho</span>
            </p>
          </div>
  
          <!-- Columna derecha -->
          <div>
            <p class="text-lg font-bold text-gray-800">Teléfono:</p>
            <p class="text-gray-700 mb-4">5555-6398</p>
  
            <p class="text-lg font-bold text-gray-800">Correo:</p>
            <p class="text-gray-700 mb-4">ej@geems.com</p>
          </div>
        </div>
  
        <!-- Lista de empleados -->
        <div class="mt-6">
          <p class="text-lg font-bold text-gray-800 mb-2">Empleados:</p>
          <div class="bg-white p-4 rounded-xl shadow-md max-h-40 overflow-y-auto">
            <ul>
              <li
                v-for="empleado in empleados"
                :key="empleado.id"
                class="flex justify-between items-center mb-4 border-b pb-2"
              >
                <span class="text-gray-700">{{ empleado.nombre }}</span>
                <div class="flex space-x-2">
                  <button
                    class="bg-blue-500 text-white px-3 py-1 rounded hover:bg-blue-600"
                  >
                    Modificar
                  </button>
                  <button
                    class="bg-red-500 text-white px-3 py-1 rounded hover:bg-red-600"
                  >
                    Eliminar
                  </button>
                </div>
              </li>
            </ul>
          </div>
        </div>
  
        <!-- Botones de acción -->
        <div class="flex justify-between mt-6">
          <button
            class="bg-blue-600 text-white font-semibold py-2 px-4 rounded hover:bg-blue-700"
          >
            Modificar
          </button>
          <button
            class="bg-red-600 text-white font-semibold py-2 px-4 rounded hover:bg-red-700"
          >
            Eliminar
          </button>
        </div>
      </div>
    </div>
  </template>
  
  <script>
  export default {
    data() {
      return {
        empleados: [
          { id: 1, nombre: "Kim Dotcom Soto" },
          { id: 2, nombre: "Mainor Castro Vargas" },
          { id: 3, nombre: "Oscar Hernández" },
          { id: 4, nombre: "Willy Wonka Morales" },
          { id: 5, nombre: "Jane Doe" },
          { id: 6, nombre: "John Smith" },
          { id: 7, nombre: "Alice Wonderland" },
          { id: 8, nombre: "Bob Builder" },
          { id: 9, nombre: "Charlie Brown" },
          { id: 10, nombre: "Diana Prince" },
        ], // Lista de empleados de ejemplo
      };
    },
  };
  </script>
  
  <style scoped>
  /* Puedes agregar estilos personalizados aquí */
  </style>