USE GEEMSDB;

-- Se agrega flag para saber si la deducción es de tipo regular (0) o porcentual (1)
ALTER TABLE Beneficio
ADD EsPorcentual BIT NOT NULL DEFAULT 0;

-- Se hace que el costo de los beneficios pueda permitir valores decimales
ALTER TABLE Beneficio
ALTER COLUMN Costo DECIMAL(10, 2) NOT NULL;

-- Se agrega columna para saber en qué fecha se matriculó el beneficio
ALTER TABLE BeneficiosEmpleado
ADD FechaMatricula DATE NOT NULL DEFAULT GETDATE();

-- Se agrega columna para determinar el estado que tiene el beneficio
ALTER TABLE Beneficio
ADD Estado NVARCHAR(20)
CHECK (Estado IN ('Activo', 'Inactivo', 'PendienteBorrado'));

-- Se agrega columna para saber si el beneficio está borrado (Borrado Lógico)
ALTER TABLE Beneficio
ADD EstaBorrado BIT NOT NULL DEFAULT 0;