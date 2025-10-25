using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace PragueParking.Core
{
    public class BananaBoat : Vehicle
    {

        public BananaBoat(string regNumber, int vehicleSize) : base(regNumber)
        {
            VehicleSize = vehicleSize;
            PricePerHour = 10;
        }
    }
}
