using Cassandra;
using Gestión_Hotelera.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gestión_Hotelera.Data.Repositories
{
    namespace Gestión_Hotelera.Data.Repositories
    {
        public class HistorialEstanciaRepository
        {
            private readonly ISession _session;

            public HistorialEstanciaRepository()
            {
                _session = CassandraConnection.GetSession();
            }

            // ==========================================================
            // INSERT (Check-Out)
            // ==========================================================
            public void Insert(HistorialEstanciaModel h)
            {
                var query = @"INSERT INTO historial_estancias (
                cliente_id, estancia_id, hotel_id, numero,
                fecha_entrada, fecha_salida,
                anticipo, monto_hospedaje, monto_servicios,
                total_factura,
                usuario_registro, fecha_registro
            ) VALUES (?,?,?,?,?,?,?,?,?,?,?,?);";

                var stmt = _session.Prepare(query);

                _session.Execute(stmt.Bind(
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
                    h.FechaCreacion
                ));
            }

            // ==========================================================
            // SELECT: obtener historial completo de un cliente
            // ==========================================================
            public List<HistorialEstanciaModel> GetByCliente(Guid clienteId)
            {
                var result = _session.Execute(
                    _session.Prepare("SELECT * FROM historial_estancias WHERE cliente_id=?;")
                    .Bind(clienteId)
                );

                return result.Select(MapRow).ToList();
            }

            // ==========================================================
            // SELECT: obtener una estancia por su ID
            // ==========================================================
            public HistorialEstanciaModel GetByEstancia(Guid clienteId, Guid estanciaId)
            {
                var row = _session.Execute(
                    _session.Prepare("SELECT * FROM historial_estancias WHERE cliente_id=? AND estancia_id=?;")
                    .Bind(clienteId, estanciaId)
                ).FirstOrDefault();

                return row == null ? null : MapRow(row);
            }

            // ==========================================================
            // SELECT: obtener todas las estancias de un hotel
            // ==========================================================
            public List<HistorialEstanciaModel> GetByHotel(Guid hotelId)
            {
                var result = _session.Execute("SELECT * FROM historial_estancias;");

                return result
                    .Where(r => r.GetValue<Guid>("hotel_id") == hotelId)
                    .Select(MapRow)
                    .ToList();
            }

            // ==========================================================
            // MAPEO (Row → Modelo)
            // ==========================================================
            private HistorialEstanciaModel MapRow(Row row)
            {
                return new HistorialEstanciaModel
                {
                    ClienteId = row.GetValue<Guid>("cliente_id"),
                    EstanciaId = row.GetValue<Guid>("estancia_id"),
                    HotelId = row.GetValue<Guid>("hotel_id"),
                    NumeroHabitacion = row.GetValue<int>("numero"),

                    FechaEntrada = row.GetValue<DateTime>("fecha_entrada"),
                    FechaSalida = row.GetValue<DateTime>("fecha_salida"),

                    Anticipo = row.GetValue<decimal>("anticipo"),
                    MontoHospedaje = row.GetValue<decimal>("monto_hospedaje"),
                    MontoServicios = row.GetValue<decimal>("monto_servicios"),
                    TotalFactura = row.GetValue<decimal>("total_factura"),

                    UsuarioRegistro = row.GetValue<string>("usuario_registro"),
                    FechaCreacion = row.GetValue<DateTime>("fecha_registro")
                };
            }
        }
    }
