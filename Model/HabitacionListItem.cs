using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gestión_Hotelera.ViewModel
{
    public class HabitacionListItem
    {
        public Guid HabitacionId { get; set; }
        public string HotelNombre { get; set; }
        public int Numero { get; set; }
        public int Piso { get; set; }
        public string Estado { get; set; }
        public string TipoNombre { get; set; }
        public decimal PrecioNoche { get; set; }
    }
}