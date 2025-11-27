using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gestión_Hotelera.Model
{
    public class FacturaModel
    {
        public Guid FolioFactura { get; set; }
        public Guid ClienteId { get; set; }
        public Guid HotelId { get; set; }
        public Guid ReservaId { get; set; }
        public Guid EstanciaId { get; set; }

        public DateTime FechaEmision { get; set; }
        public DateTime FechaEntrada { get; set; }
        public DateTime FechaSalida { get; set; }

        public decimal Anticipo { get; set; }
        public decimal MontoHospedaje { get; set; }
        public decimal MontoServicios { get; set; }
        public decimal Descuento { get; set; }
        public decimal TotalFactura { get; set; }
    }
}
