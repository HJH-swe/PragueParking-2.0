using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PragueParking.Core
{
    public class Car : Vehicle
    {

        public Car(string regNumber, int vehicleSize, int pricePerHour) : base(regNumber)
        {
            VehicleSize = vehicleSize;
            PricePerHour = pricePerHour;      // Change to load from pricelist
        }
    }
}
