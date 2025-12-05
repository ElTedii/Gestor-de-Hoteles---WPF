using Cassandra;
using Gestión_Hotelera.Model;
using System;
using System.Collections.Generic;
using System.Linq;

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
                    usuario_creacion, fecha_creacion
                ) VALUES (?, ?, ?, ?, ?, ?);
            ";

            _session.Execute(_session.Prepare(query).Bind(
                h.HotelId,
                h.NumeroHabitacion,
                h.TipoId,
                h.Piso,
                h.UsuarioRegistro,
                h.FechaRegistro
            ));
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
                WHERE hotel_id = ? AND numero = ?;
            ";

            _session.Execute(_session.Prepare(query).Bind(
                h.TipoId,
                h.Piso,
                h.UsuarioModificacion,
                h.FechaModificacion,
                h.HotelId,
                h.NumeroHabitacion
            ));
        }

        // ============================================================
        // ELIMINAR HABITACIÓN
        // ============================================================
        public void Delete(Guid hotelId, int numero)
        {
            const string query = "DELETE FROM habitaciones WHERE hotel_id=? AND numero=?;";
            _session.Execute(_session.Prepare(query).Bind(hotelId, numero));
        }

        // ============================================================
        // GET TODAS LAS HABITACIONES DE UN HOTEL
        // ============================================================
        public List<HabitacionModel> GetByHotel(Guid hotelId)
        {
            var rows = _session.Execute(
                _session.Prepare("SELECT * FROM habitaciones WHERE hotel_id=?;")
                .Bind(hotelId)
            );

            var habitaciones = rows.Select(MapRow).ToList();

            // Cargar tipo y precio desde tipos_habitacion_por_hotel
            var tipoRepo = new TipoHabitacionRepository();

            foreach (var h in habitaciones)
            {
                var tipo = tipoRepo.GetByHotelAndTipo(h.HotelId, h.TipoId);
                if (tipo != null)
                {
                    h.PrecioNoche = tipo.PrecioNoche;
                    h.TipoNombre = tipo.NombreTipo;
                }
            }

            return habitaciones;
        }

        // ============================================================
        // GET UNA HABITACIÓN ESPECÍFICA
        // ============================================================
        public HabitacionModel GetByHotelAndNumero(Guid hotelId, int numero)
        {
            var row = _session.Execute(
                _session.Prepare("SELECT * FROM habitaciones WHERE hotel_id=? AND numero=?;")
                .Bind(hotelId, numero)
            ).FirstOrDefault();

            if (row == null)
                return null;

            var hab = MapRow(row);

            var tipo = new TipoHabitacionRepository().GetByHotelAndTipo(hab.HotelId, hab.TipoId);

            if (tipo != null)
            {
                hab.PrecioNoche = tipo.PrecioNoche;
                hab.TipoNombre = tipo.NombreTipo;
            }

            return hab;
        }

        // ============================================================
        // VERIFICAR SI YA EXISTE EL NÚMERO
        // ============================================================
        public bool ExistsNumero(Guid hotelId, int numero)
        {
            var row = _session.Execute(
                _session.Prepare("SELECT numero FROM habitaciones WHERE hotel_id=? AND numero=?;")
                .Bind(hotelId, numero)
            ).FirstOrDefault();

            return row != null;
        }

        // ============================================================
        // HABITACIONES LIBRES (CHECK-IN)
        // ============================================================
        public List<HabitacionModel> GetHabitacionesLibres(Guid hotelId, DateTime entrada, DateTime salida)
        {
            // Todas las habitaciones del hotel
            var todas = GetByHotel(hotelId);

            // TODAS las estancias activas DEL HOTEL
            var estancias = new EstanciaActivaRepository()
                .GetByHotel(hotelId);   // ← METODO NUEVO, YA FUNCIONA

            // Habitaciones ocupadas POR CRUCE DE FECHAS
            var ocupadas = estancias
                .Where(e =>
                    entrada < e.FechaSalida &&
                    salida > e.FechaEntrada)
                .Select(e => e.NumeroHabitacion)
                .ToList();

            // Habitaciones que NO estén en ocupadas
            return todas.Where(h => !ocupadas.Contains(h.NumeroHabitacion)).ToList();
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

                UsuarioRegistro = row.IsNull("usuario_creacion") ? "" : row.GetValue<string>("usuario_creacion"),
                FechaRegistro = row.IsNull("fecha_creacion") ? DateTime.UtcNow : row.GetValue<DateTime>("fecha_creacion"),

                UsuarioModificacion = row.IsNull("usuario_modificacion") ? "" : row.GetValue<string>("usuario_modificacion"),
                FechaModificacion = row.IsNull("fecha_modificacion") ? DateTime.UtcNow : row.GetValue<DateTime>("fecha_modificacion")
            };
        }
    }
}