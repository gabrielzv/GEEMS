<template>
  <header class="bg-white shadow flex justify-between items-center px-6 py-3">
    <!-- Logo -->
    <router-link to="/home" class="flex items-center gap-2">
      <img src="@/assets/GEEMSLogo.png" class="h-14 w-auto" alt="GEEMS Logo" />
    </router-link>

    <!-- Navegación principal -->
    <nav v-if="isAuthenticated" class="flex items-center gap-2 sm:gap-4 text-gray-700">
      <!-- SuperAdmin -->
      <template v-if="user?.tipo === 'SuperAdmin'">
        <Dropdown label = "Reportes" icon="report">
          <NavLink to="/reporte3">Pagos de Planilla de la Empresa</NavLink>
          <NavLink to="/reporte4">Desglose de Planillas</NavLink>
          <NavLink to="/reporte5">Pagos por Empleado</NavLink>
          </Dropdown>
        <NavLink to="/gestionarUsuarios" icon="list">Gestionar usuarios</NavLink>
        <NavLink to="/configuracion" icon="edit">Configuración</NavLink>
        <NavLink to="/ConsulEmpresa" icon="summary">Empresas</NavLink>
      </template>

      <!-- DuenoEmpresa -->
      <template v-else-if="user?.tipo === 'DuenoEmpresa'">
        <Dropdown label = "Reportes" icon="report">
          <NavLink to="/reporte3">Pagos de Planilla de la Empresa</NavLink>
          <NavLink to="/reporte4">Desglose de Planillas</NavLink>
          <NavLink to="/reporte5">Pagos por Empleado</NavLink>
          </Dropdown>
        <Dropdown label="Empresa" icon="summary">
          <NavLink
            :to="{ name: 'editarEmpresa', params: { cedulaDueno: user?.cedulaPersona } }"
            icon="edit"
            >Editar empresa</NavLink
          >
          <NavLink to="/verEmpresaIndv" icon="summary">Ver mi empresa</NavLink>
        </Dropdown>
        <Dropdown label="Beneficios" icon="gift">
          <NavLink to="/companyBenefits" icon="list">Ver Beneficios</NavLink>
          <NavLink to="/benefitCreation" icon="plus">Crear Beneficios</NavLink>
        </Dropdown>
        <NavLink
          :to="{ path: '/anadirEmpleado', query: { duenoCedula: user?.cedulaPersona } }"
          icon="plus"
          >Añadir empleado</NavLink
        >
      </template>

      <!-- Empleado -->
      <template v-else-if="user?.tipo === 'Empleado'">
        <Dropdown label="Reportes" icon="report">
          <NavLink to="/reporte1" icon="salary">Desglose de Mi Salario</NavLink>
          <NavLink to="/reporte2" icon="summary">Resumen de Pagos</NavLink>

          <!-- Payroll tambien puede ver reporte 4-->
           <template v-if="empleado?.tipo === 'Payroll'">
            <NavLink to="/reporte4" icon="salary">Desglose de Planillas</NavLink>
          </template>
        </Dropdown>
        <Dropdown label="Registros" icon="clock">
          <NavLink to="/registrarHoras" icon="edit">Registrar horas</NavLink>
          <NavLink to="/employeeRegHistory" icon="history">Historial</NavLink>
        </Dropdown>
        <Dropdown label="Beneficios" icon="gift">
          <NavLink to="/matricularBeneficio" icon="plus">Matricular Beneficios</NavLink>
          <NavLink to="/employeeBenefits" icon="list">Mis beneficios</NavLink>
        </Dropdown>
        <template v-if="empleado?.tipo === 'Supervisor'">
          <NavLink to="/aprobarHoras" icon="edit">Aprobar horas</NavLink>
        </template>
        <template v-if="empleado?.tipo === 'Payroll'">
          <NavLink to="/selectCreatePayroll" icon="salary">Realizar Pago</NavLink>
          <NavLink to="/aprobarHoras" icon="edit">Aprobar horas</NavLink>
        </template>
      </template>
    </nav>

    <!-- Usuario + Logout -->
      <div class="flex items-center gap-3 min-w-0">
        <span
          v-if="user?.nombreUsuario"
          class="hidden sm:inline text-gray-700"
          :title="user.nombreUsuario"
        >
          Hola, {{ user.nombreUsuario }}
        </span>
        <span
          v-if="empleado?.tipo"
          class="hidden sm:inline text-gray-500 text-sm"
          :title="empleado.tipo"
        >
          {{ empleado.tipo }}
        </span>
        <img
          :src="user?.avatar || 'https://i.pravatar.cc/40'"
          alt="Avatar"
          class="w-9 h-9 rounded-full cursor-pointer border border-gray-200"
          @click="goToUserPage"
        />
      
      <button
      @click="handleLogout"
      class="px-2 py-0.5 text-[10px] text-gray-500 hover:text-red-500 hover:bg-gray-50 rounded-md transition-colors border border-gray-200"
    >
      Salir
    </button>
    </div>
  </header>
