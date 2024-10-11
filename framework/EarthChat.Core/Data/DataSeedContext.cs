using EarthChat.Core.System.Extensions;

namespace EarthChat.Core;

public class DataSeedContext
{
    public string? TenantId { get; set; }

    /// <summary>
    /// Gets/sets a key-value on the <see cref="P:Volo.Abp.Data.DataSeedContext.Properties" />.
    /// </summary>
    /// <param name="name">Name of the property</param>
    /// <returns>
    /// Returns the value in the <see cref="P:Volo.Abp.Data.DataSeedContext.Properties" /> dictionary by given <paramref name="name" />.
    /// Returns null if given <paramref name="name" /> is not present in the <see cref="P:Volo.Abp.Data.DataSeedContext.Properties" /> dictionary.
    /// </returns>
    public object? this[string name]
    {
        get => this.Properties.GetOrDefault<string, object>(name);
        set => this.Properties[name] = value;
    }

    /// <summary>Can be used to get/set custom properties.</summary>
    public Dictionary<string, object?> Properties { get; }

    public DataSeedContext(string? tenantId = null)
    {
        this.TenantId = tenantId;
        this.Properties = new Dictionary<string, object>();
    }

    /// <summary>
    /// Sets a property in the <see cref="P:Volo.Abp.Data.DataSeedContext.Properties" /> dictionary.
    /// This is a shortcut for nested calls on this object.
    /// </summary>
    public virtual DataSeedContext WithProperty(string key, object? value)
    {
        this.Properties[key] = value;
        return this;
    }
}