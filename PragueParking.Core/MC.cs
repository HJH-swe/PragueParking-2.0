using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace PragueParking.Core
{
    public class MC : Vehicle
    {

        public MC(string regNumber, int vehicleSize, int pricePerHour) : base(regNumber)
        {
            VehicleSize = vehicleSize;
            PricePerHour = pricePerHour;
        }
    }
}
