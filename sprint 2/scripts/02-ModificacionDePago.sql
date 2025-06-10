USE GEEMSDB;

ALTER TABLE Pago
DROP COLUMN TipoDeduccion;

ALTER TABLE Pago
DROP COLUMN MontoDeduccion;

ALTER TABLE Pago ADD MontoBruto INT NOT NULL;

ALTER TABLE Pago
DROP COLUMN Periodo;

ALTER TABLE Pago ADD FechaInicio DATETIME NOT NULL,
FechaFinal DATETIME NOT NULL;

---------------------------------------------------------------------------------------------------------------
CREATE TABLE
    Deducciones (
        Id UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
        IdPago UNIQUEIDENTIFIER NOT NULL,
        TipoDeduccion VARCHAR(20) CHECK (TipoDeduccion IN ('Obligatoria', 'Voluntaria')),
        IdBeneficio UNIQUEIDENTIFIER NULL,
        Monto INT NOT NULL,
        FOREIGN KEY (IdPago) REFERENCES Pago (Id) ON UPDATE CASCADE,
        FOREIGN KEY (IdBeneficio) REFERENCES Beneficio (Id)
    );

----------------------------------------------------------------------------------------------------------------    
DROP TABLE HistorialDePagos;

----------------------------------------------------------------------------------------------------------------
CREATE TABLE
    Planilla (
        Id UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
        FechaInicio DATETIME NOT NULL,
        FechaFinal DATETIME NOT NULL,
        IdPayroll UNIQUEIDENTIFIER NOT NULL,
        FOREIGN KEY (IdPayroll) REFERENCES Empleado (Id) ON UPDATE CASCADE
    );

-----------------------------------------------------------------------------------------------------------------
ALTER TABLE Pago ADD IdPlanilla UNIQUEIDENTIFIER NOT NULL,
FOREIGN KEY (IdPlanilla) REFERENCES Planilla (Id)
SELECT
    *
FROM
    Persona;

SELECT
    *
FROM
    Usuario;

SELECT
    *
FROM
    SuperAdmin;

SELECT
    *
FROM
    Empresa;

SELECT
    *
FROM
    SuperAdminAdministraEmpresa;

SELECT
    *
FROM
    DuenoEmpresa;

SELECT
    *
FROM
    DatosPrivadosEmpresa;

SELECT
    *
FROM
    Beneficio;

SELECT
    *
FROM
    BeneficioContratoElegible;

SELECT
    *
FROM
    Empleado;

SELECT
    *
FROM
    SupervisorSupervisaEmpleado;

SELECT
    *
FROM
    Pago;

SELECT
    *
FROM
    Registro;

SELECT
    *
FROM
    BeneficiosEmpleado;

SELECT
    *
FROM
    Planilla