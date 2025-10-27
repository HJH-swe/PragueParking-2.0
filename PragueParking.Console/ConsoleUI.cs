using System;
using System.Collections.Generic;
using Spectre.Console;
using PragueParking.Core;
using PragueParking.Data;
using System.Collections.Specialized;

namespace PragueParking.Console
{
    public class ConsoleUI
    {

        public static void MainMenu(ParkingGarage garage, PriceList priceList, out bool breaker)
        {
            FileManager fileManager = new FileManager();
            breaker = true;
            WritePanel("MAIN MENU", "#ff00ff", "#5fffd7");

            List<string> menuOptions = new List<string>
            {
                "[greenyellow]Park Vehicle[/]\n",
                "[greenyellow]Check Out Vehicle[/]\n",
                "[greenyellow]Search for Vehicle[/]\n",
                "[greenyellow]Move Vehicle[/]\n",
                "[greenyellow]Parking Overview[/]\n",
                "[greenyellow]Reload Price List[/]\n",
                "[greenyellow]Close Prague Parking[/]\n"
            };

            string menuSelect = AnsiConsole.Prompt(new SelectionPrompt<string>()
                .Title("[#ff00ff]\n\nSelect in the menu using the arrow keys[/]")
                .AddChoices(menuOptions)
                );

            string cleanSelect = Markup.Remove(menuSelect).Trim();

            switch (cleanSelect)
            {
                case "Park Vehicle":
                    {
                        Vehicle? vehicleToPark = SelectVehicleType(garage.MCVehicleSize, garage.CarVehicleSize, garage.AllowedVehicles, priceList);
                        if (vehicleToPark == null)
                        {
                            break;
                        }
                        else
                        {
                            bool parked = garage.ParkVehicle(vehicleToPark, out int parkedSpace);
                            if (parked)
                            {
                                AnsiConsole.Write(new Markup("\n[blue]Vehicle successfully parked.[/]\n\n"));
                                AnsiConsole.Write(new Markup(garage.GetParkingSpace(parkedSpace).ToString(), Color.Magenta1));

                                fileManager.SaveParkingData(garage.GetAllSpaces(), "../../../parkingdata.json");
                            }
                            else
                            {
                                AnsiConsole.Write(new Markup("\n[blue]No free space available.[/]\n\n"));
                            }

                            // TODO: Add vehicle to spot, add to list in spot and amend list in garage
                            AnsiConsole.Write(new Markup(vehicleToPark.ToString(), Color.GreenYellow));
                        }
                        break;
                    }
                case "Check Out Vehicle":
                    {
                        string regNumber = CollectRegNumber();
                        ParkingSpace space = garage.FindVehicleSpace(regNumber);
                        if (space == null)
                        {
                            AnsiConsole.Write(new Markup("Error! Vehicle not found.", Color.Blue));
                            break;
                        }
                        Vehicle vehicle = space.FindVehicleInSpace(regNumber);
                        if (vehicle == null)
                        {
                            AnsiConsole.Write(new Markup("Error! Vehicle not found.", Color.Blue));
                            break;
                        }
                        space.RemoveVehicle(vehicle);
                        AnsiConsole.Write(new Markup("\nVehicle successfully checked out.\n", Color.Magenta1));
                        AnsiConsole.Write(new Markup(vehicle.PrintParkingReceipt(), Color.Aquamarine1));

                        //Update parkingdata file here - no earlier in case check out fails
                        fileManager.SaveParkingData(garage.GetAllSpaces(), "../../../parkingdata.json");
                        break;
                    }
                case "Search for Vehicle":
                    {
                        string regNumber = CollectRegNumber();
                        ParkingSpace space = garage.FindVehicleSpace(regNumber);
                        if (space == null)
                        {

                            AnsiConsole.Write(new Markup("Error! Vehicle not found.", Color.Blue));
                            break;

                        }
                        Vehicle vehicle = space.FindVehicleInSpace(regNumber);
                        if (vehicle == null)
                        {
                            AnsiConsole.Write(new Markup("Error! Vehicle not found.", Color.Blue));
                            break;
                        }

                        AnsiConsole.Write(new Markup(vehicle.ToString(), Color.Aquamarine1));
                        break;
                    }
                case "Move Vehicle":
                    {
                        // Find vehicle by regnumber
                        string regNumber = CollectRegNumber();
                        // Save space number vehicle is in - space.SpaceNumber
                        ParkingSpace space = garage.FindVehicleSpace(regNumber);
                        if (space == null)
                        {

                            AnsiConsole.Write(new Markup("Error! Vehicle not found.", Color.Blue));
                            break;

                        }
                        // "Get" vehicle from space
                        Vehicle vehicle = space.FindVehicleInSpace(regNumber);
                        if (vehicle == null)
                        {
                            AnsiConsole.Write(new Markup("Error! Vehicle not found.", Color.Blue));
                            break;
                        }
                        // Try to park in new space - need space number
                        string spaceNumberString = AnsiConsole.Prompt(new TextPrompt<string>("[#ff00ff]\nEnter parking space to move vehicle to:[/]")
                            .AllowEmpty()
                            .Validate(input =>
                            {
                                if (string.IsNullOrWhiteSpace(input))
                                {
                                    return ValidationResult.Error($"[greenyellow]\nError! Please enter a number from 1 to {garage.GarageSize}.[/]");
                                }

                                if (Convert.ToInt32(input) < 1 || Convert.ToInt32(input) > garage.GarageSize)
                                { return ValidationResult.Error($"[greenyellow]\nError! Parking spaces are numbered from 1 to {garage.GarageSize}.[/]"); }

                                return ValidationResult.Success();     // default case
                            })
                            );
                        int spaceNumber = int.Parse(spaceNumberString);
                        ParkingSpace spaceMoveTo = garage.GetParkingSpace(spaceNumber - 1);     // minus 1 for correct index
                        // Try to add the vehicle from above to new parking space
                        bool isParked = spaceMoveTo.AddVehicle(vehicle);
                        if (isParked)
                        {
                            AnsiConsole.Write(new Markup($"\n[aquamarine1]Vehicle successfully moved to space: {spaceMoveTo.SpaceNumber}.[/]\n\n"));
                            AnsiConsole.Write(new Markup(spaceMoveTo.ToString(), Color.GreenYellow));

                            // remove vehicle from original spot
                            space.RemoveVehicle(vehicle);
                            fileManager.SaveParkingData(garage.GetAllSpaces(), "../../../parkingdata.json");
                        }
                        else
                        {
                            AnsiConsole.Write(new Markup($"\n[blue]Unable to move vehicle to space: {spaceMoveTo.SpaceNumber}.[/]\n\n"));
                            // Load data from last save - cancels out incomplete move
                            fileManager.LoadParkingData("../../../parkingdata.json");
                        }
                        break;
                    }
                case "Parking Overview":
                    {
                        // Change later to image overview
                        AnsiConsole.Write(new Markup(garage.ToString(), Color.Aquamarine1));
                        break;
                    }
                case "Reload Price List":
                    {
                        List<string> updateOptions = new List<string>
                        {
                            "[#ff00ff]YES[/]",
                            "[#ff00ff]NO[/]\n\n",
                            "[#ff00ff]Exit to Main Menu[/]"
                        };
                        string updateSelect = AnsiConsole.Prompt(new SelectionPrompt<string>()
                             .Title("[#ff00ff]\n\nDo you want to update prices of parked vehicles?[/]")
                             .AddChoices(updateOptions)
                        );
                        string cleanUpdateSelect = Markup.Remove(updateSelect).Trim();
                        if (cleanUpdateSelect == "YES")
                        {
                            // Update pricelist here, in case user selects "Exit to Main Menu"
                            priceList = LoadPriceList(fileManager);
                            garage.UpdateAllVehiclePrices(priceList);
                            AnsiConsole.Write(new Markup($"\n\nPrices for all parked vehicles have been updated.", Color.Aquamarine1));
                            // Re-save parked vehicles with new prices
                            fileManager.SaveParkingData(garage.GetAllSpaces(), "../../../parkingdata.json");
                        }
                        else if (cleanUpdateSelect == "NO")
                        {
                            // Update pricelist here, in case user selects "Exit to Main Menu"
                            priceList = LoadPriceList(fileManager);
                            AnsiConsole.Write(new Markup($"\n\nNew prices will apply to new parked vehicles.", Color.Aquamarine1));
                        }
                        // No "else" needed. If user selects "Exit to Main Menu" --> break and return to Main Menu
                        break;
                    }
                case "Close Prague Parking":
                    {
                        breaker = false;
                        break;
                    }
                default:
                    {
                        throw new Exception("Unexpected error!");
                    }
            }
            //Remove later - just to check
            //AnsiConsole.Write(garage.ToString());
            AnsiConsole.Console.Input.ReadKey(false);
            AnsiConsole.Clear();
        }



