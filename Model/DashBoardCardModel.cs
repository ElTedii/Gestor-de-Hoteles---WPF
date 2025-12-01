using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FontAwesome.Sharp;

namespace Gestión_Hotelera.Model
{
    public class DashboardCardModel
    {
        public string Titulo { get; set; }
        public string Valor { get; set; }
        public IconChar Icono { get; set; }
        public string Color { get; set; }
    }
}