</template>

<script setup lang="jsx">
import { useRouter } from "vue-router";
import { useUserStore } from "@/store/user";
import { computed, ref, onMounted, defineComponent } from "vue";

// Componente NavLink para enlaces bonitos y reutilizables
const NavLink = defineComponent({
  props: {
    to: [String, Object],
    icon: String,
  },
  setup(props, { slots }) {
    const icons = {
      report: `<svg class="w-5 h-5 text-blue-500" fill="none" stroke="currentColor" stroke-width="2" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" d="M9 17v-2a2 2 0 012-2h2a2 2 0 012 2v2m-6 4h6a2 2 0 002-2V7a2 2 0 00-2-2H7a2 2 0 00-2 2v12a2 2 0 002 2z"/></svg>`,
      salary: `<svg class="w-5 h-5 text-blue-400" fill="none" stroke="currentColor" stroke-width="2" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" d="M12 8c-1.657 0-3 1.343-3 3s1.343 3 3 3 3-1.343 3-3-1.343-3-3-3zm0 10c-4.418 0-8-1.79-8-4V6a2 2 0 012-2h12a2 2 0 012 2v8c0 2.21-3.582 4-8 4z"/></svg>`,
      summary: `<svg class="w-5 h-5 text-blue-400" fill="none" stroke="currentColor" stroke-width="2" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" d="M17 9V7a5 5 0 00-10 0v2a2 2 0 00-2 2v6a2 2 0 002 2h10a2 2 0 002-2v-6a2 2 0 00-2-2z"/></svg>`,
      clock: `<svg class="w-5 h-5 text-blue-400" fill="none" stroke="currentColor" stroke-width="2" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" d="M12 8v4l3 3m6-3a9 9 0 11-18 0 9 9 0 0118 0z"/></svg>`,
      edit: `<svg class="w-5 h-5 text-blue-400" fill="none" stroke="currentColor" stroke-width="2" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" d="M11 5h2m-1 0v14m-7-7h14"/></svg>`,
      history: `<svg class="w-5 h-5 text-blue-400" fill="none" stroke="currentColor" stroke-width="2" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" d="M12 8v4l3 3m6-3a9 9 0 11-18 0 9 9 0 0118 0z"/></svg>`,
      gift: `<svg class="w-5 h-5 text-blue-400" fill="none" stroke="currentColor" stroke-width="2" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" d="M20 12v7a2 2 0 01-2 2H6a2 2 0 01-2-2v-7m16-2V7a2 2 0 00-2-2h-3.5a2.5 2.5 0 010-5A2.5 2.5 0 0117 5.5V7m-10 3V7a2 2 0 012-2h3.5a2.5 2.5 0 010-5A2.5 2.5 0 017 5.5V7"/></svg>`,
      plus: `<svg class="w-5 h-5 text-blue-400" fill="none" stroke="currentColor" stroke-width="2" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" d="M12 4v16m8-8H4"/></svg>`,
      list: `<svg class="w-5 h-5 text-blue-400" fill="none" stroke="currentColor" stroke-width="2" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" d="M4 6h16M4 12h16M4 18h16"/></svg>`,
    };
    return () => (
      <router-link
        to={props.to}
        class="flex items-center gap-2 px-4 py-2 rounded-md bg-blue-50 hover:bg-blue-100 text-blue-700 font-semibold shadow transition-all duration-200 focus:outline-none"
        active-class="text-blue-600 font-semibold bg-blue-50"
      >
        {props.icon && <span innerHTML={icons[props.icon]}></span>}
        {slots.default()}
      </router-link>
    );
  },
});

