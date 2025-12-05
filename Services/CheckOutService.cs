using Gestión_Hotelera.Data.Repositories;
using Gestión_Hotelera.Helpers;
using Gestión_Hotelera.Model;
using System;

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
            // 1) Estancia activa
            var estancia = _estanciaRepo.GetByHotelAndNumero(hotelId, numeroHabitacion);
            if (estancia == null)
                throw new Exception("No existe estancia activa para esta habitación.");

            var hotel = _hotelRepo.GetById(estancia.HotelId);
            var cliente = _clienteRepo.GetById(estancia.ClienteId);

            // 2) Cálculo de noches
            int noches = (int)(estancia.FechaSalida.Date - estancia.FechaEntrada.Date).TotalDays;
            if (noches < 1) noches = 1;

            var subtotalHospedaje = noches * estancia.PrecioNoche;
            var total = subtotalHospedaje + montoServicios - estancia.Anticipo - descuento;
            if (total < 0) total = 0;

            // 3) Historial
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

            // 4) Factura
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

            // 5) Reserva FINALIZADA
            if (estancia.ReservaId != Guid.Empty)
                _reservaRepo.UpdateEstado(estancia.ClienteId, estancia.ReservaId, "FINALIZADA");

            // 6) PDF (FacturaPrinter es estático)
            string ruta = FacturaPrinter.GenerarFacturaPDF(factura, cliente, hotel, estancia, descuento);

            // 7) Borrar estancia activa
            _estanciaRepo.Delete(estancia.HotelId, estancia.NumeroHabitacion, estancia.FechaEntrada);

            return new CheckOutResult
            {
                Factura = factura,
                Cliente = cliente,
                Hotel = hotel,
                RutaPdf = ruta
            };
        }
    }
}