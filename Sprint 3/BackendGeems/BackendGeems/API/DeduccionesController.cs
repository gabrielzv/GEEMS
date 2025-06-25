using BackendGeems.Application;
using BackendGeems.Domain;
using BackendGeems.Infraestructure;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace BackendGeems.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class DeduccionesController : ControllerBase
    {
        private readonly IPagoRepo _pagoRepo;

        public DeduccionesController(IPagoRepo pagoRepo)
        {
            _pagoRepo = pagoRepo;
        }

        [HttpGet("{idPago}")]
        public ActionResult<List<Deduccion>> GetDeduccionesPorPago(Guid idPago)
        {
            try
            {
                var deducciones = _pagoRepo.ObtenerDeduccionesPorPago(idPago);
                return Ok(deducciones);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al obtener deducciones: {ex.Message}");
            }
        }
    }
}