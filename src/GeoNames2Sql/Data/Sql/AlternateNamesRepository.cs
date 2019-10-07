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
    class AlternateNamesRepository
    {
        private readonly string _connectionString;
        private readonly IOptions<AppSettings> _settings;

        public AlternateNamesRepository(IOptions<AppSettings> settings)
        {
            _connectionString = settings.Value.ConnectionString;
            _settings = settings;
        }

        public async Task SaveAlternateNames()
        {
            Console.WriteLine("Getting ready to populate alternate names...");
            
            var filePath = Path.Combine(_settings.Value.DataDirectory, "alternateNamesV2.txt");

            if (!File.Exists(filePath))
            {
                Console.WriteLine("Downloading alternate names data...");
                var downloader = GeoFileDownloader.CreateGeoFileDownloader();
                downloader.DownloadFile("alternateNamesV2.zip", _settings.Value.DataDirectory);
            }

            var results = GeoFileReader.ReadAlternateNamesV2(filePath)
                .OrderBy(p => p.Id);

            await WriteAlternateNames(results);
        }

        private async Task WriteAlternateNames(IEnumerable<AlternateNameV2> records)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                Console.WriteLine("Populating alternate names...");
                Console.WriteLine();

                const string sql = @"MERGE INTO AlternateNames AS t
                    USING (VALUES (@Id)) AS s(Id) ON (s.Id = t.Id)
                    WHEN MATCHED THEN 
                        UPDATE SET t.Id = @Id, t.GeoNameId = @GeoNameId, t.ISOLanguage = @ISOLanguage, t.AlternateName = @AlternateName,
                            t.IsPreferredName = @IsPreferredName, t.IsShortName = @IsShortName, t.IsColloquial = @IsColloquial,
                            t.IsHistoric = @IsHistoric, t.FromDate = @FromDate, t.ToDate = @ToDate
                    WHEN NOT MATCHED THEN
                        INSERT (Id, GeoNameId, ISOLanguage, AlternateName, IsPreferredName, IsShortName,
                                IsColloquial, IsHistoric, FromDate, ToDate) 
                            VALUES (@Id, @GeoNameId, @ISOLanguage, @AlternateName, @IsPreferredName, @IsShortName,
                                @IsColloquial, @IsHistoric, @FromDate, @ToDate);";

                var command = conn.CreateCommand();
                command.CommandText = sql;

                var parameterNames = new[]
                {
                    "@Id",
                    "@GeoNameId",
                    "@ISOLanguage",
                    "@AlternateName",
                    "@IsPreferredName",
                    "@IsShortName",
                    "@IsColloquial",
                    "@IsHistoric",
                    "@FromDate",
                    "@ToDate"
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
                    if (_settings.Value.GeoNames.AlternateNamesLanguages.Count > 0
                        && !_settings.Value.GeoNames.AlternateNamesLanguages.Contains(r.ISOLanguage))
                        continue;

                    parameters[0].Value = r.Id;
                    parameters[1].Value = r.GeoNameId;
                    parameters[2].Value = r.ISOLanguage.HasValueOrDBNull();
                    parameters[3].Value = r.Name.HasValueOrDBNull();
                    parameters[4].Value = r.IsPreferredName;
                    parameters[5].Value = r.IsShortName;
                    parameters[6].Value = r.IsColloquial;
                    parameters[7].Value = r.IsHistoric;
                    parameters[8].Value = r.From.HasValueOrDBNull();
                    parameters[9].Value = r.To.HasValueOrDBNull();
                    await command.ExecuteNonQueryAsync();
                    Console.WriteLine($"Alternate Name ID: {r.Id}, Language: {r.ISOLanguage}");
                }

                Console.WriteLine();
                Console.WriteLine("Alternate names added to database.");
            }
        }
    }
}