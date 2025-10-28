using Spectre.Console;
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
        public void VisualParkingGarage()
        {
            // Header to match other menu headings        
            Table header = new Table()
                .Centered()
                .Border(TableBorder.HeavyEdge)
                .BorderColor(Color.Aquamarine1)
                .Width(70);
            header.AddColumn(new TableColumn("[#ff00ff bold] PRAGUE PARKING OVERVIEW[/]").Centered());
            AnsiConsole.Write(header);

            // One table for subheading
            Table subHeader = new Table();
            subHeader.AddColumns("[#d7ffff]EMPTY SPACE:[/] [lime]GREEN[/]",
                "[#d7ffff]PARTIALLY OCCUPIED:[/] [yellow]YELLOW[/]",
                "[#d7ffff]FULL SPACE:[/] [red]RED[/]")
                .Centered()
                .Alignment(Justify.Center);
            subHeader.Border(TableBorder.HeavyEdge);
            subHeader.BorderColor(Color.Aquamarine1);
            subHeader.Width(70);
            AnsiConsole.Write(subHeader);

            // Another table for visual over parking garage
            Table allSpaces = new Table().Centered();
            var colorString = string.Empty;
            var printSpots = string.Empty;
            int spaceCounter = 1, emptyCounter = 0, halfCounter = 0, fullCounter = 0;       // Will use counters for bar chart
            foreach (var space in parkingSpaces)
            {
                if (space.AvailableSize == space.TotalSize)       // If available == total --> empty
                {
                    colorString = "lime";
                    emptyCounter++;
                }
                else if (space.AvailableSize > 0 && space.AvailableSize < space.TotalSize)  // greater than 0, less than total --> partially occupied
                {
                    colorString = "yellow";
                    halfCounter++;
                }
                else                                                                        // Only reasonable option left --> space occupied 
                {
                    colorString = "red";
                    fullCounter++;
                }
                // Format spaces so numbers align nicely
                if (spaceCounter < 10)
                {
                    printSpots += $"[{colorString}]{spaceCounter}    [/]";
                }
                else
                {
                    printSpots += $"[{colorString}]{spaceCounter}   [/]";        // TODO: Stick each number in panel?
                }
                spaceCounter++;
            }

            allSpaces.AddColumn(new TableColumn(printSpots).PadLeft(3)).Centered();
            allSpaces.Border(TableBorder.HeavyEdge);
            allSpaces.BorderColor(Color.Aquamarine1);
            allSpaces.Width(70);
            AnsiConsole.Write(allSpaces);

            // Quick bar chart to show free/partially occupied/full spaces

            var barChart = new BarChart()
                //.Width(65)
                .AddItem("[red]FULL SPACES[/]", fullCounter, Color.Red)
                .AddItem("[yellow]PARTIALLY OCCUPIED[/]", halfCounter, Color.Yellow)
                .AddItem("[lime]EMPTY SPACES[/]", emptyCounter, Color.Lime);

            // Put chart in a table for nice format
            Table chartTable = new Table()
                .Width(70)
                .Centered()
                .Border(TableBorder.HeavyEdge)
                .BorderColor(Color.Aquamarine1)
                .AddColumn(new TableColumn(barChart).Centered());

            AnsiConsole.Write(chartTable);

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

// MATRIX FROM V1
//string[,] parkingMatrix = new string[10, 10];
//// Använder en räknare som börjar på 1 (för att p-plats 0 inte ska användas)
//int counter = 1;

//// Lägger in strängarna från vektorn parkingSpaces i matrisen
//for (int i = 0; i < parkingMatrix.GetLength(0); i++)
//{
//    for (int j = 0; j < parkingMatrix.GetLength(1); j++)
//    {
//        // Räknaren används som index på parkingSpaces[] 
//        parkingMatrix[i, j] = parkingSpaces[counter];
//        counter++;
//    }
//}

//// Räknaren börjar om på 1 (för att p-platserna börjar på 1)
//counter = 1;
//for (int i = 0; i < parkingMatrix.GetLength(0); i++)
//{
//    for (int j = 0; j < parkingMatrix.GetLength(1); j++)
//    {
//        if (parkingMatrix[i, j] != null)
//        {
//            // OM det står en bil eller två mc på platsen --> upptagen
//            if (parkingMatrix[i, j].Contains("BIL") || parkingMatrix[i, j].Contains('|'))
//            {
//                Console.ForegroundColor = ConsoleColor.Red;
//                Console.Write("\t" + counter.ToString().PadLeft(4));
//            }
//            // ANNARS OM det står en mc på platsen --> halvfylld
//            else if (parkingMatrix[i, j].Contains("MC") && !(parkingMatrix[i, j].Contains('|')))
//            {
//                Console.ForegroundColor = ConsoleColor.Yellow;
//                Console.Write("\t" + counter.ToString().PadLeft(4));
//            }
//        }
//        // ANNARS: ingen bil eller mc på platsen --> tom
//        else
//        {
//            Console.ForegroundColor = ConsoleColor.Green;
//            Console.Write("\t" + counter.ToString().PadLeft(4));
//        }
//        counter++;
//    }