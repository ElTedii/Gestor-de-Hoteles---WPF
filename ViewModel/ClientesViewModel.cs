using Gestión_Hotelera.Data.Repositories;
using Gestión_Hotelera.Model;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Gestión_Hotelera.ViewModel
{
    public class ClientesViewModel : ViewModelBase
    {
        private readonly ClienteRepository _repo;
        private readonly MainViewModel _main;

        public ObservableCollection<ClienteModel> Clientes { get; set; }

        public ICommand NuevoClienteCommand { get; }

        public ClientesViewModel(MainViewModel main)
        {
            _repo = new ClienteRepository();
            _main = main;

            Clientes = new ObservableCollection<ClienteModel>(_repo.GetAll());

            NuevoClienteCommand = new ViewModelCommand(ExecuteNuevoCliente);
        }

        private void ExecuteNuevoCliente(object obj)
        {
            _main.CurrentChildView = new RegistroClienteViewModel(_main);
        }
    }
}