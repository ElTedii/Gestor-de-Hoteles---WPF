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

        // ============================================================
        // INSERTAR ESTANCIA ACTIVA
        // ============================================================
        public void Insert(EstanciaActivaModel e)
        {
            const string query = @"
                INSERT INTO estancias_activas (
                    hotel_id, numero, estancia_id,
                    cliente_id, reserva_id,
                    fecha_entrada, fecha_salida,
                    anticipo, precio_noche,
                    adultos, menores,
                    usuario_registro, fecha_registro
                ) VALUES (?,?,?,?,?,?,?,?,?,?,?,?,?);";

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
                e.Menores,
                e.UsuarioRegistro,
                e.FechaRegistro
            ));
        }

        // ============================================================
        // ACTUALIZAR ESTANCIA
        // ============================================================
        public void Update(EstanciaActivaModel e)
        {
            const string query = @"
                UPDATE estancias_activas SET
                    fecha_entrada = ?, fecha_salida = ?,
                    anticipo = ?, precio_noche = ?,
                    adultos = ?, menores = ?
                WHERE hotel_id = ? AND numero = ?;";

            var stmt = _session.Prepare(query);

            _session.Execute(stmt.Bind(
                e.FechaEntrada,
                e.FechaSalida,
                e.Anticipo,
                e.PrecioNoche,
                e.Adultos,
                e.Menores,
                e.HotelId,
                e.NumeroHabitacion
            ));
        }

        // ============================================================
        // ELIMINAR ESTANCIA
        // ============================================================
        public void Delete(Guid hotelId, int numeroHabitacion)
        {
            var query = "DELETE FROM estancias_activas WHERE hotel_id=? AND numero=?;";

            _session.Execute(
                _session.Prepare(query).Bind(hotelId, numeroHabitacion)
            );
        }

        // ============================================================
        // OBTENER POR RESERVA
        // ============================================================
        public EstanciaActivaModel GetByReserva(Guid reservaId)
        {
            // Debemos usar ALLOW FILTERING porque reserva_id no es clave
            const string query = @"
                SELECT * FROM estancias_activas
                WHERE reserva_id = ? ALLOW FILTERING;";

            var row = _session.Execute(
                _session.Prepare(query).Bind(reservaId)
            ).FirstOrDefault();

            return row == null ? null : MapRow(row);
        }

        // ============================================================
        // OBTENER POR HOTEL
        // ============================================================
        public List<EstanciaActivaModel> GetByHotel(Guid hotelId)
        {
            const string query = @"
                SELECT * FROM estancias_activas
                WHERE hotel_id = ?;";

            var rows = _session.Execute(
                _session.Prepare(query).Bind(hotelId)
            );

            return rows.Select(MapRow).ToList();
        }

        // ============================================================
        // OBTENER POR HOTEL Y HABITACIÓN
        // ============================================================
        public EstanciaActivaModel GetByHotelAndNumero(Guid hotelId, int numero)
        {
            const string query = @"
                SELECT * FROM estancias_activas
                WHERE hotel_id = ? AND numero = ?;";

            var row = _session.Execute(
                _session.Prepare(query).Bind(hotelId, numero)
            ).FirstOrDefault();

            return row == null ? null : MapRow(row);
        }

        // ============================================================
        // OBTENER TODAS (solo para UI/debug)
        // ============================================================
        public List<EstanciaActivaModel> GetAll()
        {
            var result = _session.Execute("SELECT * FROM estancias_activas;");
            return result.Select(MapRow).ToList();
        }

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

            var stmt = new SimpleStatement(query, inicio, fin);
            var row = _session.Execute(stmt).FirstOrDefault();

            return row == null ? 0 : (int)row.GetValue<long>("total");
        }

        public EstanciaActivaModel GetByHotelAndHabitacion(Guid hotelId, int numero)
        {
            var query = "SELECT * FROM estancias_activas WHERE hotel_id=? AND numero=?;";

            var row = _session.Execute(
                _session.Prepare(query).Bind(hotelId, numero)
            ).FirstOrDefault();

            if (row == null)
                return null;

            return new EstanciaActivaModel
            {
                EstanciaId = row.GetValue<Guid>("estancia_id"),
                ClienteId = row.GetValue<Guid>("cliente_id"),
                HotelId = row.GetValue<Guid>("hotel_id"),
                NumeroHabitacion = row.GetValue<int>("numero"),
                ReservaId = row.GetValue<Guid>("reserva_id"),

                FechaEntrada = row.GetValue<DateTime>("fecha_entrada"),
                FechaSalida = row.GetValue<DateTime>("fecha_salida"),

                Anticipo = row.GetValue<decimal>("anticipo"),
                PrecioNoche = row.GetValue<decimal>("precio_noche"),

                Adultos = row.GetValue<int>("adultos"),
                Menores = row.GetValue<int>("menores"),

                UsuarioRegistro = row.GetValue<string>("usuario_registro"),
                FechaRegistro = row.GetValue<DateTime>("fecha_registro")
            };
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
                NumeroHabitacion = row.GetValue<int>("numero"),

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
