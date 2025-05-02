using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using AsoSolidaristaAPI.Models.Requests;
using AsoSolidaristaAPI.Models.Responses;

namespace AsoSolidaristaAPI.Controllers.Private
{
    [ApiController]
    [Route("api/private/calculator")]
    [Authorize]
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
            try
            {
                var (amount, formula) = _calculator.CalculateFee(request);
                return Ok(new PrivateResult
                {
                    Percentage = (amount / request.EmployeeSalary) * 100,
                    AmountToCharge = amount,
                    FormulaUsed = $"Fórmula para {request.AssociationName}: {formula}"
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}