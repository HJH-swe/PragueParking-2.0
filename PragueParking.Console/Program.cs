using System;
using System.IO;
using PragueParking.Core;
using PragueParking.Data;
using Spectre.Console;

namespace PragueParking.Console
{
    public class Program
    {
        private static void Main(string[] args)
        {
           // try
            //{
                var (garage, priceList) = ConsoleUI.Initialize();

                bool breaker = true;
                do
                {
                    ConsoleUI.MainMenu(garage,priceList, out breaker);
                }
                while (breaker);

                AnsiConsole.Write(new Markup("[blue]\n\nPress any key to close.[/]"));
                AnsiConsole.Console.Input.ReadKey(false);
            //}
            //catch (Exception ex)
            //{
            //    AnsiConsole.Write(new Markup($"[blue]Error! {ex.Message}[/]"));
            //    AnsiConsole.Console.Input.ReadKey(false);
            //}
        }
    }
}
