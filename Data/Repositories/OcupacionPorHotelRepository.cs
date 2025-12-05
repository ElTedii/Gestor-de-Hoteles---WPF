using Cassandra;
using Gestión_Hotelera.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gestión_Hotelera.Data.Repositories
{
    public class OcupacionPorHotelRepository
    {
        private readonly ISession _session;

        public OcupacionPorHotelRepository()
        {
            _session = CassandraConnection.GetSession();
        }

        public List<OcupacionHotelModel> Get(Guid hotelId, int año, int mes)
        {
            var query = _session.Prepare(
                "SELECT * FROM ocupacion_por_hotel WHERE hotel_id = ? AND año = ? AND mes = ?;"
            );

            var rows = _session.Execute(query.Bind(hotelId, año, mes));

            return rows.Select(r => new OcupacionHotelModel
            {
                HotelId = r.GetValue<Guid>("hotel_id"),
                Año = r.GetValue<int>("año"),
                Mes = r.GetValue<int>("mes"),
                FechaEntrada = r.GetValue<DateTime>("fecha_entrada"),
                EstanciaId = r.GetValue<Guid>("estancia_id"),
                ClienteId = r.GetValue<Guid>("cliente_id"),
                TipoHabitacion = r.GetValue<string>("tipo_habitacion"),
                NumeroHabitacion = r.GetValue<int>("numero__habitacion"),
                FechaSalida = r.GetValue<DateTime>("fecha_salida"),
                Estado = r.GetValue<string>("estado"),
                PagoHospedaje = r.GetValue<decimal>("pago_hospedaje"),
                PagoServicios = r.GetValue<decimal>("pago_servicios")
            }).ToList();
        }
    }
}
