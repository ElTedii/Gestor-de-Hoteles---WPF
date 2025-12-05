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

        // ================== INSERT ==================
        public void Insert(EstanciaActivaModel e)
        {
            if (e.EstanciaId == Guid.Empty)
                e.EstanciaId = Guid.NewGuid();

            e.FechaRegistro = DateTime.UtcNow;

            const string query = @"
                INSERT INTO estancias_activas (
                    hotel_id, numero_habitacion, fecha_entrada,
                    fecha_salida, estancia_id,
                    cliente_id, reserva_id,
                    anticipo, precio_noche,
                    adultos, menores,
                    usuario_registro, fecha_registro
                ) VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?);";

            _session.Execute(_session.Prepare(query).Bind(
                e.HotelId, e.NumeroHabitacion, e.FechaEntrada,
                e.FechaSalida, e.EstanciaId,
                e.ClienteId, e.ReservaId,
                e.Anticipo, e.PrecioNoche,
                e.Adultos, e.Menores,
                e.UsuarioRegistro, e.FechaRegistro
            ));
        }

        // =============== GET POR HOTEL + HABITACIÓN =================
        public EstanciaActivaModel GetByHotelAndNumero(Guid hotelId, int numeroHabitacion)
        {
            const string query = @"
                SELECT * FROM estancias_activas
                WHERE hotel_id=? AND numero_habitacion=?
                ORDER BY fecha_entrada DESC
                LIMIT 1;";

            var row = _session.Execute(
                _session.Prepare(query).Bind(hotelId, numeroHabitacion)
            ).FirstOrDefault();

            return row == null ? null : Map(row);
        }

        // =============== GET CHECK-INS DEL DÍA =================
        public List<EstanciaActivaModel> GetCheckInsDelDia(DateTime dia)
        {
            var inicio = dia.Date;
            var fin = inicio.AddDays(1);

            // Clustering es fecha_entrada, así que podemos filtrar con ALLOW FILTERING
            const string query = @"
                SELECT * FROM estancias_activas
                WHERE fecha_entrada >= ? AND fecha_entrada < ?
                ALLOW FILTERING;";

            var rows = _session.Execute(
                _session.Prepare(query).Bind(inicio, fin)
            );

            return rows.Select(Map).ToList();
        }

        // =============== DELETE =================
        public void Delete(Guid hotelId, int numeroHabitacion, DateTime fechaEntrada)
        {
            const string query = @"
                DELETE FROM estancias_activas
                WHERE hotel_id=? AND numero_habitacion=? AND fecha_entrada=?;";

            _session.Execute(
                _session.Prepare(query).Bind(hotelId, numeroHabitacion, fechaEntrada)
            );
        }

        public List<EstanciaActivaModel> GetByHotel(Guid hotelId)
        {
            const string query = @"SELECT * FROM estancias_activas WHERE hotel_id=? ALLOW FILTERING;";

            var rows = _session.Execute(
                _session.Prepare(query).Bind(hotelId)
            );

            return rows.Select(Map).ToList();
        }

        // =============== MAP =================
        private EstanciaActivaModel Map(Row r)
        {
            return new EstanciaActivaModel
            {
                HotelId = r.GetValue<Guid>("hotel_id"),
                NumeroHabitacion = r.GetValue<int>("numero_habitacion"),
                FechaEntrada = r.GetValue<DateTime>("fecha_entrada"),
                FechaSalida = r.GetValue<DateTime>("fecha_salida"),
                EstanciaId = r.GetValue<Guid>("estancia_id"),
                ClienteId = r.GetValue<Guid>("cliente_id"),
                ReservaId = r.GetValue<Guid>("reserva_id"),
                Anticipo = r.GetValue<decimal?>("anticipo") ?? 0,
                PrecioNoche = r.GetValue<decimal?>("precio_noche") ?? 0,
                Adultos = r.GetValue<int?>("adultos") ?? 0,
                Menores = r.GetValue<int?>("menores") ?? 0,
                UsuarioRegistro = r.GetValue<string>("usuario_registro") ?? "",
                FechaRegistro = r.GetValue<DateTime?>("fecha_registro") ?? DateTime.UtcNow
            };
        }
    }
}