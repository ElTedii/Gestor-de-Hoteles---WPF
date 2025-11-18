using Cassandra;
using Gestión_Hotelera.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gestión_Hotelera.Data.Repositories
{
    public class CheckOutRepository
    {
        private readonly ISession _session;

        public CheckOutRepository()
        {
            _session = CassandraConnection.Session;
        }

        // Check-Out completo:
        // - historial_estancias
        // - libera disponibilidad
        // - borra estancias_activas
        // - actualiza reservas a 'finalizada'
        public bool RealizarCheckOut(
            HistorialEstanciaModel historial,
            DateTime fechaEntrada,
            DateTime fechaSalida,
            Guid reservaId
        )
        {
            try
            {
                var batch = new BatchStatement().SetBatchType(BatchType.Logged);

                // 1) historial_estancias
                var stmtHist = _session.Prepare(@"
                    INSERT INTO historial_estancias
                    (cliente_id, estancia_id, hotel_id, numero,
                     fecha_entrada, fecha_salida, anticipo,
                     monto_hospedaje, monto_servicios, total_factura,
                     usuario_registro, fecha_registro)
                    VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, toTimestamp(now()));
                ");

                batch.Add(stmtHist.Bind(
                    historial.ClienteId,
                    historial.EstanciaId,
                    historial.HotelId,
                    historial.Numero,
                    historial.FechaEntrada.Date,
                    historial.FechaSalida.Date,
                    historial.Anticipo,
                    historial.MontoHospedaje,
                    historial.MontoServicios,
                    historial.TotalFactura,
                    historial.UsuarioRegistro
                ));

                // 2) liberar disponibilidad (disponible = true)
                var stmtDisp = _session.Prepare(@"
                    INSERT INTO disponibilidad_habitacion
                    (hotel_id, numero, fecha, disponible)
                    VALUES (?, ?, ?, true);
                ");

                var fecha = fechaEntrada.Date;
                while (fecha < fechaSalida.Date)
                {
                    batch.Add(stmtDisp.Bind(
                        historial.HotelId,
                        historial.Numero,
                        fecha
                    ));

                    fecha = fecha.AddDays(1);
                }

                // 3) borrar de estancias_activas
                var stmtDel = _session.Prepare(@"
                    DELETE FROM estancias_activas
                    WHERE hotel_id = ? AND numero = ?;
                ");

                batch.Add(stmtDel.Bind(
                    historial.HotelId,
                    historial.Numero
                ));

                // 4) actualizar estado de reservas -> 'finalizada'
                var stmtCli = _session.Prepare(@"
                    UPDATE reservas_por_cliente
                    SET estado = 'finalizada'
                    WHERE cliente_id = ? AND reserva_id = ?;
                ");
                batch.Add(stmtCli.Bind(
                    historial.ClienteId,
                    reservaId
                ));

                var stmtHotel = _session.Prepare(@"
                    UPDATE reservas_por_hotel
                    SET estado = 'finalizada'
                    WHERE hotel_id = ? AND fecha_entrada = ? AND reserva_id = ?;
                ");
                batch.Add(stmtHotel.Bind(
                    historial.HotelId,
                    fechaEntrada.Date,
                    reservaId
                ));

                var stmtFecha = _session.Prepare(@"
                    UPDATE reservas_por_fecha
                    SET estado = 'finalizada'
                    WHERE fecha_entrada = ? AND reserva_id = ?;
                ");
                batch.Add(stmtFecha.Bind(
                    fechaEntrada.Date,
                    reservaId
                ));

                _session.Execute(batch);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error en RealizarCheckOut: " + ex.Message);
                return false;
            }
        }
    }
}
