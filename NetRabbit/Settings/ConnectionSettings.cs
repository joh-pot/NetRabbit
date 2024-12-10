using System;

namespace NetRabbit.Settings;

public class ConnectionSettings : IConnectionSettings
{
    public ushort RequestedHeartBeat { get; set; } = 10;
    public string? VirtualHost { get; }
    public string? Username { get; }
    public string? Password { get; }
    public string? ConnectionString { get; }

    public ConnectionSettings(IBasicConnectionSettings basicConnectionSettings)
    {
        ArgumentNullException.ThrowIfNull(basicConnectionSettings);
        Username = basicConnectionSettings.Username;
        Password = basicConnectionSettings.Password;
        ConnectionString = basicConnectionSettings.ConnectionString;
        VirtualHost = basicConnectionSettings.VirtualHost;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(VirtualHost, Username, Password, ConnectionString);
    }

    public override bool Equals(object? obj)
    {
        if (obj is not ConnectionSettings settings) return false;
        return
            string.Equals(VirtualHost, settings.VirtualHost, StringComparison.Ordinal) &&
            string.Equals(Username, settings.Username, StringComparison.Ordinal) &&
            string.Equals(Password, settings.Password, StringComparison.Ordinal) &&
            string.Equals(ConnectionString, settings.ConnectionString, StringComparison.Ordinal);
    }
}