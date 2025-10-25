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



        ////Change to load all the data - but return string?
        //public List<ParkingSpace> LoadParkingData(string filePath)
        //{
        //    try
        //    {
        //        if (!File.Exists(filePath))
        //        {
        //            AnsiConsole.Write(new Markup($"File not found: {filePath}", Color.Blue));
        //            return new List<ParkingSpace>();
        //        }

        //        string jsonString = File.ReadAllText(filePath);
        //        List<ParkingSpace>? savedData = JsonSerializer.Deserialize<List<ParkingSpace>>(jsonString);

        //        AnsiConsole.Write(new Markup($"Parking data loaded from {filePath}", Color.Blue));
        //        return savedData ?? new List<ParkingSpace>();
        //    }
        //    catch (Exception e)
        //    {
        //        AnsiConsole.Write(new Markup($"Error loading data from {filePath}: {e.Message}", Color.Blue));
        //        return new List<ParkingSpace>();
        //    }
        //}

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

        public Configuration? LoadConfiguration(string filePath)
        {
            if (!File.Exists(filePath))
                return null;

            string jsonString = File.ReadAllText(filePath);
            return JsonSerializer.Deserialize<Configuration>(jsonString);
        }

    }

    public class Configuration
    {
        public Configuration(int garageSize)
        {
            GarageSize = garageSize;
        }
        public int GarageSize { get; set; }
    }
}
