using Gestión_Hotelera.Model;
using Gestión_Hotelera.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Gestión_Hotelera.ViewModel
{
    public class NuevaReservaViewModel : ViewModelBase
    {
        private MainViewModel _mainVM;

        public ObservableCollection<ClienteModel> Clientes => ClienteService.Instance.Clientes;
        //public ObservableCollection<HotelModel> Hoteles => HotelService.Instance.Hoteles;
        public ObservableCollection<HabitacionModel> Habitaciones { get; set; }

        private HotelModel _hotelSeleccionado;
        public HotelModel HotelSeleccionado
        {
            get => _hotelSeleccionado;
            set
            {
                _hotelSeleccionado = value;
                //OnPropertyChanged();
                CargarHabitaciones();
            }
        }

        public ClienteModel ClienteSeleccionado { get; set; }
        public HabitacionModel HabitacionSeleccionada { get; set; }

        public DateTime FechaEntrada { get; set; } = DateTime.Today;
        public DateTime FechaSalida { get; set; } = DateTime.Today.AddDays(1);
        public int NumPersonas { get; set; }
        public decimal PrecioTotal { get; set; }

        public ICommand CrearReservaCommand { get; }

        public NuevaReservaViewModel(MainViewModel vm)
        {
            _mainVM = vm;

            Habitaciones = new ObservableCollection<HabitacionModel>();

            CrearReservaCommand = new ViewModelCommand(CrearReserva);
        }

        private void CargarHabitaciones()
        {
            Habitaciones.Clear();

            //if (HotelSeleccionado?.Habitaciones != null)
            //{
            //    foreach (var h in HotelSeleccionado.Habitaciones)
            //        Habitaciones.Add(h);
            //}
        }

        private void CrearReserva(object obj)
        {
            var r = new ReservationModel
            {
                ReservationId = Guid.NewGuid(),
                ClienteId = ClienteSeleccionado.ClienteId,
                ClienteNombre = $"{ClienteSeleccionado.Nombre} {ClienteSeleccionado.Apellidos}",

                HotelId = HotelSeleccionado.HotelId,
                HotelNombre = HotelSeleccionado.Name,

                HabitacionId = HabitacionSeleccionada.HabitacionId,
                //NumeroHabitacion = HabitacionSeleccionada.NumeroHabitacion,

                FechaEntrada = FechaEntrada,
                FechaSalida = FechaSalida,
                NumPersonas = NumPersonas,
                PrecioTotal = PrecioTotal
            };

            ReservationService.Instance.AgregarReserva(r);

            _mainVM.ShowReservasViewCommand.Execute(null);
        }
    }
}
