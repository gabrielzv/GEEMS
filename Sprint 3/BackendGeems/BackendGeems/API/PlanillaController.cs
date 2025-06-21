using Microsoft.AspNetCore.Mvc;
using BackendGeems.Infraestructure;
using System;
using BackendGeems.Domain;

namespace BackendGeems.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlanillaController : ControllerBase
    {
        private readonly GEEMSRepo _repo;

        public PlanillaController()
        {
            _repo = new GEEMSRepo();
        }

    [HttpGet("listar")]
    public IActionResult ListarPlanillas([FromQuery] string nombreEmpresa)
    {
        try
        {
            var planillas = _repo.ObtenerPlanillasPorEmpresa(nombreEmpresa);
            return Ok(planillas);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error al obtener planillas: " + ex.Message });
        }
    }

        [HttpPost("crear")]
        public IActionResult CrearPlanilla([FromBody] CrearPlanillaDto dto)
        {
            try
            {
                if (dto.IdPayroll == Guid.Empty)
                    return BadRequest(new { message = "idPayroll es obligatorio." });

                if (!DateTime.TryParse(dto.fechaInicio, out var fechaInicio))
                    return BadRequest(new { message = "fechaInicio inválida." });

                if (!DateTime.TryParse(dto.fechaFinal, out var fechaFinal))
                    return BadRequest(new { message = "fechaFinal inválida." });

                var nuevaPlanilla = new Planilla
                {
                    Id = Guid.NewGuid(),
                    FechaInicio = fechaInicio,
                    FechaFinal = fechaFinal,
                    IdPayroll = dto.IdPayroll,
                };

                _repo.CrearPlanilla(nuevaPlanilla);

                return Ok(new { id = nuevaPlanilla.Id });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al crear la planilla: " + ex.Message });
            }
        }

        // DTO para recibir los datos del frontend
        public class CrearPlanillaDto
        {
            public Guid IdPayroll { get; set; }
            public string fechaInicio { get; set; }
            public string fechaFinal { get; set; }
        }
    }
}