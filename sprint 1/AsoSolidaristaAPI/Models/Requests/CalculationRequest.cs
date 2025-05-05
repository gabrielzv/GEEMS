namespace AsoSolidaristaAPI.Models.Requests
{
    public class CalculationRequest
    {
        public required string AssociationName { get; set; }
        public decimal EmployeeSalary { get; set; }
    }
}