using BackendGeems.Application;
using BackendGeems.Domain;
using Microsoft.AspNetCore.Mvc;

namespace BackendGeems.API
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReporteController : ControllerBase
    {
        private readonly IReporteService _reporteService;

        public ReporteController(IReporteService reporteService)
        {
            _reporteService = reporteService;
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
    }
}
