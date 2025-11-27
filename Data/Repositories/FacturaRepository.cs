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
            _session = CassandraConnection.GetSession();
        }

        // INSERT ===================================================
        public void Insert(FacturaModel f)
        {
            var query = @"INSERT INTO facturas (
                folio_factura, cliente_id, hotel_id, reserva_id, estancia_id,
                fecha_emision, fecha_entrada, fecha_salida,
                anticipo, monto_hospedaje, monto_servicios,
                descuento, total_factura
            ) VALUES (?,?,?,?,?,?,?,?,?,?,?,?,?);";

            var stmt = _session.Prepare(query);
            _session.Execute(stmt.Bind(
                f.FolioFactura, f.ClienteId, f.HotelId, f.ReservaId, f.EstanciaId,
                f.FechaEmision, f.FechaEntrada, f.FechaSalida,
                f.Anticipo, f.MontoHospedaje, f.MontoServicios,
                f.Descuento, f.TotalFactura
            ));
        }

        // GET BY ID ================================================
        public FacturaModel GetById(Guid folio)
        {
            var row = _session.Execute(
                _session.Prepare("SELECT * FROM facturas WHERE folio_factura=?;")
                .Bind(folio)
            ).FirstOrDefault();

            return row == null ? null : MapRow(row);
        }

        // GET BY CLIENTE ===========================================
        public List<FacturaModel> GetByCliente(Guid clienteId)
        {
            var result = _session.Execute("SELECT * FROM facturas;");
            return result
                .Where(r => r.GetValue<Guid>("cliente_id") == clienteId)
                .Select(MapRow)
                .ToList();
        }

        // GET BY HOTEL =============================================
        public List<FacturaModel> GetByHotel(Guid hotelId)
        {
            var result = _session.Execute("SELECT * FROM facturas;");
            return result
                .Where(r => r.GetValue<Guid>("hotel_id") == hotelId)
                .Select(MapRow)
                .ToList();
        }

        // GET BY RESERVA ===========================================
        public FacturaModel GetByReserva(Guid reservaId)
        {
            var result = _session.Execute("SELECT * FROM facturas;");
            return result
                .Where(r => r.GetValue<Guid>("reserva_id") == reservaId)
                .Select(MapRow)
                .FirstOrDefault();
        }

        // MAPEO =====================================================
        private FacturaModel MapRow(Row row)
        {
            return new FacturaModel
            {
                FolioFactura = row.GetValue<Guid>("folio_factura"),
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
    }
}
