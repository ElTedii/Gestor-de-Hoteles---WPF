using Cassandra;
using Gestión_Hotelera.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gestión_Hotelera.Data.Repositories
{
    public class CheckInRepository
    {
        private readonly ISession _session;

        public CheckInRepository()
        {
            _session = CassandraConnection.Session;
        }

        // Check-in completo usando EstanciaActivaModel
        public bool RealizarCheckIn(EstanciaActivaModel estancia)
        {
            try
            {
                var batch = new BatchStatement().SetBatchType(BatchType.Logged);

                // 1) Estancia activa
                var stmtEstancia = _session.Prepare(@"
                    INSERT INTO estancias_activas
                    (hotel_id, numero, estancia_id, cliente_id, reserva_id,
                     fecha_entrada, fecha_salida, anticipo, precio_noche,
                     adultos, menores)
                    VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?);
                ");

                batch.Add(stmtEstancia.Bind(
                    estancia.HotelId,
                    estancia.Numero,
                    estancia.EstanciaId,
                    estancia.ClienteId,
                    estancia.ReservaId,
                    estancia.FechaEntrada.Date,
                    estancia.FechaSalida.Date,
                    estancia.Anticipo,
                    estancia.PrecioPorNoche,
                    estancia.Adultos,
                    estancia.Menores
                ));

                // 2) Marcar días como ocupados en disponibilidad_habitacion
                var stmtDisp = _session.Prepare(@"
                    INSERT INTO disponibilidad_habitacion
                    (hotel_id, numero, fecha, disponible)
                    VALUES (?, ?, ?, false);
                ");

                var fecha = estancia.FechaEntrada.Date;
                while (fecha < estancia.FechaSalida.Date)
                {
                    batch.Add(stmtDisp.Bind(
                        estancia.HotelId,
                        estancia.Numero,
                        fecha
                    ));

                    fecha = fecha.AddDays(1);
                }

                // 3) Actualizar estado de reserva -> 'checkin'
                var stmtCli = _session.Prepare(@"
                    UPDATE reservas_por_cliente
                    SET estado = 'checkin'
                    WHERE cliente_id = ? AND reserva_id = ?;
                ");

                batch.Add(stmtCli.Bind(
                    estancia.ClienteId,
                    estancia.ReservaId
                ));

                var stmtHotel = _session.Prepare(@"
                    UPDATE reservas_por_hotel
                    SET estado = 'checkin'
                    WHERE hotel_id = ? AND fecha_entrada = ? AND reserva_id = ?;
                ");

                batch.Add(stmtHotel.Bind(
                    estancia.HotelId,
                    estancia.FechaEntrada.Date,
                    estancia.ReservaId
                ));

                var stmtFecha = _session.Prepare(@"
                    UPDATE reservas_por_fecha
                    SET estado = 'checkin'
                    WHERE fecha_entrada = ? AND reserva_id = ?;
                ");

                batch.Add(stmtFecha.Bind(
                    estancia.FechaEntrada.Date,
                    estancia.ReservaId
                ));

                _session.Execute(batch);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error en RealizarCheckIn: " + ex.Message);
                return false;
            }
        }
    }
}
