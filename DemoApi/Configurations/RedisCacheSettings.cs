namespace DemoApi.Configurations
{
    public class RedisCacheSettings
    {
        public string ConnectionStrings { get; set; }
        public string ConnectionStringsPep { get; set; }

        public TimeSpan ExpirationTime { get; set; }

    }
}
