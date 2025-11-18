using Gestión_Hotelera.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gestión_Hotelera.Services
{
    public class ClienteService
    {
        private static ClienteService _instance;
        public static ClienteService Instance => _instance ??= new ClienteService();

        public ObservableCollection<ClienteModel> Clientes { get; private set; }

        private ClienteService()
        {
            Clientes = new ObservableCollection<ClienteModel>
            {
                //new ClienteModel { ClienteId = Guid.NewGuid(), NombreCompleto="Juan Perez", Telefono="5551234567", Email="juan@gmail.com", Documento="INE" },
                //new ClienteModel { ClienteId = Guid.NewGuid(), NombreCompleto="María Perez", Telefono="5559876543", Email="maria@gmail.com", Documento="Pasaporte" }
            };
        }

        public void AgregarCliente(ClienteModel c)
        {
            Clientes.Add(c);
        }

        public void EliminarCliente(Guid id)
        {
            var cliente = Clientes.FirstOrDefault(x => x.ClienteId == id);
            if (cliente != null)
                Clientes.Remove(cliente);
        }

        public void ActualizarCliente(ClienteModel actualizado)
        {
            var cliente = Clientes.FirstOrDefault(x => x.ClienteId == actualizado.ClienteId);
            if (cliente != null)
            {
                //cliente.Nombre = actualizado.Nombre;
                //cliente.Apellidos = actualizado.Apellidos;
                //cliente.Telefono = actualizado.Telefono;
                //cliente.Email = actualizado.Email;
                //cliente.Documento = actualizado.Documento;
            }
        }
    }
}
