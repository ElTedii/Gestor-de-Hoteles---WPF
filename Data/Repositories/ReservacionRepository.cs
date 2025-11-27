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
            _session = CassandraConnection.GetSession();
        }

        // ==========================================================
        // INSERT (BATCH)
        // ==========================================================
        public void Insert(ReservacionModel r)
        {
            var batch = new BatchStatement();

            // Tabla 1: reservas_por_cliente
            var q1 = _session.Prepare(@"
                INSERT INTO reservas_por_cliente (
                    cliente_id, reserva_id, hotel_id,
                    fecha_entrada, fecha_salida,
                    anticipo, estado,
                    usuario_registro, fecha_registro, fecha_modificacion
                ) VALUES (?,?,?,?,?,?,?,?,?,?);
            ");

            batch.Add(q1.Bind(
                r.ClienteId, r.ReservaId, r.HotelId,
                r.FechaEntrada, r.FechaSalida,
                r.Anticipo, r.Estado,
                r.UsuarioRegistro, r.FechaCreacion, r.FechaModificacion
            ));

            // Tabla 2: reservas_por_hotel
            var q2 = _session.Prepare(@"
                INSERT INTO reservas_por_hotel (
                    hotel_id, fecha_entrada, reserva_id,
                    cliente_id, fecha_salida, estado
                ) VALUES (?,?,?,?,?,?);
            ");

            batch.Add(q2.Bind(
                r.HotelId, r.FechaEntrada, r.ReservaId,
                r.ClienteId, r.FechaSalida, r.Estado
            ));

            // Tabla 3: reservas_por_fecha
            var q3 = _session.Prepare(@"
                INSERT INTO reservas_por_fecha (
                    fecha_entrada, reserva_id,
                    hotel_id, cliente_id, estado
                ) VALUES (?,?,?,?,?);
            ");

            batch.Add(q3.Bind(
                r.FechaEntrada, r.ReservaId,
                r.HotelId, r.ClienteId, r.Estado
            ));

            // Ejecutamos todo
            _session.Execute(batch);
        }

        // ==========================================================
        // UPDATE (solo en reservas_por_cliente)
        // ==========================================================
        public void Update(ReservacionModel r)
        {
            var q = _session.Prepare(@"
                UPDATE reservas_por_cliente SET
                    fecha_entrada=?, fecha_salida=?,
                    anticipo=?, estado=?,
                    usuario_modificacion=?, fecha_modificacion=?
                WHERE cliente_id=? AND reserva_id=?;
            ");

            _session.Execute(q.Bind(
                r.FechaEntrada, r.FechaSalida,
                r.Anticipo, r.Estado,
                r.UsuarioRegistro, r.FechaModificacion,
                r.ClienteId, r.ReservaId
            ));
        }

        // ==========================================================
        // DELETE (BATCH)
        // ==========================================================
        public void Delete(Guid clienteId, Guid reservaId, Guid hotelId, DateTime fechaEntrada)
        {
            var batch = new BatchStatement();

            batch.Add(_session.Prepare(
                "DELETE FROM reservas_por_cliente WHERE cliente_id=? AND reserva_id=?;"
            ).Bind(clienteId, reservaId));

            batch.Add(_session.Prepare(
                "DELETE FROM reservas_por_hotel WHERE hotel_id=? AND fecha_entrada=? AND reserva_id=?;"
            ).Bind(hotelId, fechaEntrada, reservaId));

            batch.Add(_session.Prepare(
                "DELETE FROM reservas_por_fecha WHERE fecha_entrada=? AND reserva_id=?;"
            ).Bind(fechaEntrada, reservaId));

            _session.Execute(batch);
        }

        // ==========================================================
        // SELECT: POR CLIENTE
        // ==========================================================
        public List<ReservacionModel> GetByCliente(Guid clienteId)
        {
            var result = _session.Execute(
                _session.Prepare("SELECT * FROM reservas_por_cliente WHERE cliente_id=?;")
                .Bind(clienteId)
            );

            return result.Select(MapRow).ToList();
        }

        // ==========================================================
        // SELECT: POR HOTEL
        // ==========================================================
        public List<ReservacionModel> GetByHotel(Guid hotelId, DateTime fechaEntrada)
        {
            var result = _session.Execute(
                _session.Prepare("SELECT * FROM reservas_por_hotel WHERE hotel_id=? AND fecha_entrada=?;")
                .Bind(hotelId, fechaEntrada)
            );

            return result.Select(MapRow).ToList();
        }

        // ==========================================================
        // SELECT: POR FECHA GENERAL
        // ==========================================================
        public List<ReservacionModel> GetByFecha(DateTime fecha)
        {
            var result = _session.Execute(
                _session.Prepare("SELECT * FROM reservas_por_fecha WHERE fecha_entrada=?;")
                .Bind(fecha)
            );

            return result.Select(MapRow).ToList();
        }

        // ==========================================================
        // SELECT ÚNICO
        // ==========================================================
        public ReservacionModel GetById(Guid clienteId, Guid reservaId)
        {
            var row = _session.Execute(
                _session.Prepare("SELECT * FROM reservas_por_cliente WHERE cliente_id=? AND reserva_id=?;")
                .Bind(clienteId, reservaId)
            ).FirstOrDefault();

            return row != null ? MapRow(row) : null;
        }

        // ==========================================================
        // MAPEO
        // ==========================================================
        private ReservacionModel MapRow(Row row)
        {
            return new ReservacionModel
            {
                ClienteId = row.GetValue<Guid>("cliente_id"),
                ReservaId = row.GetValue<Guid>("reserva_id"),
                HotelId = row.GetValue<Guid>("hotel_id"),
                FechaEntrada = row.GetValue<DateTime>("fecha_entrada"),
                FechaSalida = row.GetValue<DateTime>("fecha_salida"),
                Anticipo = row.GetValue<decimal>("anticipo"),
                Estado = row.GetValue<string>("estado"),

                UsuarioRegistro = row.GetValue<string>("usuario_registro"),
                FechaCreacion = row.GetValue<DateTime>("fecha_registro"),
                FechaModificacion = row.GetValue<DateTime>("fecha_modificacion")
            };
        }
    }
}
