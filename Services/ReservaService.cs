using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gestión_Hotelera.Data.Repositories;
using Gestión_Hotelera.Model;

namespace Gestión_Hotelera.Services
{
    public class ReservaService
    {
        private readonly ReservaRepository _reservaRepo;
        private readonly HabitacionRepository _habitacionRepo;
        private readonly TipoHabitacionRepository _tipoRepo;

        public ReservaService()
        {
            _reservaRepo = new ReservaRepository();
            _habitacionRepo = new HabitacionRepository();
            _tipoRepo = new TipoHabitacionRepository();
        }

        // ----------------------------------------------------------
        // A) Obtener habitaciones disponibles (simple)
        // ----------------------------------------------------------
        public List<HabitacionModel> ObtenerHabitacionesDisponibles(
            Guid hotelId,
            DateTime fechaEntrada,
            DateTime fechaSalida)
        {
            // Todas las habitaciones del hotel
            var habs = _habitacionRepo.GetByHotel(hotelId);

            // Para cada habitación buscamos su tipo y precio
            foreach (var h in habs)
            {
                var tipo = _tipoRepo.GetByHotelAndTipo(h.HotelId, h.TipoId); // ✅ CORREGIDO

                if (tipo != null)
                    h.PrecioBase = tipo.PrecioNoche;  // Asegúrate que HabitacionModel tenga esta propiedad
            }

            // (Versión simple: no filtramos por fechas)
            return habs;
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

            if (string.IsNullOrEmpty(r.Estado))
                r.Estado = "PENDIENTE";

            _reservaRepo.Insert(r);
        }

        // Versión por parámetros (azúcar sintáctico)
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
