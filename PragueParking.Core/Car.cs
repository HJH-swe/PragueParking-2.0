using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PragueParking.Core
{
    public class Car : Vehicle
    {

        public Car(string regNumber) : base(regNumber)
        {
            VehicleSize = 4;
            PricePerHour = 20;      // Change to load from pricelist
        }
    }
}
