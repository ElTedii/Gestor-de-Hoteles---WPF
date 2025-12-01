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
    public class ReservasViewModel : ViewModelBase
    {

        private readonly ReservaRepository _reservaRepo;
        private readonly ClienteRepository _clienteRepo;
        private readonly HotelRepository _hotelRepo;
        private readonly EstanciaActivaRepository _estanciaRepo;

        public ReservasViewModel()
        {
            _reservaRepo = new ReservaRepository();
            _clienteRepo = new ClienteRepository();
            _hotelRepo = new HotelRepository();
            _estanciaRepo = new EstanciaActivaRepository();

            Reservas = new ObservableCollection<ReservaListadoModel>();

            AbrirNuevaReservaCommand = new ViewModelCommand(ExecuteNuevaReserva);
            AbrirEditarReservaCommand = new ViewModelCommand(ExecuteEditarReserva);
            EliminarReservaCommand = new ViewModelCommand(ExecuteEliminarReserva);

            CargarReservas();
        }

        // ============================================================
        // LISTA PARA EL DATAGRID
        // ============================================================
        public ObservableCollection<ReservaListadoModel> Reservas { get; set; }

        private ReservaListadoModel _reservaSeleccionada;
        public ReservaListadoModel ReservaSeleccionada
        {
            get => _reservaSeleccionada;
            set { _reservaSeleccionada = value; OnPropertyChanged(nameof(ReservaSeleccionada)); }
        }

        // ============================================================
        // COMMANDS
        // ============================================================
        public ICommand AbrirNuevaReservaCommand { get; }
        public ICommand AbrirEditarReservaCommand { get; }
        public ICommand EliminarReservaCommand { get; }

        public Action AbrirNuevaReservaAction { get; set; }
        public Action<ReservaListadoModel> AbrirEditarReservaAction { get; set; }

        // ============================================================
        // CARGAR RESERVAS
        // ============================================================
        private void CargarReservas()
        {
            try
            {
                Reservas.Clear();

                var listaReservas = _reservaRepo.GetAll();

                foreach (var r in listaReservas)
                {
                    var cliente = _clienteRepo.GetById(r.ClienteId);
                    var hotel = _hotelRepo.GetById(r.HotelId);

                    // La habitación y el precio vienen de la estancia (si ya está asignada)
                    var estancia = _estanciaRepo.GetByReserva(r.ReservaId);

                    // calcular noches
                    int noches = (int)(r.FechaSalida - r.FechaEntrada).TotalDays;
                    if (noches < 1) noches = 1;

                    // si hay estancia usamos su precio_noche, si no, 0
                    decimal precioNoche = estancia?.PrecioNoche ?? 0m;

                    decimal total = precioNoche * noches;

                    // si todavía no hay precio (no hay estancia), mostramos el anticipo
                    if (total <= 0)
                        total = r.Anticipo;

                    Reservas.Add(new ReservaListadoModel
                    {
                        ReservaId = r.ReservaId,
                        ClienteId = r.ClienteId,
                        HotelId = r.HotelId,

                        ClienteNombre = cliente?.NombreCompleto ?? "N/D",
                        HotelNombre = hotel?.Nombre ?? "N/D",
                        NumeroHabitacion = estancia?.NumeroHabitacion ?? 0,

                        FechaEntrada = r.FechaEntrada,
                        FechaSalida = r.FechaSalida,

                        Adultos = r.Adultos,
                        Menores = r.Menores,
                        NumPersonas = r.Adultos + r.Menores,

                        PrecioTotal = total,
                        Estado = r.Estado
                    });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error cargando reservaciones: {ex.Message}");
            }
        }

        // ============================================================
        // NUEVA RESERVA
        // ============================================================
        private void ExecuteNuevaReserva(object obj)
        {
            AbrirNuevaReservaAction?.Invoke();
        }

        // ============================================================
        // EDITAR RESERVA
        // ============================================================
        private void ExecuteEditarReserva(object obj)
        {
            if (obj is ReservaListadoModel r)
                AbrirEditarReservaAction?.Invoke(r);
        }

        // ============================================================
        // ELIMINAR RESERVA
        // ============================================================
        private void ExecuteEliminarReserva(object obj)
        {
            if (obj is ReservaListadoModel r)
            {
                if (MessageBox.Show("¿Eliminar reservación?",
                    "Confirmar", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    _reservaRepo.DeleteByCliente(r.ClienteId, r.ReservaId);
                    CargarReservas();
                }
            }
        }
    }
}
