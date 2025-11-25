using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gestión_Hotelera.Model
{
    public class CheckOutModel
    {
        public Guid ReservacionID { get; set; }
        public DateTime FechaCheckOut { get; set; }

        public List<string> ServiciosAdicionales { get; set; }
        public decimal descuento { get; set; }

        public decimal TotalFinal { get; set; }

        //Facturacion
        public string FacturaXml { get; set; }
        public byte[] FacturaPdf { get; set; }

        //Auditoria
        public string UsuarioRegistro { get; set; }
        public DateTime FechaRegistro { get; set; }
    }
}
