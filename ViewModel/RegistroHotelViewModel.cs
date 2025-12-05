using Gestión_Hotelera.Data.Repositories;
using Gestión_Hotelera.Model;
using Gestión_Hotelera.ViewModel;
using Gestion_Hotelera.ViewModel;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace Gestión_Hotelera.ViewModel
{
    public class RegistroHotelViewModel : ViewModelBase
    {
        private readonly HotelRepository _repo;
        private readonly MainViewModel _main;

        public RegistroHotelViewModel(MainViewModel main)
        {
            _main = main;
            _repo = new HotelRepository();

            TiposHabitacion = new ObservableCollection<string>();
            ServiciosSeleccionados = new ObservableCollection<string>();

            GuardarCommand = new RelayCommand(ExecuteGuardar);
            AgregarTipoCommand = new RelayCommand(ExecuteAgregarTipo);
        }

        // PROPIEDADES
        private string _name;
        public string Name
        {
            get => _name;
            set { _name = value; OnPropertyChanged(); }
        }

        private string _city;
        public string City
        {
            get => _city;
            set { _city = value; OnPropertyChanged(); }
        }

        private string _address;
        public string Address
        {
            get => _address;
            set { _address = value; OnPropertyChanged(); }
        }

        private int _numFloors;
        public int NumFloors
        {
            get => _numFloors;
            set { _numFloors = value; OnPropertyChanged(); }
        }

        public ObservableCollection<string> TiposHabitacion { get; set; }

        private bool _frentePlaya;
        public bool FrentePlaya
        {
            get => _frentePlaya;
            set { _frentePlaya = value; OnPropertyChanged(); }
        }

        private int _numPiscinas;
        public int NumPiscinas
        {
            get => _numPiscinas;
            set { _numPiscinas = value; OnPropertyChanged(); }
        }

        public ObservableCollection<string> ServiciosSeleccionados { get; set; }

        // COMANDOS
        public ICommand GuardarCommand { get; }
        public ICommand AgregarTipoCommand { get; }

        private void ExecuteAgregarTipo(object obj)
        {
            TiposHabitacion.Add("");
        }

        // GUARDAR HOTEL
        private async void ExecuteGuardar(object obj)
        {
            if (string.IsNullOrWhiteSpace(Name) ||
                string.IsNullOrWhiteSpace(City) ||
                string.IsNullOrWhiteSpace(Address))
            {
                MessageBox.Show("Completa todos los campos obligatorios.", "Advertencia",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var model = new HotelModel
            {
                /*HotelId = Guid.NewGuid(),
                Name = Name,
                City = City,
                Address = Address,
                NumFloors = NumFloors,
                TiposHabitacion = new System.Collections.Generic.List<string>(TiposHabitacion),
                Servicios = new System.Collections.Generic.List<string>(ServiciosSeleccionados),
                FrentePlaya = FrentePlaya,
                NumPiscinas = NumPiscinas*/
            };

            /*bool ok = await _repo.InsertHotel(model);

            if (!ok)
            {
                MessageBox.Show("Error al guardar el hotel en Cassandra.",
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }*/

            MessageBox.Show("¡Hotel registrado correctamente!",
                "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);

            _main.ShowHotelsViewCommand.Execute(null);
        }
    }
}
