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
                    <p v-if="errores.diaRepetido" class="text-sm text-red-500 mt-1">{{ errores.diaRepetido }}</p>
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
import { API_BASE_URL } from "../config";
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
            const url = `${API_BASE_URL}Horas/Register`;
            try {
                const response = await axios.get(url, {
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

                if(!(this.registroAnterior.Estado === "NoRevisado")){
                    alert("No puede editar este registro ya que fue "+this.registroAnterior.Estado+".");
                    this.$router.push("/home");
                    return;
                }
            } catch {
                alert("No se pudieron obtener los datos anteriores del registro.");
                this.$router.push("/home");
            }
        },
        async getEmpleadoId() {
            const url = `${API_BASE_URL}GetEmpleado/${this.cedulaPersona}`;
            try {
                const response = await axios.get(url);
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
        async fechaRepetida() {
            const url = `${API_BASE_URL}Horas`;
            try {
                const response = await axios.get(url, {
                    params: {
                        date: this.diaRegistrado,
                        employeeId: this.guidEmpleado,
                    },
                });
                this.errores.diaRepetido = "";

                const fechaSeleccionada = this.diaRegistrado;
                const fechaAnterior = this.registroAnterior.Fecha ? this.registroAnterior.Fecha.split('T')[0] : null;
                // Si la fecha YA existe (response.data === false) y NO es la misma que la anterior, marca error
                if (response.data === false && fechaSeleccionada !== fechaAnterior) {
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
            // Validar con el API si las horas mensuales superan 160
            try {
                // Calcular las horas que se van a agregar a la base, se descuentan las anteriores de las actuales
                const horasNuevas = horas - Number(this.registroAnterior.NumHoras);
                console.log(horasNuevas, " Horas nuevas valor");
                const url = `${API_BASE_URL}Horas/ValidHours`;
                const response = await axios.get(url, {
                    params: {
                        date: this.diaRegistrado,
                        employeeId: this.guidEmpleado,
                        hours: horasNuevas
                    }
                });
                console.log("Se tiene response.data de horas invalidas en: ", response.data);
                if (response.data == false) {
                    this.errores.horasInvalidas = "No se pueden registrar más de 160 horas en el mes.";
                }
            } catch (error) {
                console.error("Error al validar las horas mensuales:", error);
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
            await this.fechaValida();
            await this.fechaRepetida();
            await this.horasValidas();
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
                    const url = `${API_BASE_URL}Horas/Editar?oldId=${this.registroAnterior.Id}`;
                    await axios.post(
                        url,
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