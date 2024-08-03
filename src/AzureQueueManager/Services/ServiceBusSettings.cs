namespace AzureQueueManager.Services
{
    public static class ServiceBusSettings
    {
        public static string ConnectionString { get; set; } = "";
        public static string Entity { get; set; } = "";
        public static string Subscription { get; set; } = "";
        public static int PrefetchCount { get; set; } = 1000;
        public static int MessageCount { get; set; } = 100;
        public static int Timeout { get; set; } = 10;
    }
}