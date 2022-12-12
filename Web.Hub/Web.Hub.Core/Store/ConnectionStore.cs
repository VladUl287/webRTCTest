using Web.Hub.Core.Models;

namespace Web.Hub.Core.Store;

public sealed class ConnectionStore<TKey> where TKey : notnull
{
    private readonly Dictionary<TKey, HashSet<string>> _connections = new();

    public void Add(TKey key, string value)
    {
        lock (_connections)
        {
            HashSet<string> connections;
            if (!_connections.TryGetValue(key, out connections!))
            {
                connections = new HashSet<string>();
                _connections.Add(key, connections);
            }

            connections.Add(value);
        }
    }

    public string[] GetArray(TKey key)
    {
        HashSet<string> connections;
        if (_connections.TryGetValue(key, out connections!))
        {
            return connections.ToArray();
        }

        return Array.Empty<string>();
    }
}