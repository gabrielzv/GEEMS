USE GEEMSDB;
GO

CREATE OR ALTER FUNCTION dbo.fnResumenPlanilla
(
    @nombreEmpresa NVARCHAR(100),
    @fechaInicio DATE,
    @fechaFin DATE
)
RETURNS TABLE
AS
RETURN
(
    SELECT 
        SUM(pg.MontoBruto) AS totalBruto,
        SUM(pg.MontoPago) AS totalNeto,
        SUM(pg.MontoBruto - pg.MontoPago) AS totalDeducciones
    FROM Pago pg
    JOIN Empleado e ON pg.IdEmpleado = e.Id
    WHERE e.NombreEmpresa = @nombreEmpresa
      AND pg.FechaInicio = @fechaInicio
      AND pg.FechaFinal = @fechaFin
)