# 📋 Proyecto Cálculo de Planillas 💰

**💎 GEEMS 💎**  
_CI-0128 - Proyecto Integrador de Ingeniería de Software y Bases de Datos_  
_Universidad de Costa Rica_  
_Período: I-2025_

---

## Integrantes del Equipo

| Nombre                   | Carné  |
| ------------------------ | ------ |
| Gabriel Zúñiga Vega      | C28741 |
| Emanuel García Rojas     | C23166 |
| Esteban Cháves Obando    | C12149 |
| Mainor Castro Vargas     | C21955 |
| Sebastián Blanco Quesada | C11085 |

## Profesores

- **M.Sc. Ricardo Sánchez**
- **M.Sc. Rebeca Obando**

---

## Descripción

Sistema para automatizar el cálculo de planillas con:

- Cálculo de salarios brutos/netos
- Deducciones (CCSS, impuestos)
- Manejo de beneficios por Empleado
- Generación de reportes
- Gestión de empleados

## Visión del producto

Ser un software que ofrece simplificar y automatizar el cálculo de pagos, reduciendo errores en el proceso, agilizando y garantizando el cumplimiento de regulaciones laborales y fiscales en Costa Rica a través de una plataforma intuitiva la cual funciona como portal entre los trabajadores y los dueños de una determinada empresa para generar reportes detallados sobre las planillas.

## User story ready

En nuestro proyecto para que consideremos un user story como lista para ser trabajada por el equipo debe cumplir los siguientes requerimientos:
Tiene criterios de aceptación.
Tiene los detalles necesarios para trabajar en ella, ningún miembro del equipo tiene dudas.
Sigue los criterios INVEST siempre que esto sea posible.

- INDEPENDENT: El user story es independiente de los demás user stories, permite el trabajo paralelo y el éxito de uno no depende de los otros.
- NEGOTIABLE: El user story es negociable, durante el desarrollo pueden realizarse modificaciones para permitir una colaboración ágil.
- VALUABLE: El user story tiene un valor calculable, este valor puede ser para los stakeholders o para el equipo de trabajo.
- ESTIMABLE: El tiempo que se requiere para completar un user story debe poder ser estimado, hay tareas donde esto no es posible debido a su ambigüedad.
- SMALL: Los user stories deben ser pequeñas y no generar confusión sobre lo que significan, deben poder ser completadas en un periodo de tiempo corto.
- TESTABLE: Los user stories deben tener la capacidad de ser probados, se debe poder verificar si un user story está completo.
  El user story tiene una prioridad definida.
  El user story debe ser asignado para trabajar a una persona como mínimo.
  Si el user story story necesita la implementación de una interfaz debe hacerse un mockup de esta.

## User story done

En nuestro proyecto para que consideremos un user story como completada debe cumplir los siguientes requerimientos:
Debe cumplir los criterios de aceptación.
Debe realizarse un code review.
Debe ser aceptada por el Product Owner.

---

## 🛠 Stack Tecnológico

| Área       | Tecnologías   |
| ---------- | ------------- |
| Frontend   | Vue           |
| Backend    | .NET          |
| Base Datos | Microsoft SQL |

---

## Instalación

Para correr el programa es necesario primero crear la base de datos con el script, acto seguido se debe crear un archivo appsettings.json en la carpeta BackendGeeems que tenga el siguiente contenido:

```javaScript
{
    "Logging": {
        "LogLevel": {
            "Default": "Information",
            "Microsoft.AspNetCore": "Warning"
        }
    },
    "AllowedHosts": "*",
    "ConnectionStrings": {
        "DefaultConnection": "CadenadeConexion"
    }
}
```

Para Correr El frontEnd se requiere tener instalado Node.js y correr los siguientes comandos

```console
npm install
npm install @vue/cli
npm install pinia
npm run serve
```
