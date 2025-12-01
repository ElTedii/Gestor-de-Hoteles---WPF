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
        public int Numero { get; set; }

        public Guid TipoId { get; set; }
        public int Piso { get; set; }

        // AUDITORÍA
        public string UsuarioCreacion { get; set; }
        public DateTime FechaCreacion { get; set; }

        public string UsuarioModificacion { get; set; }
        public DateTime? FechaModificacion { get; set; }

        // SOLO PARA CÁLCULO DE PRECIO (NO se guarda en Cassandra)
        public decimal PrecioBase { get; set; }
    }
}
