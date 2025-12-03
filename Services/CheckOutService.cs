using System;
using Gestión_Hotelera.Data.Repositories;
using Gestión_Hotelera.Helpers;
using Gestión_Hotelera.Model;

namespace Gestión_Hotelera.Services
{
    public class CheckOutResult
    {
        public FacturaModel Factura { get; set; }
        public ClienteModel Cliente { get; set; }
        public HotelModel Hotel { get; set; }
        public string RutaPdf { get; set; }
    }

    public class CheckOutService
    {
        private readonly EstanciaActivaRepository _estanciaRepo;
        private readonly HistorialEstanciaRepository _historialRepo;
        private readonly FacturaRepository _facturaRepo;
        private readonly HotelRepository _hotelRepo;
        private readonly ClienteRepository _clienteRepo;
        private readonly ReservaRepository _reservaRepo;

        public CheckOutService()
        {
            _estanciaRepo = new EstanciaActivaRepository();
            _historialRepo = new HistorialEstanciaRepository();
            _facturaRepo = new FacturaRepository();
            _hotelRepo = new HotelRepository();
            _clienteRepo = new ClienteRepository();
            _reservaRepo = new ReservaRepository();

        }

        public CheckOutResult RealizarCheckOut(
            Guid hotelId,
            int numeroHabitacion,
            decimal montoServicios,
            decimal descuento,
            string usuario)
        {
            var estancia = _estanciaRepo.GetByHotelAndNumero(hotelId, numeroHabitacion);
            if (estancia == null)
                throw new Exception("No existe una estancia activa para esta habitación.");

            var hotel = _hotelRepo.GetById(estancia.HotelId);
            var cliente = _clienteRepo.GetById(estancia.ClienteId);
            if (hotel == null || cliente == null)
                throw new Exception("No se pudo cargar información del cliente u hotel.");

            int noches = (int)(estancia.FechaSalida.Date - estancia.FechaEntrada.Date).TotalDays;
            if (noches < 1) noches = 1;

            decimal subtotalHospedaje = noches * estancia.PrecioNoche;
            decimal total = subtotalHospedaje + montoServicios - estancia.Anticipo - descuento;
            if (total < 0) total = 0;

            // ----- Guardar en historial_estancias -----
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

            // ----- Crear factura -----
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

            // ----- Generar PDF -----
            string rutaPdf = FacturaPrinter.GenerarFacturaPDF(factura, cliente, hotel, estancia, descuento);
            string rutaXml = FacturaPrinter.GenerarFacturaXML(factura, cliente, hotel, estancia, descuento);
            string rutaJson = FacturaPrinter.GenerarFacturaJSON(factura, cliente, hotel, estancia, descuento);

            _reservaRepo.UpdateEstado(estancia.ClienteId, estancia.ReservaId, "FINALIZADA");

            // ----- Eliminar estancia activa -----
            _estanciaRepo.Delete(estancia.HotelId, estancia.NumeroHabitacion);

            return new CheckOutResult
            {
                Factura = factura,
                Cliente = cliente,
                Hotel = hotel,
                RutaPdf = rutaPdf
            };
        }
    }
}