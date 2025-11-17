using Gestión_Hotelera.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Gestión_Hotelera.ViewModel
{
    public class HotelesViewModel : ViewModelBase
    {

        private MainViewModel _mainViewModel;

        public ICommand AbrirRegistroHotelCommand { get; }
        public ObservableCollection<HotelModel> Hoteles { get; set; }

        public HotelesViewModel(MainViewModel mainVM)
        {
            _mainViewModel = mainVM;

            AbrirRegistroHotelCommand =
                new ViewModelCommand(_ => _mainViewModel.ShowRegistroHotelViewCommand.Execute(null));
            // Datos de ejemplo: más adelante se llenará desde Cassandra
            Hoteles = new ObservableCollection<HotelModel>
            {
                new HotelModel { HotelId = Guid.NewGuid(), Name = "Hotel Sol", City = "Cancún", Address="Zona Hotelera", NumFloors=5 },
                new HotelModel { HotelId = Guid.NewGuid(), Name = "Hotel Mar", City = "Mazatlán", Address="Centro", NumFloors=8 },
                new HotelModel { HotelId = Guid.NewGuid(), Name = "City Business Inn", City = "CDMX", Address="Insurgentes Sur", NumFloors=10 }
            };
        }
    }
}
