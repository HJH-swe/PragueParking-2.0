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

        public Configuration? LoadConfiguration(string configFilePath, string priceListFilePath)
        {
            if (!File.Exists(configFilePath) || !File.Exists(priceListFilePath))
            {
                return null;
            }

            // First we need somewhere to store the prices before we create the new file
            PriceList pricelist = new PriceList();
            using (StreamReader sr = new StreamReader(priceListFilePath))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    // if the line is a comment, starts with #, skip
                    if (line.StartsWith("#"))
                    {
                        continue;
                    }

                    if (line.StartsWith("MC"))
                    {
                        // mc.price=10
                        pricelist.MC = Convert.ToInt32(line.Substring(line.IndexOf("=") + 1)); // mc.price[=]10
                    }
                    else if (line.StartsWith("CAR"))
                    {
                        pricelist.CAR = Convert.ToInt32(line.Substring(line.IndexOf("=") + 1));
                    }
                }
            }

            // "Filerna för prislista och konfiguration kan gärna kombineras till en fil"
            // Combine the two json files - either as files or when we have the two json strings
            // Then deserialize into a "Configuration"

            string jsonString = File.ReadAllText(configFilePath);
            var configuration = JsonSerializer.Deserialize<Configuration>(jsonString);

            configuration.PriceList = pricelist;
            return configuration;
        }
    }

    public class Configuration
    {
        public Configuration(int garageSize, List<string> allowedVehicles, int mcVehicleSize, int carVehicleSize)
        {
            GarageSize = garageSize;
            AllowedVehicles = allowedVehicles;
            MCVehicleSize = mcVehicleSize;
            CarVehicleSize = carVehicleSize;
        }
        public int GarageSize { get; }
        public List<string> AllowedVehicles { get; }        // Don't need a setter, shouldn't change after configuration
        public int MCVehicleSize { get; }
        public int CarVehicleSize { get; }
        public PriceList PriceList { get; set; }
    }

    public class PriceList
    {
        public PriceList()
        {

        }
        public int MC { get; set; }
        public int CAR { get; set; }
    }
}
