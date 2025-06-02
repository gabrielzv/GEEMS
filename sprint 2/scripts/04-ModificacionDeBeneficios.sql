USE GEEMSDB;

-- Se agrega atributo Frecuencia a la tabla beneficio
ALTER TABLE Beneficio
ADD Frecuencia NVARCHAR(20)
CHECK (Frecuencia IN ('Mensual', 'Semanal', 'Trimestral', 'Único'));

-- Se elimina constraints existentes en BeneficioContratoElegible
IF EXISTS (SELECT * FROM sys.check_constraints WHERE parent_object_id = OBJECT_ID('BeneficioContratoElegible'))
BEGIN
    DECLARE @CheckConstraintName NVARCHAR(256);
    SELECT @CheckConstraintName = name 
    FROM sys.check_constraints 
    WHERE parent_object_id = OBJECT_ID('BeneficioContratoElegible');
    
    EXEC('ALTER TABLE BeneficioContratoElegible DROP CONSTRAINT ' + @CheckConstraintName);
END

-- Se elimina la PK si existe
IF EXISTS (SELECT * FROM sys.key_constraints WHERE type = 'PK' AND parent_object_id = OBJECT_ID('BeneficioContratoElegible'))
BEGIN
    DECLARE @PKName NVARCHAR(256);
    SELECT @PKName = name 
    FROM sys.key_constraints 
    WHERE type = 'PK' AND parent_object_id = OBJECT_ID('BeneficioContratoElegible');
    
    EXEC('ALTER TABLE BeneficioContratoElegible DROP CONSTRAINT ' + @PKName);
END

-- Se modifica la columna ContratoEmpleado a VARCHAR(23) (Daba error al agregar Servicios Profesionales) y se asegura que sea NOT NULL
ALTER TABLE BeneficioContratoElegible 
ALTER COLUMN ContratoEmpleado VARCHAR(23) NOT NULL;

-- Se hace que IdBeneficio no permita valores NULL
ALTER TABLE BeneficioContratoElegible 
ALTER COLUMN IdBeneficio UNIQUEIDENTIFIER NOT NULL;

-- Se hace una nueva PK compuesta
ALTER TABLE BeneficioContratoElegible
ADD CONSTRAINT PK_BeneficioContratoElegible 
PRIMARY KEY (IdBeneficio, ContratoEmpleado);

-- Se agrega un nuevo CHECK constraint (Antes Por Horas era PorHoras)
ALTER TABLE BeneficioContratoElegible
ADD CONSTRAINT CK_Beneficio_ContratoEmpleado 
CHECK (ContratoEmpleado IN (
    'Tiempo Completo',
    'Medio Tiempo',
    'Servicios Profesionales',
    'Por Horas'
));

-- SELECT * FROM BeneficioContratoElegible;
-- SELECT * FROM Beneficio;