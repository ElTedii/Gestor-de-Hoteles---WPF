using Cassandra;
using Gestión_Hotelera.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gestión_Hotelera.Data.Repositories
{
    public class HistorialEstanciaRepository
    {
        private readonly ISession _session;

        public HistorialEstanciaRepository()
        {
            _session = CassandraConnection.Session;
        }

        public async Task GuardarHistorialAsync(HistorialEstanciaModel h)
        {
            string delete = @"DELETE FROM estancias_activas
                             WHERE hotel_id = ? AND numero = ?;";

            string insert = @"INSERT INTO historial_estancias
                             (cliente_id, estancia_id, hotel_id, numero, fecha_entrada, fecha_salida,
                              anticipo, monto_hospedaje, monto_servicios, total_factura,
                              usuario_registro, fecha_registro)
                             VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, toTimestamp(now()));";

            await _session.ExecuteAsync(_session.Prepare(delete).Bind(h.HotelId, h.Numero));

            await _session.ExecuteAsync(_session.Prepare(insert).Bind(
                h.ClienteId, h.EstanciaId, h.HotelId, h.Numero,
                h.FechaEntrada.Date, h.FechaSalida.Date,
                h.Anticipo, h.MontoHospedaje, h.MontoServicios, h.TotalFactura,
                h.UsuarioRegistro
            ));
        }
    }
}
