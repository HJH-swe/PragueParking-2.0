using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using PragueParking.Core;

namespace PragueParking.Data
{
    public class FileManager
    {
        public FileManager()
        {
        }

        public string SaveParkingData<T>(T parkingSpaces, string filePath)
        {
            try
            {
                var options = new JsonSerializerOptions { WriteIndented = true };
                string jsonString = JsonSerializer.Serialize(parkingSpaces, options);
                File.WriteAllText(filePath, jsonString);
                return $"Parking data saved to {filePath}";
            }
            catch (Exception e)
            {
                return $"Error saving data to {filePath}: {e.Message}";
            }
        }

        public List<ParkingSpace> LoadParkingData(string filePath)
        {
            if (!File.Exists(filePath))
            {
                return new List<ParkingSpace>();
            }

            string jsonString = File.ReadAllText(filePath);
            List<ParkingSpace>? savedData = JsonSerializer.Deserialize<List<ParkingSpace>>(jsonString);
            return savedData ?? new List<ParkingSpace>();
        }

        public GarageConfiguration? LoadGarageConfiguration(string configFilePath)
        {
            if (!File.Exists(configFilePath))
            {
                return null;
            }

            string jsonString = File.ReadAllText(configFilePath);
            var configuration = JsonSerializer.Deserialize<GarageConfiguration>(jsonString);

            return configuration;
        }

        public PriceListConfiguration? ConfigurePriceList(string priceListFilePath)
        {
            if (!File.Exists(priceListFilePath))
            {
                return null;
            }

            // First we need somewhere to store the prices before we create the new file
            using (StreamReader sr = new StreamReader(priceListFilePath))
            {
                PriceListConfiguration priceConfiguration = new PriceListConfiguration();

                priceConfiguration.PriceList = new PriceList();

                // Go through pricelist.txt line by line - find what's relevant
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    // if the line is a comment, starts with #, skip
                    if (line.StartsWith("#"))
                    {
                        continue;
                    }
                    // Set PriceList properties directly
                    if (line.StartsWith("CAR"))
                    {

                        priceConfiguration.PriceList.CarVehiclePrice = Convert.ToInt32(line.Substring(line.IndexOf("=") + 1)); // CAR.price[=]20
                    }
                    else if (line.StartsWith("MC"))
                    {
                        priceConfiguration.PriceList.MCVehiclePrice = Convert.ToInt32(line.Substring(line.IndexOf("=") + 1));
                    }
                }
                return priceConfiguration;
            }
        }
    }



    public class GarageConfiguration
    {
        public GarageConfiguration(int garageSize, List<string> allowedVehicles,
            int mcVehicleSize, int carVehicleSize, int parkingSpaceSize)
        {
            GarageSize = garageSize;
            AllowedVehicles = allowedVehicles;
            MCVehicleSize = mcVehicleSize;
            CarVehicleSize = carVehicleSize;
            ParkingSpaceSize = parkingSpaceSize;
        }
        public int GarageSize { get; }                  // Props don't need setters --> shouldn't change after configuration
        public List<string> AllowedVehicles { get; }        
        public int MCVehicleSize { get; }
        public int CarVehicleSize { get; }
        public int ParkingSpaceSize { get; }

    }
    public class PriceListConfiguration
    {
        public PriceListConfiguration()
        {

        }
        public PriceList PriceList { get; set; }
    }


}
