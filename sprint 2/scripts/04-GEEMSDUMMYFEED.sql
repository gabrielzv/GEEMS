USE GEEMSDB;

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
DECLARE @uidPlanilla UNIQUEIDENTIFIER = NEWID(); -- Planilla
DECLARE @uidDeduccion1 UNIQUEIDENTIFIER = NEWID(); -- Deduccion obligatoria
DECLARE @uidDeduccion2 UNIQUEIDENTIFIER = NEWID(); -- Deduccion voluntaria

-- Personas
INSERT INTO Persona VALUES 
(101010101, 'Av Central', 'Carlos', 'Perez', 'Mora', 'cperez@geems.com', '8888-8888'),
(202020202, 'Calle 2', 'Ana', 'Lopez', 'Soto', 'alopez@geems.com', '8777-7777'),
(303030303, 'Calle 3', 'Luis', 'Ramirez', 'Acosta', 'lramirez@geems.com', '8666-6666'),
(404040404, 'Calle 4', 'María', 'Castro', 'Jiménez', 'mcastro@geems.com', '8555-5555'),
(505050505, 'Calle 5', 'Pedro', 'Gomez', 'Sánchez', 'pgomez@geems.com', '8444-4444');

-- Usuarios
INSERT INTO Usuario VALUES 
(@uid2, 'alopez', 'pass5678', 'DuenoEmpresa', 202020202, 'alopez@geems.com'),
(@uid3, 'lramirez', 'pass9999', 'Empleado', 303030303, 'lramirez@geems.com'),
(@uid5, 'mcastro', 'clave123', 'Empleado', 404040404, 'mcastro@geems.com'),
(@uid6, 'pgomez', 'clave321', 'Empleado', 505050505, 'pgomez@geems.com'),
(@uid9, 'cperez', 'pass777', 'SuperAdmin', 101010101, 'cperez@geems.com');

-- SuperAdmin
INSERT INTO SuperAdmin VALUES (@uid9, 101010101);

-- Empresas
INSERT INTO Empresa VALUES 
('3-101-1234567', 'GEEMS Solutions', 'Consultora en gestión empresarial', '2222-2222', 'info@geems.com', 'San José', 'San José', 'Carmen', '300 m norte del parque central'),
('1-555-9876543', 'Innova Corp', 'Soluciones digitales y tecnológicas', '2100-3333', 'contacto@innova.com', 'Heredia', 'Heredia', 'San Francisco', 'Detrás del parque industrial');

-- Asociación SuperAdmin → Empresa
INSERT INTO SuperAdminAdministraEmpresa VALUES (@uid9, '3-101-1234567');

-- Dueño de Empresa
INSERT INTO DuenoEmpresa VALUES (@uid2, '3-101-1234567', 202020202);

-- Datos privados empresa
INSERT INTO DatosPrivadosEmpresa VALUES 
('3-101-1234567', 'Mensual', 3),
('1-555-9876543', 'Trimestral', 2);

-- Beneficio
INSERT INTO Beneficio VALUES (@uid4, 10000, 6, 'Membresía en gimnasio premium', 'Gimnasio Premium', '3-101-1234567');

-- Elegibilidad del beneficio
INSERT INTO BeneficioContratoElegible VALUES (@uid4, 'Tiempo Completo');

-- Empleados
INSERT INTO Empleado VALUES 
(@uid3, 303030303, 'Tiempo Completo', 160, 'M', 'Activo', 850000, 'Colaborador', '2024-06-15', 'GEEMS Solutions'),
(@uid5, 404040404, 'Tiempo Completo', 160, 'F', 'Activo', 950000, 'Supervisor', '2023-11-01', 'GEEMS Solutions'),
(@uid6, 505050505, 'Tiempo Completo', 160, 'M', 'Activo', 1000000, 'Payroll', '2023-01-20', 'GEEMS Solutions');

-- Supervisor supervisa a colaborador
INSERT INTO SupervisorSupervisaEmpleado VALUES (@uid5, 303030303);

-- Beneficio otorgado al colaborador
INSERT INTO BeneficiosEmpleado VALUES (@uid3, @uid4);

-- Planilla
INSERT INTO Planilla VALUES (@uidPlanilla, '2025-04-01', '2025-04-30', @uid6);

-- Pago (bruto: 850000, neto calculado tras deducciones)
INSERT INTO Pago (Id, MontoPago, IdEmpleado, IdPayroll, IdPlanilla, MontoBruto, FechaInicio, FechaFinal)
VALUES (@uid7, 765000, @uid3, @uid6, @uidPlanilla, 850000, '2025-04-01', '2025-04-30');

-- Deducción obligatoria
INSERT INTO Deducciones (Id, IdPago, TipoDeduccion, IdBeneficio, Monto)
VALUES (@uidDeduccion1, @uid7, 'Obligatoria', NULL, 50000);

-- Deducción voluntaria por beneficio
INSERT INTO Deducciones (Id, IdPago, TipoDeduccion, IdBeneficio, Monto)
VALUES (@uidDeduccion2, @uid7, 'Voluntaria', @uid4, 35000);


-- Registro de horas del colaborador (ya aprobadas)
INSERT INTO Registro VALUES (@uid8, 160, '2025-04-20', 'Aprobado', @uid3);

--Horas De prueba 
-- DECLARE @uid23 UNIQUEIDENTIFIER = '4E24A20B-6787-43D1-AF95-953E752015DE';
-- INSERT INTO Registro VALUES (NEWID(), 160, '2025-04-20', 'Aprobado', @uid23);
-- INSERT INTO Registro VALUES (NEWID(), 160, '2025-04-20', 'Aprobado', @uid23);
-- INSERT INTO Registro VALUES (NEWID(), 160, '2025-04-20', 'Aprobado', @uid23);
-- INSERT INTO Registro VALUES (NEWID(), 80, '2025-04-20', 'Aprobado', @uid23);
-- INSERT INTO Registro VALUES (NEWID(), 80, '2025-04-20', 'NoRevisado', @uid23);

SELECT * FROM Usuario

