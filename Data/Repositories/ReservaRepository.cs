using Cassandra;
using Gestión_Hotelera.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Gestión_Hotelera.Data.Repositories
{
    public class ReservaRepository
    {
        private readonly ISession _session;

        public ReservaRepository()
        {
            _session = CassandraConnection.GetSession();
        }

        // ============================================================
        // INSERTAR RESERVA  (Escribe en 3 tablas)
        // ============================================================
        public void Insert(ReservacionModel r)
        {
            r.ReservaId = r.ReservaId == Guid.Empty ? Guid.NewGuid() : r.ReservaId;
            r.FechaRegistro = DateTime.UtcNow;
            r.Estado ??= "PENDIENTE";

            // ---------------- reservas_por_cliente ----------------
            const string qCliente = @"
                INSERT INTO reservas_por_cliente (
                    cliente_id, reserva_id, hotel_id,
                    fecha_entrada, fecha_salida,
                    anticipo, estado,
                    adultos, menores,
                    usuario_registro, fecha_registro
                ) VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?);";

            _session.Execute(_session.Prepare(qCliente).Bind(
                r.ClienteId, r.ReservaId, r.HotelId,
                r.FechaEntrada, r.FechaSalida,
                r.Anticipo, r.Estado,
                r.Adultos, r.Menores,
                r.UsuarioRegistro, r.FechaRegistro
            ));

            // ---------------- reservas_por_hotel ----------------
            const string qHotel = @"
                INSERT INTO reservas_por_hotel (
                    hotel_id, fecha_entrada, reserva_id,
                    cliente_id, fecha_salida, estado
                ) VALUES (?, ?, ?, ?, ?, ?);";

            _session.Execute(_session.Prepare(qHotel).Bind(
                r.HotelId, r.FechaEntrada, r.ReservaId,
                r.ClienteId, r.FechaSalida, r.Estado
            ));

            // ---------------- reservas_por_fecha ----------------
            const string qFecha = @"
                INSERT INTO reservas_por_fecha (
                    fecha_entrada, reserva_id,
                    hotel_id, cliente_id,
                    estado
                ) VALUES (?, ?, ?, ?, ?);";

            _session.Execute(_session.Prepare(qFecha).Bind(
                r.FechaEntrada, r.ReservaId,
                r.HotelId, r.ClienteId,
                r.Estado
            ));
        }

        // ============================================================
        // UPDATE ESTADO EN 3 TABLAS
        // ============================================================
        public void UpdateEstado(Guid clienteId, Guid reservaId, string nuevoEstado)
        {
            // --- reservas_cliente ---
            _session.Execute(
                _session.Prepare(
                    @"UPDATE reservas_por_cliente SET estado=? 
                      WHERE cliente_id=? AND reserva_id=?;")
                .Bind(nuevoEstado, clienteId, reservaId)
            );

            var r = GetByClienteAndReserva(clienteId, reservaId);
            if (r == null) return;

            // --- reservas_hotel ---
            _session.Execute(
                _session.Prepare(
                    @"UPDATE reservas_por_hotel SET estado=? 
                      WHERE hotel_id=? AND fecha_entrada=? AND reserva_id=?;")
                .Bind(nuevoEstado, r.HotelId, r.FechaEntrada, r.ReservaId)
            );

            // --- reservas_fecha ---
            _session.Execute(
                _session.Prepare(
                    @"UPDATE reservas_por_fecha SET estado=? 
                      WHERE fecha_entrada=? AND reserva_id=?;")
                .Bind(nuevoEstado, r.FechaEntrada, r.ReservaId)
            );
        }

        // ============================================================
        // DELETE EN 3 TABLAS
        // ============================================================
        public void DeleteByCliente(Guid clienteId, Guid reservaId)
        {
            var r = GetByClienteAndReserva(clienteId, reservaId);
            if (r == null) return;

            _session.Execute(
                _session.Prepare("DELETE FROM reservas_por_cliente WHERE cliente_id=? AND reserva_id=?;")
                .Bind(clienteId, reservaId)
            );

            _session.Execute(
                _session.Prepare("DELETE FROM reservas_por_hotel WHERE hotel_id=? AND fecha_entrada=? AND reserva_id=?;")
                .Bind(r.HotelId, r.FechaEntrada, reservaId)
            );

            _session.Execute(
                _session.Prepare("DELETE FROM reservas_por_fecha WHERE fecha_entrada=? AND reserva_id=?;")
                .Bind(r.FechaEntrada, reservaId)
            );
        }

        // ============================================================
        // GET RESERVA (cliente + reserva)
        // ============================================================
        public ReservacionModel GetByClienteAndReserva(Guid clienteId, Guid reservaId)
        {
            var row = _session.Execute(
                _session.Prepare(
                    "SELECT * FROM reservas_por_cliente WHERE cliente_id=? AND reserva_id=?;")
                .Bind(clienteId, reservaId)
            ).FirstOrDefault();

            return row == null ? null : MapFromCliente(row);
        }

        // ============================================================
        // GET SOLO POR CLIENTE
        // ============================================================
        public List<ReservacionModel> GetByCliente(Guid clienteId)
        {
            var rows = _session.Execute(
                _session.Prepare("SELECT * FROM reservas_por_cliente WHERE cliente_id=?;")
                .Bind(clienteId)
            );

            return rows.Select(MapFromCliente).ToList();
        }

        // ============================================================
        // GET TODAS
        // ============================================================
        public List<ReservacionModel> GetAll()
        {
            var rows = _session.Execute("SELECT * FROM reservas_por_cliente;");
            return rows.Select(MapFromCliente).ToList();
        }

        // ============================================================
        // UPDATE ANTICIPO
        // ============================================================
        public void UpdateAnticipo(Guid clienteId, Guid reservaId, decimal anticipo)
        {
            _session.Execute(
                _session.Prepare(
                    @"UPDATE reservas_por_cliente SET anticipo=? 
                      WHERE cliente_id=? AND reserva_id=?;")
                .Bind(anticipo, clienteId, reservaId)
            );
        }

        // ============================================================
        // LISTADO COMPLETO PARA PANTALLA DE RESERVAS
        // ============================================================
        public List<ReservaListadoModel> GetListadoReservas()
        {
            var rows = _session.Execute("SELECT * FROM reservas_por_cliente ALLOW FILTERING;");

            var lista = new List<ReservaListadoModel>();

            var clienteRepo = new ClienteRepository();
            var hotelRepo = new HotelRepository();
            var estanciaRepo = new EstanciaActivaRepository();
            var habRepo = new HabitacionRepository();
            var tipoRepo = new TipoHabitacionRepository();

            foreach (var r in rows)
            {
                var model = new ReservaListadoModel
                {
                    ReservaId = r.GetValue<Guid>("reserva_id"),
                    ClienteId = r.GetValue<Guid>("cliente_id"),
                    HotelId = r.GetValue<Guid>("hotel_id"),

                    FechaEntrada = SafeDate(r, "fecha_entrada"),
                    FechaSalida = SafeDate(r, "fecha_salida"),

                    Anticipo = r.GetValue<decimal?>("anticipo") ?? 0,
                    Estado = r.GetValue<string>("estado") ?? "PENDIENTE",

                    Adultos = r.GetValue<int?>("adultos") ?? 0,
                    Menores = r.GetValue<int?>("menores") ?? 0,

                    NumPersonas = (r.GetValue<int?>("adultos") ?? 0) + (r.GetValue<int?>("menores") ?? 0)
                };

                // ---------------- Datos relacionados ----------------

                var cliente = clienteRepo.GetById(model.ClienteId);
                if (cliente != null)
                    model.ClienteNombre = cliente.NombreCompleto;

                var hotel = hotelRepo.GetById(model.HotelId);
                if (hotel != null)
                    model.HotelNombre = hotel.Nombre;

                // buscar estancia activa → habitación asignada
                var estancia = estanciaRepo.GetByReserva(model.ReservaId);

                if (estancia != null)
                {
                    var hab = habRepo.GetByHotelAndNumero(estancia.HotelId, estancia.NumeroHabitacion);
                    if (hab != null)
                    {
                        model.NumeroHabitacion = hab.NumeroHabitacion;

                        var tipo = tipoRepo.GetByHotelAndTipo(model.HotelId, hab.TipoId);
                        if (tipo != null)
                        {
                            model.TipoNombre = tipo.NombreTipo;

                            int dias = (model.FechaSalida - model.FechaEntrada).Days;
                            if (dias < 1) dias = 1;

                            model.PrecioTotal = tipo.PrecioNoche * dias;
                        }
                    }
                }

                lista.Add(model);
            }

            return lista;
        }

        // ============================================================
        // HELPERS
        // ============================================================
        private DateTime SafeDate(Row r, string col)
        {
            return r.IsNull(col) ? DateTime.UtcNow : r.GetValue<DateTime>(col);
        }

        private ReservacionModel MapFromCliente(Row row)
        {
            return new ReservacionModel
            {
                ClienteId = row.GetValue<Guid>("cliente_id"),
                ReservaId = row.GetValue<Guid>("reserva_id"),
                HotelId = row.GetValue<Guid>("hotel_id"),

                FechaEntrada = SafeDate(row, "fecha_entrada"),
                FechaSalida = SafeDate(row, "fecha_salida"),

                Anticipo = row.GetValue<decimal?>("anticipo") ?? 0,
                Estado = row.GetValue<string>("estado") ?? "PENDIENTE",

                Adultos = row.GetValue<int?>("adultos") ?? 0,
                Menores = row.GetValue<int?>("menores") ?? 0,

                UsuarioRegistro = row.GetValue<string>("usuario_registro") ?? "",
                FechaRegistro = SafeDate(row, "fecha_registro")
            };
        }
    }
}