using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gestión_Hotelera.Model
{
    public class FacturaModel
    {
        public Guid FacturaId { get; set; }
        public Guid EstanciaId { get; set; }
        public Guid ClienteId { get; set; }
        public Guid HotelId { get; set; }

        public DateTime FechaEmision { get; set; }

        public decimal MontoHospedaje { get; set; }
        public decimal MontoServicios { get; set; }
        public decimal Total { get; set; }

        public string UsuarioRegistro { get; set; }
        public DateTime FechaRegistro { get; set; }
    }
}
