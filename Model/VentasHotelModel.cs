using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gestión_Hotelera.Model
{
    public class VentasHotelModel
    {
        public string Pais { get; set; }
        public string Ciudad { get; set; }
        public Guid HotelId { get; set; }
        public int Año { get; set; }
        public int Mes { get; set; }
        public decimal IngresosHospedaje { get; set; }
        public decimal IngresosServicios { get; set; }
        public decimal Total { get; set; }
    }
}
