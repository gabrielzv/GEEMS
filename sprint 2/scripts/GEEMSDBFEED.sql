USE GEEMSDB;

DROP TABLE IF EXISTS Deduccion;
DROP TABLE IF EXISTS Pago;
DROP TABLE IF EXISTS SupervisorSupervisaEmpleado;
DROP TABLE IF EXISTS BeneficiosEmpleado;
DROP TABLE IF EXISTS Registro;
DROP TABLE IF EXISTS Empleado;
DROP TABLE IF EXISTS BeneficioContratoElegible;
DROP TABLE IF EXISTS Beneficio;
DROP TABLE IF EXISTS DatosPrivadosEmpresa;
DROP TABLE IF EXISTS DuenoEmpresa;
DROP TABLE IF EXISTS SuperAdminAdministraEmpresa;
DROP TABLE IF EXISTS Planilla;
DROP TABLE IF EXISTS SuperAdmin;
DROP TABLE IF EXISTS Usuario;
DROP TABLE IF EXISTS Empresa;
DROP TABLE IF EXISTS Persona;
GO
-- Declaración de IDs
DECLARE @uid1 UNIQUEIDENTIFIER = NEWID(); -- Libre
DECLARE @uid2 UNIQUEIDENTIFIER = NEWID(); -- DuenoEmpresa
DECLARE @uid3 UNIQUEIDENTIFIER = NEWID(); -- Colaborador
DECLARE @uid4 UNIQUEIDENTIFIER = NEWID(); -- Beneficio
DECLARE @uid5 UNIQUEIDENTIFIER = NEWID(); -- Supervisor
DECLARE @uid6 UNIQUEIDENTIFIER = NEWID(); -- Payroll
DECLARE @uid7 UNIQUEIDENTIFIER = NEWID(); -- Pago
DECLARE @uid8 UNIQUEIDENTIFIER = NEWID(); -- Registro
DECLARE @uid9 UNIQUEIDENTIFIER = NEWID(); -- SuperAdmin
DECLARE @uid10 UNIQUEIDENTIFIER = NEWID(); -- Planilla

-- Personas
INSERT INTO Persona VALUES (101010101, 'Av Central', 'Carlos', 'Perez', 'Mora', 'cperez@geems.com', '8888-8888');
INSERT INTO Persona VALUES (202020202, 'Calle 2', 'Ana', 'Lopez', 'Soto', 'alopez@geems.com', '8777-7777');
INSERT INTO Persona VALUES (303030303, 'Calle 3', 'Luis', 'Ramirez', 'Acosta', 'lramirez@geems.com', '8666-6666');
INSERT INTO Persona VALUES (404040404, 'Calle 4', 'María', 'Castro', 'Jiménez', 'mcastro@geems.com', '8555-5555');
INSERT INTO Persona VALUES (505050505, 'Calle 5', 'Pedro', 'Gomez', 'Sánchez', 'pgomez@geems.com', '8444-4444');

-- Usuarios
INSERT INTO Usuario VALUES (@uid2, 'alopez', 'pass5678', 'DuenoEmpresa', 202020202, 'alopez@geems.com');
INSERT INTO Usuario VALUES (@uid3, 'lramirez', 'pass9999', 'Empleado', 303030303, 'lramirez@geems.com');
INSERT INTO Usuario VALUES (@uid5, 'mcastro', 'clave123', 'Empleado', 404040404, 'mcastro@geems.com');
INSERT INTO Usuario VALUES (@uid6, 'pgomez', 'clave321', 'Empleado', 505050505, 'pgomez@geems.com');
INSERT INTO Usuario VALUES (@uid9, 'cperez', 'pass777', 'SuperAdmin', 101010101, 'cperez@geems.com');

-- SuperAdmin
INSERT INTO SuperAdmin VALUES (@uid9, 101010101);

-- Empresas
INSERT INTO Empresa VALUES ('3-101-1234567', 'GEEMS Solutions', 'Consultora en gestión empresarial', '2222-2222', 'info@geems.com', 'San José', 'San José', 'Carmen', '300 m norte del parque central');
INSERT INTO Empresa VALUES ('1-555-9876543', 'Innova Corp', 'Soluciones digitales y tecnológicas', '2100-3333', 'contacto@innova.com', 'Heredia', 'Heredia', 'San Francisco', 'Detrás del parque industrial');

-- Asociación SuperAdmin → Empresa
INSERT INTO SuperAdminAdministraEmpresa VALUES (@uid9, '3-101-1234567');

-- Dueño de Empresa
INSERT INTO DuenoEmpresa VALUES (@uid2, '3-101-1234567', 202020202);

-- Datos privados empresa
INSERT INTO DatosPrivadosEmpresa VALUES ('3-101-1234567', 'Mensual', 3);
INSERT INTO DatosPrivadosEmpresa VALUES ('1-555-9876543', 'Trimestral', 2);

-- Beneficio
INSERT INTO Beneficio VALUES (@uid4, 10000, 6, 'Membresía en gimnasio premium', 'Gimnasio Premium', '3-101-1234567');

-- Elegibilidad del beneficio
INSERT INTO BeneficioContratoElegible VALUES (@uid4, 'Tiempo Completo');

-- Empleados
INSERT INTO Empleado VALUES (@uid3, 303030303, 'Tiempo Completo', 160, 'M', 'Activo', 850000, 'Colaborador', '2024-06-15', 'GEEMS Solutions');
INSERT INTO Empleado VALUES (@uid5, 404040404, 'Tiempo Completo', 160, 'F', 'Activo', 950000, 'Supervisor', '2023-11-01', 'GEEMS Solutions');
INSERT INTO Empleado VALUES (@uid6, 505050505, 'Tiempo Completo', 160, 'M', 'Activo', 1000000, 'Payroll', '2023-01-20', 'GEEMS Solutions');

-- Supervisor supervisa a colaborador
INSERT INTO SupervisorSupervisaEmpleado VALUES (@uid5, 303030303);

INSERT INTO Planilla VALUES (@uid10, 200000, 0, GETDATE(), GETDATE());
-- Pago a colaborador
INSERT INTO Pago VALUES (@uid7, GETDATE(), 75000, 'Abril 2025', @uid3, @uid6,@uid10);
INSERT INTO Deduccion VALUEs(@uid7,'Renta',2000000);
-- Registro de horas del colaborador
INSERT INTO Registro VALUES (@uid8, 40, GETDATE(), 'Aprobado', @uid3);

-- Beneficio otorgado
INSERT INTO BeneficiosEmpleado VALUES (@uid3, @uid4);

-- Historial de pagos


