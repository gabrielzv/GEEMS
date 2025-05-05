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
            try
            {
                var (amount, _) = _calculator.CalculateFee(request);
                return Ok(new PublicResult { AmountToCharge = amount });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}