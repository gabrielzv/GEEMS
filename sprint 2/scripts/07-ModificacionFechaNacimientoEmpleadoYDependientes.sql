use GEEMSDB

ALTER TABLE Empleado
ADD 
    FechaNacimiento DATE,
    NumDependientes INT DEFAULT 0;