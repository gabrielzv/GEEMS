USE GEEMSDB;

-- Se agrega atributo ModalidadPago a la tabla empresa
ALTER TABLE Empresa
ADD ModalidadPago NVARCHAR(10)
CHECK (ModalidadPago IN ('Semanal', 'Quincenal', 'Mensual'));

-- Se agrega el atributo para definir la máxima cantidad de beneficios que puede matricular un empleado
--ALTER TABLE Empresa
--ADD MaxBeneficiosXEmpleado INT;