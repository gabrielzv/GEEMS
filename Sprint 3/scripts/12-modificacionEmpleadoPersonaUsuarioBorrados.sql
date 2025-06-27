
ALTER TABLE Empleado
ADD EstaBorrado BIT NOT NULL DEFAULT 0;

ALTER TABLE Persona
ADD EstaBorrado BIT NOT NULL DEFAULT 0;

ALTER TABLE Usuario
ADD EstaBorrado BIT NOT NULL DEFAULT 0;

select * from Empleado;
select *from Persona;
select * from Usuario;
select * from Planilla;
select * from Registro;
select * from Pago;
select * from DuenoEmpresa;

ALTER TABLE DuenoEmpresa
ADD EstaBorrado BIT NOT NULL DEFAULT 0;

delete from  Persona where cedula ='208360895';
delete from  Usuario where CedulaPersona ='208360895';

delete from  Empleado where CedulaPersona ='208360895';
ALTER TABLE Empresa
ADD EstaBorrado BIT NOT NULL DEFAULT 0;

