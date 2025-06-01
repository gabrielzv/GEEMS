<template>
    <div class="min-h-screen flex items-center justify-center bg-gray-100 px-4">
        <form
            @submit.prevent="editarHoras"
            novalidate
            class="bg-white p-6 rounded-xl shadow-md w-full max-w-md space-y-5"
        >
            <p class="text-3xl font-bold text-center text-gray-800">
                Editar registro de horas
            </p>
            
            <div class="bg-gray-50 rounded-lg p-4 mb-4 text-sm text-gray-700 shadow-inner">
                <p><strong>Día anterior:</strong> {{ fechaAnteriorFormateada }}</p>
                <p><strong>Horas anteriores:</strong> {{ registroAnterior.NumHoras }}</p>
            </div>

            <div class="grid grid-cols-1 gap-4">
                <div>
                    <label for="dia" class="block text-sm font-medium text-gray-700">
                        Día a registrar
                    </label>
                    <input 
                        type="date"
                        id="dia"
                        v-model="diaRegistrado"
                        :class="getInputClass(errores.diaInvalido || errores.diaRepetido)"
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
                        maxlength="2"
                        placeholder="8"
                        :class="getInputClass(errores.horasVacias || errores.horasInvalidas)"
                    />
                    <p v-if="errores.horasVacias" class="text-sm text-red-500 mt-1">{{ errores.horasVacias }}</p>
                    <p v-if="errores.horasInvalidas && !errores.horasVacias" class="text-sm text-red-500 mt-1">{{ errores.horasInvalidas }}</p>
                </div>
            </div>

            <button
                type="submit"
                class="w-full bg-blue-600 hover:bg-blue-700 text-white font-semibold py-2 px-4 rounded transition-colors">
                Editar registro
            </button>

        </form>
    </div>
</template>

<script>
import axios from "axios";
import { useUserStore } from "../store/user";

export default {
    data() {
        return {
            horasRegistradas: null,
            diaRegistrado: null,
            guidEmpleado: null,
            cedulaPersona: null,
            errores: {
                horasVacias: "",
                horasInvalidas: "",
                diaRepetido: "",
                diaInvalido: "",
            },
            registroAnterior: {
                Id: null,
                NumHoras: null,
                Fecha: null,
                Estado: null,
                IdEmpleado: null
            }
        };
    },
    async created() {
        const userStore = useUserStore();
        if (userStore.usuario && userStore.usuario.cedulaPersona) {
            this.cedulaPersona = userStore.usuario.cedulaPersona;
            await this.getEmpleadoId();
        }
        this.registroAnterior.Id = this.$route.params.registroId;
        await this.getRegistroAnterior();
    },
    computed: {
        fechaAnteriorFormateada() {
            if (this.registroAnterior.Fecha) {
                const [year, month, day] = this.registroAnterior.Fecha.split('T')[0].split('-');
                return `${day}-${month}-${year}`;
            }
            return '';
        }
    },
    methods: {
        async getRegistroAnterior() {
            try {
                const response = await axios.get("https://localhost:7014/api/Horas/Register", {
                    params: {
                        Id: this.registroAnterior.Id,
                    },
                });

                if (!response.data || Object.keys(response.data).length === 0) {
                    alert("No se encontró este registro.");
                    this.$router.push("/home");
                    return;
                }

                this.registroAnterior.NumHoras = response.data.numHoras;
                this.registroAnterior.Fecha = response.data.fecha;
                this.registroAnterior.Estado = response.data.estado;
                this.registroAnterior.IdEmpleado = response.data.idEmpleado;

                if (this.registroAnterior.IdEmpleado !== this.guidEmpleado) {
                    alert("No puede editar este registro porque no le pertenece.");
                    this.$router.push("/home");
                    return;
                }
            } catch {
                alert("No se pudieron obtener los datos anteriores del registro.");
                this.$router.push("/home");
            }
        },
        async getEmpleadoId() {
            try {
                const response = await axios.get(`https://localhost:7014/api/GetEmpleado/${this.cedulaPersona}`);
                this.guidEmpleado = response.data.id;
            } catch (error) {
                alert("No se pudo obtener el ID del empleado.");
            }
        },
        getInputClass(error) {
            return [
                "w-full px-4 py-2 rounded border focus:outline-none focus:ring-2",
                error ? "border-red-500 focus:ring-red-300" : "border-gray-300 focus:ring-blue-300",
            ];
        },
        async fechaRepetida(){
            try {
                const response = await axios.get("https://localhost:7014/api/Horas", {
                    params: {
                        date: this.diaRegistrado,
                        employeeId: this.guidEmpleado,
                    },
                });
                this.errores.diaRepetido = "";
                const fechaSeleccionada = new Date(this.diaRegistrado);
                const fechaSeleccionadaAnterior = new Date(this.registroAnterior.Fecha)
                if(!response.data && fechaSeleccionada != fechaSeleccionadaAnterior){
                    this.errores.diaRepetido = "El registro de horas para este día ya se realizó.";
                }
            } catch (error) {
                alert("Error al validar la fecha:", error);
            }
        },
        async horasValidas(){
            this.errores.horasVacias = this.horasRegistradas != null ? "" : "Las horas a registrar son obligatorias.";
            this.errores.horasInvalidas = "";
            const horas = Number(this.horasRegistradas);
            if (
                isNaN(horas) ||
                !Number.isFinite(horas) ||
                horas < 1 ||
                horas > 24
            ) {
                this.errores.horasInvalidas = "Las horas a registrar deben ser un número entre 1 y 24.";
            }
        },
        async fechaValida() {
            this.errores.diaInvalido = "";
            if (!this.diaRegistrado) {
                this.errores.diaInvalido = "Debe seleccionar una fecha válida.";
                return;
            }
            const fechaSeleccionada = new Date(this.diaRegistrado);
            if (
                isNaN(fechaSeleccionada.getTime()) ||
                this.diaRegistrado.length !== 10
            ) {
                this.errores.diaInvalido = "Debe seleccionar una fecha válida.";
                return;
            }
            const hoy = new Date();
            hoy.setHours(0, 0, 0, 0);
            if (fechaSeleccionada > hoy) {
                this.errores.diaInvalido = "No se puede registrar horas en una fecha futura.";
                return;
            }
            const fechaMinima = new Date("2000-01-01");
            if (fechaSeleccionada < fechaMinima) {
                this.errores.diaInvalido = "No se puede registrar horas en fechas anteriores al año 2000.";
                return;
            }
        },
        async registroValido(){
            this.fechaValida();
            this.fechaRepetida();
            this.horasValidas();
            return !(this.errores.horasVacias || this.errores.horasInvalidas || 
                     this.errores.diaRepetido || this.errores.diaInvalido);
        },
        async editarHoras() {
            if(await this.registroValido()){
                const registroPayload = {
                    NumHoras: this.horasRegistradas,
                    Fecha: this.diaRegistrado,
                    IdEmpleado: this.guidEmpleado,
                    Estado: "NoRevisado",
                };
                try {
                    console.log("SE ENVIA AL BACKEND EL GUID OLD ID: "+this.registroAnterior.Id);
                    await axios.post(
                        `https://localhost:7014/api/Horas/Editar?oldId=${this.registroAnterior.Id}`,
                        registroPayload
                    );
                    alert("Edición de horas realizada correctamente.");
                    this.$router.push("/home");
                }
                catch (error) {
                    alert ("Ocurrió un error al realizar la edición de horas.");
                }
            }
        },
    }
}
</script>

<style scoped></style>