        public static Vehicle? SelectVehicleType(int mcVehicleSize, int carVehicleSize, List<string> allowedVehicles, PriceList pricelist)
        {
            string regNumber = CollectRegNumber();
            List<string> vehicleOptions = new List<string>
            {
                "[#ff00ff]CAR[/]",
                "[#ff00ff]MC[/]",
                "[#ff00ff]BANANA BOAT[/]",
                "[#ff00ff]TANDEM BICYCLE[/]\n\n",
                "[#ff00ff]Exit to Main Menu[/]"
            };
            string vehicleSelect = AnsiConsole.Prompt(new SelectionPrompt<string>()
                .Title("[#ff00ff]\n\nSelect vehicle type using the arrow keys[/]")
                .AddChoices(vehicleOptions)
            );

            string cleanSelect = Markup.Remove(vehicleSelect).Trim();
            switch (cleanSelect)
            {
                case "CAR":
                    {
                        if (allowedVehicles.Contains(cleanSelect))
                        {
                            return new Car(regNumber, carVehicleSize, pricelist.CarVehiclePrice);
                        }
                        AnsiConsole.Write(new Markup($"\n\n{cleanSelect} is not an allowed vehicle.", Color.Aquamarine1));
                        return null;
                    }
                case "MC":
                    {
                        if (allowedVehicles.Contains(cleanSelect))
                        {
                            return new MC(regNumber, mcVehicleSize, pricelist.MCVehiclePrice);
                        }
                        AnsiConsole.Write(new Markup($"\n\n{cleanSelect} is not an allowed vehicle.", Color.Aquamarine1));
                        return null;
                    }
                case "BANANA BOAT":
                    {
                        if (allowedVehicles.Contains(cleanSelect))
                        {
                            return new BananaBoat(regNumber);
                        }
                        AnsiConsole.Write(new Markup($"\n\n{cleanSelect} is not an allowed vehicle.", Color.Aquamarine1));
                        return null;
                    }
                case "TANDEM BICYCLE":
                    {
                        if (allowedVehicles.Contains(cleanSelect))
                        {
                            return new TandemBicycle(regNumber);
                        }
                        AnsiConsole.Write(new Markup($"\n\n{cleanSelect} is not an allowed vehicle.", Color.Aquamarine1));
                        return null;
                    }

                case "Exit to Main Menu":
                    { return null; }
                default:
                    { return null; }

            }

        }
        // Method: user input regnumber
        private static string CollectRegNumber()
        {
            string regNumber = AnsiConsole.Prompt(
                new TextPrompt<string>("[#ff00ff]\nEnter registration number: [/]")
                .AllowEmpty()                                                // Without AllowEmpty() nothing happened if the user just pressed Enter
                                                                             // although, is that better?
                .Validate(input =>                                          // Validation borrowed from Tim Corey
                {
                    input = input.Trim();
                    if (string.IsNullOrWhiteSpace(input))
                    { return ValidationResult.Error("[greenyellow]\nError! Registration number cannot be empty.[/]"); }

                    if (input.Length < 1 || input.Length > 10)
                    { return ValidationResult.Error("[greenyellow]\nError! Invalid registration number.[/]"); }

                    return ValidationResult.Success();     // default case
                })
                );
            // always want reg number in capital letters
            return regNumber.ToUpper();
        }

