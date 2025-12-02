using Gestión_Hotelera.Data.Repositories;
using Gestión_Hotelera.Model;
using System;
using System.Windows;
using System.Windows.Input;

namespace Gestión_Hotelera.ViewModel
{
    public class RegistroClienteViewModel : ViewModelBase
    {
        private readonly ClienteRepository _repo;
        private readonly MainViewModel _main;

        public ClienteModel Cliente { get; set; }

        public ICommand GuardarCommand { get; }

        public RegistroClienteViewModel(MainViewModel main)
        {
            _repo = new ClienteRepository();
            _main = main;

            Cliente = new ClienteModel
            {
                UsuarioRegistro = LoginViewModel.UsuarioActual?.Correo ?? "sistema"
            };

            GuardarCommand = new ViewModelCommand(ExecuteGuardar);
        }

        private void ExecuteGuardar(object obj)
        {
            try
            {
                _repo.Insert(Cliente);
                MessageBox.Show("Cliente registrado correctamente.");
                _main.CurrentChildView = new ClientesViewModel(_main);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }
    }
}