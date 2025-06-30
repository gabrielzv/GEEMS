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
    public class EmpresasController : ControllerBase
    {
        private readonly IQueryEmpresa _queryEmpresa;
        private readonly IGeneralRepo _repo;
        public EmpresasController(IQueryEmpresa queryEmpresa)
        {
            _queryEmpresa = queryEmpresa;
            _repo = new GeneralRepo();
        }

        [HttpDelete("borrar")]
        public void EliminarEmpresa(string cedula)
        {
            _queryEmpresa.EliminarEmpresa(cedula);
        }
        [HttpGet("EstadoEliminadoEmpresaPersona")]
        public bool GetEstadoEliminadoEmpresaPersona(int cedulaPersona)
        {
            return _queryEmpresa.GetEstadoEliminadoEmpresaPersona(cedulaPersona);
        }
        [HttpGet("Estado")]
        public bool GetEstado(string NombreEmpresa)
        {
            return _queryEmpresa.GetEstadoEliminadoEmpresa(NombreEmpresa);
        }
        [HttpGet("todas")]
        public ActionResult<List<Empresa>> GetTodasEmpresas()
        {
            var empresas = _repo.ObtenerTodas();
            return Ok(empresas);
        }
    }
}