using BackendGeems.Application;
using BackendGeems.Domain;
using Microsoft.AspNetCore.Mvc;
using BackendGeems.Infraestructure;

namespace BackendGeems.API
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReporteController : ControllerBase
    {
        private readonly IReporteService _reporteService;
        private readonly IReporteRepo _reporteRepo;

        public ReporteController(IReporteService reporteService)
        {
            _reporteService = reporteService;
            _reporteRepo = new ReporteRepo();
        }

        [HttpPost("Reporte")]
        public async Task<IActionResult> EnviarReporte([FromForm] EnviarReporteRequest request)
        {
            if (request.Archivo == null || request.Archivo.Length == 0)
                return BadRequest("Archivo no válido.");

            using var stream = new MemoryStream();
            await request.Archivo.CopyToAsync(stream);

            await _reporteService.EnviarReportePorCorreoAsync(
                request.Correo,
                stream.ToArray(),
                request.Archivo.FileName,
                request.NombreUsuario
            );


            return Ok("El reporte se mando bien");
        }

        [HttpGet("salariosPorContrato/{idPlanilla}")]
        public ActionResult<List<SalarioPorContratoDto>> GetSalariosPorContrato(Guid idPlanilla)
        {
            try
            {
                var resultado = _reporteRepo.ObtenerSalariosPorContrato(idPlanilla);
                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al obtener los salarios: " + ex.Message });
            }
        }

        [HttpGet("deduccionesPorPlanilla/{idPlanilla}")]
        public ActionResult<List<DeduccionResumenDto>> GetDeduccionesPorPlanilla(Guid idPlanilla)
        {
            try
            {
                var resultado = _reporteRepo.ObtenerDeduccionesPorPlanilla(idPlanilla);
                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al obtener las deducciones: " + ex.Message });
            }
        }
    }
}
