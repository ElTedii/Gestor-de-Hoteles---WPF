using Cassandra;
using Gestión_Hotelera.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gestión_Hotelera.Data.Repositories
{
    public class HistorialClienteRepository
    {
        private readonly ISession _session;

        public HistorialClienteRepository()
        {
            _session = CassandraConnection.GetSession();
        }

        public List<HistorialClienteModel> Get(Guid clienteId, int año)
        {
            var query = _session.Prepare(
                "SELECT * FROM historial_por_cliente WHERE cliente_id = ? AND año = ?;"
            );

            var rows = _session.Execute(query.Bind(clienteId, año));

            return rows.Select(r => new HistorialClienteModel
            {
                ClienteId = r.GetValue<Guid>("cliente_id"),
                Año = r.GetValue<int>("año"),
                FechaReserva = r.GetValue<DateTime>("fecha_reserva"),
                ReservaId = r.GetValue<Guid>("reserva_id"),
                HotelId = r.GetValue<Guid>("hotel_id"),
                TipoHabitacion = r.GetValue<string>("tipo_habitacion"),
                NumeroHabitacion = r.GetValue<int>("numero_habitacion"),
                Personas = r.GetValue<int>("personas"),
                FechaCheckIn = r.GetValue<DateTime>("fecha_check_in"),
                FechaCheckOut = r.GetValue<DateTime>("fecha_check_out"),
                Estado = r.GetValue<string>("estado"),
                Anticipo = r.GetValue<decimal>("anticipo"),
                Hospedaje = r.GetValue<decimal>("hospedaje"),
                Servicios = r.GetValue<decimal>("servicios"),
                Total = r.GetValue<decimal>("total")
            }).ToList();
        }
    }
}
