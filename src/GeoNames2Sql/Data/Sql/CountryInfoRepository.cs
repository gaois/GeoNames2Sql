using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using NGeoNames;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace GeoNames2Sql
{
    class CountryInfoRepository
    {
        private readonly string _connectionString;
        private readonly IOptions<AppSettings> _settings;

        public CountryInfoRepository(IOptions<AppSettings> settings)
        {
            _connectionString = settings.Value.ConnectionString;
            _settings = settings;
        }

        public async Task SaveCountryInfo()
        {
            Console.WriteLine("Getting ready to populate country info...");

            var filePath = Path.Combine(_settings.Value.DataDirectory, "countryInfo.txt");

            if (!File.Exists(filePath))
            {
                Console.WriteLine("Downloading country info...");
                var downloader = GeoFileDownloader.CreateGeoFileDownloader();
                downloader.DownloadFile("countryInfo.txt", _settings.Value.DataDirectory);
            }

            var results = GeoFileReader.ReadCountryInfo(filePath)
                .OrderBy(p => p.GeoNameId);

            using (var conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                Console.WriteLine("Populating country info...");
                Console.WriteLine();

                const string sql = @"MERGE INTO CountryInfo AS t
                    USING (VALUES (@ISO_Alpha2, @ISO_Alpha3)) AS s(ISO_Alpha2, ISO_Alpha3)
                        ON (s.ISO_Alpha2 = t.ISO_Alpha2 AND s.ISO_Alpha3 = t.ISO_Alpha3)
                    WHEN MATCHED THEN 
                        UPDATE SET t.ISO_Numeric = @ISO_Numeric, t.FIPS = @FIPS, t.Country = @Country, t.Capital = @Capital,
                            t.Area = @Area, t.Population = @Population, t.Continent = @Continent, t.Tld = @Tld,
                            t.CurrencyCode = @CurrencyCode, t.CurrencyName = @CurrencyName, t.Phone = @Phone,
                            t.PostalCodeFormat = @PostalCodeFormat, PostalCodeRegex = @PostalCodeRegex, t.GeoNameId = @GeoNameId,
                            t.EquivalentFipsCode = @EquivalentFipsCode
                    WHEN NOT MATCHED THEN
                        INSERT (ISO_Alpha2, ISO_Alpha3, ISO_Numeric, FIPS, Country, Capital,
                                Area, Population, Continent, Tld, CurrencyCode, CurrencyName, Phone, PostalCodeFormat, 
                                PostalCodeRegex, GeoNameId, EquivalentFipsCode) 
                            VALUES (@ISO_Alpha2, @ISO_Alpha3, @ISO_Numeric, @FIPS, @Country, @Capital,
                                @Area, @Population, @Continent, @Tld, @CurrencyCode, @CurrencyName, @Phone, @PostalCodeFormat, 
                                @PostalCodeRegex, @GeoNameId, @EquivalentFipsCode);";

                var command = conn.CreateCommand();
                command.CommandText = sql;

                var parameterNames = new[]
                {
                    "@ISO_Alpha2",
                    "@ISO_Alpha3",
                    "@ISO_Numeric",
                    "@FIPS",
                    "@Country",
                    "@Capital",
                    "@Area",
                    "@Population",
                    "@Continent",
                    "@Tld",
                    "@CurrencyCode",
                    "@CurrencyName",
                    "@Phone",
                    "@PostalCodeFormat",
                    "@PostalCodeRegex",
                    "@GeoNameId",
                    "@EquivalentFipsCode"
                };

                var parameters = parameterNames.Select(pn =>
                {
                    var parameter = command.CreateParameter();
                    parameter.ParameterName = pn;
                    command.Parameters.Add(parameter);
                    return parameter;
                })
                .ToArray();

                foreach (var r in results)
                {
                    parameters[0].Value = r.ISO_Alpha2;
                    parameters[1].Value = r.ISO_Alpha3;
                    parameters[2].Value = r.ISO_Numeric;
                    parameters[3].Value = r.FIPS;
                    parameters[4].Value = r.Country;
                    parameters[5].Value = r.Capital;
                    parameters[6].Value = r.Area;
                    parameters[7].Value = r.Population;
                    parameters[8].Value = r.Continent;
                    parameters[9].Value = r.Tld;
                    parameters[10].Value = r.CurrencyCode;
                    parameters[11].Value = r.CurrencyName;
                    parameters[12].Value = r.Phone;
                    parameters[13].Value = r.PostalCodeFormat;
                    parameters[14].Value = r.PostalCodeRegex;
                    parameters[15].Value = r.GeoNameId;
                    parameters[16].Value = r.EquivalentFipsCode;
                    await command.ExecuteNonQueryAsync();
                    Console.WriteLine($"Country: {r.Country}");
                }

                Console.WriteLine();
                Console.WriteLine("Country info added to database.");
            }
        }
    }
}