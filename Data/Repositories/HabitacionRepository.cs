using Cassandra;
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
                    hotel_id, numero, tipo_id, piso,
                    usuario_creacion, fecha_creacion,
                    usuario_modificacion, fecha_modificacion
                ) VALUES (?, ?, ?, ?, ?, ?, ?, ?);";

            _session.Execute(
                _session.Prepare(query).Bind(
                    h.HotelId,
                    h.Numero,
                    h.TipoId,
                    h.Piso,
                    h.UsuarioCreacion,
                    h.FechaCreacion,
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
                    tipo_id = ?, piso = ?,
                    usuario_modificacion = ?, fecha_modificacion = ?
                WHERE hotel_id = ? AND numero = ?;";

            _session.Execute(
                _session.Prepare(query).Bind(
                    h.TipoId,
                    h.Piso,
                    h.UsuarioModificacion,
                    h.FechaModificacion,
                    h.HotelId,
                    h.Numero
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
            var result = _session.Execute(
                _session.Prepare("SELECT * FROM habitaciones WHERE hotel_id=?;")
                .Bind(hotelId)
            );

            return result.Select(MapRow).ToList();
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

            return todas.Where(h => !ocupadas.Contains(h.Numero)).ToList();
        }

        // ============================================================
        // MAPEO
        // ============================================================
        private HabitacionModel MapRow(Row row)
        {
            return new HabitacionModel
            {
                HotelId = row.GetValue<Guid>("hotel_id"),
                Numero = row.GetValue<int>("numero"),
                TipoId = row.GetValue<Guid>("tipo_id"),
                Piso = row.GetValue<int>("piso"),

                UsuarioCreacion = row.IsNull("usuario_creacion")
                    ? ""
                    : row.GetValue<string>("usuario_creacion"),

                FechaCreacion = row.IsNull("fecha_creacion")
                    ? DateTime.UtcNow
                    : row.GetValue<DateTime>("fecha_creacion"),

                UsuarioModificacion = row.IsNull("usuario_modificacion")
                    ? ""
                    : row.GetValue<string>("usuario_modificacion"),

                FechaModificacion = row.IsNull("fecha_modificacion")
                    ? DateTime.UtcNow
                    : row.GetValue<DateTime>("fecha_modificacion"),
            };
        }
    }
}