// Componente Dropdown
const Dropdown = defineComponent({
  props: {
    label: String,
    icon: String,
  },
  setup(props, { slots }) {
    const open = ref(false);
    const icons = {
      report: `<svg class="w-5 h-5 text-blue-500" fill="none" stroke="currentColor" stroke-width="2" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" d="M9 17v-2a2 2 0 012-2h2a2 2 0 012 2v2m-6 4h6a2 2 0 002-2V7a2 2 0 00-2-2H7a2 2 0 00-2 2v12a2 2 0 002 2z"/></svg>`,
      clock: `<svg class="w-5 h-5 text-blue-400" fill="none" stroke="currentColor" stroke-width="2" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" d="M12 8v4l3 3m6-3a9 9 0 11-18 0 9 9 0 0118 0z"/></svg>`,
      gift: `<svg class="w-5 h-5 text-blue-400" fill="none" stroke="currentColor" stroke-width="2" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" d="M20 12v7a2 2 0 01-2 2H6a2 2 0 01-2-2v-7m16-2V7a2 2 0 00-2-2h-3.5a2.5 2.5 0 010-5A2.5 2.5 0 0117 5.5V7m-10 3V7a2 2 0 012-2h3.5a2.5 2.5 0 010-5A2.5 2.5 0 017 5.5V7"/></svg>`,
      summary: `<svg class="w-5 h-5 text-blue-400" fill="none" stroke="currentColor" stroke-width="2" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" d="M17 9V7a5 5 0 00-10 0v2a2 2 0 00-2 2v6a2 2 0 002 2h10a2 2 0 002-2v-6a2 2 0 00-2-2z"/></svg>`,
      edit: `<svg class="w-5 h-5 text-blue-400" fill="none" stroke="currentColor" stroke-width="2" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" d="M11 5h2m-1 0v14m-7-7h14"/></svg>`,
      plus: `<svg class="w-5 h-5 text-blue-400" fill="none" stroke="currentColor" stroke-width="2" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" d="M12 4v16m8-8H4"/></svg>`,
      list: `<svg class="w-5 h-5 text-blue-400" fill="none" stroke="currentColor" stroke-width="2" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" d="M4 6h16M4 12h16M4 18h16"/></svg>`,
    };
    let timeout;
    const show = () => {
      clearTimeout(timeout);
      open.value = true;
    };
    const hide = () => {
      timeout = setTimeout(() => (open.value = false), 120);
    };
    return () => (
      <div class="relative" onMouseenter={show} onMouseleave={hide}>
        <button
          class="flex items-center gap-2 px-4 py-2 rounded-md bg-blue-50 hover:bg-blue-100 text-blue-700 font-semibold shadow transition-all duration-200 focus:outline-none"
          type="button"
        >
          {props.icon && <span innerHTML={icons[props.icon]}></span>}
          {props.label}
          <svg class="w-4 h-4 ml-1 text-blue-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 9l-7 7-7-7"/>
          </svg>
        </button>
        <transition
          enter-active-class="transition ease-out duration-200"
          enter-from-class="opacity-0 translate-y-2"
          enter-to-class="opacity-100 translate-y-0"
          leave-active-class="transition ease-in duration-150"
          leave-from-class="opacity-100 translate-y-0"
          leave-to-class="opacity-0 translate-y-2"
        >
          <div
            v-show={open.value}
            class="absolute left-0 mt-2 w-56 rounded-xl shadow-lg bg-white ring-1 ring-black ring-opacity-5 z-20"
          >
            {slots.default()}
          </div>
        </transition>
      </div>
    );
  },
});

// Lógica principal
const router = useRouter();
const userStore = useUserStore();
userStore.cargarDesdeSessionStorage();

const isAuthenticated = computed(() => !!userStore.usuario);
const user = computed(() => userStore.usuario);

const empleado = ref(null);

const fetchEmpleado = async () => {
  if (user.value?.tipo === "Empleado" && user.value?.cedulaPersona) {
    try {
      await userStore.fetchEmpleado(user.value.cedulaPersona);
      empleado.value = userStore.empleado;
    } catch (err) {
      console.error("Error al obtener datos del empleado:", err);
    }
  }
};

onMounted(() => {
  fetchEmpleado();
});

const goToUserPage = () => {
  router.push("/user");
};

const handleLogout = () => {
  userStore.logout();
  router.push("/login");
};
</script>