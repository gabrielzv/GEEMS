<template>
  <div class="flex justify-center items-center min-h-screen bg-gray-100">
    <form class="bg-white shadow rounded p-6 w-full max-w-md space-y-4">
      <h1 class="text-2xl font-bold mb-4 text-center">
        Creación de Beneficios
      </h1>
      <!-- APIs disponibles -->
      <div>
        <label class="block text-sm font-medium text-gray-700 mb-1"
          >Beneficios nacionales:</label
        >
        <div class="flex flex-col gap-3 items-center">
          <div class="flex gap-6 text-sm justify-center">
            <label class="flex items-center">
              <input
                type="radio"
                name="apis"
                value="BeneficioNormal"
                v-model="form.nombreDeAPI"
                class="mr-2"
                @change="handleApiSelection"
              />
              No seleccionado
            </label>
            <label class="flex items-center">
              <input
                type="radio"
                name="apis"
                value="Poliza Seguros"
                v-model="form.nombreDeAPI"
                class="mr-2"
                @change="handleApiSelection"
              />
              Póliza de Seguros
            </label>
            <label class="flex items-center">
              <input
                type="radio"
                name="apis"
                value="Asociacion Calculator"
                v-model="form.nombreDeAPI"
                class="mr-2"
                @change="handleApiSelection"
              />
              Asociación Solidarista
            </label>
            <label class="flex items-center">
              <input
                type="radio"
                name="apis"
                value="MediSeguro"
                v-model="form.nombreDeAPI"
                class="mr-2"
                @change="handleApiSelection"
              />
              Seguro Médico
            </label>
          </div>
        </div>
        <p v-if="seleccionApisError" class="text-sm text-red-500 mt-1">
          {{ seleccionApisError }}
        </p>
      </div>
      <!-- Nombre -->
      <div>
        <label class="block text-sm font-medium text-gray-700 mb-1"
          >Nombre:</label
        >
        <input
          v-model="form.nombre"
          type="text"
          placeholder="Ej: Gimnasio de la empresa"
          @blur="validateNombre"
          @input="validateNombre"
          :class="inputClass(nombreError)"
          maxlength="32"
        />
        <p v-if="nombreError" class="text-sm text-red-500 mt-1">
          {{ nombreError }}
        </p>
      </div>
      <!-- Descripción -->
      <div>
        <label class="block text-sm font-medium text-gray-700 mb-1"
          >Descripción:</label
        >
        <textarea
          v-model="form.descripcion"
          rows="2"
          placeholder="Ej: Este beneficio incluye acceso a un gimnasio local, clases de yoga y pilates."
          @blur="validateDescripcion"
          @input="validateDescripcion"
          :class="inputClass(descripcionError)"
          maxlength="200"
        ></textarea>
        <p v-if="descripcionError" class="text-sm text-red-500 mt-1">
          {{ descripcionError }}
        </p>
      </div>
      <!-- Tipo de deducción (Regular o Porcentual) -->
      <div>
        <label class="block text-sm font-medium text-gray-700 mb-1"
          >Tipo de deducción:</label
        >
        <div class="flex flex-col gap-3 items-center">
          <div class="flex gap-6 text-sm justify-center">
            <label class="flex items-center">
              <input
                type="radio"
                name="deducciones"
                :value="false"
                v-model="form.esPorcentual"
                class="mr-2"
                @change="validateDeduction"
              />
              Regular
            </label>
            <label class="flex items-center">
              <input
                type="radio"
                name="deducciones"
                :value="true"
                v-model="form.esPorcentual"
                class="mr-2"
                @change="validateDeduction"
              />
              Porcentual
            </label>
          </div>
        </div>
        <p v-if="seleccionDeduccionError" class="text-sm text-red-500 mt-1">
          {{ seleccionDeduccionError }}
        </p>
      </div>
      <!-- Costo -->
      <div>
        <label class="block text-sm font-medium text-gray-700 mb-1"
          >Costo / Porcentaje:</label
        >
        <input
          v-model="form.costo"
          type="float"
          min="0"
          placeholder="Ej: 12000 / 5.5"
          @blur="validateCosto"
          :class="inputClass(costoError)"
        />
        <p v-if="costoError" class="text-sm text-red-500 mt-1">
          {{ costoError }}
        </p>
      </div>
      <!-- Tiempo Mínimo en Empresa -->
      <div>
        <label class="block text-sm font-medium text-gray-700 mb-1"
          >Tiempo Mínimo en Empresa (meses):</label
        >
        <input
          v-model="form.tiempoMinimo"
          type="number"
          min="0"
          placeholder="Ej: 6"
          @blur="validateTiempoMinimo"
          :class="inputClass(tiempoMinimoError)"
        />
        <p v-if="tiempoMinimoError" class="text-sm text-red-500 mt-1">
          {{ tiempoMinimoError }}
        </p>
      </div>
      <!-- Frecuencia -->
      <div>
        <label class="block text-sm font-medium text-gray-700 mb-1"
          >Frecuencia:</label
        >
        <div class="flex gap-4 text-sm">
          <label class="flex items-center">
            <input
              type="radio"
              name="contrato"
              value="Mensual"
              v-model="form.frecuencia"
              class="mr-2"
              @change="validateFrecuencia"
            />
            Mensual
          </label>
          <label class="flex items-center">
            <input
              type="radio"
              name="contrato"
              value="Semanal"
              v-model="form.frecuencia"
              class="mr-2"
              @change="validateFrecuencia"
            />
            Semanal
          </label>
          <label class="flex items-center">
            <input
              type="radio"
              name="contrato"
              value="Trimestral"
              v-model="form.frecuencia"
              class="mr-2"
              @change="validateFrecuencia"
            />
            Trimestral
          </label>
          <label class="flex items-center">
            <input
              type="radio"
              name="contrato"
              value="Único"
              v-model="form.frecuencia"
              class="mr-2"
              @change="validateFrecuencia"
            />
            Único
          </label>
        </div>
        <p v-if="frecuenciaError" class="text-sm text-red-500 mt-1">
          {{ frecuenciaError }}
        </p>
      </div>
      <!-- Contratos Elegibles -->
      <div>
        <label class="block text-sm font-medium text-gray-700 mb-1"
          >Contratos elegibles:</label
        >
        <div
          class="grid grid-cols-2 md:grid-cols-4 gap-4 text-sm place-items-center"
        >
          <label class="flex items-center">
            <input
              type="checkbox"
              value="Tiempo Completo"
              v-model="form.contratosElegibles"
              class="mr-2"
              @change="validateContratosElegibles"
            />
            Tiempo completo
          </label>
          <label class="flex items-center">
            <input
              type="checkbox"
              value="Medio Tiempo"
              v-model="form.contratosElegibles"
              class="mr-2"
              @change="validateContratosElegibles"
            />
            Medio tiempo
          </label>
          <label class="flex items-center">
            <input
              type="checkbox"
              value="Servicios Profesionales"
              v-model="form.contratosElegibles"
              class="mr-2"
              @change="validateContratosElegibles"
            />
            Servicios profesionales
          </label>
          <label class="flex items-center">
            <input
              type="checkbox"
              value="Por Horas"
              v-model="form.contratosElegibles"
              class="mr-2"
              @change="validateContratosElegibles"
            />
            Por horas
          </label>
        </div>
        <p v-if="contratosElegiblesError" class="text-sm text-red-500 mt-1">
          {{ contratosElegiblesError }}
        </p>
      </div>

      <!-- Botón -->
      <div class="flex justify-center pt-2">
        <button
          @click.prevent="crearBeneficio"
          :disabled="isSubmitting"
          class="bg-blue-600 hover:bg-blue-700 text-white text-sm font-semibold py-1.5 px-4 rounded transition duration-200 disabled:bg-gray-400 disabled:cursor-not-allowed"
          type="button"
        >
          {{ isSubmitting ? "Creando..." : "Crear Beneficio" }}
        </button>
      </div>
      <p
        v-if="mensaje"
        :class="[
          'text-center text-sm',
          isSubmitting ? 'text-gray-500' : 'text-red-600',
        ]"
      >
        {{ mensaje }}
      </p>
    </form>
  </div>
