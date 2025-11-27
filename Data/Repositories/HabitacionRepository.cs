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

        // -------------------------------------------------------
        // MAPEO: Row → HabitacionModel
        // -------------------------------------------------------
        private HabitacionModel MapRow(Row row)
        {
            if (row == null) return null;

            return new HabitacionModel
            {
                HotelId = row.GetValue<Guid>("hotel_id"),
                Numero = row.GetValue<int>("numero"),
                TipoId = row.GetValue<Guid>("tipo_id"),
                Piso = row.GetValue<int>("piso"),

                UsuarioCreacion = row.GetValue<string>("usuario_creacion"),
                FechaCreacion = row.GetValue<DateTime>("fecha_creacion"),
                UsuarioModificacion = row.GetValue<string>("usuario_modificacion"),
                FechaModificacion = row.GetValue<DateTime>("fecha_modificacion")
            };
        }

        // -------------------------------------------------------
        // INSERT
        // -------------------------------------------------------
        public void Insert(HabitacionModel model)
        {
            var query = @"
                INSERT INTO habitaciones (
                    hotel_id, numero, tipo_id, piso,
                    usuario_creacion, fecha_creacion,
                    usuario_modificacion, fecha_modificacion
                ) VALUES (?, ?, ?, ?, ?, ?, ?, ?);";

            var stmt = _session.Prepare(query);

            _session.Execute(stmt.Bind(
                model.HotelId,
                model.Numero,
                model.TipoId,
                model.Piso,
                model.UsuarioCreacion,
                model.FechaCreacion,
                model.UsuarioModificacion,
                model.FechaModificacion
            ));
        }

        // -------------------------------------------------------
        // GET → Habitaciones por hotel
        // -------------------------------------------------------
        public List<HabitacionModel> GetByHotel(Guid hotelId)
        {
            var stmt = _session.Prepare(
                "SELECT * FROM habitaciones WHERE hotel_id = ?;"
            );

            var rows = _session.Execute(stmt.Bind(hotelId));
            var lista = new List<HabitacionModel>();

            foreach (var row in rows)
                lista.Add(MapRow(row));

            return lista;
        }

        // -------------------------------------------------------
        // GET → Una habitación específica
        // -------------------------------------------------------
        public HabitacionModel GetByNumero(Guid hotelId, int numero)
        {
            var stmt = _session.Prepare(
                "SELECT * FROM habitaciones WHERE hotel_id = ? AND numero = ?;"
            );

            var result = _session.Execute(stmt.Bind(hotelId, numero));

            return MapRow(result.FirstOrDefault());
        }

        // -------------------------------------------------------
        // UPDATE
        // -------------------------------------------------------
        public void Update(HabitacionModel model)
        {
            var query = @"
                UPDATE habitaciones SET
                    tipo_id = ?, piso = ?,
                    usuario_modificacion = ?, fecha_modificacion = ?
                WHERE hotel_id = ? AND numero = ?;";

            var stmt = _session.Prepare(query);

            _session.Execute(stmt.Bind(
                model.TipoId,
                model.Piso,
                model.UsuarioModificacion,
                model.FechaModificacion,
                model.HotelId,
                model.Numero
            ));
        }

        // -------------------------------------------------------
        // DELETE
        // -------------------------------------------------------
        public void Delete(Guid hotelId, int numero)
        {
            var stmt = _session.Prepare(
                "DELETE FROM habitaciones WHERE hotel_id = ? AND numero = ?;"
            );

            _session.Execute(stmt.Bind(hotelId, numero));
        }
    }
}
