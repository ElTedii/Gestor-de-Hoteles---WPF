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
        public Action CloseAction { get; set; }

        private readonly EstanciaActivaRepository _estanciaRepo;
        private readonly HabitacionRepository _habRepo;
        private readonly ReservaRepository _reservaRepo;

        private readonly MainViewModel _main;

        public RealizarCheckInModel Datos { get; }

        public ObservableCollection<HabitacionModel> HabitacionesDisponibles { get; set; }

        private HabitacionModel _habSeleccionada;
        public HabitacionModel HabSeleccionada
        {
            get => _habSeleccionada;
            set
            {
                _habSeleccionada = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(PrecioNoche));
            }
        }

        public decimal PrecioNoche => HabSeleccionada?.PrecioNoche ?? 0;

        public ICommand ConfirmarCommand { get; }
        public ICommand CancelarCommand { get; }

        public RealizarCheckInViewModel(RealizarCheckInModel datos, MainViewModel main)
        {
            Datos = datos;
            _main = main;

            _estanciaRepo = new EstanciaActivaRepository();
            _habRepo = new HabitacionRepository();
            _reservaRepo = new ReservaRepository();

            HabitacionesDisponibles = new ObservableCollection<HabitacionModel>();

            ConfirmarCommand = new ViewModelCommand(ExecuteConfirmar);
            CancelarCommand = new ViewModelCommand(_ => CloseAction?.Invoke());   // ← FALTABA

            CargarHabitaciones();
        }

        private void CargarHabitaciones()
        {
            HabitacionesDisponibles.Clear();

            var libres = _habRepo.GetHabitacionesLibres(
                Datos.HotelId,
                Datos.FechaEntrada,
                Datos.FechaSalida);

            foreach (var h in libres)
                HabitacionesDisponibles.Add(h);
        }

        private void ExecuteConfirmar(object obj)
        {
            if (HabSeleccionada == null)
            {
                MessageBox.Show("Seleccione una habitación.");
                return;
            }

            _estanciaRepo.Insert(new EstanciaActivaModel
            {
                HotelId = Datos.HotelId,
                NumeroHabitacion = HabSeleccionada.NumeroHabitacion,
                ClienteId = Datos.ClienteId,
                ReservaId = Datos.ReservaId,
                FechaEntrada = Datos.FechaEntrada,
                FechaSalida = Datos.FechaSalida,
                Adultos = Datos.Adultos,
                Menores = Datos.Menores,
                UsuarioRegistro = Datos.UsuarioRegistro,
                FechaRegistro = DateTime.UtcNow,
                PrecioNoche = HabSeleccionada.PrecioNoche  // recomendable
            });

            _reservaRepo.UpdateEstado(Datos.ClienteId, Datos.ReservaId, "EN_ESTANCIA");

            MessageBox.Show("Check-In realizado correctamente.");

            CloseAction?.Invoke();    // ← FALTABA PARA VOLVER A LA LISTA
        }
    }
}