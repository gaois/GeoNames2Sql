using Microsoft.Extensions.Options;
using System.Threading.Tasks;

namespace GeoNames2Sql
{
    class GeoNamesService
    {
        private readonly AlternateNamesRepository _alternateNames;
        private readonly CountryInfoRepository _countryInfo;
        private readonly GeoNamesRepository _geoNames;
        private readonly IOptions<AppSettings> _settings;

        public GeoNamesService(
            AlternateNamesRepository alternateNames,
            CountryInfoRepository countryInfo,
            GeoNamesRepository geoNames,
            IOptions<AppSettings> settings)
        {
            _alternateNames = alternateNames;
            _countryInfo = countryInfo;
            _geoNames = geoNames;
            _settings = settings;
        }

        public async Task PerformOperations()
        {
            await _geoNames.SaveGeoNames();

            if (_settings.Value.GeoNames.AlternateNamesLanguages.Count > 0)
                await _alternateNames.SaveAlternateNames();

            if (_settings.Value.GeoNames.CountryInfo)
                await _countryInfo.SaveCountryInfo();
        }
    }
}
