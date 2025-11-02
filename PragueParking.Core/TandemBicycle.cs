using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace PragueParking.Core
{
    // Not an allowed vehicle. Basically empty class
    public class TandemBicycle : Vehicle
    {

        public TandemBicycle(string regNumber) : base(regNumber)
        {
            
        }
    }
}
