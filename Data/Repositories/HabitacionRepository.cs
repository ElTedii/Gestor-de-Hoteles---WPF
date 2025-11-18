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
            _session = CassandraConnection.Session;
        }

        public async Task InsertarHabitacionAsync(HabitacionModel h)
        {
            var query = @"INSERT INTO habitaciones
            (hotel_id, numero, tipo_id, piso)
            VALUES (?, ?, ?, ?)";

            var ps = _session.Prepare(query);

            await _session.ExecuteAsync(ps.Bind(
                h.HotelId, h.Numero, h.TipoId, h.Piso
            ));
        }

        public async Task<IEnumerable<HabitacionModel>> ObtenerPorHotelAsync(Guid hotelId)
        {
            var query = "SELECT * FROM habitaciones WHERE hotel_id = ?";
            var ps = _session.Prepare(query);
            var rs = await _session.ExecuteAsync(ps.Bind(hotelId));

            return rs.Select(row => new HabitacionModel
            {
                HotelId = hotelId,
                Numero = row.GetValue<int>("numero"),
                TipoId = row.GetValue<Guid>("tipo_id"),
                Piso = row.GetValue<int>("piso")
            });
        }
    }
}
