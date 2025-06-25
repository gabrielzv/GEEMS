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
    }
}