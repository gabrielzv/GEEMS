USE GEEMSDB;
GO

-- Permitir NULL en la columna CedulaJuridica de Beneficio
ALTER TABLE Beneficio
ALTER COLUMN CedulaJuridica NVARCHAR(20) NULL;
GO

-- Buscar el nombre del constraint actual en Beneficio
DECLARE @constraintName NVARCHAR(200);
SELECT @constraintName = fk.name
FROM sys.foreign_keys fk
JOIN sys.tables t ON fk.parent_object_id = t.object_id
WHERE t.name = 'Beneficio' AND fk.referenced_object_id = OBJECT_ID('Empresa');

-- Eliminar el constraint actual
IF @constraintName IS NOT NULL
    EXEC('ALTER TABLE Beneficio DROP CONSTRAINT ' + @constraintName);
GO

-- Crear el nuevo constraint con ON DELETE SET NULL
ALTER TABLE Beneficio
ADD CONSTRAINT FK_Beneficio_Empresa
FOREIGN KEY (CedulaJuridica) REFERENCES Empresa(CedulaJuridica)
ON DELETE SET NULL;
GO