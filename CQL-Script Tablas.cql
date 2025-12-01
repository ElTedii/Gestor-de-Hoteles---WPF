CREATE KEYSPACE IF NOT EXISTS hotel_ks
WITH REPLICATION = {'class': 'SimpleStrategy', 'replication_factor': 1};

USE hotel_ks;

SELECT * FROM usuarios;

INSERT INTO usuarios(correo, contrasena, nombre_completo, nomina, fecha_nacimiento, tel_casa, tel_celular, usuario_creacion, fecha_creacion,usuario_modificacion, fecha_modificacion)
VALUES('t@tedo.com', '1234', 'Ismael Cruz', 9499, '2001-02-24', '8186892157', '812984346', 'tedo', '2025-11-30', 'si', '2025-11-30');

CREATE TABLE IF NOT EXISTS usuarios (
    correo text PRIMARY KEY,
    contrasena text,
    nombre_completo text,

    nomina int,
    fecha_nacimiento timestamp,
    tel_casa text,
    tel_celular text,

    usuario_creacion text,
    fecha_creacion timestamp,
    usuario_modificacion text,
    fecha_modificacion timestamp
);


CREATE TABLE IF NOT EXISTS clientes (
    cliente_id uuid PRIMARY KEY,
    nombre_completo text,

    pais text,
    estado text,
    ciudad text,

    rfc text,
    correo text,
    tel_casa text,
    tel_celular text,
    fecha_nacimiento timestamp,
    estado_civil text,

    usuario_registro text,
    fecha_registro timestamp,
    usuario_modificacion text,
    fecha_modificacion timestamp
);

CREATE TABLE hoteles (
    hotel_id uuid PRIMARY KEY,
    nombre text,
    pais text,
    estado text,
    ciudad text,
    domicilio text,
    num_pisos int,
    zona_turistica text,
    servicios list<text>,
    frente_playa boolean,
    num_piscinas int,
    salones_eventos int,        -- ya quedamos que se queda como INT
    usuario_registro text,
    fecha_registro timestamp,
    usuario_modificacion text,
    fecha_modificacion timestamp
);

CREATE TABLE IF NOT EXISTS tipos_habitacion_por_hotel (
    hotel_id            uuid,
    tipo_id             uuid,
    nombre_tipo         text,
    capacidad           int,
    precio_noche        decimal,
    cantidad            int,
    caracteristicas     list<text>,
    amenidades          list<text>,
    nivel               text,
    vista               text,
    usuario_registro    text,
    fecha_registro      timestamp,
    fecha_modificacion  timestamp,
    PRIMARY KEY (hotel_id, tipo_id)
);


CREATE TABLE habitaciones (
    hotel_id uuid,
    numero int,
    tipo_id uuid,
    piso int,
    usuario_creacion text,
    fecha_creacion timestamp,
    usuario_modificacion text,
    fecha_modificacion timestamp,
    PRIMARY KEY (hotel_id, numero)
);

CREATE TABLE IF NOT EXISTS reservas_por_cliente (
    cliente_id          uuid,
    reserva_id          uuid,
    hotel_id            uuid,
    fecha_entrada       timestamp,
    fecha_salida        timestamp,
    anticipo            decimal,
    estado              text,
    adultos             int,
    menores             int,
    usuario_registro    text,
    fecha_registro      timestamp,
    PRIMARY KEY (cliente_id, reserva_id)
);

use hotel_ks;
INSERT INTO reservas_por_cliente (cliente_id, reserva_id, hotel_id, fecha_entrada, fecha_salida, anticipo, estado, adultos, menores, usuario_registro, fecha_registro) 
VALUES (11111111-1111-1111-1111-111111111111, 33333333-3333-3333-3333-333333333333, 22222222-2222-2222-2222-222222222222, toTimestamp(now()), '2025-12-03', 500, 'PENDIENTE', 2, 0, 'admin', toTimestamp(now()));

INSERT INTO reservas_por_hotel (
    hotel_id, fecha_entrada, reserva_id,
    cliente_id, fecha_salida,
    estado
) VALUES (
    22222222-2222-2222-2222-222222222222,
    toTimestamp(now()),
    33333333-3333-3333-3333-333333333333,
    11111111-1111-1111-1111-111111111111,
    '2025-12-03',
    'PENDIENTE'
);

INSERT INTO reservas_por_fecha (
    fecha_entrada, reserva_id,
    hotel_id, cliente_id,
    estado
) VALUES (
    toTimestamp(now()),
    33333333-3333-3333-3333-333333333333,
    22222222-2222-2222-2222-222222222222,
    11111111-1111-1111-1111-111111111111,
    'PENDIENTE'
);

CREATE TABLE IF NOT EXISTS reservas_por_hotel (
    hotel_id            uuid,
    fecha_entrada       timestamp,
    reserva_id          uuid,
    cliente_id          uuid,
    fecha_salida        timestamp,
    estado              text,
    PRIMARY KEY (hotel_id, fecha_entrada, reserva_id)
) WITH CLUSTERING ORDER BY (fecha_entrada ASC);

CREATE TABLE IF NOT EXISTS reservas_por_fecha (
    fecha_entrada       timestamp,
    reserva_id          uuid,
    hotel_id            uuid,
    cliente_id          uuid,
    estado              text,
    PRIMARY KEY (fecha_entrada, reserva_id)
);

CREATE TABLE IF NOT EXISTS estancias_activas (
    hotel_id uuid,
    numero int,
    estancia_id uuid,
    cliente_id uuid,
    reserva_id uuid,
    fecha_entrada timestamp,
    fecha_salida timestamp,
    anticipo decimal,
    precio_noche decimal,
    adultos int,
    menores int,

    -- AUDITORÃ�A -------------------------
    usuario_registro text,
    fecha_registro timestamp,
    --------------------------------------

    PRIMARY KEY (hotel_id, numero)
);

CREATE TABLE IF NOT EXISTS historial_estancias (
    cliente_id          uuid,
    estancia_id         uuid,
    hotel_id            uuid,
    numero_habitacion   int,
    fecha_entrada       timestamp,
    fecha_salida        timestamp,
    anticipo            decimal,
    monto_hospedaje     decimal,
    monto_servicios     decimal,
    total_factura       decimal,
    usuario_registro    text,
    fecha_registro      timestamp,
    PRIMARY KEY (cliente_id, estancia_id)
);

CREATE TABLE IF NOT EXISTS facturas (
    factura_id        uuid,
    estancia_id       uuid,
    cliente_id        uuid,
    hotel_id          uuid,
    fecha_emision     timestamp,
    monto_hospedaje   decimal,
    monto_servicios   decimal,
    total             decimal,
    usuario_registro  text,
    fecha_registro    timestamp,
    PRIMARY KEY (factura_id)
);

use hotel_ks;

