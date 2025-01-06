namespace EarthChat.Infrastructure.Gateway.Tunnels;

internal interface ICloseable
{
    bool IsClosed { get; }
    void Abort();
}