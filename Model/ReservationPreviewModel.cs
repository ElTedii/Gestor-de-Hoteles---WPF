using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gestión_Hotelera.Model
{
    public class ReservationPreviewModel
    {
        public string Code { get; set; }
        public string Client { get; set; }
        public string Hotel { get; set; }
        public string CheckIn { get; set; }
        public string CheckOut { get; set; }
        public string Total { get; set; }
    }
}
