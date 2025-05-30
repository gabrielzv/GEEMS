using BackendGeems.Application;
using BackendGeems.Domain;
using BackendGeems.Infraestructure;
using Microsoft.AspNetCore.Mvc;

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

    }
}
