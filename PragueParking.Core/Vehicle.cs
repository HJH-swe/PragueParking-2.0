using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PragueParking.Core
{

    public class Vehicle
    {
        // Properties
        public string? RegNumber { get; init; }     // Found init online - property only set when object is constructed
        public int VehicleSize { get; init; }     // protected so Size can be set in classes Car, MC etc.
        public DateTime ArrivalTime { get; init; }
        public DateTime DepartureTime { get; set; }
        public decimal PricePerHour { get; init; }     // protected, same as Size
        public ParkingSpace SpaceParked { get; set; }       // TODO: type parking space or int??


        // Constructor
        public Vehicle(string regNumber)
        {
            RegNumber = regNumber;
            ArrivalTime = DateTime.Now;
        }

        // Methods
        public int HoursToCharge(DateTime arrival, DateTime departure)
        {
            TimeSpan parkedTime = departure - arrival;
            double parkedMinutes = parkedTime.TotalMinutes - 10.0;     // first 10 mins free 
            int hoursToCharge;                                         // must be an integer

            // Price is per started hour 
            if (parkedMinutes % 60 == 0)
            {
                hoursToCharge = (int)parkedMinutes / 60;
            }
            else
            {
                hoursToCharge = (int)(parkedMinutes / 60) + 1;
            }

            return hoursToCharge;
        }
        public override string ToString()
        {
            StringBuilder vehicle = new StringBuilder();
            if (VehicleSize == 2)
            {
                vehicle.AppendLine($"\nMC  {RegNumber}");
            }
            if (VehicleSize == 4)
            {
                vehicle.AppendLine($"\nCAR  {RegNumber}");
            }
            vehicle.AppendLine($"Arrival Time: {ArrivalTime:dd/MM/yyyy HH:mm}\nPrice per Hour: {PricePerHour} CZK");
            return vehicle.ToString();
        }
    }
}
