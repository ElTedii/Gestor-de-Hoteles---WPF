using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gestión_Hotelera.Model
{
    public class HabitacionModel
    {
        public Guid HabitacionId { get; set; }
        public Guid HotelId { get; set; }
        public string TipoHabitacion { get; set; }

        public int NumeroCamas { get; set; }
        public List<string> TiposCama { get; set; }
        public decimal PrecioNochePorPersona { get; set; }
        public int CapacidadPersonas { get; set; }
        public string NivelHabitacion { get; set; }
        public string Vista { get; set; }

        public List<string> Caracteristicas { get; set; }
        public List<string> Amenidades { get; set; }

        public int CantidadHabitacionesDisponibles { get; set; }

        // Auditoría
        public string UsuarioRegistro { get; set; }
        public DateTime FechaRegistro { get; set; }
        public string UsuarioModificacion { get; set; }
        public DateTime FechaModificacion { get; set; }
    }
}
