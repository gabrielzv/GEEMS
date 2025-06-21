﻿using BackendGeems.Domain;
namespace BackendGeems.Application
{
    public interface IQueryHoras
    {
        bool ValidDate(DateTime date, Guid employeeId);
        void InsertRegister(Registro inserting);
        Registro GetRegister(Guid Id);
        void EditRegister(Registro editing, Guid oldId);
        bool ValidHours(DateTime date, Guid employeeId, int hours);
    }
}
