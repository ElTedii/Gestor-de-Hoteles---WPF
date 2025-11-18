using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gestión_Hotelera.Model
{
    public class DisponibilidadHabitacionModel
    {
        public Guid HotelId { get; set; }      // hotel_id (PK part)
        public int Numero { get; set; }        // numero (PK part)
        public DateTime Fecha { get; set; }    // fecha (clustering)
        public bool Disponible { get; set; }   // disponible
    }
}
