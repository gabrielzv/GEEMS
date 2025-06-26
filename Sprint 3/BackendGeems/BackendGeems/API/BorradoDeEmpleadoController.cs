using Microsoft.AspNetCore.Http;
using BackendGeems.Infraestructure;
using BackendGeems.Application;
using Microsoft.AspNetCore.Mvc;

namespace BackendGeems.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class BorradoDeEmpleadoController : ControllerBase
    {
        private readonly GeneralRepo _generalRepo;
        private readonly BorradoDeEmpleados borradoDeEmpleados;

        public BorradoDeEmpleadoController(GeneralRepo generalRepo, BorradoDeEmpleados borradoDeEmpleados)
        {
            _generalRepo = generalRepo;
            this.borradoDeEmpleados = borradoDeEmpleados;
        }

        [HttpPut]
        public IActionResult BorrarEmpleado([FromQuery] string Cedula)
        {
            try
            {
                borradoDeEmpleados.BorrarEmpleado(Cedula);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}
