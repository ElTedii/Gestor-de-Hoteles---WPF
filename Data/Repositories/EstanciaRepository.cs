using Cassandra;
using Gestión_Hotelera.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gestión_Hotelera.Data.Repositories
{
    public class EstanciaActivaRepository
    {
        private readonly ISession _session;

        public EstanciaActivaRepository()
        {
            _session = CassandraConnection.GetSession();
        }

        // ==========================================================
        // CHECK IN: Insertar estancia activa
        // ==========================================================
        public void Insert(EstanciaActivaModel e)
        {
            var query = @"INSERT INTO estancias_activas (
                hotel_id, numero, estancia_id, cliente_id, reserva_id,
                fecha_entrada, fecha_salida,
                anticipo, precio_noche,
                adultos, menores
            ) VALUES (?,?,?,?,?,?,?,?,?,?,?);";

            var stmt = _session.Prepare(query);

            _session.Execute(stmt.Bind(
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
                e.Menores
            ));
        }

        // ==========================================================
        // CHECK OUT: borrado de estancia activa
        // ==========================================================
        public void Delete(Guid hotelId, int numeroHabitacion)
        {
            var stmt = _session.Prepare(
                "DELETE FROM estancias_activas WHERE hotel_id=? AND numero=?;"
            );

            _session.Execute(stmt.Bind(hotelId, numeroHabitacion));
        }

        // ==========================================================
        // CONSULTA: Todas las estancias activas de un hotel
        // ==========================================================
        public List<EstanciaActivaModel> GetByHotel(Guid hotelId)
        {
            var result = _session.Execute(
                _session.Prepare("SELECT * FROM estancias_activas WHERE hotel_id=?;")
                .Bind(hotelId)
            );

            return result.Select(MapRow).ToList();
        }

        // ==========================================================
        // CONSULTA: Estancia activa por habitación
        // ==========================================================
        public EstanciaActivaModel GetByRoom(Guid hotelId, int numero)
        {
            var row = _session.Execute(
                _session.Prepare("SELECT * FROM estancias_activas WHERE hotel_id=? AND numero=?;")
                .Bind(hotelId, numero)
            ).FirstOrDefault();

            return row != null ? MapRow(row) : null;
        }

        // ==========================================================
        // CONSULTA: Buscar estancia activa por reserva
        // ==========================================================
        public EstanciaActivaModel GetByReserva(Guid hotelId, Guid reservaId)
        {
            var result = _session.Execute(
                _session.Prepare("SELECT * FROM estancias_activas WHERE hotel_id=?;")
                .Bind(hotelId)
            );

            foreach (var row in result)
            {
                if (row.GetValue<Guid>("reserva_id") == reservaId)
                    return MapRow(row);
            }

            return null;
        }

        // ==========================================================
        // MAPEO
        // ==========================================================
        private EstanciaActivaModel MapRow(Row row)
        {
            return new EstanciaActivaModel
            {
                HotelId = row.GetValue<Guid>("hotel_id"),
                NumeroHabitacion = row.GetValue<int>("numero"),
                EstanciaId = row.GetValue<Guid>("estancia_id"),
                ClienteId = row.GetValue<Guid>("cliente_id"),
                ReservaId = row.GetValue<Guid>("reserva_id"),

                FechaEntrada = row.GetValue<DateTime>("fecha_entrada"),
                FechaSalida = row.GetValue<DateTime>("fecha_salida"),

                Anticipo = row.GetValue<decimal>("anticipo"),
                PrecioNoche = row.GetValue<decimal>("precio_noche"),

                Adultos = row.GetValue<int>("adultos"),
                Menores = row.GetValue<int>("menores")
            };
        }
    }
}
