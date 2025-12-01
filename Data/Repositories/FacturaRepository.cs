using Cassandra;
using Gestión_Hotelera.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Gestión_Hotelera.Data.Repositories
{
    public class FacturaRepository
    {
        private readonly ISession _session;

        public FacturaRepository()
        {
            _session = CassandraConnection.GetSession();
        }

        // ==============================================================
        // INSERTAR FACTURA
        // ==============================================================
        public void Insert(FacturaModel f)
        {
            var ps = _session.Prepare(@"
                INSERT INTO facturas (
                    factura_id,
                    estancia_id,
                    cliente_id,
                    hotel_id,
                    fecha_emision,
                    monto_hospedaje,
                    monto_servicios,
                    total,
                    usuario_registro,
                    fecha_registro
                ) VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?);
            ");

            var bs = ps.Bind(
                f.FacturaId,
                f.EstanciaId,
                f.ClienteId,
                f.HotelId,
                f.FechaEmision,
                f.MontoHospedaje,
                f.MontoServicios,
                f.Total,
                f.UsuarioRegistro,
                f.FechaRegistro
            );

            _session.Execute(bs);
        }

        // ==============================================================
        // OBTENER POR ID
        // ==============================================================
        public FacturaModel GetById(Guid facturaId)
        {
            var ps = _session.Prepare("SELECT * FROM facturas WHERE factura_id = ?;");
            var row = _session.Execute(ps.Bind(facturaId)).FirstOrDefault();
            return row == null ? null : MapRow(row);
        }

        // ==============================================================
        // OBTENER TODAS LAS FACTURAS
        // ==============================================================
        public List<FacturaModel> GetAll()
        {
            var result = _session.Execute("SELECT * FROM facturas;");
            return result.Select(MapRow).ToList();
        }

        // ==============================================================
        // OBTENER INGRESOS DESDE UNA FECHA
        // (Usado en Dashboard)
        // ==============================================================
        public decimal GetIngresosDesde(DateTime fecha)
        {
            var result = _session.Execute("SELECT * FROM facturas;");

            return result
                .Where(r =>
                    r.GetValue<DateTime>("fecha_emision") >= fecha)
                .Sum(r => r.GetValue<decimal>("total"));
        }

        // ==============================================================
        // MAPEO SEGURO
        // ==============================================================
        private FacturaModel MapRow(Row row)
        {
            return new FacturaModel
            {
                FacturaId = row.GetValue<Guid>("factura_id"),
                EstanciaId = row.GetValue<Guid>("estancia_id"),
                ClienteId = row.GetValue<Guid>("cliente_id"),
                HotelId = row.GetValue<Guid>("hotel_id"),

                FechaEmision = row.GetValue<DateTime?>("fecha_emision") ?? DateTime.MinValue,

                MontoHospedaje = row.GetValue<decimal?>("monto_hospedaje") ?? 0,
                MontoServicios = row.GetValue<decimal?>("monto_servicios") ?? 0,
                Total = row.GetValue<decimal?>("total") ?? 0,

                UsuarioRegistro = row.GetValue<string>("usuario_registro"),
                FechaRegistro = row.GetValue<DateTime?>("fecha_registro") ?? DateTime.MinValue
            };
        }
    }
}