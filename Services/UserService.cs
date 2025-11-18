using Gestión_Hotelera.Data.Repositories;
using Gestión_Hotelera.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gestión_Hotelera.Services
{
    public class UsuarioService
    {
        private readonly UsuarioRepository _repo;

        public UsuarioService()
        {
            _repo = new UsuarioRepository();
        }

        public UsersModel Login(string correo, string password)
        {
            return _repo.ValidarLogin(correo, password);
        }

        public bool RegistrarUsuario(UsersModel u)
        {
            if (string.IsNullOrWhiteSpace(u.Correo)) return false;
            if (string.IsNullOrWhiteSpace(u.NombreCompleto)) return false;

            return _repo.InsertarUsuario(u);
        }
    }
}
