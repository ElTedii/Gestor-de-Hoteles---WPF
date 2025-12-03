using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gestión_Hotelera.Model
{
    public class HabitacionModel
    {
        public Guid HotelId { get; set; }
        public int NumeroHabitacion { get; set; }
        public Guid TipoId { get; set; }
        public int Piso { get; set; }
        public string Estado { get; set; }

        public decimal PrecioNoche { get; set; }
        public string TipoNombre { get; set; }

        public string UsuarioRegistro { get; set; }
        public DateTime FechaRegistro { get; set; }
        public string UsuarioModificacion { get; set; }
        public DateTime? FechaModificacion { get; set; }
    }
}
