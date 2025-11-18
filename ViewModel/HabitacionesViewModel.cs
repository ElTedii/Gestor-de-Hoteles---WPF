using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Gestión_Hotelera.Model;

namespace Gestión_Hotelera.ViewModel
{
    public class HabitacionesViewModel : ViewModelBase
    {
        private MainViewModel _mainVM;

        public ICommand AbrirRegistroHabitacionCommand { get; }

        public ObservableCollection<HabitacionModel> Habitaciones { get; set; }

        public HabitacionesViewModel(MainViewModel mvm)
        {
            _mainVM = mvm;

            AbrirRegistroHabitacionCommand =
                new ViewModelCommand(_ => _mainVM.ShowRegistroHabitacionViewCommand.Execute(null));

            Habitaciones = new ObservableCollection<HabitacionModel>
        {
            //new HabitacionModel { HabitacionId = Guid.NewGuid(), Tipo = "Suite",    Capacidad = 4, Precio = 2500, TipoCama = "KingSize",    NombreHotel="Hotel Sol", Descripcion = "Habitación adaptada con todas las necesidades" },
            //new HabitacionModel { HabitacionId = Guid.NewGuid(), Tipo = "Estándar", Capacidad = 2, Precio = 1200, TipoCama = "QueenSize",   NombreHotel="Hotel Mar", Descripcion = "Habitación básica"},
        };
        }
    }

}
