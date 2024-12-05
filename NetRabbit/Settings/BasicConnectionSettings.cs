using System;

namespace NetRabbit.Settings;

public class BasicConnectionSettings : IBasicConnectionSettings
{
    public string? Username { get; set; }
    public string? Password { get; set; }
    public string? ConnectionString { get; set; }
    public string? VirtualHost { get; set; } = "/";
    public void Build(IBasicConnectionSettings settings)
    {
        ArgumentNullException.ThrowIfNull(settings);
        Username = settings.Username;
        Password = settings.Password;
        ConnectionString = settings.ConnectionString;
        VirtualHost = settings.VirtualHost;
    }
}