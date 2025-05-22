using Microsoft.AspNetCore.Mvc;
using AsoSolidaristaAPI.Models.Requests;
using AsoSolidaristaAPI.Models.Responses;

namespace AsoSolidaristaAPI.Controllers.Public
{
    [ApiController]
    [Route("api/public/calculator")]
    public class CalculatorController : ControllerBase
    {
        private readonly CalculatorService _calculator;

        public CalculatorController(CalculatorService calculator)
        {
            _calculator = calculator;
        }

        [HttpPost("calculate")]
        public IActionResult Calculate([FromBody] CalculationRequest request)
        {
            if (request == null)
            {
                return BadRequest(new ErrorResponse("El cuerpo de la solicitud no puede estar vacío"));
            }

            if (string.IsNullOrWhiteSpace(request.AssociationName))
            {
                return BadRequest(new ErrorResponse("El nombre de la asociación es requerido"));
            }

            if (request.EmployeeSalary <= 0)
            {
                return BadRequest(new ErrorResponse("El salario debe ser un número positivo mayor a cero"));
            }

            try
            {
                var (amount, _) = _calculator.CalculateFee(request);
                return Ok(new PublicResult { AmountToCharge = amount });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new ErrorResponse(ex.Message));
            }
        }
    }

    public class ErrorResponse
    {
        public string Message { get; set; }

        public ErrorResponse(string message)
        {
            Message = message;
        }
    }
}