CREATE DATABASE MiAppWeb;



CREATE TABLE Usuarios (
    Id INT PRIMARY KEY IDENTITY(1,1),
    NombreUsuario NVARCHAR(50) NOT NULL UNIQUE,
    Contrasena NVARCHAR(255) NOT NULL
);

INSERT INTO Usuarios (NombreUsuario, Contrasena)
VALUES ('admin', '1234');
