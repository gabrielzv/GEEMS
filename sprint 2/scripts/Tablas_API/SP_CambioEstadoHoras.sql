CREATE OR ALTER PROCEDURE sp_ActualizarEstadoRegistro
(
    @IdRegistro UNIQUEIDENTIFIER,
    @OpcionEstado INT,
    @Resultado VARCHAR(50) OUTPUT
)
AS
BEGIN
    DECLARE @NuevoEstado VARCHAR(20);

    -- Asignar estado según la opción recibida
    SET @NuevoEstado = CASE 
        WHEN @OpcionEstado = 1 THEN 'Aprobado'
        WHEN @OpcionEstado = 2 THEN 'NoRevisado'
        WHEN @OpcionEstado = 3 THEN 'Denegado'
        ELSE NULL
    END;

    -- Verificar si la opción fue válida
    IF @NuevoEstado IS NULL
    BEGIN
        SET @Resultado = 'Opción no válida. Use 1 (Aprobado), 2 (NoRevisado) o 3 (Denegado)';
        RETURN;
    END;

    -- Verificar si el registro existe
    IF NOT EXISTS (SELECT 1 FROM Registro WHERE Id = @IdRegistro)
    BEGIN
        SET @Resultado = 'Registro no encontrado';
        RETURN;
    END;

    -- Actualizar el estado
    UPDATE Registro
    SET Estado = @NuevoEstado
    WHERE Id = @IdRegistro;

    SET @Resultado = 'Estado actualizado correctamente a ' + @NuevoEstado;
END;