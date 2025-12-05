using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gestión_Hotelera.Model
{
    public class CheckInListadoModel
    {
        public Guid ClienteId { get; set; }
        public Guid ReservaId { get; set; }
        public Guid HotelId { get; set; }

        public string ClienteNombre { get; set; }
        public string HotelNombre { get; set; }

        public DateTime FechaEntrada { get; set; }
        public DateTime FechaSalida { get; set; }

        public int Adultos { get; set; }
        public int Menores { get; set; }

        public string Habitacion { get; set; } 
    }
}
