USE GEEMSDB;
GO

-- Permitir NULL en la columna CedulaEmpresa
ALTER TABLE DuenoEmpresa
ALTER COLUMN CedulaEmpresa NVARCHAR(20) NULL;
GO

-- Buscar el nombre del constraint actual
DECLARE @constraintName NVARCHAR(200);
SELECT @constraintName = fk.name
FROM sys.foreign_keys fk
JOIN sys.tables t ON fk.parent_object_id = t.object_id
WHERE t.name = 'DuenoEmpresa' AND fk.referenced_object_id = OBJECT_ID('Empresa');

-- Eliminar el constraint actual
IF @constraintName IS NOT NULL
    EXEC('ALTER TABLE DuenoEmpresa DROP CONSTRAINT ' + @constraintName);
GO

-- Crear el nuevo constraint con ON DELETE SET NULL
ALTER TABLE DuenoEmpresa
ADD CONSTRAINT FK_DuenoEmpresa_Empresa
FOREIGN KEY (CedulaEmpresa) REFERENCES Empresa(CedulaJuridica)
ON DELETE SET NULL;
GO