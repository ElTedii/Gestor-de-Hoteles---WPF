using Gestión_Hotelera.Data.Repositories;
using Gestión_Hotelera.Model;
using System;

namespace Gestión_Hotelera.Services
{
    public class CheckOutService
    {
        private readonly EstanciaActivaRepository _estanciaRepo;
        private readonly HistorialEstanciaRepository _historialRepo;
        private readonly FacturaRepository _facturaRepo;
        private readonly HotelRepository _hotelRepo;
        private readonly ClienteRepository _clienteRepo;

        public CheckOutService()
        {
            _estanciaRepo = new EstanciaActivaRepository();
            _historialRepo = new HistorialEstanciaRepository();
            _facturaRepo = new FacturaRepository();
            _hotelRepo = new HotelRepository();
            _clienteRepo = new ClienteRepository();
        }

        // ============================================================
        // REALIZAR CHECK-OUT COMPLETO
        // ============================================================
        public Guid RealizarCheckOut(Guid hotelId, int numeroHabitacion, decimal montoServicios, decimal descuento, string usuario)
        {
            // 1) Obtener estancia activa
            var estancia = _estanciaRepo.GetByHotelAndHabitacion(hotelId, numeroHabitacion);

            if (estancia == null)
                throw new Exception("No existe una estancia activa para esta habitación.");

            // 2) Obtener datos auxiliares
            var hotel = _hotelRepo.GetById(estancia.HotelId);
            var cliente = _clienteRepo.GetById(estancia.ClienteId);

            if (hotel == null || cliente == null)
                throw new Exception("No se pudo cargar información del cliente u hotel.");

            // 3) Calcular noches
            int noches = (int)(estancia.FechaSalida.Date - estancia.FechaEntrada.Date).TotalDays;
            if (noches < 1) noches = 1;

            // 4) Subtotal hospedaje
            decimal subtotalHospedaje = noches * estancia.PrecioNoche;

            // 5) Total final
            decimal total = subtotalHospedaje + montoServicios - estancia.Anticipo - descuento;
            if (total < 0) total = 0;

            // ============================================================
            // GUARDAR EN HISTORIAL_ESTANCIAS
            // ============================================================
            var historial = new HistorialEstanciaModel
            {
                ClienteId = estancia.ClienteId,
                EstanciaId = estancia.EstanciaId,
                HotelId = estancia.HotelId,
                NumeroHabitacion = estancia.NumeroHabitacion,

                FechaEntrada = estancia.FechaEntrada,
                FechaSalida = estancia.FechaSalida,

                Anticipo = estancia.Anticipo,
                MontoHospedaje = subtotalHospedaje,
                MontoServicios = montoServicios,
                TotalFactura = total,

                UsuarioRegistro = usuario,
                FechaRegistro = DateTime.UtcNow
            };

            _historialRepo.Insert(historial);

            // ============================================================
            // CREAR FACTURA FINAL
            // ============================================================
            var factura = new FacturaModel
            {
                FacturaId = Guid.NewGuid(),
                EstanciaId = estancia.EstanciaId,
                ClienteId = estancia.ClienteId,
                HotelId = estancia.HotelId,

                FechaEmision = DateTime.UtcNow,

                MontoHospedaje = subtotalHospedaje,
                MontoServicios = montoServicios,
                Total = total,

                UsuarioRegistro = usuario,
                FechaRegistro = DateTime.UtcNow
            };

            _facturaRepo.Insert(factura);

            // ============================================================
            // ELIMINAR ESTANCIA ACTIVA
            // ============================================================
            _estanciaRepo.Delete(estancia.HotelId, estancia.NumeroHabitacion);

            // ============================================================
            // (Opcional) GENERAR PDF
            // ============================================================
            // var printer = new FacturaPrinter();
            // string path = printer.GenerarPDF(factura, cliente.NombreCompleto, hotel.Nombre);
            // Process.Start(path);

            return factura.FacturaId;
        }
    }
}