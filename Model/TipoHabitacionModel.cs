using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gestión_Hotelera.Model
{
    public class TipoHabitacionModel
    {
        public Guid HotelId { get; set; }              // hotel_id (PK part)
        public Guid TipoId { get; set; }               // tipo_id (PK part)
        public string NombreTipo { get; set; }         // nombre_tipo
        public int Capacidad { get; set; }             // capacidad
        public decimal PrecioNoche { get; set; }       // precio_noche
        public int Cantidad { get; set; }              // cantidad de habitaciones de este tipo
        public List<string> Caracteristicas { get; set; } // caracteristicas
        public List<string> Amenidades { get; set; }       // amenidades
        public string Nivel { get; set; }              // nivel (estándar, suite, etc.)
        public string Vista { get; set; }              // vista (jardín, playa, piscina)
        public int? UsuarioRegistro { get; set; }      // usuario_registro
        public DateTime? FechaRegistro { get; set; }   // fecha_registro

        public TipoHabitacionModel()
        {
            Caracteristicas = new List<string>();
            Amenidades = new List<string>();
        }
    }
}
