using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gestión_Hotelera.Model
{
    public class EstanciaActivaModel
    {
        public Guid HotelId { get; set; }          // hotel_id (PK part)
        public int Numero { get; set; }            // numero (PK part)
        public Guid EstanciaId { get; set; }       // estancia_id
        public Guid ClienteId { get; set; }        // cliente_id
        public Guid ReservaId { get; set; }        // reserva_id (código reservación)
        public DateTime FechaEntrada { get; set; } // fecha_entrada
        public DateTime FechaSalida { get; set; }  // fecha_salida
        public decimal Anticipo { get; set; }      // anticipo
        public decimal PrecioPorNoche { get; set; } // precio_noche
        public int Adultos { get; set; }           // adultos
        public int Menores { get; set; }           // menores
    }
}
