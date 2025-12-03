using Gestión_Hotelera.Data.Repositories;
using Gestión_Hotelera.Model;
using Gestión_Hotelera.Services;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace Gestión_Hotelera.ViewModel
{
    public class NuevaReservaViewModel : ViewModelBase
    {
        private readonly MainViewModel _main;
        private readonly ClienteRepository _clienteRepo;
        private readonly HotelRepository _hotelRepo;
        private readonly ReservaService _reservaService;

        public NuevaReservaViewModel(MainViewModel main)
        {
            _main = main;

            _clienteRepo = new ClienteRepository();
            _hotelRepo = new HotelRepository();
            _reservaService = new ReservaService();

            Clientes = new ObservableCollection<ClienteModel>(_clienteRepo.GetAll());
            Hoteles = new ObservableCollection<HotelModel>(_hotelRepo.GetAll());
            Habitaciones = new ObservableCollection<HabitacionModel>();

            Adultos = 1;
            Menores = 0;

            CrearReservaCommand = new ViewModelCommand(ExecuteCrearReserva);
        }

        // LISTAS
        public ObservableCollection<ClienteModel> Clientes { get; }
        public ObservableCollection<HotelModel> Hoteles { get; }
        public ObservableCollection<HabitacionModel> Habitaciones { get; }

        // SELECCIÓN
        private ClienteModel _clienteSeleccionado;
        public ClienteModel ClienteSeleccionado
        {
            get => _clienteSeleccionado;
            set { _clienteSeleccionado = value; OnPropertyChanged(); }
        }

        private HotelModel _hotelSeleccionado;
        public HotelModel HotelSeleccionado
        {
            get => _hotelSeleccionado;
            set
            {
                _hotelSeleccionado = value;
                OnPropertyChanged();
                CargarHabitacionesDisponibles();
            }
        }

        private HabitacionModel _habitacionSeleccionada;
        public HabitacionModel HabitacionSeleccionada
        {
            get => _habitacionSeleccionada;
            set
            {
                _habitacionSeleccionada = value;
                OnPropertyChanged();
                CalcularPrecio();
            }
        }

        private DateTime? _fechaEntrada;
        public DateTime? FechaEntrada
        {
            get => _fechaEntrada;
            set
            {
                _fechaEntrada = value;
                OnPropertyChanged();
                CargarHabitacionesDisponibles();
                CalcularPrecio();
            }
        }

        private DateTime? _fechaSalida;
        public DateTime? FechaSalida
        {
            get => _fechaSalida;
            set
            {
                _fechaSalida = value;
                OnPropertyChanged();
                CargarHabitacionesDisponibles();
                CalcularPrecio();
            }
        }

        private int _adultos;
        public int Adultos
        {
            get => _adultos;
            set { _adultos = value; OnPropertyChanged(); }
        }

        private int _menores;
        public int Menores
        {
            get => _menores;
            set { _menores = value; OnPropertyChanged(); }
        }

        private decimal _anticipo;
        public decimal Anticipo
        {
            get => _anticipo;
            set { _anticipo = value; OnPropertyChanged(); }
        }

        private decimal _precioTotal;
        public decimal PrecioTotal
        {
            get => _precioTotal;
            set { _precioTotal = value; OnPropertyChanged(); }
        }

        // HABITACIONES DISPONIBLES (usa ReservaService con lógica avanzada)
        private void CargarHabitacionesDisponibles()
        {
            if (HotelSeleccionado == null || FechaEntrada == null || FechaSalida == null)
                return;

            Habitaciones.Clear();

            var disponibles = _reservaService.ObtenerHabitacionesDisponibles(
                HotelSeleccionado.HotelId,
                FechaEntrada.Value,
                FechaSalida.Value);

            foreach (var h in disponibles)
                Habitaciones.Add(h);
        }

        // CALCULAR PRECIO
        private void CalcularPrecio()
        {
            if (HabitacionSeleccionada == null || FechaEntrada == null || FechaSalida == null)
            {
                PrecioTotal = 0;
                return;
            }

            int dias = (FechaSalida.Value - FechaEntrada.Value).Days;
            if (dias < 1) dias = 1;

            PrecioTotal = HabitacionSeleccionada.PrecioNoche * dias;
        }

        // GUARDAR RESERVA
        public ICommand CrearReservaCommand { get; }

        private void ExecuteCrearReserva(object obj)
        {
            if (ClienteSeleccionado == null ||
                HotelSeleccionado == null ||
                HabitacionSeleccionada == null ||
                FechaEntrada == null ||
                FechaSalida == null)
            {
                MessageBox.Show("Complete todos los campos.");
                return;
            }

            if (Anticipo < 0)
            {
                MessageBox.Show("El anticipo no puede ser negativo.");
                return;
            }

            if (Anticipo > PrecioTotal)
            {
                MessageBox.Show("El anticipo no puede ser mayor al precio total.");
                return;
            }

            // Estado según anticipo
            var estado = Anticipo > 0 ? "CONFIRMADA" : "PENDIENTE";

            var r = new ReservacionModel
            {
                ReservaId = Guid.NewGuid(),
                ClienteId = ClienteSeleccionado.ClienteId,
                HotelId = HotelSeleccionado.HotelId,
                FechaEntrada = FechaEntrada.Value,
                FechaSalida = FechaSalida.Value,
                Adultos = Adultos,
                Menores = Menores,
                Anticipo = Anticipo,
                UsuarioRegistro = LoginViewModel.UsuarioActual?.Correo ?? "sistema",
                Estado = estado,
                FechaRegistro = DateTime.UtcNow
            };

            _reservaService.CrearReserva(r);

            MessageBox.Show("Reservación registrada correctamente.");

            _main.ShowReservasViewCommand.Execute(null);
        }
    }
}