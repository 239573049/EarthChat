using System.Runtime.CompilerServices;

namespace EarthChat.Gateway.Sdk.Tunnels;

public class GatewayConnection : IAsyncDisposable
{
    private readonly Stream _stream;
    private readonly Timer? _keepAliveTimer;
    private readonly TimeSpan _keepAliveTimeout;

    private static readonly string Ping = "PING";
    private static readonly string Pong = "PONG";
    private static readonly ReadOnlyMemory<byte> PingLine = "PING\r\n"u8.ToArray();
    private static readonly ReadOnlyMemory<byte> PongLine = "PONG\r\n"u8.ToArray();

    public GatewayConnection(Stream stream, TimeSpan keepAliveInterval)
    {
        this._stream = stream;

        if (keepAliveInterval > TimeSpan.Zero)
        {
            this._keepAliveTimeout = keepAliveInterval.Add(TimeSpan.FromSeconds(10d));
            this._keepAliveTimer = new Timer(this.KeepAliveTimerTick, null, keepAliveInterval, keepAliveInterval);
        }
        else
        {
            this._keepAliveTimeout = Timeout.InfiniteTimeSpan;
        }
    }

    /// <summary>
    /// 心跳timer
    /// </summary>
    /// <param name="state"></param>
    private async void KeepAliveTimerTick(object? state)
    {
        try
        {
            Console.WriteLine("Send PING");
            await this._stream.WriteAsync(PingLine);
        }
        catch (Exception)
        {
            if (_keepAliveTimer != null)
            {
                await this._keepAliveTimer.DisposeAsync();
            }
        }
    }

    public async IAsyncEnumerable<Guid> ReadTunnelIdAsync([EnumeratorCancellation] CancellationToken cancellationToken)
    {
        using var textReader = new StreamReader(this._stream, leaveOpen: true);
        while (cancellationToken.IsCancellationRequested == false)
        {
            var textTask = textReader.ReadLineAsync(cancellationToken);
            var text = this._keepAliveTimeout <= TimeSpan.Zero
                ? await textTask
                : await textTask.AsTask().WaitAsync(this._keepAliveTimeout, cancellationToken);

            if (text == null)
            {
                yield break;
            }
            else if (text == Ping)
            {
                Console.WriteLine("receive PING");
                await this._stream.WriteAsync(PongLine, cancellationToken);
            }
            else if (text == Pong)
            {
                Console.WriteLine("receive PONG");
            }
            else if (Guid.TryParse(text, out var tunnelId))
            {
                Console.WriteLine("新的隧道Id");
                yield return tunnelId;
            }
            else
            {
                Console.WriteLine($"Unknown news：{text}");
            }
        }
    }

    public ValueTask DisposeAsync()
    {
        this._keepAliveTimer?.Dispose();
        return this._stream.DisposeAsync();
    }
}