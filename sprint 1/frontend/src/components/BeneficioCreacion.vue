<template>
    <div class="flex items-center justify-center min-h-screen px-4">
      <div class="w-full max-w-xl bg-white p-6 rounded-lg shadow-lg">
        <h1 class="text-center text-2xl font-bold mb-4">Crear Beneficio</h1>
        <form @submit.prevent="crearBeneficio">
          <div class="mb-4">
            <label for="nombre" class="block font-medium mb-1">Nombre del Beneficio</label>
            <input type="text" id="nombre" v-model="nombre" required class="w-full border rounded px-3 py-2">
          </div>
  
          <div class="mb-4">
            <label for="descripcion" class="block font-medium mb-1">Descripción</label>
            <textarea id="descripcion" v-model="descripcion" required class="w-full border rounded px-3 py-2"></textarea>
          </div>
  
          <div class="mb-4">
            <label for="TiempoMinimoRequerido" class="block font-medium mb-1">Tiempo mínimo requerido</label>
            <input type="date" id="TiempoMinimoRequerido" v-model="TiempoMinimoRequerido" required class="w-full border rounded px-3 py-2">
          </div>
  
          <div class="mb-4">
            <label for="coste" class="block font-medium mb-1">Coste:</label>
            <input type="text" id="coste" v-model="precio" @input="formatearPrecio">
          </div>
  
            <div class="mb-4">
                <label class="block font-medium mb-1">Tipo de empleado elegible</label>
                <div class="grid grid-cols-2 gap-x-4 gap-y-2">
                    <label class="flex items-center">
                    <input type="checkbox" v-model="empleadoElegible" value="Tiempo Completo" class="mr-2">
                    Tiempo Completo
                    </label>
                    <label class="flex items-center">
                    <input type="checkbox" v-model="empleadoElegible" value="Medio Tiempo" class="mr-2">
                    Medio Tiempo
                    </label>
                    <label class="flex items-center">
                    <input type="checkbox" v-model="empleadoElegible" value="Servicios Profesionales" class="mr-2">
                    Servicios Profesionales
                    </label>
                    <label class="flex items-center">
                    <input type="checkbox" v-model="empleadoElegible" value="Por Horas" class="mr-2">
                    Por Horas
                    </label>
                </div>
            </div>
    
          <p class="mb-4 text-sm text-gray-600">Seleccionado: {{ empleadoElegible.join(', ') }}</p>
  
          <div class="mb-4">
            <label for="tipoBeneficio" class="block font-medium mb-1">Tipo de beneficio</label>
            <input type="text" id="tipoBeneficio" v-model="tipoBeneficio" required class="w-full border rounded px-3 py-2">
          </div>
  
          <button type="submit" class="w-full bg-blue-600 text-white py-2 rounded hover:bg-blue-700 transition">Crear Beneficio</button>
        </form>
      </div>
    </div>
</template>
  
  
<script>

export default {
    data() {
        return {
            nombre: "",
            descripcion: "",
            TiempoMinimoRequerido: "",
            coste: "",
            empleadoElegible: [],
            tipoBeneficio: ""
        };
    },
    methods: {
        crearBeneficio() {
            console.log("Beneficio creado:", {
                nombre: this.nombre,
                descripcion: this.descripcion,
                TiempoMinimoRequerido: this.TiempoMinimoRequerido,
                coste: this.coste,
                empleadoElegible: this.empleadoElegible,
                tipoBeneficio: this.tipoBeneficio
            });
        }
    }
};
</script>

  
<style lang="css" scoped>

</style>