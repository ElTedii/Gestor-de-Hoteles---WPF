using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gestión_Hotelera.Model
{
    public class FacturaModel
    {
        public Guid FolioFactura { get; set; }      // folio_factura (PK)
        public Guid ClienteId { get; set; }         // cliente_id
        public Guid HotelId { get; set; }           // hotel_id
        public Guid ReservaId { get; set; }         // reserva_id (código de reservación)
        public Guid EstanciaId { get; set; }        // estancia_id
        public DateTime FechaEmision { get; set; }  // fecha_emision
        public DateTime FechaEntrada { get; set; }  // fecha_entrada
        public DateTime FechaSalida { get; set; }   // fecha_salida
        public decimal Anticipo { get; set; }       // anticipo
        public decimal MontoHospedaje { get; set; } // monto_hospedaje
        public decimal MontoServicios { get; set; } // monto_servicios
        public decimal Descuento { get; set; }      // descuento aplicado
        public decimal TotalFactura { get; set; }   // total_factura
    }
}
