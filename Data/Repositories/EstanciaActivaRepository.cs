using Cassandra;
using Gestión_Hotelera.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Gestión_Hotelera.Data.Repositories
{
    public class EstanciaActivaRepository
    {
        private readonly ISession _session;

        public EstanciaActivaRepository()
        {
            _session = CassandraConnection.GetSession();
        }

        // ============================================================
        // INSERTAR ESTANCIA (Check-in)
        // ============================================================
        public void Insert(EstanciaActivaModel e)
        {
            var query = @"
                INSERT INTO estancias_activas (
                    hotel_id, numero_habitacion, estancia_id, cliente_id, reserva_id,
                    fecha_entrada, fecha_salida, anticipo, precio_noche,
                    adultos, menores, usuario_registro, fecha_registro
                ) VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?);
            ";

            _session.Execute(_session.Prepare(query).Bind(
                e.HotelId,
                e.NumeroHabitacion,
                e.EstanciaId,
                e.ClienteId,
                e.ReservaId,
                e.FechaEntrada,
                e.FechaSalida,
                e.Anticipo,
                e.PrecioNoche,
                e.Adultos,
                e.Menores,
                e.UsuarioRegistro,
                e.FechaRegistro
            ));
        }

        // ============================================================
        // ELIMINAR ESTANCIA ACTIVA (cuando se hace Check-Out)
        // ============================================================
        public void Delete(Guid hotelId, int numeroHabitacion)
        {
            var query = "DELETE FROM estancias_activas WHERE hotel_id=? AND numero_habitacion=?;";
            _session.Execute(_session.Prepare(query).Bind(hotelId, numeroHabitacion));
        }

        // ============================================================
        // OBTENER ESTANCIA POR RESERVA (para ReservasDetalle)
        // ============================================================
        public EstanciaActivaModel GetByReserva(Guid reservaId)
        {
            var rows = _session.Execute(
                _session.Prepare(
                    "SELECT * FROM estancias_activas WHERE reserva_id=? ALLOW FILTERING;"
                ).Bind(reservaId)
            );

            var row = rows.FirstOrDefault();
            return row == null ? null : MapRow(row);
        }

        // ============================================================
        // OBTENER TODAS LAS ESTANCIAS DE UN HOTEL
        // ============================================================
        public List<EstanciaActivaModel> GetByHotel(Guid hotelId)
        {
            var rows = _session.Execute(
                _session.Prepare(
                    "SELECT * FROM estancias_activas WHERE hotel_id=?;"
                ).Bind(hotelId)
            );

            return rows.Select(MapRow).ToList();
        }

        // ============================================================
        // OBTENER ESTANCIA POR HOTEL Y HABITACIÓN (*VERSIÓN FINAL*)
        // ============================================================
        public EstanciaActivaModel GetByHotelAndNumero(Guid hotelId, int numeroHabitacion)
        {
            // 1) Consultar solo por partition key
            var rows = _session.Execute(
                _session.Prepare(
                    "SELECT * FROM estancias_activas WHERE hotel_id=?;"
                ).Bind(hotelId)
            );

            // 2) Filtrar en memoria porque Cassandra no permite buscar por clustering key sola
            var row = rows
                .Where(r => r.GetValue<int>("numero_habitacion") == numeroHabitacion)
                .OrderByDescending(r => r.GetValue<DateTime>("fecha_entrada"))
                .FirstOrDefault();

            return row == null ? null : MapRow(row);
        }

        // ============================================================
        // ESTADÍSTICA: CANTIDAD DE CHECK-INS EN EL DÍA
        // ============================================================
        public int GetCheckInsDelDia(DateTime fecha)
        {
            var inicio = fecha.Date;
            var fin = inicio.AddDays(1);

            var query = @"
                SELECT COUNT(*) AS total
                FROM estancias_activas
                WHERE fecha_registro >= ? AND fecha_registro < ?
                ALLOW FILTERING;
            ";

            var row = _session.Execute(new SimpleStatement(query, inicio, fin)).FirstOrDefault();

            return row == null ? 0 : (int)row.GetValue<long>("total");
        }

        // ============================================================
        // OBTENER TODAS (DEBUG/UI)
        // ============================================================
        public List<EstanciaActivaModel> GetAll()
        {
            var rows = _session.Execute("SELECT * FROM estancias_activas;");
            return rows.Select(MapRow).ToList();
        }

        // ============================================================
        // MAPEO
        // ============================================================
        private EstanciaActivaModel MapRow(Row row)
        {
            return new EstanciaActivaModel
            {
                EstanciaId = row.GetValue<Guid>("estancia_id"),
                HotelId = row.GetValue<Guid>("hotel_id"),
                NumeroHabitacion = row.GetValue<int>("numero_habitacion"),

                ClienteId = row.GetValue<Guid>("cliente_id"),
                ReservaId = row.GetValue<Guid>("reserva_id"),

                FechaEntrada = row.IsNull("fecha_entrada")
                    ? DateTime.UtcNow
                    : row.GetValue<DateTime>("fecha_entrada"),

                FechaSalida = row.IsNull("fecha_salida")
                    ? DateTime.UtcNow.AddDays(1)
                    : row.GetValue<DateTime>("fecha_salida"),

                Anticipo = row.IsNull("anticipo")
                    ? 0
                    : row.GetValue<decimal>("anticipo"),

                PrecioNoche = row.IsNull("precio_noche")
                    ? 0
                    : row.GetValue<decimal>("precio_noche"),

                Adultos = row.IsNull("adultos")
                    ? 1
                    : row.GetValue<int>("adultos"),

                Menores = row.IsNull("menores")
                    ? 0
                    : row.GetValue<int>("menores"),

                UsuarioRegistro = row.IsNull("usuario_registro")
                    ? "sistema"
                    : row.GetValue<string>("usuario_registro"),

                FechaRegistro = row.IsNull("fecha_registro")
                    ? DateTime.UtcNow
                    : row.GetValue<DateTime>("fecha_registro")
            };
        }
    }
}