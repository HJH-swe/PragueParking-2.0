using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Quic;
using System.Text;
using System.Threading.Tasks;

namespace PragueParking.Core
{
    public class ParkingGarage
    {
        private readonly List<ParkingSpace> parkingSpaces = new List<ParkingSpace>();

        public ParkingGarage(List<ParkingSpace> savedData, int garageSize, List<string> allowedVehicles, 
            int mcVehicleSize, int carVehicleSize, int parkingSpaceSize)
        {
            parkingSpaces = savedData;
            GarageSize = garageSize;
            AllowedVehicles = allowedVehicles;
            MCVehicleSize = mcVehicleSize;
            CarVehicleSize = carVehicleSize;
            ParkingSpaceSize = parkingSpaceSize;

            for (int i = parkingSpaces.Count; i < GarageSize; i++)
            {
                parkingSpaces.Add(new ParkingSpace(parkingSpaceSize, i + 1, new List<Vehicle>()));
            }
        }

        public int GarageSize { get; private set; }
        public List<string> AllowedVehicles { get; set; }
        public int MCVehicleSize { get; set; }
        public int CarVehicleSize { get; set; }
        public int ParkingSpaceSize { get; set; }

        // Methods
        public List<ParkingSpace> GetAllSpaces()
        {
            return parkingSpaces;
        }
        public int FindFreeSpace(Vehicle vehicle)
        {
            if (vehicle is Car)
            {

                for (int i = 0; i < parkingSpaces.Count; i++)
                {
                    if (parkingSpaces[i].AvailableSize == 4)
                    {
                        return i;
                    }
                }
            }
            else if (vehicle is MC)
            {
                // Optimize parking - first check to fill 1 space with 2 MC
                for (int i = 1; i < parkingSpaces.Count; i++)               //TODO: Change i = 0?
                {
                    if (parkingSpaces[i].AvailableSize == 2)
                    {
                        return i;
                    }
                }
                // If no space next to MC, look for any space
                for (int i = 1; i < parkingSpaces.Count; i++)
                {
                    if (parkingSpaces[i].AvailableSize > 2)
                    {
                        return i;
                    }
                }
            }
            // Return -1 if no free spaces, garage is full 
            return -1;
        }
        public ParkingSpace FindVehicleSpace(string regNumber)
        {
            int spaceCounter = 0;
            foreach (var parkingSpace in parkingSpaces)
            {
                foreach (var vehicle in parkingSpace.ParkedVehicles)
                {
                    if (vehicle.RegNumber == regNumber)
                    {
                        return parkingSpace;
                    }
                }
                spaceCounter++;
            }
            return null;
        }
        public bool ParkVehicle(Vehicle vehicle, out int freeSpace)
        {
            freeSpace = FindFreeSpace(vehicle);
            if (freeSpace == -1)
            {
                return false;
            }
            else
            {
                var spaceToUse = parkingSpaces[freeSpace];
                return spaceToUse.AddVehicle(vehicle);
            }
        }
        public ParkingSpace GetParkingSpace(int spaceNumber)
        {
            if (spaceNumber < 0 || spaceNumber > parkingSpaces.Count)
            {
                //AnsiConsole.Write(new Markup("[blue]Error! invalid parking space.[/]"));
                return null;
            }
            else
            {
                return parkingSpaces[spaceNumber];
            }
        }
        // Method to remove extra spaces if new configuration of Garage Size is smaller than before
        public void RemoveRangeOfSpaces(int fromIndex, int toIndex)
        {
            parkingSpaces.RemoveRange(fromIndex, (toIndex - fromIndex));        // toIndex - fromIndex = number of spaces to remove
                                                                                // fromIndex = 51, toIndex = 100, toIndex-fromIndex = 49
        }
        public void UpdateAllVehiclePrices(PriceList priceList)
        {
            foreach (var space in parkingSpaces)
            {
                foreach (var vehicle in space.ParkedVehicles)
                {
                    if (vehicle.VehicleSize == 4)
                    {
                        vehicle.PricePerHour = priceList.CarVehiclePrice;
                    }
                    else if (vehicle.VehicleSize == 2)
                    {
                        vehicle.PricePerHour = priceList.MCVehiclePrice;
                    }
                }
            }
        }
        public override string ToString()
        {
            StringBuilder parkingGarage = new StringBuilder();
            parkingGarage.AppendLine("\nStatus of entire garage:\n");

            if (parkingSpaces.Count > 0)
            {
                //parkingGarage.Append(string.Join("\n", parkingSpaces));
                foreach (var parkingSpace in parkingSpaces)
                {
                    parkingGarage.Append(parkingSpace + "\n");

                    //if (parkingSpace != null)
                    //{
                    //    parkingGarage.Append(parkingSpace + "\n");
                    //}
                }
            }
            else
            {
                parkingGarage.AppendLine("The garage is empty.");
            }
            return parkingGarage.ToString();
        }
    }
}
