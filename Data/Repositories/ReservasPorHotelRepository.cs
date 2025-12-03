using Cassandra;
using Gestión_Hotelera.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Gestión_Hotelera.Data.Repositories
{
    public class ReservasPorHotelRepository
    {
        private readonly ISession _session;

        public ReservasPorHotelRepository()
        {
            _session = CassandraConnection.GetSession();
        }

        // LISTAR RESERVAS DE UN HOTEL (MODELO COMPLETO PARA LISTA)
        public List<ReservaListadoModel> GetListadoByHotel(Guid hotelId)
        {
            var rows = _session.Execute(
                _session.Prepare("SELECT * FROM reservas_por_hotel WHERE hotel_id=?;")
                .Bind(hotelId)
            );

            var lista = new List<ReservaListadoModel>();
            var clienteRepo = new ClienteRepository();
            var habitacionRepo = new HabitacionRepository();
            var tipoRepo = new TipoHabitacionRepository();

            foreach (var r in rows)
            {
                var reserva = new ReservaListadoModel
                {
                    ReservaId = r.GetValue<Guid>("reserva_id"),
                    ClienteId = r.GetValue<Guid>("cliente_id"),
                    HotelId = r.GetValue<Guid>("hotel_id"),
                    FechaEntrada = r.GetValue<DateTime>("fecha_entrada"),
                    FechaSalida = r.GetValue<DateTime>("fecha_salida"),
                    Estado = r.GetValue<string>("estado")
                };

                // --- Completar datos del cliente ---
                var cliente = clienteRepo.GetById(reserva.ClienteId);
                reserva.ClienteNombre = cliente?.NombreCompleto ?? "Sin nombre";

                // --- Recuperar datos completos desde reservas_por_cliente ---
                var rc = GetReservaCompleta(reserva.ClienteId, reserva.ReservaId);
                if (rc != null)
                {
                    reserva.Adultos = rc.Adultos;
                    reserva.Menores = rc.Menores;
                    reserva.NumPersonas = rc.Adultos + rc.Menores;
                    reserva.Anticipo = rc.Anticipo;
                }

                // --- Obtener habitación y calcular precio ---
                var hab = habitacionRepo.GetByHotelAndNumero(reserva.HotelId, rc?.NumeroHabitacion ?? 0);
                if (hab != null)
                {
                    reserva.NumeroHabitacion = hab.NumeroHabitacion;

                    var tipo = tipoRepo.GetByHotelAndTipo(hab.HotelId, hab.TipoId);
                    if (tipo != null)
                    {
                        int dias = (reserva.FechaSalida - reserva.FechaEntrada).Days;
                        if (dias < 1) dias = 1;

                        reserva.PrecioTotal = tipo.PrecioNoche * dias;
                    }
                }

                lista.Add(reserva);
            }

            return lista;
        }

        public ReservacionModel GetReservaCompleta(Guid clienteId, Guid reservaId)
        {
            var row = _session.Execute(
                _session.Prepare("SELECT * FROM reservas_por_cliente WHERE cliente_id=? AND reserva_id=?;")
                .Bind(clienteId, reservaId)
            ).FirstOrDefault();

            if (row == null) return null;

            return new ReservacionModel
            {
                ClienteId = clienteId,
                ReservaId = reservaId,
                HotelId = row.GetValue<Guid>("hotel_id"),

                FechaEntrada = row.GetValue<DateTime>("fecha_entrada"),
                FechaSalida = row.GetValue<DateTime>("fecha_salida"),

                Adultos = row.GetValue<int>("adultos"),
                Menores = row.GetValue<int>("menores"),
                Anticipo = row.GetValue<decimal>("anticipo"),
                Estado = row.GetValue<string>("estado"),

                UsuarioRegistro = row.GetValue<string>("usuario_registro"),
                FechaRegistro = row.GetValue<DateTime>("fecha_registro")
            };
        }
    }
}