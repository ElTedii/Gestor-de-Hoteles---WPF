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
    public class ReservasViewModel : ViewModelBase
    {
        private MainViewModel _mainVM;

        public ObservableCollection<ReservationModel> Reservas { get; set; }

        public ICommand AbrirNuevaReservaCommand { get; }
        public ICommand AbrirEditarReservaCommand { get; }
        public ICommand EliminarReservaCommand { get; }

        public ReservasViewModel(MainViewModel vm)
        {
            _mainVM = vm;

            Reservas = ReservationService.Instance.Reservas;

            AbrirNuevaReservaCommand = new ViewModelCommand(_ =>
                _mainVM.ShowNuevaReservaCommand.Execute(null));

            AbrirEditarReservaCommand = new ViewModelCommand(EditarReserva);
        }

        private void EditarReserva(object obj)
        {
            if (obj is ReservationModel r)
                _mainVM.ShowEditarReservaCommand.Execute(r);
        }
    }
}
