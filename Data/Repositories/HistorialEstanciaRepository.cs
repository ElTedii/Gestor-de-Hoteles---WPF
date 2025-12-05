using System;
using System.Collections.Generic;
using Cassandra;
using Gestión_Hotelera.Model;

namespace Gestión_Hotelera.Data.Repositories
{
    public class HistorialEstanciaRepository
    {
        private readonly ISession _session;

        public HistorialEstanciaRepository()
        {
            _session = CassandraConnection.GetSession();
        }

        // ======================================================
        // INSERTAR HISTORIAL (POST CHECK-OUT)
        // ======================================================
        public void Insert(HistorialEstanciaModel h)
        {
            var ps = _session.Prepare(@"
                INSERT INTO historial_estancias (
                    cliente_id,
                    estancia_id,
                    hotel_id,
                    numero_habitacion,
                    fecha_entrada,
                    fecha_salida,
                    anticipo,
                    monto_hospedaje,
                    monto_servicios,
                    total_factura,
                    usuario_registro,
                    fecha_registro
                ) VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?);
            ");

            var bs = ps.Bind(
                h.ClienteId,
                h.EstanciaId,
                h.HotelId,
                h.NumeroHabitacion,
                h.FechaEntrada,
                h.FechaSalida,
                h.Anticipo,
                h.MontoHospedaje,
                h.MontoServicios,
                h.TotalFactura,
                h.UsuarioRegistro,
                h.FechaRegistro
            );

            _session.Execute(bs);
        }

        // ======================================================
        // OBTENER POR CLIENTE
        // ======================================================
        public List<HistorialEstanciaModel> GetByCliente(Guid clienteId)
        {
            var ps = _session.Prepare("SELECT * FROM historial_estancias WHERE cliente_id = ?;");
            var bs = ps.Bind(clienteId);
            var result = _session.Execute(bs);

            var lista = new List<HistorialEstanciaModel>();

            foreach (var row in result)
                lista.Add(MapRow(row));

            return lista;
        }

        // ======================================================
        // OBTENER POR CLIENTE + ESTANCIA
        // ======================================================
        public HistorialEstanciaModel GetById(Guid clienteId, Guid estanciaId)
        {
            var ps = _session.Prepare("SELECT * FROM historial_estancias WHERE cliente_id = ? AND estancia_id = ?;");
            var bs = ps.Bind(clienteId, estanciaId);
            var result = _session.Execute(bs).FirstOrDefault();

            return result == null ? null : MapRow(result);
        }

        // ======================================================
        // OBTENER TODO EL HISTORIAL DE UN HOTEL
        // ======================================================
        public List<HistorialEstanciaModel> GetByHotel(Guid hotelId)
        {
            // Cassandra no permite WHERE con hotel_id → debemos filtrar en memoria
            var result = _session.Execute("SELECT * FROM historial_estancias;");

            var lista = new List<HistorialEstanciaModel>();

            foreach (var row in result)
            {
                if (row.GetValue<Guid>("hotel_id") == hotelId)
                    lista.Add(MapRow(row));
            }

            return lista;
        }

        // ======================================================
        // OBTENER TODO EL HISTORIAL
        // ======================================================
        public List<HistorialEstanciaModel> GetAll()
        {
            var result = _session.Execute("SELECT * FROM historial_estancias;");

            var lista = new List<HistorialEstanciaModel>();

            foreach (var row in result)
                lista.Add(MapRow(row));

            return lista;
        }

        // ======================================================
        // MAPEO CENTRAL — NULL SAFE
        // ======================================================
        private HistorialEstanciaModel MapRow(Row row)
        {
            return new HistorialEstanciaModel
            {
                ClienteId = row.GetValue<Guid>("cliente_id"),
                EstanciaId = row.GetValue<Guid>("estancia_id"),
                HotelId = row.GetValue<Guid>("hotel_id"),

                NumeroHabitacion = row.GetValue<int?>("numero_habitacion") ?? 0,

                FechaEntrada = row.GetValue<DateTime?>("fecha_entrada") ?? DateTime.MinValue,
                FechaSalida = row.GetValue<DateTime?>("fecha_salida") ?? DateTime.MinValue,

                Anticipo = row.GetValue<decimal?>("anticipo") ?? 0,
                MontoHospedaje = row.GetValue<decimal?>("monto_hospedaje") ?? 0,
                MontoServicios = row.GetValue<decimal?>("monto_servicios") ?? 0,
                TotalFactura = row.GetValue<decimal?>("total_factura") ?? 0,

                UsuarioRegistro = row.GetValue<string>("usuario_registro"),
                FechaRegistro = row.GetValue<DateTime?>("fecha_registro") ?? DateTime.MinValue
            };
        }
    }
}