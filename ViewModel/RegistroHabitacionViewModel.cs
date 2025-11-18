using Gestión_Hotelera.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gestión_Hotelera.ViewModel
{
    public class RegistroHabitacionViewModel : ViewModelBase
    {
        public ObservableCollection<HotelModel> Hoteles { get; set; }

        public RegistroHabitacionViewModel()
        {
            Hoteles = new ObservableCollection<HotelModel>
        {
            //new HotelModel{ HotelId=Guid.NewGuid(), Name="Hotel Sol"},
            //new HotelModel{ HotelId=Guid.NewGuid(), Name="Hotel Mar"},
        };
        }
    }

}
