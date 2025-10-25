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
        public ParkingSpace SpaceParked { get; set; }       // TODO: keep or remove?


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
            double chargedHours = parkedTime.TotalHours - (10.0/ 60.0);     // first 10 mins free, so subract 10 mins as a fraction of an hour

            // If no fee to be charged, return 0
            if (chargedHours <= 0)
            {
                return 0;
            }

            // Math.Ceiling rounds up to next whole number --> same as hours started
            return (int)Math.Ceiling(chargedHours);

        }
        public string PrintParkingReceipt()
        {
            DepartureTime = DateTime.Now;
            decimal parkingFee = HoursToCharge(ArrivalTime, DepartureTime) * PricePerHour;

            StringBuilder checkedOutVehicle = new StringBuilder();
            if (VehicleSize == 2)
            {
                checkedOutVehicle.AppendLine($"\nMC  {RegNumber}");
            }
            if (VehicleSize == 4)
            {
                checkedOutVehicle.AppendLine($"\nCAR  {RegNumber}");
            }
            checkedOutVehicle.AppendLine($"Arrival Time: {ArrivalTime:dd/MM/yyyy HH:mm}\nDeparture Time: {DepartureTime:dd/MM/yyyy HH:mm}\n" +
                $"Price per Hour: {PricePerHour} CZK\nParking fee: {parkingFee} CZK");
            return checkedOutVehicle.ToString();
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
