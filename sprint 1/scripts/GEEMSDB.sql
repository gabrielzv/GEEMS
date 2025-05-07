USE master;

GO
ALTER DATABASE GEEMSDB
SET
	SINGLE_USER
WITH
	ROLLBACK IMMEDIATE;

GO
DROP DATABASE GEEMSDB;


Create DATABASE GEEMSDB;
GO
 use GEEMSDB;

CREATE TABLE
	Persona (
		Cedula INT NOT NULL PRIMARY KEY,
		Direccion NVARCHAR (200),
		NombrePila NVARCHAR (30),
		Apellido1 NVARCHAR (30),
		Apellido2 NVARCHAR (30),
		Correo VARCHAR(100) UNIQUE,
		Telefono VARCHAR(100)
	);

CREATE TABLE
	Usuario (
		Id UNIQUEIDENTIFIER NOT NULL,
		Username NVARCHAR (32) UNIQUE NOT NULL,
		Contrasena VARCHAR(32) NOT NULL,
		Tipo VARCHAR(20) CHECK (
			Tipo IN ('DuenoEmpresa', 'Empleado', 'SuperAdmin')
		),
		CedulaPersona INT UNIQUE NOT NULL,
		CorreoPersona VARCHAR(100) NOT NULL,
		FOREIGN KEY (CorreoPersona) REFERENCES Persona (Correo),
		FOREIGN KEY (CedulaPersona) REFERENCES Persona (Cedula) ON UPDATE CASCADE
	);

CREATE TABLE
	SuperAdmin (
		Id UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
		CedulaPersona INT UNIQUE NOT NULL,
		FOREIGN KEY (CedulaPersona) REFERENCES Persona (Cedula) ON UPDATE CASCADE
	);

CREATE TABLE
	Empresa (
		CedulaJuridica NVARCHAR (20) NOT NULL PRIMARY KEY, -- Cambiado a NVARCHAR para permitir tamano estandar
		Nombre NVARCHAR (100) UNIQUE NOT NULL,
		Descripcion NVARCHAR (420) NOT NULL,
		Telefono VARCHAR(30) NOT NULL,
		Correo VARCHAR(30) NOT NULL,
		Provincia NVARCHAR (50) NOT NULL,
		Canton NVARCHAR (50) NOT NULL,
		Distrito NVARCHAR (50) NOT NULL,
		Senas NVARCHAR (250) NOT NULL,
	);

CREATE TABLE
	SuperAdminAdministraEmpresa (
		IdAdministrador UNIQUEIDENTIFIER NOT NULL,
		CedulaJuridicaEmpresa NVARCHAR (20) NOT NULL,
		PRIMARY KEY (IdAdministrador, CedulaJuridicaEmpresa),
		FOREIGN KEY (IdAdministrador) REFERENCES SuperAdmin (Id) ON UPDATE CASCADE,
		FOREIGN KEY (CedulaJuridicaEmpresa) REFERENCES Empresa (CedulaJuridica) ON UPDATE CASCADE
	)
CREATE TABLE
	DuenoEmpresa (
		Id UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
		CedulaEmpresa NVARCHAR (20) UNIQUE NOT NULL,
		CedulaPersona INT UNIQUE NOT NULL,
		FOREIGN KEY (CedulaEmpresa) REFERENCES Empresa (CedulaJuridica) ON UPDATE CASCADE,
		FOREIGN KEY (CedulaPersona) REFERENCES Persona (Cedula) ON UPDATE CASCADE
	);

CREATE TABLE
	DatosPrivadosEmpresa (
		CedulaJuridica NVARCHAR (20) NOT NULL PRIMARY KEY,
		PlazoPago VARCHAR(20) NOT NULL,
		CantidadBeneficiosXEmpleado INT NOT NULL,
		FOREIGN KEY (CedulaJuridica) REFERENCES Empresa (CedulaJuridica) ON UPDATE CASCADE
	);

CREATE TABLE
	Beneficio (
		Id UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
		Costo INT NOT NULL,
		--Formula NVARCHAR (20) NOT NULL,
		TiempoMinimoEnEmpresa int NOT NULL,
		Descripcion NVARCHAR (200) NOT NULL,
		Nombre NVARCHAR (32) NOT NULL,
		CedulaJuridica NVARCHAR (20) NOT NULL,
		FOREIGN KEY (CedulaJuridica) REFERENCES Empresa (CedulaJuridica) ON UPDATE CASCADE
	);

