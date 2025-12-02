using Gestión_Hotelera.Data.Repositories;
using Gestión_Hotelera.Model;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace Gestión_Hotelera.ViewModel
{
    public class RegistroHabitacionViewModel : ViewModelBase
    {
        private readonly HabitacionRepository _habRepo;
        private readonly HotelRepository _hotelRepo;
        private readonly TipoHabitacionRepository _tipoRepo;

        public ObservableCollection<HotelModel> Hoteles { get; set; }
        public ObservableCollection<TipoHabitacionModel> TiposHab { get; set; }

        private Guid _hotelSeleccionado;
        public Guid HotelSeleccionado
        {
            get => _hotelSeleccionado;
            set
            {
                _hotelSeleccionado = value;
                OnPropertyChanged();
                CargarTipos();
            }
        }

        private Guid _tipoSeleccionado;
        public Guid TipoSeleccionado
        {
            get => _tipoSeleccionado;
            set { _tipoSeleccionado = value; OnPropertyChanged(); }
        }

        public int NumeroHabitacion { get; set; }
        public int Piso { get; set; }

        public ICommand GuardarCommand { get; }

        public RegistroHabitacionViewModel()
        {
            _habRepo = new HabitacionRepository();
            _hotelRepo = new HotelRepository();
            _tipoRepo = new TipoHabitacionRepository();

            Hoteles = new ObservableCollection<HotelModel>(_hotelRepo.GetAll());
            TiposHab = new ObservableCollection<TipoHabitacionModel>();

            GuardarCommand = new ViewModelCommand(ExecuteGuardar);
        }

        private void CargarTipos()
        {
            TiposHab.Clear();

            if (HotelSeleccionado == Guid.Empty)
                return;

            var tipos = _tipoRepo.GetByHotel(HotelSeleccionado);

            foreach (var t in tipos)
                TiposHab.Add(t);
        }

        private void ExecuteGuardar(object obj)
        {
            if (HotelSeleccionado == Guid.Empty || TipoSeleccionado == Guid.Empty)
            {
                MessageBox.Show("Seleccione un hotel y un tipo.", "Aviso");
                return;
            }

            var h = new HabitacionModel
            {
                HotelId = HotelSeleccionado,
                TipoId = TipoSeleccionado,
                NumeroHabitacion = NumeroHabitacion,
                Piso = Piso,
                UsuarioRegistro = LoginViewModel.UsuarioActual?.Correo ?? "sistema"
            };

            _habRepo.Insert(h);

            MessageBox.Show("Habitación registrada correctamente.");
        }
    }
}