using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gestión_Hotelera.Model
{
    public class ReservationModel
    {
        public Guid ReservaId { get; set; }        // reserva_id
        public Guid ClienteId { get; set; }        // cliente_id
        public Guid HotelId { get; set; }          // hotel_id
        public DateTime FechaEntrada { get; set; } // fecha_entrada
        public DateTime FechaSalida { get; set; }  // fecha_salida
        public decimal Anticipo { get; set; }      // anticipo
        public string Estado { get; set; }         // estado (pendiente, activa, cancelada, etc.)
        public int? UsuarioRegistro { get; set; }  // usuario_registro
        public DateTime? FechaRegistro { get; set; } // fecha_registro
    }
}
