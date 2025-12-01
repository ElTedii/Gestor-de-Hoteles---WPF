using Gestión_Hotelera.Data.Repositories;
using Gestión_Hotelera.Model;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace Gestión_Hotelera.ViewModel
{
    public class RealizarCheckInViewModel : ViewModelBase
    {
        private readonly HabitacionRepository _habitacionRepo;
        private readonly EstanciaActivaRepository _estanciaRepo;
        private readonly ReservaRepository _reservaRepo;

        // Datos enviados desde CheckInViewModel
        public RealizarCheckInModel Datos { get; }

        // Habitaciones disponibles
        public ObservableCollection<int> Habitaciones { get; set; }

        private int _habitacionSeleccionada;
        public int HabitacionSeleccionada
        {
            get => _habitacionSeleccionada;
            set
            {
                _habitacionSeleccionada = value;
                OnPropertyChanged(nameof(HabitacionSeleccionada));
            }
        }

        public ICommand RealizarCheckInCommand { get; }

        public Action CloseAction { get; set; }

        public RealizarCheckInViewModel(RealizarCheckInModel datos)
        {
            Datos = datos ?? throw new ArgumentNullException(nameof(datos));

            _habitacionRepo = new HabitacionRepository();
            _estanciaRepo = new EstanciaActivaRepository();
            _reservaRepo = new ReservaRepository();

            Habitaciones = new ObservableCollection<int>();

            CargarHabitaciones();

            RealizarCheckInCommand = new ViewModelCommand(ExecuteCheckIn, CanExecuteCheckIn);
        }

        // ============================================================
        // CARGAR HABITACIONES DEL HOTEL
        // ============================================================
        private void CargarHabitaciones()
        {
            Habitaciones.Clear();

            var habitaciones = _habitacionRepo.GetByHotel(Datos.HotelId);

            foreach (var h in habitaciones)
                Habitaciones.Add(h.Numero);
        }

        // ============================================================
        // VALIDAR
        // ============================================================
        private bool CanExecuteCheckIn(object obj)
        {
            return HabitacionSeleccionada > 0;
        }

        // ============================================================
        // REALIZAR CHECK-IN
        // ============================================================
        private void ExecuteCheckIn(object obj)
        {
            if (HabitacionSeleccionada == 0)
            {
                MessageBox.Show("Seleccione una habitación.", "Advertencia");
                return;
            }

            // Validar que no esté ocupada
            var estancia = _estanciaRepo.GetByHotelAndHabitacion(Datos.HotelId, HabitacionSeleccionada);

            if (estancia != null)
            {
                MessageBox.Show("La habitación ya tiene una estancia activa.", "Error");
                return;
            }

            try
            {
                // Crear estancia activa
                var nueva = new EstanciaActivaModel
                {
                    EstanciaId = Guid.NewGuid(),
                    HotelId = Datos.HotelId,
                    NumeroHabitacion = HabitacionSeleccionada,

                    ClienteId = Datos.ClienteId,
                    ReservaId = Datos.ReservaId,

                    FechaEntrada = Datos.FechaEntrada,
                    FechaSalida = Datos.FechaSalida,

                    Adultos = Datos.Adultos,
                    Menores = Datos.Menores,

                    Anticipo = 0, // se actualiza en Checkout
                    PrecioNoche = 0, // si luego agregas tabla de precios se actualiza

                    UsuarioRegistro = Datos.UsuarioRegistro,
                    FechaRegistro = DateTime.UtcNow
                };

                _estanciaRepo.Insert(nueva);

                // Actualizar reserva → EN_ESTANCIA
                _reservaRepo.UpdateEstado(Datos.ClienteId, Datos.ReservaId, "EN_ESTANCIA");

                MessageBox.Show("Check-In realizado correctamente.", "Éxito");

                CloseAction?.Invoke();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al realizar Check-In:\n" + ex.Message, "Error");
            }
        }
    }
}