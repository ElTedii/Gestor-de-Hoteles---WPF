using Cassandra;
using Gestión_Hotelera.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Gestión_Hotelera.Data.Repositories
{
    public class TipoHabitacionRepository
    {
        private readonly ISession _session;

        public TipoHabitacionRepository()
        {
            _session = CassandraConnection.GetSession();
        }

        // ============================================================
        // INSERTAR TIPO HABITACIÓN
        // ============================================================
        public void Insert(TipoHabitacionModel t)
        {
            if (t.TipoId == Guid.Empty)
                t.TipoId = Guid.NewGuid();

            if (t.FechaRegistro == default)
                t.FechaRegistro = DateTime.UtcNow;

            const string query = @"
            INSERT INTO tipos_habitacion_por_hotel (
                hotel_id, tipo_id, nombre_tipo, capacidad, precio_noche,
                cantidad, caracteristicas, amenidades, nivel, vista,
                usuario_registro, fecha_registro, fecha_modificacion
            ) VALUES (?,?,?,?,?,?,?,?,?,?,?,?,?);";

            var stmt = _session.Prepare(query);

            _session.Execute(stmt.Bind(
                t.HotelId,
                t.TipoId,
                t.NombreTipo,
                t.Capacidad,
                t.PrecioNoche,
                t.Cantidad,
                t.Caracteristicas,
                t.Amenidades,
                t.Nivel,
                t.Vista,
                t.UsuarioRegistro,
                t.FechaRegistro,
                t.FechaModificacion
            ));
        }

        // ============================================================
        // OBTENER POR HOTEL + TIPO  (EL MÉTODO QUE FALTABA)
        // ============================================================
        public TipoHabitacionModel GetByHotelAndTipo(Guid hotelId, Guid tipoId)
        {
            var row = _session.Execute(
                _session.Prepare("SELECT * FROM tipos_habitacion_por_hotel WHERE hotel_id=? AND tipo_id=?;")
                .Bind(hotelId, tipoId)
            ).FirstOrDefault();

            return row == null ? null : MapRow(row);
        }

        // ============================================================
        // OBTENER UN TIPO POR ID (corregido)
        // ============================================================
        public TipoHabitacionModel GetById(Guid hotelId, Guid tipoId)
        {
            var stmt = _session.Prepare(
                "SELECT * FROM tipos_habitacion_por_hotel WHERE hotel_id=? AND tipo_id=?;"
            );

            var row = _session.Execute(stmt.Bind(hotelId, tipoId)).FirstOrDefault();

            return row == null ? null : MapRow(row);
        }

        // ============================================================
        // OBTENER TODOS LOS TIPOS DEL HOTEL
        // ============================================================
        public List<TipoHabitacionModel> GetByHotel(Guid hotelId)
        {
            var rows = _session.Execute(
                _session.Prepare("SELECT * FROM tipos_habitacion_por_hotel WHERE hotel_id=?;")
                .Bind(hotelId)
            );

            return rows.Select(MapRow).ToList();
        }

        // ============================================================
        // ACTUALIZAR TIPO HABITACIÓN
        // ============================================================
        public void Update(TipoHabitacionModel t)
        {
            t.FechaModificacion = DateTime.UtcNow;

            const string query = @"
            UPDATE tipos_habitacion_por_hotel SET
                nombre_tipo=?, capacidad=?, precio_noche=?, cantidad=?,
                caracteristicas=?, amenidades=?, nivel=?, vista=?,
                usuario_modificacion=?, fecha_modificacion=?
            WHERE hotel_id=? AND tipo_id=?;";

            var stmt = _session.Prepare(query);

            _session.Execute(stmt.Bind(
                t.NombreTipo,
                t.Capacidad,
                t.PrecioNoche,
                t.Cantidad,
                t.Caracteristicas,
                t.Amenidades,
                t.Nivel,
                t.Vista,
                t.UsuarioModificacion,
                t.FechaModificacion,
                t.HotelId,
                t.TipoId
            ));
        }

        // ============================================================
        // ELIMINAR
        // ============================================================
        public void Delete(Guid hotelId, Guid tipoId)
        {
            var stmt = _session.Prepare(
                "DELETE FROM tipos_habitacion_por_hotel WHERE hotel_id=? AND tipo_id=?;"
            );

            _session.Execute(stmt.Bind(hotelId, tipoId));
        }

        // ============================================================
        // MAPEO GENERAL
        // ============================================================
        private TipoHabitacionModel MapRow(Row row)
        {
            return new TipoHabitacionModel
            {
                HotelId = row.GetValue<Guid>("hotel_id"),
                TipoId = row.GetValue<Guid>("tipo_id"),
                NombreTipo = row.GetValue<string>("nombre_tipo"),
                Capacidad = row.GetValue<int>("capacidad"),
                PrecioNoche = row.GetValue<decimal>("precio_noche"),
                Cantidad = row.GetValue<int>("cantidad"),

                Caracteristicas = row.GetValue<List<string>>("caracteristicas"),
                Amenidades = row.GetValue<List<string>>("amenidades"),
                Nivel = row.GetValue<string>("nivel"),
                Vista = row.GetValue<string>("vista"),

                UsuarioRegistro = row.GetValue<string>("usuario_registro"),
                FechaRegistro = row.GetValue<DateTime>("fecha_registro"),
                FechaModificacion = row.GetValue<DateTime?>("fecha_modificacion")
            };
        }
    }
}