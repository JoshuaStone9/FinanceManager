

namespace FM
{
    public static class DatabaseHelper
    {
        public static string BuildConnStr()
        {
            var builder = new SqlConnectionStringBuilder
            {
                DataSource = "STONEYMINI",
                InitialCatalog = "Finance_Manager",
                IntegratedSecurity = true,
                Encrypt = true,
                TrustServerCertificate = true
            };

            return builder.ConnectionString;
        }
    }
}