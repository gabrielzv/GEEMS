namespace BackendGeems.Domain
{
    public class Empresa
    {
        public string CedulaJuridica { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public string Telefono { get; set; }
        public string Correo { get; set; }
        public string Provincia { get; set; }
        public string Canton { get; set; }
        public string Distrito { get; set; }
        public string Senas { get; set; }
        public string? ModalidadPago { get; set; }
        public int MaxBeneficiosXEmpleado { get; set; }
        public bool EstaBorrado { get; set; }

    }
}
