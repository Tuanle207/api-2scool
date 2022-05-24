namespace Scool.RealTime
{
    public static class RealTimeDbProperties
    {
        public static string DbTablePrefix { get; set; } = "RealTime";

        public static string DbSchema { get; set; } = null;

        public const string ConnectionStringName = "RealTime";
    }
}
