using Gestión_Hotelera.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gestión_Hotelera.Services
{
    public class ReservationService
    {
        private static ReservationService _instance;
        public static ReservationService Instance => _instance ??= new ReservationService();

        public ObservableCollection<ReservationModel> Reservas { get; private set; }

        private ReservationService()
        {
            Reservas = new ObservableCollection<ReservationModel>();
        }

        public void AgregarReserva(ReservationModel r)
        {
            Reservas.Add(r);
        }

        public void EliminarReserva(Guid id)
        {
            var reserva = Reservas.FirstOrDefault(r => r.ReservationId == id);
            if (reserva != null)
                Reservas.Remove(reserva);
        }

        public void ActualizarReserva(ReservationModel r)
        {
            var original = Reservas.FirstOrDefault(x => x.ReservationId == r.ReservationId);
            if (original != null)
            {
                original.ClienteId = r.ClienteId;
                original.ClienteNombre = r.ClienteNombre;
                original.HotelId = r.HotelId;
                original.HotelNombre = r.HotelNombre;
                original.HabitacionId = r.HabitacionId;
                original.NumeroHabitacion = r.NumeroHabitacion;
                original.FechaEntrada = r.FechaEntrada;
                original.FechaSalida = r.FechaSalida;
                original.NumPersonas = r.NumPersonas;
                original.PrecioTotal = r.PrecioTotal;
            }
        }
    }
}