        //Method to load saved cars, configuration, and initialize all parking spaces
        public static (ParkingGarage garage, PriceList priceList) Initialize()
        {
            // both configuration methods need a FileManager
            var fileManager = new FileManager();

            ParkingGarage garage = InitializeGarage(fileManager);
            PriceList priceList = LoadPriceList(fileManager);

            return (garage, priceList);
        }
        // Method to initialize Parking Garage
        public static ParkingGarage InitializeGarage(FileManager fileManager)
        {
            string parkingPath = "../../../parkingdata.json";
            string configPath = "../../../configuration.json";

            List<ParkingSpace> parkingData = fileManager.LoadParkingData(parkingPath);
            GarageConfiguration config = fileManager.LoadGarageConfiguration(configPath);
            if (config == null)
            {
                WritePanel("ERROR! Could not initialize application!", "#ff0000", "#800000");
                throw new Exception("Could not find configuration file.");
            }
            ParkingGarage garage = new ParkingGarage(parkingData, config.GarageSize, config.AllowedVehicles,
                config.MCVehicleSize, config.CarVehicleSize, config.ParkingSpaceSize);
            // Add check to see if Garage Size is smaller than amount of parking spaces in loaded data
            // TODO: This doesn't catch if there are vehicles parked in spaces removed
            if (garage.GarageSize < parkingData.Count)
            {
                garage.RemoveRangeOfSpaces(garage.GarageSize, parkingData.Count);
            }
            return garage;

        }
        // Moethod to load Price List
        public static PriceList LoadPriceList(FileManager fileManager)
        {
            string priceListPath = "../../../pricelist.txt";
            PriceListConfiguration priceConfig = fileManager.ConfigurePriceList(priceListPath);
            if (priceConfig == null)
            {
                WritePanel("ERROR! Could not load price list!", "#ff0000", "#800000");
                throw new Exception("Could not find price list file.");
            }

            PriceList priceList = new PriceList(priceConfig.PriceList.MCVehiclePrice, priceConfig.PriceList.CarVehiclePrice);

            return priceList;
        }
        private static void WritePanel(string panelText, string textColor, string borderColor)
        {
            Panel menuPanel = new Panel(new Markup($"[{textColor} bold]{panelText}[/]").Centered());
            menuPanel.Border = BoxBorder.Heavy;
            menuPanel.BorderColor(Color.FromHex(borderColor));
            menuPanel.Padding = new(2, 1);
            AnsiConsole.Write(menuPanel);
        }
    }
}