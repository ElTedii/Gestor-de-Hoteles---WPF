using Cassandra;
using Gestión_Hotelera.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Gestión_Hotelera.Data.Repositories
{
    public class OcupacionReportRepository
    {
        private readonly ISession _session;
        private readonly HotelRepository _hotelRepo;
        private readonly ClienteRepository _clienteRepo;
        private readonly HabitacionRepository _habRepo;

        public OcupacionReportRepository()
        {
            _session = CassandraConnection.GetSession();
            _hotelRepo = new HotelRepository();
            _clienteRepo = new ClienteRepository();
            _habRepo = new HabitacionRepository();
        }

        public List<OcupacionReporteModel> GetOcupacion(Guid hotelId, int año, int mes)
        {
            // Rango de fechas del mes
            var inicio = new DateTime(año, mes, 1);
            var fin = inicio.AddMonths(1);

            // NOTA: usamos ALLOW FILTERING porque la PK de historial_estancias es (cliente_id, estancia_id)
            var query = @"
                SELECT * FROM historial_estancias
                WHERE hotel_id = ?
                  AND fecha_entrada >= ?
                  AND fecha_entrada < ?
                ALLOW FILTERING;
            ";

            var stmt = _session.Prepare(query);
            var rows = _session.Execute(stmt.Bind(
                hotelId,
                inicio,
                fin
            ));

            var hotel = _hotelRepo.GetById(hotelId);
            string hotelNombre = hotel?.Nombre ?? "Hotel";

            var lista = new List<OcupacionReporteModel>();

            foreach (var r in rows)
            {
                var clienteId = r.GetValue<Guid>("cliente_id");
                var cliente = _clienteRepo.GetById(clienteId);

                int numHab = r.GetValue<int>("numero");

                // Intentamos recuperar datos de la habitación (tipo)
                var hab = _habRepo.GetByHotelAndNumero(hotelId, numHab);

                var model = new OcupacionReporteModel
                {
                    HotelId = hotelId,
                    HotelNombre = hotelNombre,
                    Año = año,
                    Mes = mes,
                    MesNombre = MesToString(mes),

                    ClienteId = clienteId,
                    ClienteNombre = cliente?.NombreCompleto ?? "(sin nombre)",

                    TipoHabitacion = hab?.TipoNombre ?? "N/A",
                    NumeroHabitacion = numHab,

                    FechaEntrada = r.GetValue<DateTime>("fecha_entrada"),
                    FechaSalida = r.GetValue<DateTime>("fecha_salida"),

                    Estado = "FINALIZADA",
                    PagoHospedaje = r.GetValue<decimal>("monto_hospedaje"),
                    PagoServicios = r.GetValue<decimal>("monto_servicios")
                };

                lista.Add(model);
            }

            return lista
                .OrderBy(x => x.FechaEntrada)
                .ThenBy(x => x.NumeroHabitacion)
                .ToList();
        }

        private string MesToString(int mes)
        {
            string[] nombres = {
                "", "Enero","Febrero","Marzo","Abril","Mayo","Junio",
                "Julio","Agosto","Septiembre","Octubre","Noviembre","Diciembre"
            };
            return (mes >= 1 && mes <= 12) ? nombres[mes] : mes.ToString();
        }
    }
}