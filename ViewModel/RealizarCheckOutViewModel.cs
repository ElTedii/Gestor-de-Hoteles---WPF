using Gestión_Hotelera.Data.Repositories;
using Gestión_Hotelera.Model;
using Gestión_Hotelera.Services;
using System;
using System.Windows;
using System.Windows.Input;

namespace Gestión_Hotelera.ViewModel
{
    public class RealizarCheckOutViewModel : ViewModelBase
    {
        private readonly CheckOutService _checkOutService;

        public EstanciaActivaModel Estancia { get; private set; }

        public RealizarCheckOutViewModel()
        {
            _checkOutService = new CheckOutService();
            ConfirmarCommand = new ViewModelCommand(ExecuteConfirmar, _ => Estancia != null);
            CancelarCommand = new ViewModelCommand(_ => CloseAction?.Invoke());
        }

        public void CargarEstancia(EstanciaActivaModel estancia)
        {
            Estancia = estancia;
            OnPropertyChanged(nameof(Estancia));
            OnPropertyChanged(nameof(TotalCalculado));
        }

        private decimal _montoServicios;
        public decimal MontoServicios
        {
            get => _montoServicios;
            set
            {
                _montoServicios = value;
                OnPropertyChanged(nameof(MontoServicios));
                OnPropertyChanged(nameof(TotalCalculado));
            }
        }

        private decimal _descuento;
        public decimal Descuento
        {
            get => _descuento;
            set
            {
                _descuento = value;
                OnPropertyChanged(nameof(Descuento));
                OnPropertyChanged(nameof(TotalCalculado));
            }
        }

        public decimal TotalCalculado
        {
            get
            {
                if (Estancia == null) return 0;

                int noches = (int)(Estancia.FechaSalida - Estancia.FechaEntrada).TotalDays;
                if (noches < 1) noches = 1;

                decimal hospedaje = noches * Estancia.PrecioNoche;

                decimal total = hospedaje + MontoServicios - Estancia.Anticipo - Descuento;
                return total < 0 ? 0 : total;
            }
        }

        public ICommand ConfirmarCommand { get; }
        public ICommand CancelarCommand { get; }
        public Action CloseAction { get; set; }

        private void ExecuteConfirmar(object obj)
        {
            try
            {
                string usuario = LoginViewModel.UsuarioActual?.Correo ?? "sistema";

                var folio = _checkOutService.RealizarCheckOut(
                    Estancia.HotelId,
                    Estancia.NumeroHabitacion,
                    MontoServicios,
                    Descuento,
                    usuario
                );

                MessageBox.Show($"Check-Out realizado.\nFolio: {folio}");
                CloseAction?.Invoke();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al realizar Check-Out:\n{ex.Message}");
            }
        }
    }
}