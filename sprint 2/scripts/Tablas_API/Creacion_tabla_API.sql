USE GEEMSDB;

CREATE TABLE API_guardadas (
    id INT PRIMARY KEY IDENTITY(1,1), -- Lo puse autoincremental
    nombre_api VARCHAR(100) NOT NULL,
    metodo VARCHAR(10) NOT NULL, -- GET/POST
    url_completa VARCHAR(500) NOT NULL,
    nombre_key_header VARCHAR(50) NOT NULL,
    valor_key_header VARCHAR(255) NOT NULL
);