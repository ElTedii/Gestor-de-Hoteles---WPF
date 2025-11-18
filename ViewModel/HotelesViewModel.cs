using Gestión_Hotelera.Data.Repositories;
using Gestión_Hotelera.Model;
using Gestion_Hotelera.ViewModel;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Gestión_Hotelera.ViewModel
{
    public class HotelesViewModel : ViewModelBase
    {
        private readonly HotelRepository _repo;
        private readonly MainViewModel _main;

        public ObservableCollection<HotelModel> Hoteles { get; set; }

        public ICommand AbrirRegistroHotelCommand { get; }

        public HotelesViewModel(MainViewModel main)
        {
            _main = main;
            _repo = new HotelRepository();

            Hoteles = new ObservableCollection<HotelModel>();

            AbrirRegistroHotelCommand = new RelayCommand(o =>
            {
                _main.ShowRegistroHotelViewCommand.Execute(null);
            });

            LoadHoteles();
        }

        private async void LoadHoteles()
        {
            //Hoteles.Clear();
            //var lista = await _repo.GetHoteles();
            //foreach (var h in lista)
            //    Hoteles.Add(h);
        }
    }
}
