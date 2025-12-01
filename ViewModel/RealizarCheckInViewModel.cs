using Gestión_Hotelera.Data.Repositories;
using Gestión_Hotelera.Model;
using Gestión_Hotelera.Services;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace Gestión_Hotelera.ViewModel
{
    public class RealizarCheckInViewModel : ViewModelBase
    {
        private readonly HabitacionRepository _habitacionRepo;
        private readonly TipoHabitacionRepository _tipoRepo;
        private readonly CheckInService _checkInService;

        public RealizarCheckInModel Modelo { get; }

        public ObservableCollection<HabitacionModel> Habitaciones { get; }

        private HabitacionModel _habitacionSeleccionada;
        public HabitacionModel HabitacionSeleccionada
        {
            get => _habitacionSeleccionada;
            set
            {
                _habitacionSeleccionada = value;
                OnPropertyChanged(nameof(HabitacionSeleccionada));
                PrecioNoche = value?.PrecioBase ?? 0;
            }
        }

        private decimal _precioNoche;
        public decimal PrecioNoche
        {
            get => _precioNoche;
            set { _precioNoche = value; OnPropertyChanged(nameof(PrecioNoche)); }
        }

        public ICommand ConfirmarCheckInCommand { get; }
        public ICommand CancelarCommand { get; }

        public Action CloseAction { get; set; }

        public RealizarCheckInViewModel(RealizarCheckInModel modelo, MainViewModel parent = null)
        {
            Modelo = modelo;

            _habitacionRepo = new HabitacionRepository();
            _tipoRepo = new TipoHabitacionRepository();
            _checkInService = new CheckInService();

            Habitaciones = new ObservableCollection<HabitacionModel>();

            ConfirmarCheckInCommand = new ViewModelCommand(
                ExecuteConfirmarCheckIn,
                _ => HabitacionSeleccionada != null
            );

            CancelarCommand = new ViewModelCommand(_ => CloseAction?.Invoke());

            CargarHabitaciones();
        }

        private void CargarHabitaciones()
        {
            Habitaciones.Clear();

            var lista = _habitacionRepo.GetByHotel(Modelo.HotelId);

            foreach (var h in lista)
            {
                var tipo = _tipoRepo.GetByHotelAndTipo(h.HotelId, h.TipoId);
                if (tipo != null)
                    h.PrecioBase = tipo.PrecioNoche;

                Habitaciones.Add(h);
            }
        }

        private void ExecuteConfirmarCheckIn(object obj)
        {
            try
            {
                var estanciaId = _checkInService.RealizarCheckIn(
                    Modelo.ClienteId,
                    Modelo.ReservaId,
                    HabitacionSeleccionada.Numero,
                    Modelo.UsuarioRegistro
                );

                MessageBox.Show("Check-In realizado con éxito.");
                CloseAction?.Invoke();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }
    }
}