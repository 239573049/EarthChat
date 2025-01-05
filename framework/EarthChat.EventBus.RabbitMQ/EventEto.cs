namespace Token.RabbitMQEvent;

/// <summary>
/// Eto for event.
/// </summary>
public sealed class EventEto
{
    /// <summary>
    /// Gets or sets the full name of the event.
    /// </summary>
    public string FullName { get; set; }

    /// <summary>
    /// Gets or sets the data of the event.
    /// </summary>
    public byte[] Data { get; set; }

    public EventEto(string fullName, byte[] data)
    {
        FullName = fullName;
        Data = data;
    }
}