USE GEEMSDB;
-- Generación de identificadores únicos
DECLARE @uid1 UNIQUEIDENTIFIER = NEWID();
DECLARE @uid2 UNIQUEIDENTIFIER = NEWID();
DECLARE @uid3 UNIQUEIDENTIFIER = NEWID();
DECLARE @uid4 UNIQUEIDENTIFIER = NEWID();
DECLARE @uid5 UNIQUEIDENTIFIER = NEWID();
DECLARE @uid6 UNIQUEIDENTIFIER = NEWID();
DECLARE @uid7 UNIQUEIDENTIFIER = NEWID();
DECLARE @uid8 UNIQUEIDENTIFIER = NEWID();

-- Persona
INSERT INTO Persona VALUES (101, 'Av Central', 'Carlos', 'Perez', 'Mora', 'geems@gmail.com', '8888-8888');
INSERT INTO Persona VALUES (102, 'Calle 2', 'Ana', 'Lopez', 'Soto', 'ana@example.com', '8777-7777');
INSERT INTO Persona VALUES (103, 'Calle 3', 'Luis', 'Ramirez', 'Acosta', 'luis@example.com', '8666-6666');

-- Usuario
INSERT INTO Usuario VALUES (@uid1, 'geems@gmail.com', '1234', 'SuperAdmin', 101, 'geems@gmail.com');
INSERT INTO Usuario VALUES (@uid2, 'alopez', 'pass5678', 'DuenoEmpresa', 102, 'ana@example.com');
INSERT INTO Usuario VALUES (@uid3, 'lramirez', 'pass9999', 'Empleado', 103,'luis@example.com');

-- SuperAdmin
INSERT INTO SuperAdmin VALUES (@uid1, 101);

-- Empresa
INSERT INTO Empresa VALUES ('1-222-3333333', 'GEEMS Solutions', 'Consultora de RRHH', '2222-2222', 'empresa@example.com', 'San José', 'San José', 'Carmen', '300 m norte del parque');
INSERT INTO Empresa VALUES ('666', 'Lol Solutions', 'Consultora de RRHH', '2222-2222', 'empresa@example.com', 'San José', 'San José', 'Carmen', '300 m norte del parque');

-- Relación SuperAdmin-Empresa
INSERT INTO SuperAdminAdministraEmpresa VALUES (@uid1, '1-222-3333333');

-- Dueño de Empresa
INSERT INTO DuenoEmpresa VALUES (@uid2, '1-222-3333333', 102);

-- Datos Privados Empresa
INSERT INTO DatosPrivadosEmpresa VALUES ('1-222-3333333', 'Mensual', 2);

-- Beneficio
INSERT INTO Beneficio VALUES (@uid4, 5000, 6, 'Gimnasio Corporativo', 'Gimnasio');

-- Beneficios disponibles
INSERT INTO BeneficiosDisponibles VALUES (@uid4, '1-222-3333333');

-- Beneficio Contrato Elegible
INSERT INTO BeneficioContratoElegible VALUES (@uid4, 'Tiempo Completo');

-- Empleado (Colaborador)
INSERT INTO Empleado VALUES (@uid3, 103, 'Tiempo Completo', 160, 'M', 'Activo', 850000, 'Colaborador', GETDATE(), 'GEEMS Solutions');

-- Supervisor (también empleado)
INSERT INTO Persona VALUES (104, 'Calle 4', 'María', 'Castro', 'Jiménez', 'maria@example.com', '8555-5555');
INSERT INTO Usuario VALUES (@uid5, 'mcastro', 'clave123', 'Empleado', 104, 'maria@example.com');
INSERT INTO Empleado VALUES (@uid5, 104, 'Tiempo Completo', 160, 'F', 'Activo', 950000, 'Supervisor', GETDATE(), 'GEEMS Solutions');

-- Supervisor supervisa empleado
INSERT INTO SupervisorSupervisaEmpleado VALUES (@uid5, 103);

-- Payroll (también empleado)
INSERT INTO Persona VALUES (105, 'Calle 5', 'Pedro', 'Gomez', 'Sánchez', 'pedro@example.com', '8444-4444');
INSERT INTO Usuario VALUES (@uid6, 'pgomez', 'clave321', 'Empleado', 105, 'pedro@example.com');
INSERT INTO Empleado VALUES (@uid6, 105, 'Tiempo Completo', 160, 'M', 'Activo', 1000000, 'Payroll', GETDATE(), 'GEEMS Solutions');

-- Pago
INSERT INTO Pago VALUES (@uid7, GETDATE(), 'Seguro Social', 850000, 75000, 'Abril 2025', @uid3, @uid6);

-- Registro de horas
INSERT INTO Registro VALUES (@uid8, 40, GETDATE(), 'Aprobado', @uid3);

-- BeneficiosEmpleado
INSERT INTO BeneficiosEmpleado VALUES (@uid3, @uid4);

-- HistorialDePagos
INSERT INTO HistorialDePagos VALUES (@uid3, @uid7, 'Abril', 1, GETDATE());
