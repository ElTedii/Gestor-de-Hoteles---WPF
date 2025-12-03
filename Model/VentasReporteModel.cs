using System;

namespace Gestión_Hotelera.Model
{
    public class VentasReporteModel
    {
        public string Ciudad { get; set; }
        public Guid HotelId { get; set; }
        public string NombreHotel { get; set; }

        public int Año { get; set; }
        public int Mes { get; set; }
        public string MesNombre { get; set; }

        public decimal IngresosHospedaje { get; set; }
        public decimal IngresosServicios { get; set; }
        public decimal Total => IngresosHospedaje + IngresosServicios;
    }
}