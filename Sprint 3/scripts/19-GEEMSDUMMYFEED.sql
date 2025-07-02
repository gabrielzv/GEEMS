-- FEED DE DATOS GEEMS - ADAPTADO Y COMPLETO

USE GEEMSDB;

-- Declaración de IDs fijos para relaciones
DECLARE @uidAdmin UNIQUEIDENTIFIER = '00000000-0000-0000-0000-000000000001';
DECLARE @uidDueno UNIQUEIDENTIFIER = '00000000-0000-0000-0000-000000000002';
DECLARE @uidColab UNIQUEIDENTIFIER = '00000000-0000-0000-0000-000000000003';
DECLARE @uidSup UNIQUEIDENTIFIER = '00000000-0000-0000-0000-000000000004';
DECLARE @uidPayroll UNIQUEIDENTIFIER = '00000000-0000-0000-0000-000000000005';
DECLARE @uidPago UNIQUEIDENTIFIER = '00000000-0000-0000-0000-000000000006';
DECLARE @uidPago2 UNIQUEIDENTIFIER = '00000000-0000-0000-0000-000000000016';
DECLARE @uidRegistro UNIQUEIDENTIFIER = '00000000-0000-0000-0000-000000000007';
DECLARE @uidRegistro2 UNIQUEIDENTIFIER = '00000000-0000-0000-0000-000000000017';
DECLARE @uidPlanilla UNIQUEIDENTIFIER = '00000000-0000-0000-0000-000000000008';
DECLARE @uidPlanilla2 UNIQUEIDENTIFIER = '00000000-0000-0000-0000-000000000018';
DECLARE @uidBeneficio UNIQUEIDENTIFIER = '00000000-0000-0000-0000-000000000009';
DECLARE @uidBeneficio2 UNIQUEIDENTIFIER = '00000000-0000-0000-0000-000000000019';
DECLARE @uidDeduccion1 UNIQUEIDENTIFIER = '00000000-0000-0000-0000-000000000010';
DECLARE @uidDeduccion2 UNIQUEIDENTIFIER = '00000000-0000-0000-0000-000000000011';

-- Personas
INSERT INTO Persona VALUES 
(101010101, 'Av Central', 'Carlos', 'Perez', 'Mora', 'cperez@geems.com', '8888-8888', 0),
(202020202, 'Calle 2', 'Ana', 'Lopez', 'Soto', 'alopez@geems.com', '8777-7777', 0),
(303030303, 'Calle 3', 'Luis', 'Ramirez', 'Acosta', 'lramirez@geems.com', '8666-6666', 0),
(404040404, 'Calle 4', 'María', 'Castro', 'Jiménez', 'mcastro@geems.com', '8555-5555', 0),
(505050505, 'Calle 5', 'Pedro', 'Gomez', 'Sánchez', 'pgomez@geems.com', '8444-4444', 0);

-- Usuarios
INSERT INTO Usuario VALUES 
(@uidAdmin, 'cperez', 'pass777', 'SuperAdmin', 101010101, 'cperez@geems.com', 0),
(@uidDueno, 'alopez', 'pass5678', 'DuenoEmpresa', 202020202, 'alopez@geems.com', 0),
(@uidColab, 'lramirez', 'pass9999', 'Empleado', 303030303, 'lramirez@geems.com', 0),
(@uidSup, 'mcastro', 'clave123', 'Empleado', 404040404, 'mcastro@geems.com', 0),
(@uidPayroll, 'pgomez', 'clave321', 'Empleado', 505050505, 'pgomez@geems.com', 0);

-- SuperAdmin
INSERT INTO SuperAdmin VALUES (@uidAdmin, 101010101);

-- Empresas
INSERT INTO Empresa VALUES 
('3-101-1234567', 'GEEMS Solutions', 'Consultora en gestión empresarial', '2222-2222', 'info@geems.com', 'San José', 'San José', 'Carmen', '300 m norte del parque central', 'Mensual', 3, 0),
('1-555-9876543', 'Innova Corp', 'Soluciones digitales y tecnológicas', '2100-3333', 'contacto@innova.com', 'Heredia', 'Heredia', 'San Francisco', 'Detrás del parque industrial', 'Quincenal', 4, 0);

-- Asociación SuperAdmin → Empresa
INSERT INTO SuperAdminAdministraEmpresa VALUES (@uidAdmin, '3-101-1234567');

-- Dueño de Empresa
INSERT INTO DuenoEmpresa VALUES (@uidDueno, '3-101-1234567', 202020202, 0);

-- Datos privados empresa
INSERT INTO DatosPrivadosEmpresa VALUES 
('3-101-1234567', 'Mensual', 3),
('1-555-9876543', 'Trimestral', 2);

