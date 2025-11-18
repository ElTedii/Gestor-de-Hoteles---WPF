using Cassandra;
using Gestión_Hotelera.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gestión_Hotelera.Data.Repositories
{
    public class FacturaRepository
    {
        private readonly ISession _session;

        public FacturaRepository()
        {
            _session = CassandraConnection.Session;
        }

        // ================================
        // INSERT: Guardar nueva factura
        // ================================
        public async Task<bool> InsertarFacturaAsync(FacturaModel f)
        {
            try
            {
                if (f.FolioFactura == Guid.Empty)
                    f.FolioFactura = Guid.NewGuid();

                var query = @"
                    INSERT INTO facturas
                    (folio_factura, cliente_id, hotel_id, reserva_id, estancia_id,
                     fecha_emision, fecha_entrada, fecha_salida,
                     anticipo, monto_hospedaje, monto_servicios,
                     descuento, total_factura)
                    VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?);";

                var ps = _session.Prepare(query);

                await _session.ExecuteAsync(ps.Bind(
                    f.FolioFactura,
                    f.ClienteId,
                    f.HotelId,
                    f.ReservaId,
                    f.EstanciaId,
                    f.FechaEmision,
                    f.FechaEntrada.Date,
                    f.FechaSalida.Date,
                    f.Anticipo,
                    f.MontoHospedaje,
                    f.MontoServicios,
                    f.Descuento,
                    f.TotalFactura
                ));

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error InsertarFacturaAsync: " + ex.Message);
                return false;
            }
        }

        // ================================
        // SELECT: Por folio
        // ================================
        public async Task<FacturaModel> ObtenerPorFolioAsync(Guid folio)
        {
            var query = "SELECT * FROM facturas WHERE folio_factura = ?";
            var ps = _session.Prepare(query);
            var rs = await _session.ExecuteAsync(ps.Bind(folio));
            var row = rs.FirstOrDefault();

            if (row == null)
                return null;

            return new FacturaModel
            {
                FolioFactura = folio,
                ClienteId = row.GetValue<Guid>("cliente_id"),
                HotelId = row.GetValue<Guid>("hotel_id"),
                ReservaId = row.GetValue<Guid>("reserva_id"),
                EstanciaId = row.GetValue<Guid>("estancia_id"),
                FechaEmision = row.GetValue<DateTime>("fecha_emision"),
                FechaEntrada = row.GetValue<DateTime>("fecha_entrada"),
                FechaSalida = row.GetValue<DateTime>("fecha_salida"),
                Anticipo = row.GetValue<decimal>("anticipo"),
                MontoHospedaje = row.GetValue<decimal>("monto_hospedaje"),
                MontoServicios = row.GetValue<decimal>("monto_servicios"),
                Descuento = row.GetValue<decimal>("descuento"),
                TotalFactura = row.GetValue<decimal>("total_factura")
            };
        }

        // ================================
        // SELECT: Facturas por cliente
        // (útil para historial de cliente)
        // ================================
        public async Task<RowSet> ObtenerFacturasRawPorClienteAsync(Guid clienteId)
        {
            // Nota: PK es folio_factura; esta consulta requiere ALLOW FILTERING
            // y no es ideal para grandes volúmenes, pero en proyecto escolar está ok.
            var query = "SELECT * FROM facturas WHERE cliente_id = ? ALLOW FILTERING;";
            var ps = _session.Prepare(query);
            return await _session.ExecuteAsync(ps.Bind(clienteId));
        }
    }
}
