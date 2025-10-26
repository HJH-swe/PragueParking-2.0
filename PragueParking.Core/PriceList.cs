using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PragueParking.Core
{
    public class PriceList
    {
        public PriceList(int mcPricePerHour, int carPricePerHour)
        {
            MCVehiclePrice = mcPricePerHour;
            CarVehiclePrice = carPricePerHour;
        }
        public int MCVehiclePrice { get; set; }
        public int CarVehiclePrice { get; set; }
    }
}
