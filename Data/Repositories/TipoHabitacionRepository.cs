using Cassandra;
using Gestión_Hotelera.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gestión_Hotelera.Data.Repositories
{
    public class TipoHabitacionRepository
    {
        private readonly ISession _session;

        public TipoHabitacionRepository()
        {
            _session = CassandraConnection.GetSession();
        }

        // ==========================================================
        // INSERT
        // ==========================================================
        public void Insert(TipoHabitacionModel t)
        {
            var query = @"INSERT INTO tipos_habitacion_por_hotel (
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
                t.FechaCreacion,
                t.FechaModificacion
            ));
        }

        // ==========================================================
        // UPDATE
        // ==========================================================
        public void Update(TipoHabitacionModel t)
        {
            var query = @"UPDATE tipos_habitacion_por_hotel SET
                nombre_tipo=?, capacidad=?, precio_noche=?, cantidad=?,
                caracteristicas=?, amenidades=?, nivel=?, vista=?,
                fecha_modificacion=?
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
                t.FechaModificacion,
                t.HotelId,
                t.TipoId
            ));
        }

        // ==========================================================
        // DELETE
        // ==========================================================
        public void Delete(Guid hotelId, Guid tipoId)
        {
            var stmt = _session.Prepare(
                "DELETE FROM tipos_habitacion_por_hotel WHERE hotel_id=? AND tipo_id=?;"
            );

            _session.Execute(stmt.Bind(hotelId, tipoId));
        }

        // ==========================================================
        // GET BY HOTEL
        // ==========================================================
        public List<TipoHabitacionModel> GetByHotel(Guid hotelId)
        {
            var result = _session.Execute(
                _session.Prepare("SELECT * FROM tipos_habitacion_por_hotel WHERE hotel_id=?;")
                .Bind(hotelId)
            );

            var list = new List<TipoHabitacionModel>();

            foreach (var row in result)
                list.Add(MapRow(row));

            return list;
        }

        // ==========================================================
        // GET BY HOTEL + TYPE
        // ==========================================================
        public TipoHabitacionModel GetByHotelAndTipo(Guid hotelId, Guid tipoId)
        {
            var row = _session.Execute(
                _session.Prepare(@"SELECT * FROM tipos_habitacion_por_hotel 
                                   WHERE hotel_id=? AND tipo_id=?;")
                .Bind(hotelId, tipoId)
            ).FirstOrDefault();

            return row != null ? MapRow(row) : null;
        }

        // ==========================================================
        // MAPEO
        // ==========================================================
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
                FechaCreacion = row.GetValue<DateTime>("fecha_registro"),
                FechaModificacion = row.GetValue<DateTime>("fecha_modificacion")
            };
        }
    }
}
