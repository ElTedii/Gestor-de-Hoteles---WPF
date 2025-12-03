using Cassandra;
using Cassandra.Mapping;
using Gestión_Hotelera.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gestión_Hotelera.Data.Repositories
{
    public class HabitacionRepository
    {
        private readonly ISession _session;

        public HabitacionRepository()
        {
            _session = CassandraConnection.GetSession();
        }

        // ============================================================
        // INSERTAR HABITACIÓN
        // ============================================================
        public void Insert(HabitacionModel h)
        {
            const string query = @"
                INSERT INTO habitaciones (
                        hotel_id, numero, tipo_id, piso, estado,
                        usuario_creacion, fecha_creacion,
                        usuario_modificacion, fecha_modificacion) 
                        VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?);";

            _session.Execute(
                _session.Prepare(query).Bind(
                    h.HotelId,
                    h.NumeroHabitacion,
                    h.TipoId,
                    h.Piso,
                    h.Estado,
                    h.UsuarioRegistro,
                    h.FechaRegistro,
                    h.UsuarioModificacion,
                    h.FechaModificacion
                )
            );
        }

        // ============================================================
        // ACTUALIZAR HABITACIÓN
        // ============================================================
        public void Update(HabitacionModel h)
        {
            const string query = @"
                UPDATE habitaciones SET
                        tipo_id = ?, piso = ?, estado = ?,
                        usuario_modificacion = ?, fecha_modificacion = ?
                WHERE hotel_id = ? AND numero = ?;";

            _session.Execute(
                _session.Prepare(query).Bind(
                    h.TipoId,
                    h.Piso,
                    h.UsuarioModificacion,
                    h.FechaModificacion,
                    h.HotelId,
                    h.NumeroHabitacion
                )
            );
        }

        // ============================================================
        // ELIMINAR HABITACIÓN
        // ============================================================
        public void Delete(Guid hotelId, int numero)
        {
            var stmt = _session.Prepare(
                "DELETE FROM habitaciones WHERE hotel_id=? AND numero=?;"
            );

            _session.Execute(stmt.Bind(hotelId, numero));
        }

        // ============================================================
        // GET: HABITACIONES DE UN HOTEL
        // ============================================================
        public List<HabitacionModel> GetByHotel(Guid hotelId)
        {
            var rows = _session.Execute(
                _session.Prepare("SELECT * FROM habitaciones WHERE hotel_id=?;")
                .Bind(hotelId)
            );

            var list = rows.Select(MapRow).ToList();

            // Traer nombre de tipo y precio desde tipos_habitacion_por_hotel
            var tipoRepo = new TipoHabitacionRepository();

            foreach (var h in list)
            {
                var tipo = tipoRepo.GetByHotelAndTipo(h.HotelId, h.TipoId);
                if (tipo != null)
                {
                    h.PrecioNoche = tipo.PrecioNoche;
                    h.TipoNombre = tipo.NombreTipo;
                }
            }

            return list;
        }

        // ============================================================
        // GET: UNA HABITACIÓN ESPECÍFICA
        // ============================================================
        public HabitacionModel GetByHotelAndNumero(Guid hotelId, int numero)
        {
            var row = _session.Execute(
                _session.Prepare(
                    "SELECT * FROM habitaciones WHERE hotel_id=? AND numero=?;"
                ).Bind(hotelId, numero)
            ).FirstOrDefault();

            return row == null ? null : MapRow(row);
        }

        // ============================================================
        // GET: HABITACIONES POR TIPO
        // ============================================================
        public List<HabitacionModel> GetByHotelAndTipo(Guid hotelId, Guid tipoId)
        {
            var result = _session.Execute(
                _session.Prepare(
                    "SELECT * FROM habitaciones WHERE hotel_id=? ALLOW FILTERING;"
                ).Bind(hotelId)
            );

            return result
                .Where(h => h.GetValue<Guid>("tipo_id") == tipoId)
                .Select(MapRow)
                .ToList();
        }

        public List<HabitacionModel> GetHabitacionesLibres(Guid hotelId, DateTime entrada, DateTime salida)
        {
            var todas = GetByHotel(hotelId);

            var estancias = new EstanciaActivaRepository().GetByHotel(hotelId);

            // Habitaciones ocupadas en el rango
            var ocupadas = estancias
                .Where(e =>
                    entrada < e.FechaSalida &&
                    salida > e.FechaEntrada)
                .Select(e => e.NumeroHabitacion)
                .ToList();

            return todas.Where(h => !ocupadas.Contains(h.NumeroHabitacion)).ToList();
        }

        public List<HabitacionModel> GetAll()
        {
            var rows = _session.Execute("SELECT * FROM habitaciones;");
            return rows.Select(MapRow).ToList();
        }

        public List<HabitacionModel> GetAllHabitaciones()
        {
            var rows = _session.Execute("SELECT * FROM habitaciones;");

            return rows.Select(r => new HabitacionModel
            {
                HotelId = r.GetValue<Guid>("hotel_id"),
                NumeroHabitacion = r.GetValue<int>("numero"),
                TipoId = r.GetValue<Guid>("tipo_id"),
                Piso = r.GetValue<int>("piso"),
                Estado = r.GetValue<string>("estado"),
                UsuarioRegistro = r.GetValue<string>("usuario_creacion"),
                FechaRegistro = r.GetValue<DateTime>("fecha_creacion")
            }).ToList();
        }

        public bool ExistsNumero(Guid hotelId, int numero)
        {
            var row = _session.Execute(
                _session.Prepare(
                    "SELECT hotel_id, numero FROM habitaciones WHERE hotel_id=? AND numero=?;"
                ).Bind(hotelId, numero)
            ).FirstOrDefault();

            return row != null;
        }

        public HabitacionModel GetByReserva(Guid reservaId)
        {
            var row = _session.Execute(
                _session.Prepare(
                    "SELECT * FROM habitaciones WHERE reserva_id=? ALLOW FILTERING;")
                .Bind(reservaId)
            ).FirstOrDefault();

            return row == null ? null : MapRow(row);
        }

        // Buscar habitación por reserva (cliente_id, reserva_id)
        public HabitacionModel GetByReserva(Guid clienteId, Guid reservaId)
        {
            var row = _session.Execute(
                _session.Prepare(
                    @"SELECT * FROM habitaciones 
              WHERE cliente_id=? AND reserva_id=? ALLOW FILTERING;")
                .Bind(clienteId, reservaId)
            ).FirstOrDefault();

            return row == null ? null : MapRow(row);
        }

        // ============================================================
        // MAPEO
        // ============================================================
        private HabitacionModel MapRow(Row row)
        {
            return new HabitacionModel
            {
                HotelId = row.GetValue<Guid>("hotel_id"),
                NumeroHabitacion = row.GetValue<int>("numero"),
                TipoId = row.GetValue<Guid>("tipo_id"),
                Piso = row.GetValue<int>("piso"),

                Estado = row.IsNull("estado") ? "DISPONIBLE" : row.GetValue<string>("estado"),

                PrecioNoche = row.IsNull("precio_noche") ? 0 : row.GetValue<decimal>("precio_noche"),
                TipoNombre = row.IsNull("tipo_nombre") ? "Sin Tipo" : row.GetValue<string>("tipo_nombre"),

                UsuarioRegistro = row.IsNull("usuario_creacion")
                    ? ""
                    : row.GetValue<string>("usuario_creacion"),

                FechaRegistro = row.IsNull("fecha_creacion")
                    ? DateTime.UtcNow
                    : row.GetValue<DateTime>("fecha_creacion"),

                UsuarioModificacion = row.IsNull("usuario_modificacion")
                    ? ""
                    : row.GetValue<string>("usuario_modificacion"),

                FechaModificacion = row.IsNull("fecha_modificacion")
                    ? DateTime.UtcNow
                    : row.GetValue<DateTime>("fecha_modificacion")
            };
        }
    }
}
