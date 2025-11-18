using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gestión_Hotelera.Model
{
    public class HistorialEstanciaModel
    {
        public Guid ClienteId { get; set; }         // PK part
        public Guid EstanciaId { get; set; }        // PK part
        public Guid HotelId { get; set; }           // hotel
        public int Numero { get; set; }             // habitación
        public DateTime FechaEntrada { get; set; }  // entrada
        public DateTime FechaSalida { get; set; }   // salida
        public decimal Anticipo { get; set; }       // anticipo
        public decimal MontoHospedaje { get; set; } // hospedaje total
        public decimal MontoServicios { get; set; } // servicios usados
        public decimal TotalFactura { get; set; }   // total final
        public int? UsuarioRegistro { get; set; }   // usuario
        public DateTime? FechaRegistro { get; set; } // timestamp
    }
}
