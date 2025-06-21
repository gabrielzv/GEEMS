using Microsoft.AspNetCore.Http;
using BackendGeems.Application;
using BackendGeems.Domain;
using BackendGeems.Infraestructure;
using Microsoft.AspNetCore.Mvc;

namespace BackendGeems.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class SalarioBrutoControler : ControllerBase
    {
        private readonly GEEMSRepo _repoInfrastructure;
        private readonly ISalarioBruto _salarioBruto;
        public SalarioBrutoControler(ISalarioBruto salarioBruto)
        {
            _salarioBruto = salarioBruto;
            _repoInfrastructure = new GEEMSRepo();
        }
        [HttpGet]
        public double Get(Guid idEmpleado, DateTime fechaInicio, DateTime fechaFin)
        {
            var salarioBruto = _salarioBruto.ObtenerSalarioBruto(idEmpleado, fechaInicio, fechaFin);
            return salarioBruto;
        }
      
    }
}
