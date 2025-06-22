USE GEEMSDB;
GO

CREATE OR ALTER TRIGGER tr_LimitarMatriculaBeneficios
ON BeneficiosEmpleado
AFTER INSERT, UPDATE
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @ExcedeLimite BIT = 0;
    
    SELECT @ExcedeLimite = 1
    FROM inserted i
    JOIN Empleado e ON i.IdEmpleado = e.Id
    JOIN Empresa emp ON e.NombreEmpresa = emp.Nombre
    JOIN (
        SELECT IdEmpleado, COUNT(*) AS TotalBeneficios
        FROM BeneficiosEmpleado
        WHERE IdEmpleado IN (SELECT IdEmpleado FROM inserted)
        GROUP BY IdEmpleado
    ) be ON i.IdEmpleado = be.IdEmpleado
    WHERE be.TotalBeneficios > emp.MaxBeneficiosXEmpleado;
    
    IF @ExcedeLimite = 1
    BEGIN
        ROLLBACK TRANSACTION;
        THROW 51000, 'El empleado no puede matricular m�s beneficios de los permitidos por su empresa', 1;
    END
END;
GO