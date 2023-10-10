namespace Chat.EventsBus.Contract;


[AttributeUsage(AttributeTargets.Class)]
public class EventsBusAttribute : Attribute
{
    public readonly string Name;

    public EventsBusAttribute(string name)
    {
        Name = name;
    }
}