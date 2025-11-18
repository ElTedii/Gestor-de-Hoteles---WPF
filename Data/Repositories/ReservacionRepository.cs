using Cassandra;
using Gestión_Hotelera.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gestión_Hotelera.Data.Repositories
{
    public class ReservacionRepository
    {
        private readonly ISession _session;

        public ReservacionRepository()
        {
            _session = CassandraConnection.Session;
        }

        // ===========================================
        // INSERT: Crear nueva reservación (3 tablas)
        // ===========================================
        public bool CrearReservacion(ReservationModel r)
        {
            try
            {
                var batch = new BatchStatement().SetBatchType(BatchType.Logged);

                // 1) reservas_por_cliente
                var stmt1 = _session.Prepare(@"
                    INSERT INTO reservas_por_cliente
                    (cliente_id, reserva_id, hotel_id, fecha_entrada, fecha_salida,
                     anticipo, estado, usuario_registro, fecha_registro)
                    VALUES (?, ?, ?, ?, ?, ?, ?, ?, toTimestamp(now()));
                ");

                batch.Add(stmt1.Bind(
                    r.ClienteId, r.ReservaId, r.HotelId,
                    r.FechaEntrada, r.FechaSalida,
                    r.Anticipo, r.Estado, r.UsuarioRegistro
                ));

                // 2) reservas_por_hotel
                var stmt2 = _session.Prepare(@"
                    INSERT INTO reservas_por_hotel
                    (hotel_id, fecha_entrada, reserva_id, cliente_id,
                     fecha_salida, estado)
                    VALUES (?, ?, ?, ?, ?, ?);
                ");

                batch.Add(stmt2.Bind(
                    r.HotelId, r.FechaEntrada, r.ReservaId,
                    r.ClienteId, r.FechaSalida, r.Estado
                ));

                // 3) reservas_por_fecha
                var stmt3 = _session.Prepare(@"
                    INSERT INTO reservas_por_fecha
                    (fecha_entrada, reserva_id, hotel_id, cliente_id, estado)
                    VALUES (?, ?, ?, ?, ?);
                ");

                batch.Add(stmt3.Bind(
                    r.FechaEntrada, r.ReservaId, r.HotelId,
                    r.ClienteId, r.Estado
                ));

                _session.Execute(batch);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error en CrearReservacion: " + ex.Message);
                return false;
            }
        }

        // ================================
        // CONSULTAS
        // ================================

        // Buscar por cliente
        public RowSet ObtenerReservasPorCliente(Guid clienteId)
        {
            return _session.Execute(
                new SimpleStatement("SELECT * FROM reservas_por_cliente WHERE cliente_id = ?", clienteId)
            );
        }

        // Buscar reservas por hotel en una fecha
        public RowSet ObtenerReservasPorHotel(Guid hotelId, DateTime fechaEntrada)
        {
            return _session.Execute(
                new SimpleStatement("SELECT * FROM reservas_por_hotel WHERE hotel_id = ? AND fecha_entrada = ?",
                hotelId, fechaEntrada)
            );
        }

        // Validar disponibilidad
        public bool ExisteReservaEnFecha(Guid hotelId, DateTime entrada)
        {
            var result = _session.Execute(new SimpleStatement(
                "SELECT reserva_id FROM reservas_por_hotel WHERE hotel_id = ? AND fecha_entrada = ?",
                hotelId, entrada));

            return !result.IsExhausted();
        }

        // Obtener por código (GUID)
        public RowSet ObtenerReservaPorCodigo(Guid reservaId)
        {
            return _session.Execute(
                new SimpleStatement("SELECT * FROM reservas_por_cliente WHERE reserva_id = ?", reservaId)
            );
        }
    }
}
