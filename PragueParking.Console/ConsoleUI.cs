using System;
using System.Collections.Generic;
using Spectre.Console;
using PragueParking.Core;
using PragueParking.Data;

namespace PragueParking.Console
{
    public class ConsoleUI
    {

        public static void MainMenu(ParkingGarage garage, out bool breaker)
        {
            FileManager fileManager = new FileManager();
            breaker = true;
            Panel menuPanel = new Panel(new Markup("[greenyellow bold]MAIN MENU[/]").Centered());
            menuPanel.Border = BoxBorder.Heavy;
            menuPanel.BorderColor(Color.Orange1);
            menuPanel.Padding = new(2, 1);
            AnsiConsole.Write(menuPanel);

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
                .Title("[orange1 bold]\n\nSelect in the menu using the arrow keys[/]")
                .AddChoices(menuOptions)
                );

            string cleanSelect = Markup.Remove(menuSelect).Trim();

            switch (cleanSelect)
            {
                case "Park Vehicle":
                    {
                        Vehicle? vehicleToPark = SelectVehicleType();
                        if (vehicleToPark == null)
                        { break; }
                        else
                        {
                            bool parked = garage.ParkVehicle(vehicleToPark, out int parkedSpace);
                            if (parked)
                            {
                                AnsiConsole.Write(new Markup("\n[blue]Vehicle successfully parked.[/]\n\n"));
                                AnsiConsole.Write(new Markup(garage.GetParkingSpace(parkedSpace).ToString(), Color.Orange1));

                                fileManager.SaveParkingData(garage.GetAllSpaces(), "../../../test_parkingdata.json");
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
                        AnsiConsole.Write(new Markup("Vehicle successfully checked out", Color.Orange1));
                        AnsiConsole.Write(new Markup(vehicle.PrintParkingReceipt(), Color.Aquamarine1));
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

                        AnsiConsole.Write(vehicle.ToString());
                        break;
                    }
                case "Move Vehicle":
                    {
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

       

        public static Vehicle? SelectVehicleType()
        {
            string regNumber = CollectRegNumber();
            List<string> vehicleOptions = new List<string>
            {
                "[orange1]CAR[/]",
                "[orange1]MC[/]\n\n",
                "[orange1]Exit to Main Menu[/]"
            };
            string vehicleSelect = AnsiConsole.Prompt(new SelectionPrompt<string>()
                .Title("[orange1 bold]\n\nSelect vehicle type using the arrow keys[/]")
                .AddChoices(vehicleOptions)
            );

            string cleanSelect = Markup.Remove(vehicleSelect).Trim();
            switch (cleanSelect)
            {
                case "CAR":
                    { return new Car(regNumber); }
                case "MC":
                    { return new MC(regNumber); }
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
                new TextPrompt<string>("[orange1]\nEnter registration number: [/]")
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

        //Method to load saved cars and configuration, and initialize all parking spaces
        public static ParkingGarage Initialize()
        {
            var fileManager = new FileManager();
            string parkingPath = "../../../parkingdata.json";
            string configPath = "../../../configuration.json";

            var parkingData = fileManager.LoadParkingData(parkingPath);
            var config = fileManager.LoadConfiguration(configPath);

            int garageSize = config.GarageSize;
            ParkingGarage garage = new ParkingGarage(parkingData, garageSize);
           
            return garage;
        }
        
    }
}