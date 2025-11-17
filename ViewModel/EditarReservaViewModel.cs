using Gestión_Hotelera.Model;
using Gestión_Hotelera.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Gestión_Hotelera.ViewModel
{
    public class EditarReservaViewModel : ViewModelBase
    {
        private MainViewModel _mainVM;

        public ReservationModel Reserva { get; set; }

        public ICommand GuardarCambiosCommand { get; }

        public EditarReservaViewModel(MainViewModel vm, ReservationModel r)
        {
            _mainVM = vm;
            Reserva = r;

            GuardarCambiosCommand = new ViewModelCommand(Guardar);
        }

        private void Guardar(object obj)
        {
            ReservationService.Instance.ActualizarReserva(Reserva);
            _mainVM.ShowReservasViewCommand.Execute(null);
        }
    }
}
