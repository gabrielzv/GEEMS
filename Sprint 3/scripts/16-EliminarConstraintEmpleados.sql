USE GEEMSDB;
GO

-- Permitir NULL en la columna NombreEmpresa de Empleado
ALTER TABLE Empleado
ALTER COLUMN NombreEmpresa NVARCHAR(100) NULL;
GO

-- Buscar el nombre del constraint actual en Empleado
DECLARE @constraintName NVARCHAR(200);
SELECT @constraintName = fk.name
FROM sys.foreign_keys fk
JOIN sys.tables t ON fk.parent_object_id = t.object_id
WHERE t.name = 'Empleado' AND fk.referenced_object_id = OBJECT_ID('Empresa');

-- Eliminar el constraint actual
IF @constraintName IS NOT NULL
    EXEC('ALTER TABLE Empleado DROP CONSTRAINT ' + @constraintName);
GO

-- Crear el nuevo constraint con ON DELETE SET NULL
ALTER TABLE Empleado
ADD CONSTRAINT FK_Empleado_Empresa
FOREIGN KEY (NombreEmpresa) REFERENCES Empresa(Nombre)
ON DELETE SET NULL;
GO