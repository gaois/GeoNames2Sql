using Ansa.Extensions;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using NGeoNames;
using NGeoNames.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace GeoNames2Sql
{
    class GeoNamesRepository
    {
        private readonly string _connectionString;
        private readonly IOptions<AppSettings> _settings;

        public GeoNamesRepository(IOptions<AppSettings> settings)
        {
            _connectionString = settings.Value.ConnectionString;
            _settings = settings;
        }

        public async Task SaveGeoNames()
        {
            Console.WriteLine("Getting ready to populate GeoNames...");

            if (_settings.Value.GeoNames.AllCountries)
            {
                var filePath = Path.Combine(_settings.Value.DataDirectory, "allCountries.txt");

                if (!File.Exists(filePath))
                {
                    Console.WriteLine("Downloading GeoNames data for all countries...");
                    var downloader = GeoFileDownloader.CreateGeoFileDownloader();
                    downloader.DownloadFile("allCountries.zip", _settings.Value.DataDirectory);
                }

                var results = GeoFileReader.ReadExtendedGeoNames(filePath)
                    .OrderBy(p => p.Id);

                await WriteGeoNames(results);
            }

            if (_settings.Value.GeoNames.Countries.Count > 0)
            {
                foreach (var countryCode in _settings.Value.GeoNames.Countries)
                {
                    var code = countryCode.ToUpper();
                    var filePath = Path.Combine(_settings.Value.DataDirectory, $"{code}.txt");

                    if (!File.Exists(filePath))
                    {
                        Console.WriteLine($"Downloading GeoNames data (Country: {code})...");
                        var downloader = GeoFileDownloader.CreateGeoFileDownloader();
                        downloader.DownloadFile($"{code}.zip", _settings.Value.DataDirectory);
                    }

                    var results = GeoFileReader.ReadExtendedGeoNames(filePath)
                        .OrderBy(p => p.Id);

                    await WriteGeoNames(results);
                }
            }

            if (_settings.Value.GeoNames.CitiesMinimumPopulation is int population)
            {
                var fileName = default(string);

                switch (population)
                {
                    case 1000:
                        fileName = "cities1000";
                        break;
                    case 5000:
                        fileName = "cities5000";
                        break;
                    case 15000:
                        fileName = "cities15000";
                        break;
                    default:
                        fileName = "cities15000";
                        break;
                }

                var filePath = Path.Combine(_settings.Value.DataDirectory, $"{fileName}.txt");

                if (!File.Exists(filePath))
                {
                    Console.WriteLine($"Downloading GeoNames data for cities with a population of {population} or greater...");
                    var downloader = GeoFileDownloader.CreateGeoFileDownloader();
                    downloader.DownloadFile($"{fileName}.zip", _settings.Value.DataDirectory);
                }

                var results = GeoFileReader.ReadExtendedGeoNames(filePath)
                    .OrderBy(p => p.Id);

                await WriteGeoNames(results);
            }
        }

        private async Task WriteGeoNames(IEnumerable<ExtendedGeoName> records)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                Console.WriteLine("Populating GeoNames...");
                Console.WriteLine();

                const string sql = @"MERGE INTO GeoNames AS t
                    USING (VALUES (@Id)) AS s(Id) ON (s.Id = t.Id)
                    WHEN MATCHED AND (@ModificationDate > t.ModificationDate) THEN
                        UPDATE SET t.Name = @Name, t.NameASCII = @NameASCII, t.Latitude = @Latitude, t.Longitude = @Longitude,
                            t.FeatureClass = @FeatureClass, t.FeatureCode = @FeatureCode, t.CountryCode = @CountryCode,
                            t.Population = @Population, t.Elevation = @Elevation, t.Dem = @Dem, t.Timezone = @Timezone,
                            t.ModificationDate = @ModificationDate
                    WHEN NOT MATCHED THEN
                        INSERT (Id, Name, NameASCII, Latitude, Longitude, FeatureClass, FeatureCode, CountryCode, Population, Elevation, Dem, Timezone, ModificationDate) 
                        VALUES (@Id, @Name, @NameASCII, @Latitude, @Longitude, @FeatureClass, @FeatureCode, @CountryCode, @Population, @Elevation, @Dem, @Timezone, @ModificationDate);";

                var command = conn.CreateCommand();
                command.CommandText = sql;

                var parameterNames = new[]
                {
                    "@Id",
                    "@Name",
                    "@NameASCII",
                    "@Latitude",
                    "@Longitude",
                    "@FeatureClass",
                    "@FeatureCode",
                    "@CountryCode",
                    "@Population",
                    "@Elevation",
                    "@Dem",
                    "@Timezone",
                    "@ModificationDate"
                };

                var parameters = parameterNames.Select(pn =>
                {
                    var parameter = command.CreateParameter();
                    parameter.ParameterName = pn;
                    command.Parameters.Add(parameter);
                    return parameter;
                })
                .ToArray();

                foreach (var r in records)
                {
                    parameters[0].Value = r.Id;
                    parameters[1].Value = r.Name.HasValueOrDBNull();
                    parameters[2].Value = r.NameASCII.HasValueOrDBNull();
                    parameters[3].Value = r.Latitude;
                    parameters[4].Value = r.Longitude;
                    parameters[5].Value = r.FeatureClass.HasValueOrDBNull();
                    parameters[6].Value = r.FeatureCode.HasValueOrDBNull();
                    parameters[7].Value = r.CountryCode.HasValueOrDBNull();
                    parameters[8].Value = r.Population;
                    parameters[9].Value = r.Elevation.HasValueOrDBNull();
                    parameters[10].Value = r.Dem;
                    parameters[11].Value = r.Timezone.HasValueOrDBNull();
                    parameters[12].Value = r.ModificationDate;
                    await command.ExecuteNonQueryAsync();
                    Console.WriteLine($"GeoName ID: {r.Id}, Name: {r.Name}");
                }

                Console.WriteLine();
                Console.WriteLine("GeoNames added to database.");
            }
        }
    }
}