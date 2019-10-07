namespace GeoNames2Sql
{
    class AppSettings
    {
        public string ConnectionString { get; set; }

        public string DataDirectory { get; set; }

        public GeoNamesSettings GeoNames { get; set; } = new GeoNamesSettings();
    }
}