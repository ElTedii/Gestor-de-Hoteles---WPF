/*QUERYS CQL
Dashboard
Estancias activas del día (por hotel)*/
SELECT * FROM estancias_activas
WHERE hotel_id = ?;
/*Reservaciones del día*/
SELECT * FROM reservaciones_por_fecha
WHERE fecha_entrada = ?;
/*Habitaciones disponibles*/
SELECT fecha, disponible 
FROM disponibilidad_habitacion
WHERE hotel_id = ? AND habitacion_numero = ? AND fecha = ?;

/*HOTELES – LISTA DE HOTELES
Pantalla donde ves todos los hoteles.*/
SELECT * FROM hoteles;

/*REGISTRO DE HOTEL
Pantalla donde registras un hotel nuevo.*/
INSERT INTO hoteles (hotel_id, nombre, ciudad, direccion, num_pisos, zona_turistica, servicios, frente_playa)
VALUES (?, ?, ?, ?, ?, ?, ?, ?);

/*HABITACIONES – LISTA POR HOTEL
Pantalla donde ves las habitaciones del hotel seleccionado.*/
SELECT * FROM habitaciones_por_hotel
WHERE hotel_id = ?;

/*REGISTRO DE HABITACIÓN
Pantalla para agregar una habitación a un hotel.*/
INSERT INTO habitaciones_por_hotel (hotel_id, habitacion_numero, tipo, precio, capacidad, descripcion)
VALUES (?, ?, ?, ?, ?, ?);
/*Crear disponibilidad (1 año por adelantado)*/
INSERT INTO disponibilidad_habitacion (hotel_id, habitacion_numero, fecha, disponible)
VALUES (?, ?, ?, true);

/*CLIENTES – LISTA
Pantalla donde ves y buscas clientes.*/
SELECT * FROM clientes_por_nombre
WHERE nombre = ?;
/*Obtener todos los clientes*/
SELECT * FROM clientes;

/*REGISTRO DE CLIENTE
Insertar cliente*/
INSERT INTO clientes (cliente_id, nombre, apellidos, telefono, correo)
VALUES (?, ?, ?, ?, ?);
/*Insertar índice por nombre*/
INSERT INTO clientes_por_nombre (nombre, cliente_id, apellidos, telefono, correo)
VALUES (?, ?, ?, ?, ?);

/*RESERVACIONES – LISTA
Pantalla donde se ven las reservas del hotel.
Mostrar reservaciones por hotel*/
SELECT * FROM reservaciones_por_hotel
WHERE hotel_id = ?;
/*Mostrar reservaciones del día*/
SELECT * FROM reservaciones_por_fecha
WHERE fecha_entrada = ?;

/*CREAR RESERVACIÓN
Pantalla para registrar una reservación.
Verificar disponibilidad*/
SELECT disponible FROM disponibilidad_habitacion
WHERE hotel_id = ? AND habitacion_numero = ? AND fecha = ?;
/*Bloquear fechas*/
UPDATE disponibilidad_habitacion
SET disponible = false
WHERE hotel_id = ? AND habitacion_numero = ? AND fecha = ?;
/*Insertar reservación por cliente*/
INSERT INTO reservaciones_por_cliente
(cliente_id, reserva_id, hotel_id, habitacion_numero, fecha_entrada, fecha_salida, estado)
VALUES (?, ?, ?, ?, ?, ?, 'pendiente');
/*Insertar reservación por hotel*/
INSERT INTO reservaciones_por_hotel
(hotel_id, fecha_entrada, reserva_id, cliente_id, habitacion_numero, fecha_salida, estado)
VALUES (?, ?, ?, ?, ?, ?, 'pendiente');
/*Insertar índice por fecha*/
INSERT INTO reservaciones_por_fecha
(fecha_entrada, reserva_id, hotel_id, cliente_id, habitacion_numero, fecha_salida, estado)
VALUES (?, ?, ?, ?, ?, ?, 'pendiente');

/*CHECK-IN
Pantalla donde confirmas entrada de huéspedes.
Marcar reservación como activa*/
UPDATE reservaciones_por_cliente
SET estado = 'activa'
WHERE cliente_id = ? AND reserva_id = ?;
/*Insertar en estancias_activas*/
INSERT INTO estancias_activas
(hotel_id, habitacion_numero, estancia_id, cliente_id, fecha_entrada, adultos, menores)
VALUES (?, ?, ?, ?, ?, ?, ?);

/*CHECK-OUT
Pantalla donde se cobra y libera la habitación.
Eliminar de estancias_activas*/
DELETE FROM estancias_activas
WHERE hotel_id = ? AND habitacion_numero = ?;
/*Guardar en historial*/
INSERT INTO historial_estancias
(cliente_id, estancia_id, hotel_id, habitacion_numero, fecha_entrada, fecha_salida, total_pagado, metodo_pago)
VALUES (?, ?, ?, ?, ?, ?, ?, ?);

/*REPORTES
Se basan en historial.
Historial por cliente*/
SELECT * FROM historial_estancias
WHERE cliente_id = ?;
/*Ventas por fecha (sumar total_pagado)*/
SELECT * FROM historial_estancias
WHERE cliente_id = ?;  -- si quieres cambiarlo por hotel_id hacemos otra tabla
