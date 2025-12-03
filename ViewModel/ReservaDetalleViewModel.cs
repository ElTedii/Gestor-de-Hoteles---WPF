using Gestión_Hotelera.Data.Repositories;
using Gestión_Hotelera.Model;
using System;
using System.Windows;
using System.Windows.Input;

namespace Gestión_Hotelera.ViewModel
{
    public class ReservaDetalleViewModel : ViewModelBase
    {
        private readonly MainViewModel _main;
        private readonly ReservaRepository _reservaRepo;
        private readonly ClienteRepository _clienteRepo;
        private readonly HotelRepository _hotelRepo;

        private readonly Guid _clienteId;
        private readonly Guid _reservaId;

        // Propiedades que se muestran
        public string CodigoReserva { get; private set; }
        public string Estado { get; private set; }

        public string ClienteNombre { get; private set; }
        public string ClienteCorreo { get; private set; }
        public string ClienteTelefono { get; private set; }

        public string HotelNombre { get; private set; }
        public string HabitacionTexto { get; private set; }

        public string FechaEntradaTexto { get; private set; }
        public string FechaSalidaTexto { get; private set; }
        public string DiasTexto { get; private set; }
        public string PersonasTexto { get; private set; }

        public string PrecioTotalTexto { get; private set; }

        private decimal _anticipo;
        public decimal Anticipo
        {
            get => _anticipo;
            set { _anticipo = value; OnPropertyChanged(); OnPropertyChanged(nameof(SaldoTexto)); }
        }

        public string SaldoTexto => $"Saldo aproximado: {(PrecioTotalEstimado - Anticipo):C}";

        private decimal PrecioTotalEstimado { get; set; }

        public ICommand GuardarCambiosCommand { get; }
        public ICommand VolverCommand { get; }

        public ReservaDetalleViewModel(ReservaListadoModel listado, MainViewModel main)
        {
            _main = main;
            _reservaRepo = new ReservaRepository();
            _clienteRepo = new ClienteRepository();
            _hotelRepo = new HotelRepository();

            _clienteId = listado.ClienteId;
            _reservaId = listado.ReservaId;

            CargarDetalle(listado);

            GuardarCambiosCommand = new ViewModelCommand(ExecuteGuardarCambios);
            VolverCommand = new ViewModelCommand(_ => _main.ShowReservasViewCommand.Execute(null));
        }

        private void CargarDetalle(ReservaListadoModel listado)
        {
            // Traemos la reserva base desde reservas_por_cliente
            var r = _reservaRepo.GetByClienteAndReserva(listado.ClienteId, listado.ReservaId);
            var cliente = _clienteRepo.GetById(listado.ClienteId);
            var hotel = _hotelRepo.GetById(listado.HotelId);

            CodigoReserva = $"RSV-{listado.ReservaId.ToString().Substring(0, 8).ToUpper()}";
            Estado = r?.Estado ?? listado.Estado;

            ClienteNombre = cliente?.NombreCompleto ?? "N/D";
            ClienteCorreo = cliente?.Correo ?? "";
            ClienteTelefono = cliente?.TelCelular ?? cliente?.TelCasa ?? "";

            HotelNombre = hotel?.Nombre ?? "N/D";
            HabitacionTexto = "Habitación: (sin asignar todavía)";

            var entrada = r?.FechaEntrada ?? listado.FechaEntrada;
            var salida = r?.FechaSalida ?? listado.FechaSalida;

            int dias = (salida - entrada).Days;
            if (dias < 1) dias = 1;

            FechaEntradaTexto = $"Entrada: {entrada:dd/MM/yyyy}";
            FechaSalidaTexto = $"Salida: {salida:dd/MM/yyyy}";
            DiasTexto = $"Estancia: {dias} noche(s)";

            PersonasTexto = $"Adultos: {r?.Adultos ?? listado.Adultos}   Menores: {r?.Menores ?? listado.Menores}";

            // De momento usamos Anticipo como total aproximado
            Anticipo = r?.Anticipo ?? listado.Anticipo;
            PrecioTotalEstimado = listado.PrecioTotal > 0 ? listado.PrecioTotal : Anticipo;
            PrecioTotalTexto = $"Total aprox.: {PrecioTotalEstimado:C}";

            OnPropertyChanged(string.Empty);
        }

        private void ExecuteGuardarCambios(object obj)
        {
            _reservaRepo.UpdateAnticipo(_clienteId, _reservaId, Anticipo);
            MessageBox.Show("Anticipo actualizado.");
            _main.ShowReservasViewCommand.Execute(null);

            // Volver a la lista
            _main.ShowReservasViewCommand.Execute(null);
        }
    }
}