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
    public class HorasController : ControllerBase
    {
        private readonly GEEMSRepo _repoInfrastructure;
        private readonly IQueryHoras _queryHoras;
        public HorasController(IQueryHoras queryHoras)
        {
            _repoInfrastructure = new GEEMSRepo();
            _queryHoras = queryHoras;
        }

        [HttpGet]
        public bool ValidDate(DateTime date, Guid employeeId)
        {
            bool response = _queryHoras.ValidDate(date, employeeId);
            return response;
        }

        [HttpPost]
        public void InsertRegister([FromBody] Registro inserting)
        {
            _queryHoras.InsertRegister(inserting);
        }
        [HttpGet("Register")]
        public Registro GetRegister(Guid id)
        {
            var registro = _queryHoras.GetRegister(id);
            return registro;
        }
        [HttpPost("Editar")]
        public void EditRegister([FromBody] Registro editing, Guid oldId)
        {
            _queryHoras.EditRegister(editing, oldId);
        }
        // GET: api/Horas/getRegister/{IdEmpleado}
        [HttpGet("getRegister/{IdEmpleado}")]
        public ActionResult<List<Registro>> GetRegisterByEmpleado(Guid IdEmpleado)
        {
            try
            {
                var registros = _repoInfrastructure.ObtenerRegistros(IdEmpleado);
                if (registros == null || registros.Count == 0)
                    return NotFound(new { message = "No se encontraron registros para este empleado." });

                return Ok(registros);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al obtener los registros: " + ex.Message });
            }
        }
        [HttpGet("ValidHours")]
        public bool ValidHours(DateTime date, Guid employeeId, int hours)
        {
            return _queryHoras.ValidHours(date, employeeId, hours);
        }
    }
}
