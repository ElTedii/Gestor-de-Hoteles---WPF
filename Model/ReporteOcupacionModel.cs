using System;

namespace Gestión_Hotelera.Model
{
    public class ReporteOcupacionModel
    {
        public string NombreHotel { get; set; }
        public int Año { get; set; }
        public string Mes { get; set; }
        public string TipoHabitacion { get; set; }
        public int NumeroHabitacion { get; set; }
        public string Cliente { get; set; }
        public Guid ReservaId { get; set; }
        public DateTime FechaCheckIn { get; set; }
        public DateTime FechaCheckOut { get; set; }
        public string Estatus { get; set; }
        public decimal PagoHospedaje { get; set; }
        public decimal PagoServicios { get; set; }
    }
}