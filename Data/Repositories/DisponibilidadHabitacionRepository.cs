using Cassandra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gestión_Hotelera.Data.Repositories
{
    public class DisponibilidadHabitacionRepository
    {
        private readonly ISession _session;

        public DisponibilidadHabitacionRepository()
        {
            _session = CassandraConnection.Session;
        }

        public async Task SetDisponibilidadAsync(Guid hotelId, int numero, DateTime fecha, bool disponible)
        {
            var query = @"INSERT INTO disponibilidad_habitacion
                          (hotel_id, numero, fecha, disponible)
                          VALUES (?, ?, ?, ?)";

            var ps = _session.Prepare(query);

            await _session.ExecuteAsync(ps.Bind(
                hotelId, numero, fecha.Date, disponible
            ));
        }

        public async Task<bool> EstaDisponibleAsync(Guid hotelId, int numero, DateTime fecha)
        {
            var query = @"SELECT disponible FROM disponibilidad_habitacion
                          WHERE hotel_id = ? AND numero = ? AND fecha = ?";

            var ps = _session.Prepare(query);
            var rs = await _session.ExecuteAsync(ps.Bind(hotelId, numero, fecha.Date));

            var row = rs.FirstOrDefault();
            return row != null && row.GetValue<bool>("disponible");
        }
    }
}
