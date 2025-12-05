using Gestión_Hotelera.Data.Repositories;
using Gestión_Hotelera.Model;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace Gestión_Hotelera.ViewModel
{
    public class CheckInViewModel : ViewModelBase
    {
        private readonly ReservaRepository _reservaRepo;
        private readonly ClienteRepository _clienteRepo;
        private readonly HotelRepository _hotelRepo;

        public ObservableCollection<CheckInListadoModel> ReservasPendientes { get; set; }

        private CheckInListadoModel _seleccionada;
        public CheckInListadoModel Seleccionada
        {
            get => _seleccionada;
            set { _seleccionada = value; OnPropertyChanged(nameof(Seleccionada)); }
        }

        public ICommand AbrirCheckInCommand { get; }

        // Acción que abre la ventana de RealizarCheckIn
        public Action<RealizarCheckInModel> AbrirRealizarCheckInAction { get; set; }

        public CheckInViewModel()
        {
            _reservaRepo = new ReservaRepository();
            _clienteRepo = new ClienteRepository();
            _hotelRepo = new HotelRepository();

            ReservasPendientes = new ObservableCollection<CheckInListadoModel>();

            AbrirCheckInCommand = new ViewModelCommand(
                ExecuteAbrirCheckIn,
                _ => Seleccionada != null
            );

            CargarReservasPendientes();
        }

        // SOLO reservas para HOY (PENDIENTE / CONFIRMADA)
        private void CargarReservasPendientes()
        {
            ReservasPendientes.Clear();

            // fecha local real
            var hoy = DateTime.Now.Date;

            // obtener todas las reservas
            var reservas = _reservaRepo.GetAll()
                .Where(r =>
                    r.FechaEntrada.ToLocalTime().Date == hoy &&
                    (r.Estado == "PENDIENTE" || r.Estado == "CONFIRMADA")
                )
                .OrderBy(r => r.FechaEntrada)
                .ToList();

            foreach (var r in reservas)
            {
                var cliente = _clienteRepo.GetById(r.ClienteId);
                var hotel = _hotelRepo.GetById(r.HotelId);

                ReservasPendientes.Add(new CheckInListadoModel
                {
                    ClienteId = r.ClienteId,
                    ReservaId = r.ReservaId,
                    HotelId = r.HotelId,

                    ClienteNombre = cliente?.NombreCompleto ?? "—",
                    HotelNombre = hotel?.Nombre ?? "—",

                    FechaEntrada = r.FechaEntrada,
                    FechaSalida = r.FechaSalida,

                    Adultos = r.Adultos,
                    Menores = r.Menores,
                    Habitacion = "(Sin asignar)"
                });
            }
        }

        private void ExecuteAbrirCheckIn(object obj)
        {
            if (Seleccionada == null)
                return;

            var modelo = new RealizarCheckInModel
            {
                ReservaId = Seleccionada.ReservaId,
                ClienteId = Seleccionada.ClienteId,
                HotelId = Seleccionada.HotelId,

                ClienteNombre = Seleccionada.ClienteNombre,
                HotelNombre = Seleccionada.HotelNombre,

                FechaEntrada = Seleccionada.FechaEntrada,
                FechaSalida = Seleccionada.FechaSalida,

                Adultos = Seleccionada.Adultos,
                Menores = Seleccionada.Menores,

                UsuarioRegistro = LoginViewModel.UsuarioActual?.Correo ?? "sistema"
            };

            AbrirRealizarCheckInAction?.Invoke(modelo);
        }
    }
}