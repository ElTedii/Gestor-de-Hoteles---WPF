using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gestión_Hotelera.Model
{
    public class TipoHabitacionModel
    {
        public Guid HotelId { get; set; }
        public Guid TipoId { get; set; }

        public string NombreTipo { get; set; }
        public int Capacidad { get; set; }
        public decimal PrecioNoche { get; set; }
        public int Cantidad { get; set; }

        public List<string> Caracteristicas { get; set; }
        public List<string> Amenidades { get; set; }

        public string Nivel { get; set; }
        public string Vista { get; set; }

        public string UsuarioRegistro { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime FechaModificacion { get; set; }
    }
}
