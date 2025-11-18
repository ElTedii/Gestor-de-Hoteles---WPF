using Gestión_Hotelera.Data.Repositories;
using Gestión_Hotelera.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gestión_Hotelera.Services
{
    public class CheckOutService
    {
        private readonly CheckOutRepository _checkOutRepository;
        private readonly FacturaRepository _facturaRepository;
        private readonly FacturaService _facturaService;

        public CheckOutService()
        {
            _checkOutRepository = new CheckOutRepository();
            _facturaRepository = new FacturaRepository();
            _facturaService = new FacturaService();
        }

        public async Task<(bool exito, string pdfPath, string xmlPath, string mensaje)>
            ProcesarCheckOutAsync(
                EstanciaActivaModel estancia,
                decimal montoServicios,
                decimal descuento,
                int usuarioId
            )
        {
            try
            {
                // 1) Calcular noches
                int noches = (int)(estancia.FechaSalida.Date - estancia.FechaEntrada.Date).TotalDays;
                if (noches <= 0) noches = 1;

                decimal montoHospedaje = noches * estancia.PrecioPorNoche;

                decimal total = montoHospedaje + montoServicios - descuento - estancia.Anticipo;
                if (total < 0) total = 0;

                // 2) Construir historial
                var historial = new HistorialEstanciaModel
                {
                    ClienteId = estancia.ClienteId,
                    EstanciaId = estancia.EstanciaId,
                    HotelId = estancia.HotelId,
                    Numero = estancia.Numero,
                    FechaEntrada = estancia.FechaEntrada,
                    FechaSalida = estancia.FechaSalida,
                    Anticipo = estancia.Anticipo,
                    MontoHospedaje = montoHospedaje,
                    MontoServicios = montoServicios,
                    TotalFactura = total,
                    UsuarioRegistro = usuarioId
                };

                // 3) Ejecutar check-out en Cassandra
                bool ok = _checkOutRepository.RealizarCheckOut(
                    historial,
                    estancia.FechaEntrada,
                    estancia.FechaSalida,
                    estancia.ReservaId
                );

                if (!ok)
                    return (false, null, null, "No se pudo completar el Check-Out en Cassandra.");

                // 4) Armar factura
                var factura = new FacturaModel
                {
                    FolioFactura = Guid.NewGuid(),
                    ClienteId = estancia.ClienteId,
                    HotelId = estancia.HotelId,
                    ReservaId = estancia.ReservaId,
                    EstanciaId = estancia.EstanciaId,
                    FechaEmision = DateTime.Now,
                    FechaEntrada = estancia.FechaEntrada,
                    FechaSalida = estancia.FechaSalida,
                    Anticipo = estancia.Anticipo,
                    MontoHospedaje = montoHospedaje,
                    MontoServicios = montoServicios,
                    Descuento = descuento,
                    TotalFactura = total
                };

                // 4.1 Generar archivos
                var (pdfPath, xmlPath) = _facturaService.GenerarFactura(factura);

                // 4.2 Guardar factura en Cassandra
                await _facturaRepository.InsertarFacturaAsync(factura);

                return (true, pdfPath, xmlPath, "Check-Out completado correctamente.");
            }
            catch (Exception ex)
            {
                return (false, null, null, $"Error en CheckOutService: {ex.Message}");
            }
        }
    }
}
