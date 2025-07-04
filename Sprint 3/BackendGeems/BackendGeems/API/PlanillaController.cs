using Microsoft.AspNetCore.Mvc;
using BackendGeems.Infraestructure;
using System;
using BackendGeems.Domain;
using BackendGeems.Application;

namespace BackendGeems.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlanillaController : ControllerBase
    {
        private readonly GeneralRepo _repo;
        private readonly PagoRepo _pagoRepo;
        private readonly EmpresaRepo _empresaRepo;
        private readonly ICalculadoraDeducciones _calculadoraDeducciones;

        public PlanillaController()
        {
            _repo = new GeneralRepo();
            _pagoRepo = new PagoRepo();
            _empresaRepo = new EmpresaRepo();
            _calculadoraDeducciones = new CalculadoraDeducciones();
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

    [HttpGet("{id}")]
    public IActionResult ObtenerPlanillaPorId(Guid id)
    {
        try
        {
            var planilla = _repo.ObtenerPlanillaPorId(id);
            if (planilla == null)
                return NotFound(new { message = "Planilla no encontrada." });

            return Ok(planilla);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error al obtener la planilla: " + ex.Message });
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

        [HttpGet("pagosPorPlanilla/{id}")]
        public IActionResult ObtenerPagosPorPlanilla(Guid id)
        {
            try
            {
                var pagos = _pagoRepo.ObtenerPagosPorPlanilla(id);
                if (pagos == null || !pagos.Any())
                    return NotFound(new { message = "No se encontraron pagos para esta planilla." });

                return Ok(pagos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al obtener los pagos: " + ex.Message });
            }
        }

        [HttpGet("historico")]
        public IActionResult HistoricoPlanillas([FromQuery] string cedulaJuridica)
        {
            try
            {
                var empresa = _empresaRepo.GetEmpresa(cedulaJuridica);
                if (empresa == null)
                    return NotFound(new { message = "Empresa no encontrada." });

                var planillas = _repo.ObtenerPlanillasPorEmpresa(empresa.Nombre);

                var resultado = planillas.Select(planilla =>
                {
                    var pagos = _pagoRepo.ObtenerPagosPorPlanilla(planilla.Id);

                    decimal salarioBruto = (decimal)pagos.Sum(p => p.MontoBruto);
                    decimal cargasSociales = (decimal)pagos.Sum(p => _calculadoraDeducciones.Calcular((decimal)p.MontoBruto).TotalDeducciones);
                    decimal deduccionesVoluntarias = (decimal)pagos.Sum(p => _pagoRepo.ObtenerDeduccionesVoluntariasPorPago(p.Id));
                    decimal costoEmpleador = salarioBruto + cargasSociales + deduccionesVoluntarias;

                    return new
                    {
                        nombreEmpresa = empresa.Nombre,
                        frecuenciaPago = empresa.ModalidadPago,
                        periodoPago = new
                        {
                            fechaInicio = planilla.FechaInicio,
                            fechaFin = planilla.FechaFinal
                        },
                        fechaPago = pagos.FirstOrDefault()?.FechaRealizada,
                        salarioBruto,
                        cargasSocialesEmpleador = cargasSociales,
                        deduccionesVoluntarias,
                        costoEmpleador
                    };
                });

                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al obtener el histórico: " + ex.Message });
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