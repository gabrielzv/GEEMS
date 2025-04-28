-- Crear la base de datos
CREATE DATABASE UsuariosDB;

-- Usar la base de datos recién creada
drop table  Usuarios;
-- Crear la tabla de usuarios
CREATE TABLE Usuarios (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    nombreUsuario NVARCHAR(100) NOT NULL,
    contrasena NVARCHAR(255) NOT NULL,
  
);

INSERT INTO Usuarios (nombreUsuario, contrasena)
VALUES ('geems@gmail.com', '1234');

select * , nombreUsuario from Usuarios;