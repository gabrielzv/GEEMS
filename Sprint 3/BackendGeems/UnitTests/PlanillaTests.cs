using BackendGeems.API;
using BackendGeems.Application;
using BackendGeems.Controllers;
using BackendGeems.Domain;
using BackendGeems.Infraestructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text.Json;

namespace UnitTests
{

    public class ResumenPlanilla
    {
        public decimal totalBruto { get; set; }
        public decimal totalNeto { get; set; }
        public decimal totalDeducciones { get; set; }
    }
    public class PlanillaTests
    {
        [SetUp]
        public void Setup()
        {
            
        }

        // Test para verificar que se pueden listar las planillas de una empresa existente
        [Test]
        public void ListarPlanillas_ValidEmpresa_ReturnsPlanillas()
        {
            // Arrange
            string nombreEmpresa = "GEEMS Solutions";
            var repo = new GeneralRepo();
            var controller = new PlanillaController();

            // Act
            var result = controller.ListarPlanillas(nombreEmpresa) as OkObjectResult;

            // Assert
            Assert.IsNotNull(result, "El resultado no debe ser nulo.");
            Assert.IsInstanceOf<List<Planilla>>(result.Value, "El resultado debe ser una lista de planillas.");
            var planillas = result.Value as List<Planilla>;
            Assert.IsTrue(planillas.Count >= 0, "Debe retornar una lista (posiblemente vacía) de planillas.");
        }

        // Test para verificar que se puede generar pagos para todos los empleados de una empresa con el resumen correcto
        [Test]
        public void ResumenPlanilla_ValidParams_ReturnsResumen()
        {
            // Arrange
            string nombreEmpresa = "GEEMS Solutions";
            DateTime fechaInicio = new DateTime(2025, 4, 1);
            DateTime fechaFin = new DateTime(2025, 4, 30);

            var repo = new PagoRepo();
            var configuration = new ConfigurationBuilder().Build();
            var correoSender = new CorreoSender(configuration);
            var queryPago = new QueryPago(repo);
            var empleadoRepo = new EmpleadoRepo();
            var servicioDeCalculo = new ServicioCalculoPago(repo);
            var gestorPagos = new GestorPagosService(repo, servicioDeCalculo,empleadoRepo);
            var generarPago = new GenerarPago(repo,gestorPagos);
            var reporteService = new ReporteService(correoSender);
            var borradoDeEmpleados = new BorradoDeEmpleados( empleadoRepo, reporteService);

            var controller = new PagosController(queryPago, generarPago,gestorPagos,borradoDeEmpleados);

            // Act
            var result = controller.ResumenPlanilla(nombreEmpresa, fechaInicio, fechaFin) as OkObjectResult;

            // Assert
            Assert.IsNotNull(result, "El resultado no debe ser nulo.");
            var json = JsonSerializer.Serialize(result.Value);
            var resumen = JsonSerializer.Deserialize<ResumenPlanilla>(json);
            Assert.IsNotNull(resumen, "El resumen no debe ser nulo.");
            Assert.GreaterOrEqual(resumen.totalBruto, 0, "El total bruto debe ser mayor o igual a 0.");
            Assert.GreaterOrEqual(resumen.totalNeto, 0, "El total neto debe ser mayor o igual a 0.");
            Assert.GreaterOrEqual(resumen.totalDeducciones, 0, "El total de deducciones debe ser mayor o igual a 0.");
        }

        // Test para verificar que se retorna una lista vacía si la empresa no existe
        [Test]
        public void ListarPlanillas_EmpresaInexistente_ReturnsListaVacia()
        {
            // Arrange
            string nombreEmpresa = "EmpresaQueNoExiste123";
            var repo = new GeneralRepo();
            var controller = new PlanillaController();

            // Act
            var result = controller.ListarPlanillas(nombreEmpresa) as OkObjectResult;

            // Assert
            Assert.IsNotNull(result, "El resultado no debe ser nulo.");
            Assert.IsInstanceOf<List<Planilla>>(result.Value, "El resultado debe ser una lista de planillas.");
            var planillas = result.Value as List<Planilla>;
            Assert.IsTrue(planillas.Count == 0, "Debe retornar una lista vacía para una empresa inexistente.");
        }
    }
}