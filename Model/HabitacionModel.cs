using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gestión_Hotelera.Model
{
    public class HabitacionModel
    {
        public Guid HabitacionId { get; set; }      // Identificador único
        public string Tipo { get; set; }            // Ej: Suite, Estándar, Deluxe
        public int Capacidad { get; set; }          // Ej: 2, 4 personas
        public decimal Precio { get; set; }         // Precio por noche
        public string TipoCama { get; set; }
        public Guid HotelId { get; set; }           // Relación con Hotel
        public string NombreHotel { get; set; }     // Para mostrar en la tabla
        public string Descripcion { get; set; }     // Para mostrar en la tabla
    }
}
