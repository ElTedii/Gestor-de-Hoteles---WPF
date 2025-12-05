using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gestión_Hotelera.Model
{
    public class ReservacionModel
    {
        public Guid ClienteId { get; set; }
        public Guid ReservaId { get; set; }
        public Guid HotelId { get; set; }

        public DateTime FechaEntrada { get; set; }
        public DateTime FechaSalida { get; set; }

        public decimal Anticipo { get; set; }
        public string Estado { get; set; }

        public int Adultos { get; set; }
        public int Menores { get; set; }

        public string UsuarioRegistro { get; set; }
        public DateTime FechaRegistro { get; set; }

        // Compatibilidad con pantallas que lo solicitaban
        public decimal PrecioTotal { get; set; }

        // Compatibilidad con asignación de habitación
        public int NumeroHabitacion { get; set; }
    }
}
