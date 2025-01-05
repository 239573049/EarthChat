using System.ComponentModel;
using System.Reflection;

namespace EarthChat.Core;

public static class AttributeExtensions
{
    public static string? GetDescription(this Enum value)
    {
        var type = value.GetType();
        var name = Enum.GetName(type, value);
        if (name == null)
        {
            return null;
        }

        var field = type.GetField(name);
        if (field == null)
        {
            return null;
        }

        var attr = Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) as DescriptionAttribute;
        return attr?.Description;
    }

    public static string? GetDescription(this Type type)
    {
        var attr = Attribute.GetCustomAttribute(type, typeof(DescriptionAttribute)) as DescriptionAttribute;
        return attr?.Description;
    }

    public static string? GetDescription(this PropertyInfo property)
    {
        var attr = Attribute.GetCustomAttribute(property, typeof(DescriptionAttribute)) as DescriptionAttribute;
        return attr?.Description;
    }
}