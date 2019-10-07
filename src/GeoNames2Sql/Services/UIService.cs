using Ansa.Extensions;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;

namespace GeoNames2Sql
{
    class UIService
    {
        private readonly GeoNamesService _geoNames;
        private readonly IOptions<AppSettings> _settings;

        public UIService(
            GeoNamesService geoNames,
            IOptions<AppSettings> settings)
        {
            _geoNames = geoNames;
            _settings = settings;
        }

        public async Task Start()
        {
            Console.WriteLine("Fáilte! Welcome!");
            Console.WriteLine();

            var validConfig = ValidateConfiguration();

            if (!validConfig)
                return;

            var ready = ConfirmOperations();

            if (!ready)
                return;

            await _geoNames.PerformOperations();

            Finish();
        }

        private bool ValidateConfiguration()
        {
            Console.WriteLine("Checking your configuration...");

            var isValid = true;

            if (_settings.Value.ConnectionString.IsNullOrWhiteSpace())
            {
                isValid = false;
                WriteWarning("You must provide a database connection string.");
            }
            
            if (_settings.Value.DataDirectory.IsNullOrWhiteSpace())
            {
                isValid = false;
                WriteWarning("You must specify a directory in which downloaded GeoNames data files will be stored.");
            }

            if (_settings.Value.GeoNames.CitiesMinimumPopulation is int population)
            {
                if (population != 500 && population != 1000 && population != 5000 && population != 15000)
                {
                    isValid = false;
                    WriteWarning("You have specified an invalid minimum city population for GeoNames data. The value must be either 500, 1000, 5000, or 15000.");
                }
            }

            if (isValid)
            {
                Console.WriteLine("Configuration is valid.");
            }
            else
            {
                Console.WriteLine("Configuration is invalid. Please check your configuration and restart the program.");
            }
            
            Console.WriteLine();

            return isValid;
        }

        private bool ConfirmOperations()
        {
            Console.WriteLine("GeoNames2Sql will perform the following operations:");
            Console.WriteLine();

            var settings = _settings.Value.GeoNames;

            if (settings.AllCountries)
                Console.WriteLine("- GeoNames data will be saved for all countries.");

            if (settings.Countries.Count > 0)
                Console.WriteLine($"- GeoNames data will be saved for the following countries: {string.Join(", ", settings.Countries)}");

            if (settings.CitiesMinimumPopulation is int population)
                Console.WriteLine($"- GeoNames data will be saved for all cities with a population of {population} or more.");

            if (settings.AlternateNamesLanguages.Count > 0)
            {
                var languages = string.Join(", ", settings.AlternateNamesLanguages);
                Console.WriteLine($"- Alternate names data will be saved for the following languages: {languages}");
            }

            if (settings.CountryInfo)
                Console.WriteLine("- Country info data will be saved.");

            Console.WriteLine();
            Console.Write("Are you ready to proceed? (Y/N) ");

            var input = Console.ReadLine().Trim();

            if (input == "Y")
            {
                Console.WriteLine();
                return true;
            }
            else
            {
                Console.WriteLine();
                return false;
            }
        }

        private void Finish()
        {
            Console.WriteLine();
            Console.WriteLine("Finished!");
        }

        private void WriteWarning(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("WARNING: ");
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write($"{message}\n");
        }
    }
}
