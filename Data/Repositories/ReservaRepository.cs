using Cassandra;
using Gestión_Hotelera.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        // INSERTAR RESERVA  (escribe en 3 tablas)
        // ============================================================
        public void Insert(ReservacionModel r)
        {
            r.ReservaId = r.ReservaId == Guid.Empty ? Guid.NewGuid() : r.ReservaId;
            r.FechaRegistro = r.FechaRegistro == default ? DateTime.UtcNow : r.FechaRegistro;
            r.Estado ??= "PENDIENTE";

            int adultos = r.Adultos;
            int menores = r.Menores;

            // ---------------- reservas_por_cliente ----------------
            const string qCliente = @"
                INSERT INTO reservas_por_cliente (
                    cliente_id, reserva_id, hotel_id,
                    fecha_entrada, fecha_salida,
                    anticipo, estado,
                    adultos, menores,
                    usuario_registro, fecha_registro
                ) VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?);";

            _session.Execute(
                _session.Prepare(qCliente).Bind(
                    r.ClienteId, r.ReservaId, r.HotelId,
                    r.FechaEntrada, r.FechaSalida,
                    r.Anticipo, r.Estado,
                    adultos, menores,
                    r.UsuarioRegistro, r.FechaRegistro
                )
            );

            // ---------------- reservas_por_hotel ----------------
            const string qHotel = @"
                INSERT INTO reservas_por_hotel (
                    hotel_id, fecha_entrada, reserva_id,
                    cliente_id, fecha_salida, estado
                ) VALUES (?, ?, ?, ?, ?, ?);";

            _session.Execute(
                _session.Prepare(qHotel).Bind(
                    r.HotelId, r.FechaEntrada, r.ReservaId,
                    r.ClienteId, r.FechaSalida, r.Estado
                )
            );

            // ---------------- reservas_por_fecha ----------------
            const string qFecha = @"
                INSERT INTO reservas_por_fecha (
                    fecha_entrada, reserva_id,
                    hotel_id, cliente_id,
                    estado
                ) VALUES (?, ?, ?, ?, ?);";

            _session.Execute(
                _session.Prepare(qFecha).Bind(
                    r.FechaEntrada, r.ReservaId,
                    r.HotelId, r.ClienteId,
                    r.Estado
                )
            );
        }

        // ============================================================
        // UPDATE ESTADO EN 3 TABLAS
        // ============================================================
        public void UpdateEstado(Guid clienteId, Guid reservaId, string nuevo)
        {
            // --- reservas_cliente ---
            _session.Execute(
                _session.Prepare(
                    @"UPDATE reservas_por_cliente SET estado=? 
                      WHERE cliente_id=? AND reserva_id=?;")
                .Bind(nuevo, clienteId, reservaId)
            );

            var r = GetByClienteAndReserva(clienteId, reservaId);
            if (r == null) return;

            // --- reservas_por_hotel ---
            _session.Execute(
                _session.Prepare(
                    @"UPDATE reservas_por_hotel SET estado=? 
                      WHERE hotel_id=? AND fecha_entrada=? AND reserva_id=?;")
                .Bind(nuevo, r.HotelId, r.FechaEntrada, r.ReservaId)
            );

            // --- reservas_por_fecha ---
            _session.Execute(
                _session.Prepare(
                    @"UPDATE reservas_por_fecha SET estado=? 
                      WHERE fecha_entrada=? AND reserva_id=?;")
                .Bind(nuevo, r.FechaEntrada, r.ReservaId)
            );
        }

        // ============================================================
        // DELETE (3 tablas)
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
                .Bind(r.HotelId, r.FechaEntrada, r.ReservaId)
            );

            _session.Execute(
                _session.Prepare("DELETE FROM reservas_por_fecha WHERE fecha_entrada=? AND reserva_id=?;")
                .Bind(r.FechaEntrada, r.ReservaId)
            );
        }

        // ============================================================
        // CONSULTAS
        // ============================================================
        public List<ReservacionModel> GetByCliente(Guid clienteId)
        {
            var rows = _session.Execute(
                _session.Prepare("SELECT * FROM reservas_por_cliente WHERE cliente_id=?;")
                .Bind(clienteId)
            );

            return rows.Select(MapFromCliente).ToList();
        }

        public List<ReservacionModel> GetByFecha(DateTime fecha)
        {
            var rows = _session.Execute(
                _session.Prepare("SELECT * FROM reservas_por_fecha WHERE fecha_entrada=?;")
                .Bind(fecha)
            );

            return rows.Select(MapFromFecha).ToList();
        }

        public ReservacionModel GetByClienteAndReserva(Guid clienteId, Guid rId)
        {
            var row = _session.Execute(
                _session.Prepare(
                    "SELECT * FROM reservas_por_cliente WHERE cliente_id=? AND reserva_id=?;")
                .Bind(clienteId, rId)
            ).FirstOrDefault();

            return row == null ? null : MapFromCliente(row);
        }

        public List<ReservacionModel> GetAll()
        {
            var rows = _session.Execute("SELECT * FROM reservas_por_cliente;");
            return rows.Select(MapFromCliente).ToList();
        }

        // ============================================================
        // MAPEO SEGURO NULL-SAFE
        // ============================================================
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

        private ReservacionModel MapFromFecha(Row row)
        {
            return new ReservacionModel
            {
                FechaEntrada = SafeDate(row, "fecha_entrada"),
                ReservaId = row.GetValue<Guid>("reserva_id"),
                HotelId = row.GetValue<Guid>("hotel_id"),
                ClienteId = row.GetValue<Guid>("cliente_id"),
                Estado = row.GetValue<string>("estado") ?? "PENDIENTE"
            };
        }

        private DateTime SafeDate(Row r, string column)
        {
            if (r.IsNull(column)) return DateTime.UtcNow;
            return r.GetValue<DateTime>(column);
        }

        public void MarcarEnEstancia(Guid clienteId, Guid reservaId)
        {
            UpdateEstado(clienteId, reservaId, "EN_ESTANCIA");
        }
    }
}