using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gestión_Hotelera.Model
{
    public class ReservaListadoModel
    {
        public Guid ReservaId { get; set; }

        // Datos base
        public string ClienteNombre { get; set; }
        public string HotelNombre { get; set; }
        public int NumeroHabitacion { get; set; }

        public DateTime FechaEntrada { get; set; }
        public DateTime FechaSalida { get; set; }

        public decimal Anticipo { get; set; }
        public string Estado { get; set; }
        public int Adultos { get; set; }
        public int Menores { get; set; }
        public int NumPersonas { get; set; }
        public decimal PrecioTotal { get; set; }
        public string TipoNombre { get; set; }

        public Guid ClienteId { get; set; }
        public Guid HotelId { get; set; }

        // ───── Aliases solo para el Dashboard (HomeView) ─────
        public string Code => ReservaId.ToString().Substring(0, 4);
        public string Client => ClienteNombre;
        public string Hotel => HotelNombre;
        public DateTime CheckIn => FechaEntrada;
        public DateTime CheckOut => FechaSalida;
        public decimal Total => PrecioTotal;
    }
}
