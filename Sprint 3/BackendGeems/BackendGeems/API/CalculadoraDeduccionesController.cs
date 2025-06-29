using BackendGeems.Domain;
using Microsoft.AspNetCore.Mvc;

namespace BackendGeems.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DeduccionController : ControllerBase
    {
        private readonly ICalculadoraDeducciones _calculadora;

        public DeduccionController(ICalculadoraDeducciones calculadora)
        {
            _calculadora = calculadora;
        }

        [HttpGet("DeduccionesDetalladas/{salarioBruto}")]
        public ActionResult<ResultadoDeducciones> CalcularDeducciones(decimal salarioBruto)
        {
            var resultado = _calculadora.Calcular(salarioBruto);
            return Ok(resultado);
        }
    }
}
