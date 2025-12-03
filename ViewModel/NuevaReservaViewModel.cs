using Gestión_Hotelera.Data.Repositories;
using Gestión_Hotelera.Model;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace Gestión_Hotelera.ViewModel
{
    public class NuevaReservaViewModel : ViewModelBase
    {
        private readonly MainViewModel _main;
        private readonly ClienteRepository _clienteRepo;
        private readonly HotelRepository _hotelRepo;
        private readonly HabitacionRepository _habitacionRepo;
        private readonly ReservaRepository _reservaRepo;

        // ===========================
        //     COMBOS
        // ===========================
        public ObservableCollection<ClienteModel> Clientes { get; set; }
        public ObservableCollection<HotelModel> Hoteles { get; set; }
        public ObservableCollection<HabitacionModel> Habitaciones { get; set; }

        private ClienteModel _clienteSeleccionado;
        public ClienteModel ClienteSeleccionado
        {
            get => _clienteSeleccionado;
            set
            {
                _clienteSeleccionado = value;
                OnPropertyChanged();
            }
        }

        private HotelModel _hotelSeleccionado;
        public HotelModel HotelSeleccionado
        {
            get => _hotelSeleccionado;
            set
            {
                _hotelSeleccionado = value;
                OnPropertyChanged();
                CargarHabitaciones();
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
                CalcularTotal();
            }
        }

        // ===========================
        //     CAMPOS
        // ===========================
        private DateTime _fechaEntrada = DateTime.Today;
        public DateTime FechaEntrada
        {
            get => _fechaEntrada;
            set
            {
                _fechaEntrada = value;
                OnPropertyChanged();
                CargarHabitaciones();
                CalcularTotal();
            }
        }

        private DateTime _fechaSalida = DateTime.Today.AddDays(1);
        public DateTime FechaSalida
        {
            get => _fechaSalida;
            set
            {
                _fechaSalida = value;
                OnPropertyChanged();
                CargarHabitaciones();
                CalcularTotal();
            }
        }

        private int _adultos = 1;
        public int Adultos
        {
            get => _adultos;
            set
            {
                _adultos = value;
                OnPropertyChanged();
            }
        }

        private int _menores = 0;
        public int Menores
        {
            get => _menores;
            set
            {
                _menores = value;
                OnPropertyChanged();
            }
        }

        private decimal _anticipo = 0;
        public decimal Anticipo
        {
            get => _anticipo;
            set
            {
                _anticipo = value;
                OnPropertyChanged();
            }
        }

        private decimal _precioTotal = 0;
        public decimal PrecioTotal
        {
            get => _precioTotal;
            set
            {
                _precioTotal = value;
                OnPropertyChanged();
            }
        }

        // ===========================
        //     COMMANDS
        // ===========================
        public ICommand CrearReservaCommand { get; }

        public NuevaReservaViewModel(MainViewModel main)
        {
            _main = main;

            _clienteRepo = new ClienteRepository();
            _hotelRepo = new HotelRepository();
            _habitacionRepo = new HabitacionRepository();
            _reservaRepo = new ReservaRepository();

            Clientes = new ObservableCollection<ClienteModel>(_clienteRepo.GetAll());
            Hoteles = new ObservableCollection<HotelModel>(_hotelRepo.GetAll());
            Habitaciones = new ObservableCollection<HabitacionModel>();

            CrearReservaCommand = new ViewModelCommand(ExecuteCrearReserva);

            HotelSeleccionado = Hoteles.FirstOrDefault();
        }

        // ===========================
        //     HABITACIONES DISPONIBLES
        // ===========================
        private void CargarHabitaciones()
        {
            Habitaciones.Clear();

            if (HotelSeleccionado == null || FechaEntrada == default || FechaSalida == default)
                return;

            var libres = _habitacionRepo.GetHabitacionesLibres(
                HotelSeleccionado.HotelId,
                FechaEntrada,
                FechaSalida);

            foreach (var h in libres)
                Habitaciones.Add(h);
        }

        // ===========================
        //     CALCULAR TOTAL
        // ===========================
        private void CalcularTotal()
        {
            if (HabitacionSeleccionada == null)
            {
                PrecioTotal = 0;
                return;
            }

            int dias = (FechaSalida - FechaEntrada).Days;
            if (dias < 1) dias = 1;

            PrecioTotal = dias * HabitacionSeleccionada.PrecioNoche;
        }

        // ===========================
        //     GUARDAR RESERVA
        // ===========================
        private void ExecuteCrearReserva(object obj)
        {
            if (ClienteSeleccionado == null ||
                HotelSeleccionado == null ||
                HabitacionSeleccionada == null)
            {
                MessageBox.Show("Complete todos los datos.");
                return;
            }

            var nueva = new ReservacionModel
            {
                ReservaId = Guid.NewGuid(),
                ClienteId = ClienteSeleccionado.ClienteId,
                HotelId = HotelSeleccionado.HotelId,

                FechaEntrada = FechaEntrada,
                FechaSalida = FechaSalida,

                Adultos = Adultos,
                Menores = Menores,

                Anticipo = Anticipo,
                PrecioTotal = PrecioTotal,

                UsuarioRegistro = LoginViewModel.UsuarioActual?.Correo ?? "sistema",
                FechaRegistro = DateTime.UtcNow,

                Estado = "PENDIENTE"
            };

            _reservaRepo.Insert(nueva);

            MessageBox.Show("Reservación creada correctamente.");
            _main.ShowReservas();
        }
    }
}