using Gestión_Hotelera.Data.Repositories;
using Gestión_Hotelera.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gestión_Hotelera.Services
{
    public class CheckInService
    {
        private readonly ReservaRepository _reservaRepo;
        private readonly EstanciaActivaRepository _estanciaRepo;
        private readonly HabitacionRepository _habitacionRepo;
        private readonly TipoHabitacionRepository _tipoRepo;

        public CheckInService()
        {
            _reservaRepo = new ReservaRepository();
            _estanciaRepo = new EstanciaActivaRepository();
            _habitacionRepo = new HabitacionRepository();
            _tipoRepo = new TipoHabitacionRepository();
        }

        public Guid RealizarCheckIn(Guid clienteId, Guid reservaId, int numeroHabitacion, string usuario)
        {
            // 1) Obtener reserva
            var reserva = _reservaRepo.GetByClienteAndReserva(clienteId, reservaId);

            if (reserva == null)
                throw new Exception("No existe la reservación.");

            if (reserva.Estado != "PENDIENTE" && reserva.Estado != "CONFIRMADA")
                throw new Exception("La reserva no puede hacer check-in.");

            // 2) Validar habitación
            var habitacion = _habitacionRepo.GetByHotelAndNumero(reserva.HotelId, numeroHabitacion);

            if (habitacion == null)
                throw new Exception("La habitación no existe en este hotel.");

            // 3) Obtener tipo de habitación (CORREGIDO)
            var tipo = _tipoRepo.GetByHotelAndTipo(reserva.HotelId, habitacion.TipoId);

            if (tipo == null)
                throw new Exception("No se encontró el tipo de habitación asociado.");

            decimal precio = tipo.PrecioNoche;

            // 4) Crear estancia activa
            Guid estanciaId = Guid.NewGuid();

            var estancia = new EstanciaActivaModel
            {
                EstanciaId = estanciaId,
                HotelId = reserva.HotelId,
                NumeroHabitacion = numeroHabitacion,

                ClienteId = clienteId,
                ReservaId = reservaId,

                FechaEntrada = reserva.FechaEntrada,
                FechaSalida = reserva.FechaSalida,

                Adultos = reserva.Adultos,
                Menores = reserva.Menores,

                Anticipo = reserva.Anticipo,
                PrecioNoche = precio,

                UsuarioRegistro = usuario,
                FechaRegistro = DateTime.UtcNow
            };

            _estanciaRepo.Insert(estancia);

            // 5) Cambiar estado de reserva
            _reservaRepo.UpdateEstado(clienteId, reservaId, "CHECKIN");

            return estanciaId;
        }
    }
}
