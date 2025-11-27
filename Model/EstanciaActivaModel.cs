using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gestión_Hotelera.Model
{
    public class EstanciaActivaModel
    {
        public Guid HotelId { get; set; }

        public int NumeroHabitacion { get; set; }

        public Guid EstanciaId { get; set; }
        public Guid ClienteId { get; set; }
        public Guid ReservaId { get; set; }

        public DateTime FechaEntrada { get; set; }
        public DateTime FechaSalida { get; set; }

        public decimal Anticipo { get; set; }
        public decimal PrecioNoche { get; set; }

        public int Adultos { get; set; }
        public int Menores { get; set; }
    }
}
