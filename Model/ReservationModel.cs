using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gestión_Hotelera.Model
{
    public class ReservationModel
    {
        public Guid ReservacionId { get; set; }
        public Guid ClienteId { get; set; }
        public Guid HotelId { get; set; }

        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }

        public Dictionary<Guid, int> HabitacionesSeleccionadas { get; set; }
        public int TotalPersonas { get; set; }

        public decimal Anticipo { get; set; }
        public decimal TotalHospedaje { get; set; }
        public decimal TotalServicios { get; set; }
        public string Estatus { get; set; }

        // Auditoría
        public string UsuarioRegistro { get; set; }
        public DateTime FechaRegistro { get; set; }
        public string UsuarioModificacion { get; set; }
        public DateTime FechaModificacion { get; set; }
    }
}
