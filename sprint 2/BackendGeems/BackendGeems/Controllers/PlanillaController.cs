using Microsoft.AspNetCore.Mvc;
using BackendGeems.Infraestructure;
using System;

namespace BackendGeems.Controllers
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
    }
}