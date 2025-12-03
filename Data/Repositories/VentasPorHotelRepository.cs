using Cassandra;
using Gestión_Hotelera.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gestión_Hotelera.Data.Repositories
{
    public class VentasPorHotelRepository
    {
        private readonly ISession _session;

        public VentasPorHotelRepository()
        {
            _session = CassandraConnection.GetSession();
        }

        public List<VentasHotelModel> Get(string pais, string ciudad, Guid hotelId, int año)
        {
            var query = _session.Prepare(
                "SELECT * FROM ventas_por_hotel WHERE pais = ? AND ciudad = ? AND hotel_id = ? AND año = ?;"
            );

            var rows = _session.Execute(query.Bind(pais, ciudad, hotelId, año));

            return rows.Select(r => new VentasHotelModel
            {
                Pais = r.GetValue<string>("pais"),
                Ciudad = r.GetValue<string>("ciudad"),
                HotelId = r.GetValue<Guid>("hotel_id"),
                Año = r.GetValue<int>("año"),
                Mes = r.GetValue<int>("mes"),
                IngresosHospedaje = r.GetValue<decimal>("ingresos_hospedaje"),
                IngresosServicios = r.GetValue<decimal>("ingresos_servicios"),
                Total = r.GetValue<decimal>("total")
            }).ToList();
        }
    }
}
