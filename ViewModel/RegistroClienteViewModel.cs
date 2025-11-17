using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Gestión_Hotelera.ViewModel
{
    public class RegistroClienteViewModel : ViewModelBase
    {
        public string Nombre { get; set; }
        public string Apellidos { get; set; }
        public string Telefono { get; set; }
        public string Email { get; set; }
        public string Nacionalidad { get; set; }
        public string Documento { get; set; }

        public ICommand RegistrarClienteCommand { get; }

        public RegistroClienteViewModel()
        {
            RegistrarClienteCommand = new ViewModelCommand(RegistrarCliente);
        }

        private void RegistrarCliente(object obj)
        {
            // Aquí conectaremos Cassandra después :)
        }
    }
}