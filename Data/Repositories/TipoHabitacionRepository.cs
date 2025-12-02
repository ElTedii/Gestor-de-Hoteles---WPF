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

        // INSERT
        public void Insert(TipoHabitacionModel t)
        {
            if (t.TipoId == Guid.Empty)
                t.TipoId = Guid.NewGuid();

            t.FechaRegistro = DateTime.UtcNow;

            const string q = @"
                INSERT INTO tipos_habitacion_por_hotel (
                    hotel_id, tipo_id, nombre_tipo, capacidad,
                    precio_noche, cantidad, caracteristicas,
                    amenidades, nivel, vista,
                    usuario_registro, fecha_registro,
                    usuario_modificacion, fecha_modificacion
                ) VALUES (?,?,?,?,?,?,?,?,?,?,?,?,?,?);";

            _session.Execute(_session.Prepare(q).Bind(
                t.HotelId, t.TipoId, t.NombreTipo, t.Capacidad,
                t.PrecioNoche, t.Cantidad, t.Caracteristicas,
                t.Amenidades, t.Nivel, t.Vista,
                t.UsuarioRegistro, t.FechaRegistro,
                t.UsuarioModificacion, t.FechaModificacion
            ));
        }

        // UPDATE
        public void Update(TipoHabitacionModel t)
        {
            t.UsuarioModificacion ??= t.UsuarioRegistro;
            t.FechaModificacion = DateTime.UtcNow;

            const string q = @"
                UPDATE tipos_habitacion_por_hotel SET
                    nombre_tipo=?, capacidad=?, precio_noche=?, cantidad=?,
                    caracteristicas=?, amenidades=?, nivel=?, vista=?,
                    usuario_modificacion=?, fecha_modificacion=?
                WHERE hotel_id=? AND tipo_id=?;";

            _session.Execute(_session.Prepare(q).Bind(
                t.NombreTipo, t.Capacidad, t.PrecioNoche, t.Cantidad,
                t.Caracteristicas, t.Amenidades, t.Nivel, t.Vista,
                t.UsuarioModificacion, t.FechaModificacion,
                t.HotelId, t.TipoId
            ));
        }

        // GET ALL BY HOTEL
        public List<TipoHabitacionModel> GetByHotel(Guid hotelId)
        {
            var rows = _session.Execute(
                _session.Prepare("SELECT * FROM tipos_habitacion_por_hotel WHERE hotel_id=?;")
                .Bind(hotelId)
            );

            return rows.Select(Map).ToList();
        }

        // GET ONE
        public TipoHabitacionModel GetByHotelAndTipo(Guid hotelId, Guid tipoId)
        {
            var row = _session.Execute(
                _session.Prepare(
                    "SELECT * FROM tipos_habitacion_por_hotel WHERE hotel_id=? AND tipo_id=?;"
                ).Bind(hotelId, tipoId)
            ).FirstOrDefault();

            return row == null ? null : Map(row);
        }

        public List<TipoHabitacionModel> GetAll()
        {
            var rows = _session.Execute("SELECT * FROM tipos_habitacion_por_hotel;");
            return rows.Select(Map).ToList();
        }

        // MAP
        private TipoHabitacionModel Map(Row r)
        {
            return new TipoHabitacionModel
            {
                HotelId = r.GetValue<Guid>("hotel_id"),
                TipoId = r.GetValue<Guid>("tipo_id"),
                NombreTipo = r.GetValue<string>("nombre_tipo"),
                Capacidad = r.GetValue<int>("capacidad"),
                PrecioNoche = r.GetValue<decimal>("precio_noche"),
                Cantidad = r.GetValue<int>("cantidad"),

                Caracteristicas = r.GetValue<List<string>>("caracteristicas") ?? new List<string>(),
                Amenidades = r.GetValue<List<string>>("amenidades") ?? new List<string>(),

                Nivel = r.GetValue<string>("nivel"),
                Vista = r.GetValue<string>("vista"),

                UsuarioRegistro = r.GetValue<string>("usuario_registro"),
                FechaRegistro = r.GetValue<DateTime>("fecha_registro"),

                UsuarioModificacion = r.IsNull("usuario_modificacion")
                    ? null
                    : r.GetValue<string>("usuario_modificacion"),

                FechaModificacion = r.IsNull("fecha_modificacion")
                    ? (DateTime?)null
                    : r.GetValue<DateTime>("fecha_modificacion")
            };
        }
    }
}