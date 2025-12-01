using Gestión_Hotelera.Data.Repositories;
using Gestión_Hotelera.Model;
using Gestión_Hotelera.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

            CrearReservaCommand = new ViewModelCommand(ExecuteCrearReserva);
        }

        // -------- listas ----------
        public ObservableCollection<ClienteModel> Clientes { get; }
        public ObservableCollection<HotelModel> Hoteles { get; }
        public ObservableCollection<HabitacionModel> Habitaciones { get; }

        // -------- selección ----------
        private ClienteModel _clienteSeleccionado;
        public ClienteModel ClienteSeleccionado
        {
            get => _clienteSeleccionado;
            set { _clienteSeleccionado = value; OnPropertyChanged(nameof(ClienteSeleccionado)); }
        }

        private HotelModel _hotelSeleccionado;
        public HotelModel HotelSeleccionado
        {
            get => _hotelSeleccionado;
            set
            {
                _hotelSeleccionado = value;
                OnPropertyChanged(nameof(HotelSeleccionado));
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
                OnPropertyChanged(nameof(HabitacionSeleccionada));
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
                OnPropertyChanged(nameof(FechaEntrada));
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
                OnPropertyChanged(nameof(FechaSalida));
                CargarHabitacionesDisponibles();
                CalcularPrecio();
            }
        }

        private int _adultos;
        public int Adultos
        {
            get => _adultos;
            set { _adultos = value; OnPropertyChanged(nameof(Adultos)); }
        }

        private int _menores;
        public int Menores
        {
            get => _menores;
            set { _menores = value; OnPropertyChanged(nameof(Menores)); }
        }

        private decimal _precioTotal;
        public decimal PrecioTotal
        {
            get => _precioTotal;
            set { _precioTotal = value; OnPropertyChanged(nameof(PrecioTotal)); }
        }

        // ============================================================
        // HABITACIONES DISPONIBLES
        // ============================================================
        private void CargarHabitacionesDisponibles()
        {
            if (HotelSeleccionado == null || FechaEntrada == null || FechaSalida == null)
                return;

            Habitaciones.Clear();

            var list = _reservaService.ObtenerHabitacionesDisponibles(
                HotelSeleccionado.HotelId,
                FechaEntrada.Value,
                FechaSalida.Value);

            foreach (var h in list)
                Habitaciones.Add(h);
        }

        // ============================================================
        // CALCULAR PRECIO
        // ============================================================
        private void CalcularPrecio()
        {
            if (HabitacionSeleccionada == null || FechaEntrada == null || FechaSalida == null)
            {
                PrecioTotal = 0;
                return;
            }

            int dias = (FechaSalida.Value - FechaEntrada.Value).Days;
            if (dias < 1) dias = 1;

            PrecioTotal = HabitacionSeleccionada.PrecioBase * dias;
        }

        // ============================================================
        // GUARDAR RESERVA
        // ============================================================
        public ICommand CrearReservaCommand { get; }

        private void ExecuteCrearReserva(object obj)
        {
            if (ClienteSeleccionado == null || HotelSeleccionado == null ||
                HabitacionSeleccionada == null || FechaEntrada == null || FechaSalida == null)
            {
                MessageBox.Show("Complete todos los campos.");
                return;
            }

            var reserva = new ReservacionModel
            {
                ReservaId = Guid.NewGuid(),
                ClienteId = ClienteSeleccionado.ClienteId,
                HotelId = HotelSeleccionado.HotelId,
                FechaEntrada = FechaEntrada.Value,
                FechaSalida = FechaSalida.Value,
                Adultos = Adultos,
                Menores = Menores,
                Anticipo = 0,
                UsuarioRegistro = LoginViewModel.UsuarioActual?.Correo ?? "sistema"
                // Estado, FechaRegistro etc. los completa el servicio
            };

            _reservaService.CrearReserva(reserva);

            MessageBox.Show("Reservación registrada correctamente.");

            // volver a la lista de reservas
            _main.ShowReservasViewCommand.Execute(null);
        }
    }
}