--CREATE TABLE
--	BeneficiosDisponibles (
--		IdBeneficio UNIQUEIDENTIFIER NOT NULL,
--		CedulaJuridicaEmpresa NVARCHAR(20) NOT NULL,
--		PRIMARY KEY (IdBeneficio, CedulaJuridicaEmpresa),
--		FOREIGN KEY (CedulaJuridicaEmpresa) REFERENCES Empresa (CedulaJuridica) ON UPDATE CASCADE,
--		FOREIGN KEY (IdBeneficio) REFERENCES Beneficio (Id) ON UPDATE CASCADE
--	);
CREATE TABLE
	BeneficioContratoElegible (
		IdBeneficio UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
		ContratoEmpleado VARCHAR(22) CHECK (
			ContratoEmpleado IN (
				'Tiempo Completo',
				'Medio Tiempo',
				'Servicios Profesionales',
				'PorHoras'
			)
		) FOREIGN KEY (IdBeneficio) REFERENCES Beneficio (Id) ON UPDATE CASCADE
	);

CREATE TABLE
	Empleado (
		Id UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
		CedulaPersona INT UNIQUE NOT NULL,
		Contrato VARCHAR(30) CHECK (
			Contrato IN (
				'Tiempo Completo',
				'Medio Tiempo',
				'Servicios Profesionales',
				'Por Horas'
			)
		),
		NumHorasTrabajadas INT,
		Genero CHAR NOT NULL CHECK (Genero IN ('M', 'F')),
		EstadoLaboral VARCHAR(10) CHECK (EstadoLaboral IN ('Activo', 'Inactivo')),
		SalarioBruto INT NOT NULL,
		Tipo VARCHAR(20) CHECK (Tipo IN ('Colaborador', 'Supervisor', 'Payroll')),
		FechaIngreso NVARCHAR (10),
		NombreEmpresa NVARCHAR (100) NOT NULL,
		FOREIGN KEY (CedulaPersona) REFERENCES Persona (Cedula) ON UPDATE CASCADE,
		FOREIGN key (NombreEmpresa) REFERENCES Empresa (Nombre) ON UPDATE CASCADE
	);

CREATE TABLE
	SupervisorSupervisaEmpleado (
		IdSupervisor UNIQUEIDENTIFIER NOT NULL,
		CedulaEmpleado INT NOT NULL,
		PRIMARY KEY (IdSupervisor, CedulaEmpleado),
		FOREIGN KEY (IdSupervisor) REFERENCES Empleado (Id) ON UPDATE CASCADE,
		FOREIGN KEY (CedulaEmpleado) REFERENCES Empleado (CedulaPersona)
	);

CREATE TABLE
	Pago (
		Id UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
		FechaRealizada DATETIME,
		TipoDeduccion VARCHAR(100) NOT NULL,
		MontoPago INT,
		MontoDeduccion INT,
		Periodo VARCHAR(100) NOT NULL,
		IdEmpleado UNIQUEIDENTIFIER NOT NULL,
		IdPayroll UNIQUEIDENTIFIER NOT NULL,
		FOREIGN KEY (IdEmpleado) REFERENCES Empleado (Id) ON UPDATE CASCADE,
		FOREIGN KEY (IdPayroll) REFERENCES Empleado (Id)
	);

CREATE TABLE
	Registro (
		Id UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
		NumHoras INT,
		Fecha DATETIME,
		Estado VARCHAR(20) CHECK (Estado IN ('NoRevisado', 'Aprobado', 'Denegado')),
		IdEmpleado UNIQUEIDENTIFIER NOT NULL,
		FOREIGN KEY (IdEmpleado) REFERENCES Empleado (Id) ON UPDATE CASCADE
	);

CREATE TABLE
	BeneficiosEmpleado (
		IdEmpleado UNIQUEIDENTIFIER NOT NULL,
		IdBeneficio UNIQUEIDENTIFIER NOT NULL,
		PRIMARY KEY (IdEmpleado, IdBeneficio),
		FOREIGN KEY (IdEmpleado) REFERENCES Empleado (Id) ON UPDATE NO ACTION,
		FOREIGN KEY (IdBeneficio) REFERENCES Beneficio (Id) ON UPDATE NO ACTION
	);

CREATE TABLE
	HistorialDePagos (
		IdEmpleado UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
		IdPago UNIQUEIDENTIFIER NOT NULL,
		Mes VARCHAR(11) CHECK (
			Mes IN (
				'Enero',
				'Febrero',
				'Marzo',
				'Abril',
				'Mayo',
				'Junio',
				'Julio',
				'Agosto',
				'Septiembre',
				'Octubre',
				'Noviembre',
				'Diciembre'
			)
		),
		Pagado BIT,
		FechadePago DATETIME,
		FOREIGN KEY (IdEmpleado) REFERENCES Empleado (Id) ON UPDATE CASCADE,
		FOREIGN KEY (IdPago) REFERENCES Pago (Id)
	);

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

--SELECT
--	*
--FROM
--	BeneficiosDisponibles;
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
	HistorialDePagos;