USE GEEMSDB;

-- Se agrega atributo ModalidadPago a la tabla empresa
ALTER TABLE Empresa
ADD ModalidadPago NVARCHAR(10)
CHECK (ModalidadPago IN ('Semanal', 'Quincenal', 'Mensual'));