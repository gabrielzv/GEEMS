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
        SUM(CAST(pg.MontoBruto AS DECIMAL(18,2))) AS totalBruto,
        SUM(CAST(pg.MontoPago AS DECIMAL(18,2))) AS totalNeto,
        SUM(CAST(pg.MontoBruto AS DECIMAL(18,2)) - CAST(pg.MontoPago AS DECIMAL(18,2))) AS totalDeducciones
    FROM Pago pg
    JOIN Empleado e ON pg.IdEmpleado = e.Id
    WHERE e.NombreEmpresa = @nombreEmpresa
      AND CAST(pg.FechaInicio AS DATE) = @fechaInicio
      AND CAST(pg.FechaFinal AS DATE) = @fechaFin
)