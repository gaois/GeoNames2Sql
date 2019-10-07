using System.Collections.Generic;

namespace GeoNames2Sql
{
    class GeoNamesSettings
    {
        public bool AllCountries { get; set; }

        public List<string> AlternateNamesLanguages { get; set; } = new List<string>();

        public int? CitiesMinimumPopulation { get; set; }

        public List<string> Countries { get; set; } = new List<string>();

        public bool CountryInfo { get; set; }
    }
}