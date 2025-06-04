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

-- Registro

INSERT INTO API_guardadas (
    nombre_api,
    metodo,
    url_completa,
    nombre_key_header,
    valor_key_header
) VALUES (
    'Registro Nacional',
    'POST',
    'https://registro-blitz-cegnaceuhnbsbdak.southcentralus-01.azurewebsites.net/api/NationalRegister/validate',
    'Authorization',
    '123Blitz'
);

INSERT INTO API_guardadas (
    nombre_api,
    metodo,
    url_completa,
    nombre_key_header,
    valor_key_header
) VALUES (
    'Poliza Seguros',
    'GET',
    'https://poliza-friends-grg0h9g5crf2hwh8.southcentralus-01.azurewebsites.net/api/LifeInsurance',
    'FRIENDS-API-TOKEN',
    '1D0B194488C091852597B9AF7D1AA8F23D55C9784815489CF0A488B6F2C6D5C4C569AD51231FACB9920E5A763FE388247E03131D1601AC234E86BC0D266EB6A7'
);

INSERT INTO API_guardadas (
    nombre_api,
    metodo,
    url_completa,
    nombre_key_header,
    valor_key_header
) VALUES (
    'Asociacion Calculator',
    'POST',
    'https://asociacion-geems-c3dfavfsapguhxbp.southcentralus-01.azurewebsites.net/api/public/calculator/calculate',
    'API-KEY',
    'Tralalerotralala'
);

INSERT INTO API_guardadas (
    nombre_api,
    metodo,
    url_completa,
    nombre_key_header,
    valor_key_header
) VALUES (
    'MediSeguro',
    'POST',
    'https://mediseguro-vorlagenersteller-d4hmbvf7frg7aqan.southcentralus-01.azurewebsites.net/api/MediSeguroMonto',
    'token',
    'TOKEN123'
);

select * FROM API_guardadas;