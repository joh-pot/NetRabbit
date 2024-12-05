namespace NetRabbit.Settings;

public interface IConnectionSettings : IBasicConnectionSettings
{
    public ushort RequestedHeartBeat { get; set; }

}