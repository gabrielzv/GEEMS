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
        [HttpGet("Register")]
        public Registro GetRegister(Guid id)
        {
            var registro = _queryHoras.GetRegister(id);
            // Imprime el objeto registro antes de retornarlo
            Console.WriteLine("Registro retornado por el API:");
            if (registro != null)
            {
                Console.WriteLine($"Id: {registro.Id}");
                Console.WriteLine($"NumHoras: {registro.NumHoras}");
                Console.WriteLine($"Fecha: {registro.Fecha}");
                Console.WriteLine($"Estado: {registro.Estado}");
                Console.WriteLine($"IdEmpleado: {registro.IdEmpleado}");
            }
            else
            {
                Console.WriteLine("registro es null");
            }
            return registro;
        }
        [HttpPost("Editar")]
        public void EditRegister([FromBody] Registro editing, Guid oldId)
        {
            _queryHoras.EditRegister(editing, oldId);
        }
    }
}
