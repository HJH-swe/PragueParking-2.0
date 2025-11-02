using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace PragueParking.Core
{

    public class ParkingSpace
    {
        // total size of parking spaces shouldn't be changed - so private readonly
        private readonly int totalSize = 4;

        public int TotalSize { get; set; }
        public int SpaceNumber { get; set; }
        public int AvailableSize { get; set; }
        public List<Vehicle> ParkedVehicles { get; set; }

        // A special constructor for JSON - found the idea online
        [JsonConstructor]
        public ParkingSpace(int totalSize, int spaceNumber, List<Vehicle> parkedVehicles)
        {
            TotalSize = totalSize;
            SpaceNumber = spaceNumber;
            AvailableSize = totalSize;
            ParkedVehicles = parkedVehicles;
        }
        

        // Methods
        public bool IsEnoughSpace(Vehicle vehicle)
        {
            return vehicle.VehicleSize <= AvailableSize;
        }
        public bool AddVehicle(Vehicle vehicle)
        {
            if (IsEnoughSpace(vehicle) == false)
            {
                return false;
            }
            else
            {
                ParkedVehicles.Add(vehicle);
                AvailableSize -= vehicle.VehicleSize;
                return true;
            }
        }
        public ParkingSpace RemoveVehicle(Vehicle vehicle)
        {            
            ParkedVehicles.Remove(vehicle);
            AvailableSize += vehicle.VehicleSize;
            return this;
        }
        public Vehicle FindVehicleInSpace(string regNumber)
        {
            foreach (var vehicle in ParkedVehicles)
            {
                if (vehicle.RegNumber == regNumber)
                {
                    return vehicle;
                }
            }
            return null;
        }
        public string PrintParkingSpace(int spaceNumber)
        {
            if (ParkedVehicles.Count == 0)
            {
                return $"Space {SpaceNumber}: (Empty)  -   Available space: {AvailableSize}";
            }
            else
            {
                string vehicles = "";
                foreach (Vehicle vehicle in ParkedVehicles)
                {
                    vehicles += vehicle.RegNumber + "  ";
                }
                return $"Space {SpaceNumber}: {vehicles}\tAvailable space: {AvailableSize}";
            }
        }
        public override string ToString()
        {
            if (ParkedVehicles.Count == 0)
            {
                return $"Space {SpaceNumber}: (Empty)  -   Available space: {AvailableSize}\n";
            }
            else
            {
                string vehicles = "";
                foreach (Vehicle vehicle in ParkedVehicles)
                {
                    vehicles += vehicle.RegNumber + "  ";
                }
                return $"Space {SpaceNumber}: {vehicles} -   Available space: {AvailableSize}\n";
            }
        }
    }
}
