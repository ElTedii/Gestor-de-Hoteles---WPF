using System;
using System.Collections.Generic;
using System.Linq;
using Gestión_Hotelera.Data.Repositories;
using Gestión_Hotelera.Model;

namespace Gestión_Hotelera.Services
{
    public class ReservaService
    {
        private readonly ReservaRepository _reservaRepo;
        private readonly HabitacionRepository _habitacionRepo;
        private readonly TipoHabitacionRepository _tipoRepo;
        private readonly ReservasPorHotelRepository _reservaHotelRepo;

        public ReservaService()
        {
            _reservaRepo = new ReservaRepository();
            _habitacionRepo = new HabitacionRepository();
            _tipoRepo = new TipoHabitacionRepository();
            _reservaHotelRepo = new ReservasPorHotelRepository();
        }

        // ----------------------------------------------------------
        // A + D) Habitaciones disponibles (estancias + reservas)
        // ----------------------------------------------------------
        public List<HabitacionModel> ObtenerHabitacionesDisponibles(
            Guid hotelId,
            DateTime fechaEntrada,
            DateTime fechaSalida)
        {
            // 1) Habitaciones libres según estancias activas
            var libres = _habitacionRepo.GetHabitacionesLibres(hotelId, fechaEntrada, fechaSalida);

            // 2) Reservas que ya bloquean habitación (PENDIENTE/CONFIRMADA)
            var reservasHotel = _reservaHotelRepo.GetListadoByHotel(hotelId);

            var reservadas = reservasHotel
                .Where(r => r.Estado == "PENDIENTE" || r.Estado == "CONFIRMADA")
                .Where(r =>
                    fechaEntrada < r.FechaSalida &&
                    fechaSalida > r.FechaEntrada)
                .Select(r => r.NumeroHabitacion)
                .Distinct()
                .ToHashSet();

            // 3) Dejar solo las que no están reservadas en ese rango
            var disponibles = libres
                .Where(h => !reservadas.Contains(h.NumeroHabitacion))
                .ToList();

            // 4) Aseguramos precio y nombre de tipo desde tipos_habitacion
            foreach (var h in disponibles)
            {
                var tipo = _tipoRepo.GetByHotelAndTipo(h.HotelId, h.TipoId);
                if (tipo != null)
                {
                    h.PrecioNoche = tipo.PrecioNoche;
                    h.TipoNombre = tipo.NombreTipo;
                }
            }

            return disponibles;
        }

        // ----------------------------------------------------------
        // B) Crear reserva a partir de un modelo
        // ----------------------------------------------------------
        public void CrearReserva(ReservacionModel r)
        {
            if (r.ReservaId == Guid.Empty)
                r.ReservaId = Guid.NewGuid();

            if (r.FechaRegistro == default)
                r.FechaRegistro = DateTime.UtcNow;

            // Si ya viene un estado desde la UI lo respetamos
            if (string.IsNullOrEmpty(r.Estado))
                r.Estado = "PENDIENTE";

            _reservaRepo.Insert(r);
        }

        // Versión por parámetros
        public void CrearReserva(
            Guid clienteId,
            Guid hotelId,
            DateTime fechaEntrada,
            DateTime fechaSalida,
            int adultos,
            int menores,
            decimal anticipo,
            string usuarioRegistro)
        {
            var r = new ReservacionModel
            {
                ClienteId = clienteId,
                HotelId = hotelId,
                FechaEntrada = fechaEntrada,
                FechaSalida = fechaSalida,
                Adultos = adultos,
                Menores = menores,
                Anticipo = anticipo,
                UsuarioRegistro = usuarioRegistro
            };

            CrearReserva(r);
        }
    }
}