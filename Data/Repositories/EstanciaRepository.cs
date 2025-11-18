using Cassandra;
using Gestión_Hotelera.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gestión_Hotelera.Data.Repositories
{
    public class EstanciaRepository
    {
        private readonly ISession _session;

        public EstanciaRepository()
        {
            _session = CassandraConnection.Session;
        }

        public async Task CrearEstanciaAsync(EstanciaActivaModel e)
        {
            e.EstanciaId = Guid.NewGuid();

            var query = @"INSERT INTO estancias_activas
            (hotel_id, numero, estancia_id, cliente_id, reserva_id, fecha_entrada, adultos, menores)
            VALUES (?, ?, ?, ?, ?, ?, ?, ?)";

            var ps = _session.Prepare(query);

            await _session.ExecuteAsync(ps.Bind(
                e.HotelId, e.Numero, e.EstanciaId, e.ClienteId, e.ReservaId,
                e.FechaEntrada.Date, e.Adultos, e.Menores
            ));
        }
    }
}
