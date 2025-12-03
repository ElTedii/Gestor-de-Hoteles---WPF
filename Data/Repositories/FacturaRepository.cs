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

        // ============================================================
        // INSERTAR FACTURA
        // ============================================================
        public void Insert(FacturaModel f)
        {
            f.FacturaId = f.FacturaId == Guid.Empty ? Guid.NewGuid() : f.FacturaId;
            f.FechaRegistro = f.FechaRegistro == default ? DateTime.UtcNow : f.FechaRegistro;

            const string query = @"
                INSERT INTO facturas (
                    factura_id, estancia_id, cliente_id, hotel_id,
                    fecha_emision, monto_hospedaje, monto_servicios,
                    total, usuario_registro, fecha_registro
                ) VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?);";

            _session.Execute(
                _session.Prepare(query).Bind(
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
                )
            );
        }

        // ============================================================
        // OBTENER FACTURA POR ID
        // ============================================================
        public FacturaModel GetById(Guid facturaId)
        {
            var row = _session.Execute(
                _session.Prepare(
                    "SELECT * FROM facturas WHERE factura_id=?;"
                ).Bind(facturaId)
            ).FirstOrDefault();

            return row == null ? null : MapRow(row);
        }

        // ============================================================
        // OBTENER FACTURAS POR CLIENTE
        // ============================================================
        public List<FacturaModel> GetByCliente(Guid clienteId)
        {
            var rows = _session.Execute(
                _session.Prepare(
                    "SELECT * FROM facturas WHERE cliente_id=? ALLOW FILTERING;"
                ).Bind(clienteId)
            );

            return rows.Select(MapRow).ToList();
        }

        // ============================================================
        // OBTENER TODAS (debug / reportes)
        // ============================================================
        public List<FacturaModel> GetAll()
        {
            var result = _session.Execute("SELECT * FROM facturas;");
            return result.Select(MapRow).ToList();
        }

        public decimal GetIngresosDesde(DateTime fechaInicio)
        {
            var query = @"
                        SELECT total 
                        FROM facturas 
                        WHERE fecha_emision >= ? 
                        ALLOW FILTERING;
                    ";

            var rows = _session.Execute(_session.Prepare(query).Bind(fechaInicio));

            decimal suma = 0;

            foreach (var row in rows)
                suma += row.GetValue<decimal>("total");

            return suma;
        }

        // ============================================================
        // MAPEO
        // ============================================================
        private FacturaModel MapRow(Row row)
        {
            return new FacturaModel
            {
                FacturaId = row.GetValue<Guid>("factura_id"),
                EstanciaId = row.GetValue<Guid>("estancia_id"),
                ClienteId = row.GetValue<Guid>("cliente_id"),
                HotelId = row.GetValue<Guid>("hotel_id"),

                FechaEmision = row.GetValue<DateTime>("fecha_emision"),
                MontoHospedaje = row.GetValue<decimal>("monto_hospedaje"),
                MontoServicios = row.GetValue<decimal>("monto_servicios"),
                Total = row.GetValue<decimal>("total"),

                UsuarioRegistro = row.GetValue<string>("usuario_registro"),
                FechaRegistro = row.GetValue<DateTime>("fecha_registro")
            };
        }
    }
}