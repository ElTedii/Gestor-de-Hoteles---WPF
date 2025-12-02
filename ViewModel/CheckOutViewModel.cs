using Gestión_Hotelera.Data.Repositories;
using Gestión_Hotelera.Model;
using Gestión_Hotelera.Services;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace Gestión_Hotelera.ViewModel
{
    public class CheckOutViewModel : ViewModelBase
    {
        private readonly HabitacionRepository _habitacionRepo;
        private readonly EstanciaActivaRepository _estanciaRepo;
        private readonly CheckOutService _checkOutService;
        private readonly ClienteRepository _clienteRepo;
        private readonly HotelRepository _hotelRepo;

        public ObservableCollection<HotelModel> Hoteles { get; set; }
        public ObservableCollection<int> Habitaciones { get; set; }

        // HOTEL SELECCIONADO
        private Guid _hotelSeleccionado;
        public Guid HotelSeleccionado
        {
            get => _hotelSeleccionado;
            set
            {
                _hotelSeleccionado = value;
                OnPropertyChanged(nameof(HotelSeleccionado));
                CargarHabitaciones();
            }
        }

        // HABITACIÓN SELECCIONADA
        private int _habitacionSeleccionada;
        public int HabitacionSeleccionada
        {
            get => _habitacionSeleccionada;
            set
            {
                _habitacionSeleccionada = value;
                OnPropertyChanged(nameof(HabitacionSeleccionada));
                CargarEstancia();
            }
        }

        // ESTANCIA CARGADA
        private EstanciaActivaModel _estancia;
        public EstanciaActivaModel Estancia
        {
            get => _estancia;
            set { _estancia = value; OnPropertyChanged(nameof(Estancia)); }
        }

        // DATOS PARA MOSTRAR
        private string _clienteNombre;
        public string ClienteNombre
        {
            get => _clienteNombre;
            set { _clienteNombre = value; OnPropertyChanged(nameof(ClienteNombre)); }
        }

        public decimal MontoServicios { get; set; }
        public decimal Descuento { get; set; }

        private decimal _totalCalculado;
        public decimal TotalCalculado
        {
            get => _totalCalculado;
            set { _totalCalculado = value; OnPropertyChanged(nameof(TotalCalculado)); }
        }

        // COMMAND
        public ICommand EjecutarCheckOutCommand { get; }

        public CheckOutViewModel()
        {
            _habitacionRepo = new HabitacionRepository();
            _estanciaRepo = new EstanciaActivaRepository();
            _checkOutService = new CheckOutService();
            _clienteRepo = new ClienteRepository();
            _hotelRepo = new HotelRepository();

            Hoteles = new ObservableCollection<HotelModel>(_hotelRepo.GetAll());
            Habitaciones = new ObservableCollection<int>();

            EjecutarCheckOutCommand = new ViewModelCommand(ExecuteCheckOut);
        }

        // ============================================================
        // CARGAR HABITACIONES
        // ============================================================
        private void CargarHabitaciones()
        {
            Habitaciones.Clear();

            if (HotelSeleccionado == Guid.Empty)
                return;

            var habs = _habitacionRepo.GetByHotel(HotelSeleccionado);
            foreach (var h in habs)
                Habitaciones.Add(h.NumeroHabitacion);
        }

        // ============================================================
        // CARGAR ESTANCIA
        // ============================================================
        private void CargarEstancia()
        {
            if (HotelSeleccionado == Guid.Empty || HabitacionSeleccionada == 0)
                return;

            Estancia = _estanciaRepo.GetByHotelAndHabitacion(HotelSeleccionado, HabitacionSeleccionada);

            if (Estancia == null)
            {
                ClienteNombre = "—";
                TotalCalculado = 0;
                return;
            }

            var cliente = _clienteRepo.GetById(Estancia.ClienteId);
            ClienteNombre = cliente?.NombreCompleto ?? "—";

            RecalcularTotal();
        }

        // ============================================================
        // RECALCULAR TOTAL
        // ============================================================
        private void RecalcularTotal()
        {
            if (Estancia == null) { TotalCalculado = 0; return; }

            int noches = (int)(Estancia.FechaSalida - Estancia.FechaEntrada).TotalDays;
            if (noches < 1) noches = 1;

            decimal hospedaje = noches * Estancia.PrecioNoche;

            TotalCalculado = hospedaje + MontoServicios - Estancia.Anticipo - Descuento;
            if (TotalCalculado < 0) TotalCalculado = 0;
        }

        // ============================================================
        // HACER CHECKOUT
        // ============================================================
        private void ExecuteCheckOut(object obj)
        {
            if (Estancia == null)
            {
                MessageBox.Show("Seleccione una habitación con estancia activa.");
                return;
            }

            try
            {
                Guid facturaId = _checkOutService.RealizarCheckOut(
                    Estancia.HotelId,
                    Estancia.NumeroHabitacion,
                    MontoServicios,
                    Descuento,
                    LoginViewModel.UsuarioActual?.Correo ?? "sistema"
                );

                MessageBox.Show($"Check-Out exitoso.\nFactura generada: {facturaId}");

                Estancia = null;
                ClienteNombre = "—";
                TotalCalculado = 0;
                HabitacionSeleccionada = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }
    }
}