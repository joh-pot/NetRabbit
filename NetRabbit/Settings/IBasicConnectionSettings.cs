namespace NetRabbit.Settings;

public interface IBasicConnectionSettings
{
    public string? Username { get; }
    public string? Password { get; }
    public string? ConnectionString { get; }
    public string? VirtualHost { get; }
}