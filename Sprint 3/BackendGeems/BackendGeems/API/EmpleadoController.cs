using Microsoft.AspNetCore.Http;
using BackendGeems.Infraestructure;
using BackendGeems.Application;
using Microsoft.AspNetCore.Mvc;

namespace BackendGeems.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmpleadoController : ControllerBase
    {
        private readonly IGeneralRepo _generalRepo;
        private readonly BorradoDeEmpleados _borradoDeEmpleados;

        public EmpleadoController(IGeneralRepo generalRepo, BorradoDeEmpleados borradoDeEmpleados)
        {
            _generalRepo = generalRepo;
            _borradoDeEmpleados = borradoDeEmpleados;
        }

        [HttpDelete]
        public IActionResult BorrarEmpleado([FromQuery] string Cedula)
        {
            try
            {
                string message=_borradoDeEmpleados.BorrarEmpleado(Cedula);
                return Ok(new { message  });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}
