using Microsoft.AspNetCore.Http;
using BackendGeems.Infraestructure;
using BackendGeems.Application;
using Microsoft.AspNetCore.Mvc;

using BackendGeems.Domain;

namespace BackendGeems.API
{
    [ApiController]
    [Route("api/[controller]")]
    public class DuenoEmpresaController : ControllerBase
    {
        private readonly IGeneralRepo _repo;
        public DuenoEmpresaController(IGeneralRepo repo)
        {
            _repo = repo;
        }

        [HttpGet("{cedula}")]
        public ActionResult<DuenoEmpresa> GetDuenoEmpresa(string cedula)
        {
            var dueno = _repo.ObtenerDuenoEmpresaPorCedula(cedula);
            if (dueno == null)
                return NotFound();
            return Ok(dueno);
        }
    }
}