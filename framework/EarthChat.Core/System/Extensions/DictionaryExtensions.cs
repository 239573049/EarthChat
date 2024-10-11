using System.Collections.Concurrent;
using System.Dynamic;

namespace EarthChat.Core.System.Extensions;

public static class DictionaryExtensions
{
    /// <summary>
    /// This method is used to try to get a value in a dictionary if it does exists.
    /// </summary>
    /// <typeparam name="T">Type of the value</typeparam>
    /// <param name="dictionary">The collection object</param>
    /// <param name="key">Key</param>
    /// <param name="value">Value of the key (or default value if key not exists)</param>
    /// <returns>True if key does exists in the dictionary</returns>
    internal static bool TryGetValue<T>(
        this IDictionary<string, object> dictionary,
        string key,
        out T? value)
    {
        object obj1;
        if (dictionary.TryGetValue(key, out obj1) && obj1 is T obj2)
        {
            value = obj2;
            return true;
        }

        value = default(T);
        return false;
    }

    /// <summary>
    /// Gets a value from the dictionary with given key. Returns default value if can not find.
    /// </summary>
    /// <param name="dictionary">Dictionary to check and get</param>
    /// <param name="key">Key to find the value</param>
    /// <typeparam name="TKey">Type of the key</typeparam>
    /// <typeparam name="TValue">Type of the value</typeparam>
    /// <returns>Value if found, default if can not found.</returns>
    public static TValue? GetOrDefault<TKey, TValue>(
        this Dictionary<TKey, TValue> dictionary,
        TKey key)
        where TKey : notnull
    {
        TValue obj;
        return !dictionary.TryGetValue(key, out obj) ? default(TValue) : obj;
    }

    /// <summary>
    /// Gets a value from the dictionary with given key. Returns default value if can not find.
    /// </summary>
    /// <param name="dictionary">Dictionary to check and get</param>
    /// <param name="key">Key to find the value</param>
    /// <typeparam name="TKey">Type of the key</typeparam>
    /// <typeparam name="TValue">Type of the value</typeparam>
    /// <returns>Value if found, default if can not found.</returns>
    public static TValue? GetOrDefault<TKey, TValue>(
        this IDictionary<TKey, TValue> dictionary,
        TKey key)
    {
        TValue obj;
        return !dictionary.TryGetValue(key, out obj) ? default(TValue) : obj;
    }

    /// <summary>
    /// Gets a value from the dictionary with given key. Returns default value if can not find.
    /// </summary>
    /// <param name="dictionary">Dictionary to check and get</param>
    /// <param name="key">Key to find the value</param>
    /// <typeparam name="TKey">Type of the key</typeparam>
    /// <typeparam name="TValue">Type of the value</typeparam>
    /// <returns>Value if found, default if can not found.</returns>
    public static TValue? GetOrDefault<TKey, TValue>(
        this IReadOnlyDictionary<TKey, TValue> dictionary,
        TKey key)
    {
        TValue obj;
        return !dictionary.TryGetValue(key, out obj) ? default(TValue) : obj;
    }

    /// <summary>
    /// Gets a value from the dictionary with given key. Returns default value if can not find.
    /// </summary>
    /// <param name="dictionary">Dictionary to check and get</param>
    /// <param name="key">Key to find the value</param>
    /// <typeparam name="TKey">Type of the key</typeparam>
    /// <typeparam name="TValue">Type of the value</typeparam>
    /// <returns>Value if found, default if can not found.</returns>
    public static TValue? GetOrDefault<TKey, TValue>(
        this ConcurrentDictionary<TKey, TValue> dictionary,
        TKey key)
        where TKey : notnull
    {
        TValue obj;
        return !dictionary.TryGetValue(key, out obj) ? default(TValue) : obj;
    }

    /// <summary>
    /// Gets a value from the dictionary with given key. Returns default value if can not find.
    /// </summary>
    /// <param name="dictionary">Dictionary to check and get</param>
    /// <param name="key">Key to find the value</param>
    /// <param name="factory">A factory method used to create the value if not found in the dictionary</param>
    /// <typeparam name="TKey">Type of the key</typeparam>
    /// <typeparam name="TValue">Type of the value</typeparam>
    /// <returns>Value if found, default if can not found.</returns>
    public static TValue GetOrAdd<TKey, TValue>(
        this IDictionary<TKey, TValue> dictionary,
        TKey key,
        Func<TKey, TValue> factory)
    {
        TValue obj;
        return dictionary.TryGetValue(key, out obj) ? obj : (dictionary[key] = factory(key));
    }

    /// <summary>
    /// Gets a value from the dictionary with given key. Returns default value if can not find.
    /// </summary>
    /// <param name="dictionary">Dictionary to check and get</param>
    /// <param name="key">Key to find the value</param>
    /// <param name="factory">A factory method used to create the value if not found in the dictionary</param>
    /// <typeparam name="TKey">Type of the key</typeparam>
    /// <typeparam name="TValue">Type of the value</typeparam>
    /// <returns>Value if found, default if can not found.</returns>
    public static TValue GetOrAdd<TKey, TValue>(
        this IDictionary<TKey, TValue> dictionary,
        TKey key,
        Func<TValue> factory)
    {
        return dictionary.GetOrAdd<TKey, TValue>(key, (Func<TKey, TValue>)(k => factory()));
    }

    /// <summary>
    /// Gets a value from the concurrent dictionary with given key. Returns default value if can not find.
    /// </summary>
    /// <param name="dictionary">Concurrent dictionary to check and get</param>
    /// <param name="key">Key to find the value</param>
    /// <param name="factory">A factory method used to create the value if not found in the dictionary</param>
    /// <typeparam name="TKey">Type of the key</typeparam>
    /// <typeparam name="TValue">Type of the value</typeparam>
    /// <returns>Value if found, default if can not found.</returns>
    public static TValue GetOrAdd<TKey, TValue>(
        this ConcurrentDictionary<TKey, TValue> dictionary,
        TKey key,
        Func<TValue> factory)
        where TKey : notnull
    {
        return dictionary.GetOrAdd(key, (Func<TKey, TValue>)(k => factory()));
    }

    /// <summary>
    /// Converts a &lt;string,object&gt; dictionary to dynamic object so added and removed at run
    /// </summary>
    /// <param name="dictionary">The collection object</param>
    /// <returns>If value is correct, return ExpandoObject that represents an object</returns>
    public static object ConvertToDynamicObject(this Dictionary<string, object> dictionary)
    {
        ExpandoObject dynamicObject = new ExpandoObject();
        ICollection<KeyValuePair<string, object>> keyValuePairs =
            (ICollection<KeyValuePair<string, object>>)dynamicObject;
        foreach (KeyValuePair<string, object> keyValuePair in dictionary)
            keyValuePairs.Add(keyValuePair);
        return (object)dynamicObject;
    }
}