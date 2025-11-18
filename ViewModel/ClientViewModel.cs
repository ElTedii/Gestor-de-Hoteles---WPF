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
    public class ClientViewModel : ViewModelBase
    {
        private MainViewModel _mainViewModel;

        public ObservableCollection<ClienteModel> Clientes { get; set; }

        public ICommand AbrirRegistroClienteCommand { get; }
        public ICommand AbrirEditarClienteCommand { get; }
        public ICommand EliminarClienteCommand { get; }

        public ClientViewModel(MainViewModel mainVM)
        {
            _mainViewModel = mainVM;

            AbrirRegistroClienteCommand = new ViewModelCommand(_ => _mainViewModel.ShowRegistroClienteCommand.Execute(null));

            Clientes = new ObservableCollection<ClienteModel>
            {
                //new ClienteModel { ClienteId = Guid.NewGuid(), Nombre = "Juan", Apellidos="Pérez", Telefono="5551234567", Email="juan@gmail.com", Documento="INE" },
                //new ClienteModel { ClienteId = Guid.NewGuid(), Nombre = "María", Apellidos="García", Telefono="5557654321", Email="maria@gmail.com", Documento="Pasaporte" }
            };
        }
    }
}
