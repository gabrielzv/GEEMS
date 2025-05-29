<template>
    <div class="min-h-screen flex items-center justify-center bg-gray-100 px-4">
        <form
            @submit.prevent="registroHoras"
            novalidate
            class="bg-white p-6 rounded-xl shadow-md w-full max-w-md space-y-5"
        >
            <p class="text-3xl font-bold text-center text-gray-800">
                Registro de Horas
            </p>
            
            <div class="grid grid-cols-1 gap-4">
                <div>
                    <label for="dia" class="block text-sm font-medium text-gray-700">
                        Día a registrar
                    </label>
                    <input 
                        type="date"
                        id="dia"
                        v-model="diaRegistrado"
                        :class="inputClass(errores.diaInvalido || errores.diaRepetido)"
                    />
                    <p v-if="errores.diaInvalido" class="text-sm text-red-500 mt-1">{{ errores.diaInvalido }}</p>
                    <p v-if="errores.diaRepetido && !errores.diaInvalido" class="text-sm text-red-500 mt-1">{{ errores.diaRepetido }}</p>
                </div>
                <div>
                    <label for="nombre" class="block text-sm font-medium text-gray-700">
                        Horas a Registrar
                    </label>
                    <input
                        type="text"
                        id="nombre"
                        v-model="horasRegistradas"
                        maxlength="1"
                        placeholder="8"
                        :class="inputClass(errores.horasVacias || errores.horasInvalidas)"
                    />
                    <p v-if="errores.horasVacias" class="text-sm text-red-500 mt-1">{{ errores.horasVacias }}</p>
                    <p v-if="errores.horasInvalidas && !errores.horasVacias" class="text-sm text-red-500 mt-1">{{ errores.horasInvalidas }}</p>
                </div>
            </div>

            <button
                type="submit"
                class="w-full bg-blue-600 hover:bg-blue-700 text-white font-semibold py-2 px-4 rounded transition-colors">
                Registrar
            </button>

        </form>
    </div>
</template>

<script>
import axios from "axios";

export default {
    data() {
        return {
            horasRegistradas: null,
            diaRegistrado: null,
            guidEmpleado: "123e4567-e89b-12d3-a456-426614174000",
            errores: {
                horasVacias: "",
                horasInvalidas: "",
                diaRepetido: "",
                diaInvalido: "",
            }
        };
    },
    created() {
        
    },
    methods: {
        inputClass(error) {
            return [
                "w-full px-4 py-2 rounded border focus:outline-none focus:ring-2",
                error ? "border-red-500 focus:ring-red-300" : "border-gray-300 focus:ring-blue-300",
            ];
        },
        diaRepetido(){
            // TODO: Comprobar con backend que no este repetido
            return true;
        },
        diaInvalido(){
            // TODO: Comprobar con backend que no sea valido
            return true;
        },
        registroValido(){
            this.errores.horasVacias = this.horasRegistradas != null ? "" : "Las horas a registrar son obligatorias.";
            this.errores.horasInvalidas = "";
            if(this.horasRegistradas < 1 || this.horasRegistradas > 8){
                this.errores.horasInvalidas = "Las horas a registrar deben estar en el rango de 1 a 8 horas.";
            }
            this.errores.diaRepetido = "";
            if(this.diaRepetido()){
                this.errores.diaRepetido = "El registro de horas para este día ya se realizó.";
            }
            this.errores.diaInvalido = "";
            if(this.diaInvalido()){
                this.errores.diaInvalido = "El día seleccionado no es válido, ya se realizó el pago de este día.";
            }

            let registroValido =  !(this.errores.horasVacias    ||
                            this.errores.horasInvalidas ||
                            this.errores.diaRepetido    ||
                            this.errores.diaInvalido);
            
            return registroValido;
        },
        async registroHoras() {
            if(this.registroValido()){
                const registroPayload = {
                    horasRegistradas: this.horasRegistradas,
                    diaRegistrado: this.diaRegistrado,
                    guidEmpleado: this.guidEmpleado,
                };

                try {
                    // TODO: Quitar este if que desactiva el linter
                    let linter = false;
                    if(linter){
                        console.log(this.guidEmpleado); // TODO: Quitar este log
                        await axios.post( // TODO: Colocar el link al API correcto
                        "https://localhost:7014/api/RegistroHoras",
                        registroPayload
                        );
                    }
                    alert("Registro de horas realizado correctamente.");
                    this.$router.push("/home");
                }
                catch (error) {
                    alert ("Ocurrió un error al realizar el registro de horas.");
                }
            }
        },
    }
}
</script>

<style scoped></style>