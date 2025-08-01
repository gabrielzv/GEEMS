﻿namespace BackendGeems.Application
{
    public interface IQueryEmpresa
    {
        void EliminarEmpresa(string cedula);
        bool GetEstadoEliminadoEmpresaPersona(int cedulaPersona);
        bool GetEstadoEliminadoEmpresa(string nombreEmpresa);
        void EliminarEmpleador(int cedula);
    }
}
