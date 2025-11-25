CREATE KEYSPACE IF NOT EXISTS hotel_ks
WITH REPLICATION = {'class': 'SimpleStrategy', 'replication_factor': 1};

use hotel_ks;
insert into usuarios (
    numero_nomina,
    nombre_completo,
    correo,
    contrasena,
    fecha_nacimiento,
    tel_casa,
    tel_celular,
    fecha_creacion,
    usuario_registro)
    Values (1, 'Ismael', 'isma.cvzz@gmail.com', '123', '2001-02-24', '4', '1', '2025-11-18', 1);
select * from usuarios;

/*Tablas*/
CREATE TABLE IF NOT EXISTS usuarios (
    numero_nomina       int PRIMARY KEY,
    nombre_completo     text,
    correo              text,
    contrasena          text,
    fecha_nacimiento    date,
    tel_casa            text,
    tel_celular         text,
    fecha_creacion      timestamp,
    usuario_registro int
);

CREATE TABLE IF NOT EXISTS hoteles (
    hotel_id            uuid PRIMARY KEY,
    nombre              text,
    pais                text,
    estado              text,
    ciudad              text,
    domicilio           text,
    num_pisos           int,
    zona_turistica      text,
    servicios           list<text>,
    frente_playa        boolean,
    num_piscinas        int,
    salones_eventos     int,
    usuario_registro    int,
    fecha_registro      timestamp,
    fecha_inicio_op     date,
    fecha_modificacion  timestamp
);

CREATE TABLE IF NOT EXISTS habitaciones (
    hotel_id            uuid,
    numero              int,
    tipo_id             uuid,
    piso                int,
    PRIMARY KEY (hotel_id, numero)
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
    usuario_registro    int,
    fecha_registro      timestamp,
    PRIMARY KEY (hotel_id, tipo_id)
);

CREATE TABLE IF NOT EXISTS disponibilidad_habitacion (
    hotel_id            uuid,
    numero              int,
    fecha               date,
    disponible          boolean,
    PRIMARY KEY ((hotel_id, numero), fecha)
) WITH CLUSTERING ORDER BY (fecha ASC);

CREATE TABLE IF NOT EXISTS clientes (
    cliente_id          uuid PRIMARY KEY,
    nombre_completo     text,
    pais                text,
    estado              text,
    ciudad              text,
    rfc                 text,
    correo              text,
    tel_casa            text,
    tel_celular         text,
    fecha_nacimiento    date,
    estado_civil        text,
    usuario_registro    int,
    usuario_modifico    int,
    fecha_creacion      timestamp,
    fecha_modificacion  timestamp
);

CREATE TABLE IF NOT EXISTS clientes_por_correo (
    correo          text PRIMARY KEY,
    cliente_id      uuid,
    nombre_completo text
);

CREATE TABLE IF NOT EXISTS clientes_por_rfc (
    rfc             text PRIMARY KEY,
    cliente_id      uuid,
    nombre_completo text
);

CREATE TABLE IF NOT EXISTS reservas_por_cliente (
    cliente_id          uuid,
    reserva_id          uuid,
    hotel_id            uuid,
    fecha_entrada       date,
    fecha_salida        date,
    anticipo            decimal,
    estado              text,
    usuario_registro    int,
    fecha_registro      timestamp,
    PRIMARY KEY (cliente_id, reserva_id)
);

CREATE TABLE IF NOT EXISTS reservas_por_hotel (
    hotel_id            uuid,
    fecha_entrada       date,
    reserva_id          uuid,
    cliente_id          uuid,
    fecha_salida        date,
    estado              text,
    PRIMARY KEY (hotel_id, fecha_entrada, reserva_id)
) WITH CLUSTERING ORDER BY (fecha_entrada ASC);

CREATE TABLE IF NOT EXISTS reservas_por_fecha (
    fecha_entrada       date,
    reserva_id          uuid,
    hotel_id            uuid,
    cliente_id          uuid,
    estado              text,
    PRIMARY KEY (fecha_entrada, reserva_id)
);

CREATE TABLE IF NOT EXISTS estancias_activas (
    hotel_id        uuid,
    numero          int,
    estancia_id     uuid,
    cliente_id      uuid,
    reserva_id      uuid,
    fecha_entrada   date,
    fecha_salida    date,
    anticipo        decimal,
    precio_noche    decimal,
    adultos         int,
    menores         int,
    PRIMARY KEY (hotel_id, numero)
);

CREATE TABLE IF NOT EXISTS historial_estancias (
    cliente_id          uuid,
    estancia_id         uuid,
    hotel_id            uuid,
    numero              int,
    fecha_entrada       date,
    fecha_salida        date,
    anticipo            decimal,
    monto_hospedaje     decimal,
    monto_servicios     decimal,
    total_factura       decimal,
    usuario_registro    int,
    fecha_registro      timestamp,
    PRIMARY KEY (cliente_id, estancia_id)
);

CREATE TABLE IF NOT EXISTS historial_estancias (
    cliente_id       UUID,
    estancia_id      UUID,
    hotel_id         UUID,
    habitacion_numero INT,
    fecha_entrada    DATE,
    fecha_salida     DATE,
    total_pagado     DECIMAL,
    metodo_pago      TEXT,
    PRIMARY KEY (cliente_id, estancia_id)
);

CREATE TABLE IF NOT EXISTS facturas (
    folio_factura    uuid PRIMARY KEY,
    cliente_id       uuid,
    hotel_id         uuid,
    reserva_id       uuid,
    estancia_id      uuid,
    fecha_emision    timestamp,
    fecha_entrada    date,
    fecha_salida     date,
    anticipo         decimal,
    monto_hospedaje  decimal,
    monto_servicios  decimal,
    descuento        decimal,
    total_factura    decimal
);