</template>

<script>
import axios from "axios";
import { useUserStore } from "../store/user";
import { onMounted, ref } from "vue";
import { useRouter } from "vue-router";
import { API_BASE_URL } from "../config";
export default {
  setup() {
    const router = useRouter();
    const userStore = useUserStore();
    const form = ref({
      nombre: "",
      descripcion: "",
      costo: "",
      tiempoMinimo: "",
      frecuencia: "",
      cedulaJuridica: "",
      contratosElegibles: [],
      nombreDeAPI: "BeneficioNormal",
      esApi: false,
      esPorcentual: "",
    });

    // Estados para errores
    const nombreError = ref("");
    const descripcionError = ref("");
    const costoError = ref("");
    const tiempoMinimoError = ref("");
    const frecuenciaError = ref("");
    const contratosElegiblesError = ref("");
    const mensaje = ref("");
    const isSubmitting = ref(false);
    const seleccionApisError = ref("");
    const seleccionDeduccionError = ref("");

    // Método para completar el nombre del beneficio según la API seleccionada
    const handleApiSelection = (event) => {
      const selectedApi = event.target.value;

      if (selectedApi === "BeneficioNormal") {
        form.value.nombreDeAPI = "BeneficioNormal";
        form.value.esApi = false;
        form.value.nombre = "";
        form.value.costo = "";
      } else {
        form.value.nombreDeAPI = selectedApi;
        form.value.esApi = true;

        switch (selectedApi) {
          case "MediSeguro":
            form.value.nombre = "Seguro Médico";
            form.value.costo = 1;
            break;
          case "Poliza Seguros":
            form.value.nombre = "Póliza de Seguros";
            form.value.costo = 1;
            break;
          case "Asociacion Calculator":
            form.value.nombre = "Asociación Solidarista";
            form.value.costo = 1;
            break;
        }
      }
      validateAPIS();
    };

    // Método para clases de input, para revisar errores
    const inputClass = (error) => {
      return [
        "w-full border rounded px-2 py-1 text-sm focus:outline-none focus:ring-2",
        error
          ? "border-red-500 focus:ring-red-300"
          : "border-gray-300 focus:ring-blue-400",
      ];
    };

    // Métodos de validación para cada uno de los campos del formulario
    const validateNombre = () => {
      if (!form.value.nombre.trim()) {
        nombreError.value = "El nombre del beneficio es obligatorio.";
        return false;
      }
      if (form.value.nombre.length > 32) {
        nombreError.value = "El nombre no puede exceder los 32 caracteres.";
        return false;
      }
      nombreError.value = "";
      return true;
    };

    const validateDescripcion = () => {
      if (!form.value.descripcion.trim()) {
        descripcionError.value = "La descripción es obligatoria.";
        return false;
      }
      if (form.value.descripcion.length > 200) {
        descripcionError.value =
          "La descripción no puede exceder los 200 caracteres.";
        return false;
      }
      descripcionError.value = "";
      return true;
    };

    const validateCosto = () => {
      if (!form.value.costo) {
        costoError.value = "El costo es obligatorio.";
        return false;
      }
      if (form.value.costo < 0) {
        costoError.value = "El costo no puede ser negativo.";
        return false;
      }
      costoError.value = "";
      return true;
    };

    const validateTiempoMinimo = () => {
      if (
        form.value.tiempoMinimo === "" ||
        form.value.tiempoMinimo === null ||
        form.value.tiempoMinimo === undefined
      ) {
        tiempoMinimoError.value = "El tiempo mínimo es obligatorio.";
        return false;
      }
      if (form.value.tiempoMinimo < 0) {
        tiempoMinimoError.value = "El tiempo mínimo no puede ser negativo.";
        return false;
      }
      tiempoMinimoError.value = "";
      return true;
    };

    const validateFrecuencia = () => {
      frecuenciaError.value = form.value.frecuencia
        ? ""
        : "Debe seleccionar una frecuencia.";
      return !frecuenciaError.value;
    };

    const validateContratosElegibles = () => {
      contratosElegiblesError.value =
        form.value.contratosElegibles.length > 0
          ? ""
          : "Debe seleccionar al menos un tipo de contrato.";
      return !contratosElegiblesError.value;
    };

    const validateAPIS = () => {
      seleccionApisError.value =
        form.value.nombreDeAPI && form.value.nombreDeAPI !== ""
          ? ""
          : "Debe seleccionar una opción";
      return !seleccionApisError.value;
    };

    const validateDeduction = () => {
      seleccionDeduccionError.value =
      form.value.esPorcentual === true ||
      form.value.esPorcentual === false
        ? ""
        : "Debe seleccionar un tipo de deducción.";
      return !seleccionDeduccionError.value;
    };

    // Método para validar el formulario, usa todos los métodos de validación de los dintintos campos
    const validateForm = () => {
      const isNombreValid = validateNombre();
      const isDescripcionValid = validateDescripcion();
      const isCostoValid = validateCosto();
      const isTiempoMinimoValid = validateTiempoMinimo();
      const isFrecuenciaValid = validateFrecuencia();
      const isContratosValid = validateContratosElegibles();
      const isAPISValid = validateAPIS();
      const isDeductionValid = validateDeduction();

      return (
        isNombreValid &&
        isDescripcionValid &&
        isCostoValid &&
        isTiempoMinimoValid &&
        isFrecuenciaValid &&
        isContratosValid &&
        isAPISValid  &&
        isDeductionValid
      );
    };

    // Método para crear el beneficio
    const crearBeneficio = async () => {
      if (!validateForm()) {
        mensaje.value =
          "Por favor, corrija los errores antes de enviar el formulario.";
        return;
      }

      isSubmitting.value = true;
      mensaje.value = "";

      let payload = { ...form.value };

      // Si el beneficio es porcentual, se convierte el costo a decimal
      if (payload.esPorcentual === true || payload.esPorcentual === "true") {
        payload.costo = Number(payload.costo) * 0.01;
      }

      const url = `${API_BASE_URL}Beneficio/crearBeneficio`;
      try {
        const response = await axios.post(
          url,
          payload
        );
        mensaje.value = response.data;
        alert(response.data);
        // Redirigir al usuario a la página de inicio después de crear el beneficio
        router.push("/home");
      } catch (error) {
        console.error("Error al crear el beneficio:", error);
        mensaje.value =
          error.response?.data ||
          "Ocurrió un error al intentar crear el beneficio.";
      } finally {
        isSubmitting.value = false;
      }
    };

    // Obtener la empresa automáticamente al montar el componente
    const fetchEmpresaData = async () => {
      try {
        await userStore.fetchEmpresa(userStore.usuario.cedulaPersona);
        if (userStore.empresa) {
          // Asigna la cédula jurídica automáticamente
          form.value.cedulaJuridica = userStore.empresa.cedulaJuridica;
        } else {
          mensaje.value = "No se pudo obtener la información de la empresa.";
        }
      } catch (error) {
        console.error("Error al obtener los datos de la empresa:", error);
        mensaje.value =
          "Ocurrió un error al cargar la información de la empresa.";
      }
    };

    // Se llama a la función al montar el componente
    onMounted(() => {
      if (!userStore.usuario || !userStore.usuario.cedulaPersona) {
        window.location.href = "/";
      } else {
        fetchEmpresaData();
      }
    });

    return {
      form,
      nombreError,
      descripcionError,
      costoError,
      tiempoMinimoError,
      frecuenciaError,
      contratosElegiblesError,
      seleccionApisError,
      seleccionDeduccionError,
      mensaje,
      isSubmitting,
      inputClass,
      validateNombre,
      validateDescripcion,
      validateCosto,
      validateTiempoMinimo,
      validateFrecuencia,
      validateContratosElegibles,
      validateAPIS,
      validateDeduction,
      crearBeneficio,
      handleApiSelection,
    };
  },
};
</script>
