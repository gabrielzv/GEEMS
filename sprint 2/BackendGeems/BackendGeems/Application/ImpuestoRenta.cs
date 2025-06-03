namespace BackendGeems.Application
{
    public class ImpuestoRenta
    {
        int CalcularImpuestoRenta(int ingresoMensual)
        {
            int impuesto = 0;

            if (ingresoMensual <= 922000)
            {
                impuesto = 0;
            }
            else if (ingresoMensual <= 1352000)
            {
                impuesto = (int)((ingresoMensual - 922000) * 0.10);
            }
            else if (ingresoMensual <= 2373000)
            {
                impuesto = (int)((1352000 - 922000) * 0.10 + (ingresoMensual - 1352000) * 0.15);
            }
            else if (ingresoMensual <= 4745000)
            {
                impuesto = (int)((1352000 - 922000) * 0.10 + (2373000 - 1352000) * 0.15 + (ingresoMensual - 2373000) * 0.20);
            }
            else
            {
                impuesto = (int)((1352000 - 922000) * 0.10 + (2373000 - 1352000) * 0.15 + (4745000 - 2373000) * 0.20 + (ingresoMensual - 4745000) * 0.25);
            }

            return impuesto;
        }

    }
}
