USE GEEMSDB;


GO 
CREATE PROCEDURE CalcularSalarioBruto
    @IdEmpleado UNIQUEIDENTIFIER,
    @FechaInicio DATETIME,
    @FechaFinal DATETIME,
    @SalarioBruto INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Contrato VARCHAR(30);
    DECLARE @SalarioBase INT;
    DECLARE @HorasTrabajadas INT;

    IF EXISTS (
        SELECT 1
        FROM Registro
        WHERE IdEmpleado = @IdEmpleado
          AND Fecha BETWEEN @FechaInicio AND @FechaFinal
          AND Estado = 'NoRevisado'
    )
    BEGIN
        SET @SalarioBruto = -2;
        
        RETURN;
    END

    SELECT @SalarioBase = SalarioBruto, @Contrato = Contrato
    FROM Empleado
    WHERE Id = @IdEmpleado;

    IF @SalarioBase IS NULL OR @Contrato IS NULL
    BEGIN
        SET @SalarioBruto = -1;
        RETURN;
    END

    SELECT @HorasTrabajadas = SUM(NumHoras)
    FROM Registro
    WHERE IdEmpleado = @IdEmpleado
      AND Fecha BETWEEN @FechaInicio AND @FechaFinal
      AND Estado = 'Aprobado';

    IF @HorasTrabajadas IS NULL
        SET @HorasTrabajadas = 0;

    IF @Contrato = 'Tiempo Completo'
    BEGIN
        SET @SalarioBruto = CAST((CAST(@SalarioBase AS DECIMAL(18,2)) / 160) * @HorasTrabajadas AS INT);
    END
    ELSE IF @Contrato = 'Medio Tiempo'
    BEGIN
        SET @SalarioBruto = CAST((CAST(@SalarioBase AS DECIMAL(18,2)) / 80) * @HorasTrabajadas AS INT);
    END
    ELSE IF @Contrato = 'Servicios Profesionales'
    BEGIN
        SET @SalarioBruto = @SalarioBase;
    END
    ELSE IF @Contrato = 'Por Horas'
    BEGIN
        SET @SalarioBruto = CAST(CAST(@SalarioBase AS DECIMAL(18,2)) * @HorasTrabajadas AS INT);
    END
    ELSE
    BEGIN
        RAISERROR('Tipo de contrato desconocido.', 16, 1);
    END
END;
