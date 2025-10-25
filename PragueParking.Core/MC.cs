using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PragueParking.Core
{
    public class MC : Vehicle
    {

        public MC(string regNumber) : base(regNumber)
        {
            VehicleSize = 2;
            PricePerHour = 10;
        }
    }
}
