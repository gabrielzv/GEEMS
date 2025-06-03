USE GEEMSDB;
GO

CREATE OR ALTER FUNCTION dbo.fnHorasTrabajadasPorMes
(
    @IdEmpleado UNIQUEIDENTIFIER,
    @Fecha DATETIME
)
RETURNS INT
AS
BEGIN
    DECLARE @Horas INT;

    SELECT @Horas = ISNULL(SUM(NumHoras), 0)
    FROM Registro
    WHERE IdEmpleado = @IdEmpleado
      AND Estado IN ('Aprobado', 'NoRevisado')
      AND YEAR(Fecha) = YEAR(@Fecha)
      AND MONTH(Fecha) = MONTH(@Fecha);

    RETURN @Horas;
END;
GO