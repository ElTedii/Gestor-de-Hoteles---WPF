---------------------------------------------------------
-- CREAR KEYSPACE
---------------------------------------------------------
CREATE KEYSPACE IF NOT EXISTS hotel_ks
WITH REPLICATION = {
    'class': 'SimpleStrategy',
    'replication_factor': 1
};

USE hotel_ks;

---------------------------------------------------------
-- 1) USUARIOS
---------------------------------------------------------
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

---------------------------------------------------------
-- 2) CLIENTES
---------------------------------------------------------
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

---------------------------------------------------------
-- 3) HOTELES
---------------------------------------------------------
CREATE TABLE IF NOT EXISTS hoteles (
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
    salones_eventos int,
    usuario_registro text,
    fecha_registro timestamp,
    usuario_modificacion text,
    fecha_modificacion timestamp
);

---------------------------------------------------------
-- 4) TIPOS DE HABITACIÓN (POR HOTEL)
---------------------------------------------------------
CREATE TABLE IF NOT EXISTS tipos_habitacion_por_hotel (
    hotel_id uuid,
    tipo_id uuid,
    nombre_tipo text,
    capacidad int,
    precio_noche decimal,
    cantidad int,
    caracteristicas list<text>,
    amenidades list<text>,
    nivel text,
    vista text,
    usuario_registro text,
    fecha_registro timestamp,
    usuario_modificacion text,
    fecha_modificacion timestamp,
    PRIMARY KEY (hotel_id, tipo_id)
) WITH CLUSTERING ORDER BY (tipo_id ASC);

---------------------------------------------------------
-- 5) HABITACIONES
---------------------------------------------------------
CREATE TABLE IF NOT EXISTS habitaciones (
    hotel_id uuid,
    numero int,
    tipo_id uuid,
    piso int,
    estado text,
    usuario_creacion text,
    fecha_creacion timestamp,
    usuario_modificacion text,
    fecha_modificacion timestamp,
    PRIMARY KEY (hotel_id, numero)
);

---------------------------------------------------------
-- 6) RESERVAS POR CLIENTE
---------------------------------------------------------
CREATE TABLE IF NOT EXISTS reservas_por_cliente (
    cliente_id uuid,
    reserva_id uuid,
    hotel_id uuid,
    fecha_entrada timestamp,
    fecha_salida timestamp,
    anticipo decimal,
    estado text,
    adultos int,
    menores int,
    usuario_registro text,
    fecha_registro timestamp,
    PRIMARY KEY (cliente_id, reserva_id)
);

---------------------------------------------------------
-- 7) RESERVAS POR HOTEL
---------------------------------------------------------
CREATE TABLE IF NOT EXISTS reservas_por_hotel (
    hotel_id uuid,
    fecha_entrada timestamp,
    reserva_id uuid,
    cliente_id uuid,
    fecha_salida timestamp,
    estado text,
    PRIMARY KEY (hotel_id, fecha_entrada, reserva_id)
) WITH CLUSTERING ORDER BY (fecha_entrada DESC, reserva_id ASC);

---------------------------------------------------------
-- 8) RESERVAS POR FECHA
---------------------------------------------------------
CREATE TABLE IF NOT EXISTS reservas_por_fecha (
    fecha_entrada timestamp,
    reserva_id uuid,
    hotel_id uuid,
    cliente_id uuid,
    estado text,
    PRIMARY KEY (fecha_entrada, reserva_id)
);

---------------------------------------------------------
-- 9) ESTANCIAS ACTIVAS
---------------------------------------------------------
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
    usuario_registro text,
    fecha_registro timestamp,
    PRIMARY KEY (hotel_id, numero)
);

---------------------------------------------------------
-- 10) HISTORIAL DE ESTANCIAS
---------------------------------------------------------
CREATE TABLE IF NOT EXISTS historial_estancias (
    cliente_id uuid,
    estancia_id uuid,
    hotel_id uuid,
    numero_habitacion int,
    fecha_entrada timestamp,
    fecha_salida timestamp,
    anticipo decimal,
    monto_hospedaje decimal,
    monto_servicios decimal,
    total_factura decimal,
    usuario_registro text,
    fecha_registro timestamp,
    PRIMARY KEY (cliente_id, estancia_id)
);

---------------------------------------------------------
-- 11) FACTURAS
---------------------------------------------------------
CREATE TABLE IF NOT EXISTS facturas (
    factura_id uuid PRIMARY KEY,
    estancia_id uuid,
    cliente_id uuid,
    hotel_id uuid,
    fecha_emision timestamp,
    monto_hospedaje decimal,
    monto_servicios decimal,
    total decimal,
    usuario_registro text,
    fecha_registro timestamp
);

---------------------------------------------------------
-- 12) REPORTES - OCUPACIÓN
---------------------------------------------------------
CREATE TABLE IF NOT EXISTS reportes_ocupacion (
    hotel_id uuid,
    anio int,
    mes int,
    fecha_checkin timestamp,
    reserva_id uuid,
    cliente_id uuid,
    cliente_nombre text,
    tipo_habitacion text,
    numero_habitacion int,
    fecha_checkout timestamp,
    estado text,
    pago_hospedaje decimal,
    pago_servicios decimal,
    PRIMARY KEY ((hotel_id, anio, mes), fecha_checkin, numero_habitacion)
) WITH CLUSTERING ORDER BY (fecha_checkin DESC, numero_habitacion ASC);

---------------------------------------------------------
-- 13) REPORTES - VENTAS POR MES (NUEVA)
---------------------------------------------------------
CREATE TABLE IF NOT EXISTS ventas_por_mes (
    hotel_id uuid,
    anio int,
    mes int,
    monto_hospedaje decimal,
    monto_servicios decimal,
    total decimal,
    PRIMARY KEY (hotel_id, anio, mes)
);