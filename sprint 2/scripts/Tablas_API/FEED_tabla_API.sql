USE GEEMSDB;

-- TSE
INSERT INTO API_guardadas (
    nombre_api,
    metodo,
    url_completa,
    nombre_key_header,
    valor_key_header
) VALUES (
    'Validacion TSE',
    'GET',
    'https://tse-infinipay-deengcb5bqazhdh0.southcentralus-01.azurewebsites.net/api/TSE',
    'Auth-Token',
    '789xyz$%&'
);

select * FROM API_guardadas;