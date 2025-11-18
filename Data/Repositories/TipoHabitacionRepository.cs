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
            _session = CassandraConnection.Session;
        }

        public async Task InsertarTipoAsync(TipoHabitacionModel t)
        {
            t.TipoId = Guid.NewGuid();

            var query = @"INSERT INTO tipos_habitacion_por_hotel
            (hotel_id, tipo_id, nombre_tipo, capacidad, precio_noche, cantidad,
             caracteristicas, amenidades, nivel, vista, usuario_registro, fecha_registro)
            VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, toTimestamp(now()));";

            var ps = _session.Prepare(query);

            await _session.ExecuteAsync(ps.Bind(
                t.HotelId, t.TipoId, t.NombreTipo, t.Capacidad, t.PrecioNoche,
                t.Cantidad, t.Caracteristicas, t.Amenidades, t.Nivel, t.Vista,
                t.UsuarioRegistro
            ));
        }

        public async Task<IEnumerable<TipoHabitacionModel>> ObtenerPorHotelAsync(Guid hotelId)
        {
            var query = "SELECT * FROM tipos_habitacion_por_hotel WHERE hotel_id = ?";
            var ps = _session.Prepare(query);
            var rs = await _session.ExecuteAsync(ps.Bind(hotelId));

            return rs.Select(row => new TipoHabitacionModel
            {
                HotelId = hotelId,
                TipoId = row.GetValue<Guid>("tipo_id"),
                NombreTipo = row.GetValue<string>("nombre_tipo"),
                Capacidad = row.GetValue<int>("capacidad"),
                PrecioNoche = row.GetValue<decimal>("precio_noche"),
                Cantidad = row.GetValue<int>("cantidad"),
                Caracteristicas = row.GetValue<List<string>>("caracteristicas"),
                Amenidades = row.GetValue<List<string>>("amenidades"),
                Nivel = row.GetValue<string>("nivel"),
                Vista = row.GetValue<string>("vista"),
                UsuarioRegistro = row.GetValue<int>("usuario_registro"),
                FechaRegistro = row.GetValue<DateTime?>("fecha_registro")
            });
        }
    }
}
