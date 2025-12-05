using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gestión_Hotelera.Model
{
    public class OcupacionHotelModel
    {
        public Guid HotelId { get; set; }
        public int Año { get; set; }
        public int Mes { get; set; }
        public DateTime FechaEntrada { get; set; }
        public Guid EstanciaId { get; set; }
        public Guid ClienteId { get; set; }
        public string TipoHabitacion { get; set; }
        public int NumeroHabitacion { get; set; }
        public DateTime FechaSalida { get; set; }
        public string Estado { get; set; }
        public decimal PagoHospedaje { get; set; }
        public decimal PagoServicios { get; set; }

    }
}
