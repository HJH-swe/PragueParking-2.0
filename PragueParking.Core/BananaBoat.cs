using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace PragueParking.Core
{
    // Not an allowed vehicle - empty class
    public class BananaBoat : Vehicle
    {

        public BananaBoat(string regNumber) : base(regNumber)
        {
            
        }
    }
}