-- Beneficios
INSERT INTO Beneficio VALUES (@uidBeneficio, 10000, 6, 'Membresía en gimnasio premium', 'Gimnasio Premium', '3-101-1234567', 'Mensual', 'BeneficioNormal', 0, 0, 'Activo', 0);
INSERT INTO Beneficio VALUES (@uidBeneficio2, 5000, 12, 'Seguro de vida', 'Seguro Vida', '3-101-1234567', 'Mensual', 'BeneficioNormal', 0, 0, 'Activo', 0);

-- Elegibilidad del beneficio
INSERT INTO BeneficioContratoElegible VALUES (@uidBeneficio, 'Tiempo Completo');
INSERT INTO BeneficioContratoElegible VALUES (@uidBeneficio2, 'Medio Tiempo');

-- Empleados (con distintos contratos y empresas)
INSERT INTO Empleado (
    Id, CedulaPersona, Contrato, NumHorasTrabajadas, Genero, EstadoLaboral, SalarioBruto, Tipo, FechaIngreso, NombreEmpresa, FechaNacimiento, NumDependientes
) VALUES 
(@uidColab, 303030303, 'Tiempo Completo', 160, 'M', 'Activo', 850000, 'Colaborador', '2024-06-15', 'GEEMS Solutions', '1990-01-01', 2),
(@uidSup, 404040404, 'Medio Tiempo', 80, 'F', 'Activo', 500000, 'Colaborador', '2024-07-01', 'GEEMS Solutions', '1992-02-02', 1),
(@uidPayroll, 505050505, 'Tiempo Completo', 160, 'M', 'Activo', 1000000, 'Payroll', '2023-01-20', 'GEEMS Solutions', '1988-03-03', 0);

-- Supervisor supervisa a colaborador
INSERT INTO SupervisorSupervisaEmpleado VALUES (@uidSup, 303030303);

-- Beneficio otorgado al colaborador
INSERT INTO BeneficiosEmpleado VALUES (@uidColab, @uidBeneficio, '2025-04-01');
INSERT INTO BeneficiosEmpleado VALUES (@uidSup, @uidBeneficio2, '2025-04-01');

-- Planillas
INSERT INTO Planilla VALUES (@uidPlanilla, '2025-04-01', '2025-04-30', @uidPayroll);
INSERT INTO Planilla VALUES (@uidPlanilla2, '2025-05-01', '2025-05-31', @uidPayroll);

-- Registros de horas (aprobadas)
INSERT INTO Registro VALUES (@uidRegistro, 40, '2025-04-20', 'Aprobado', @uidColab,0);
INSERT INTO Registro VALUES (@uidRegistro2, 20, '2025-04-21', 'Aprobado', @uidSup,0);

-- Registro para UI Test
INSERT INTO Registro (Id, NumHoras, Fecha, Estado, IdEmpleado, EstaBorrado)
VALUES ('2E168580-7ED0-43B8-B467-AC282BE870A9', 5, '2025-07-02 00:00:00.000', 'NoRevisado', '00000000-0000-0000-0000-000000000003', 0);


-- Pagos (bruto: 850000, neto calculado tras deducciones)
INSERT INTO Pago (Id, FechaRealizada, MontoPago, IdEmpleado, IdPayroll, IdPlanilla, MontoBruto, FechaInicio, FechaFinal)
VALUES (@uidPago, GETDATE(), 765000, @uidColab, @uidPayroll, @uidPlanilla, 850000, '2025-04-01', '2025-04-30');
INSERT INTO Pago (Id, FechaRealizada, MontoPago, IdEmpleado, IdPayroll, IdPlanilla, MontoBruto, FechaInicio, FechaFinal)
VALUES (@uidPago2, GETDATE(), 450000, @uidSup, @uidPayroll, @uidPlanilla, 500000, '2025-04-01', '2025-04-30');

-- Deducciones
INSERT INTO Deducciones (Id, IdPago, TipoDeduccion, IdBeneficio, Monto)
VALUES (@uidDeduccion1, @uidPago, 'Obligatoria', NULL, 50000);
INSERT INTO Deducciones (Id, IdPago, TipoDeduccion, IdBeneficio, Monto)
VALUES (@uidDeduccion2, @uidPago, 'Voluntaria', @uidBeneficio, 35000);


-- Consultas de prueba
SELECT * FROM Persona;
SELECT * FROM Usuario;
SELECT * FROM SuperAdmin;
SELECT * FROM Empresa;
SELECT * FROM SuperAdminAdministraEmpresa;
SELECT * FROM DuenoEmpresa;
SELECT * FROM DatosPrivadosEmpresa;
SELECT * FROM Beneficio;
SELECT * FROM BeneficioContratoElegible;
SELECT * FROM Empleado;
SELECT * FROM SupervisorSupervisaEmpleado;
SELECT * FROM BeneficiosEmpleado;
SELECT * FROM Planilla;
SELECT * FROM Registro;
SELECT * FROM Pago;
SELECT * FROM Deducciones;
SELECT * FROM API_guardadas;