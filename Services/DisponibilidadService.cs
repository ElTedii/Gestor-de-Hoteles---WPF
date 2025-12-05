using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gestión_Hotelera.Data.Repositories;
using Gestión_Hotelera.Model;

namespace Gestión_Hotelera.Services
{
    public class DisponibilidadService
    {
        private readonly HabitacionRepository _habitacionRepo;
        private readonly EstanciaActivaRepository _estanciaRepo;
        private readonly ReservaRepository _reservaRepo;

        public DisponibilidadService()
        {
            _habitacionRepo = new HabitacionRepository();
            _estanciaRepo = new EstanciaActivaRepository();
            _reservaRepo = new ReservaRepository();
        }

        public List<HabitacionModel> ObtenerDisponibles(Guid hotelId, DateTime entrada, DateTime salida)
        {
            // 1. Obtener todas las habitaciones del hotel
            var habitaciones = _habitacionRepo.GetByHotel(hotelId);

            // 2. Obtener estancias activas en este hotel
            var estancias = _estanciaRepo.GetByHotel(hotelId);

            // 3. Filtrar estancias que se cruzan con el rango
            var ocupadas = estancias.Where(e =>
                e.FechaEntrada < salida &&
                entrada < e.FechaSalida
            ).Select(e => e.NumeroHabitacion).ToList();

            // 4. Habitación disponible = no aparece en ocupadas
            return habitaciones
                .Where(h => !ocupadas.Contains(h.NumeroHabitacion))
                .ToList();
        }
    }
}
