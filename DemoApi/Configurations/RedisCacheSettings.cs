namespace DemoApi.Configurations
{
    public class RedisCacheSettings
    {
        public string ConnectionStrings { get; set; }
        public TimeSpan ExpirationTime { get; set; }

    }
}
