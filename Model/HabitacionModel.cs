using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gestión_Hotelera.Model
{
    public class HabitacionModel
    {
        public Guid HotelId { get; set; }      // hotel_id (PK part)
        public int Numero { get; set; }        // numero (PK part)
        public Guid TipoId { get; set; }       // tipo_id
        public int Piso { get; set; }          // piso
    }
}
