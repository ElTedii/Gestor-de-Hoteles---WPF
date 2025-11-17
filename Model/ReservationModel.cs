using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gestión_Hotelera.Model
{
    public class ReservationModel
    {
        public Guid ReservationId { get; set; }

        public Guid ClienteId { get; set; }
        public string ClienteNombre { get; set; }

        public Guid HotelId { get; set; }
        public string HotelNombre { get; set; }

        public Guid HabitacionId { get; set; }
        public string NumeroHabitacion { get; set; }

        public DateTime FechaEntrada { get; set; }
        public DateTime FechaSalida { get; set; }

        public int NumPersonas { get; set; }

        public decimal PrecioTotal { get; set; }
    }
}
