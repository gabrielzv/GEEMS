using BackendGeems.Application;
using BackendGeems.Domain;
using BackendGeems.Infraestructure;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace BackendGeems.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BeneficioController : ControllerBase
    {
        private readonly IQueryBeneficio _queryBeneficio;

        public BeneficioController(IQueryBeneficio queryBeneficio)
        {
            _queryBeneficio = queryBeneficio;
        }

        [HttpPost("crearBeneficio")]
        public IActionResult CrearBeneficio([FromBody] Beneficio beneficio)
        {
            try
            {
                _queryBeneficio.CrearBeneficio(beneficio);
                return Ok("Beneficio creado exitosamente.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al crear el beneficio: {ex.Message}");
            }
        }

        [HttpPost("editarBeneficio")]
        public IActionResult EditarBeneficio([FromBody] Beneficio beneficio)
        {
            try
            {
                _queryBeneficio.EditarBeneficio(beneficio);
                return Ok("Beneficio editado exitosamente.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al editar el beneficio: {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        public IActionResult GetBeneficio(Guid id)
        {
            try
            {
                var beneficio = _queryBeneficio.GetBeneficio(id);
                if (beneficio == null)
                    return NotFound("Beneficio no encontrado.");
                return Ok(beneficio);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al obtener el beneficio: {ex.Message}");
            }
        }

        [HttpGet("Company/{CedulaJuridica}")]
        public IActionResult GetCompanyBenfits(string CedulaJuridica)
        {
            try
            {
                var beneficios = _queryBeneficio.GetCompanyBenefits(CedulaJuridica);
                if (beneficios == null || beneficios.Count == 0)
                    return NotFound("No se encontraron beneficios para esta empresa.");
                return Ok(beneficios);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al obtener los beneficios de la empresa: {ex.Message}");
            }
        }

        [HttpGet("BenefitsEmployeeContract/{CedulaJuridica}/{IdEmpleado}")]
        public IActionResult GetBenefitsEmployeeContract(string CedulaJuridica, string IdEmpleado)
        {
            try
            {
                var beneficios = _queryBeneficio.GetBenefitsEmployeeContract(CedulaJuridica, IdEmpleado);
                if (beneficios == null || beneficios.Count == 0)
                    return NotFound("No se encontraron beneficios para el contrato de este empleado.");
                return Ok(beneficios);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al obtener los beneficios del contrato del empleado: {ex.Message}");
            }
        }

        [HttpGet("Employee/{IdEmpleado}")]
        public IActionResult GetEmployeeBenefits(string IdEmpleado)
        {
            try
            {
                var beneficios = _queryBeneficio.GetEmployeeBenefits(IdEmpleado);
                if (beneficios == null || beneficios.Count == 0)
                    return NotFound("No se encontraron beneficios matriculados.");
                return Ok(beneficios);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al obtener los beneficios del empleado: {ex.Message}");
            }
        }

        [HttpPost("matricularBeneficio")]
        public IActionResult MatricularBeneficio([FromBody] BeneficiosEmpleado beneficioEmpleado)
        {
            try
            {
                _queryBeneficio.MatricularBeneficio(beneficioEmpleado);
                return Ok("Beneficio matriculado exitosamente.");
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al matricular el beneficio: {ex.Message}");
            }
        }

        [HttpDelete("eliminarBeneficio/{IdBeneficio}")]
        public IActionResult EliminarBeneficio(string IdBeneficio)
        {
            try
            {
                _queryBeneficio.EliminarBeneficio(IdBeneficio);
                return Ok("Beneficio eliminado exitosamente.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al eliminar el beneficio: {ex.Message}");
            }
        }
    }
}