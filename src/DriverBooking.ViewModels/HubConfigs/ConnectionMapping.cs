using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Migrations.Operations;

namespace DriverBooking.Core.HubConfigs
{
    public class ConnectionMapping<T>
    {
        private readonly Dictionary<T, string> _connections = new Dictionary<T, string>();

        public int Count
        {
            get
            {
                return _connections.Count;  
            }
        }

        // Add mapping key -> hashSet of connectionId

        public void Add(T key, string connectionId)
        {
            // Thread safety
            lock(_connections)
            {
                // hold value of connectionIds
                //string connection;
                
                // if key have not have any connection
                if (!_connections.TryGetValue(key, out var connection)) {
                    connection = connectionId;
                    _connections.Add(key, connection);
                }
            }
        }

        // Get connection associated with key
        public string GetConnection(T key)
        {
            string connectionId;
            if (_connections.TryGetValue(key, out connectionId))
            {
                return connectionId;
            }

            return string.Empty;
        }

        // Remove a connectionId from a key
        public void Remove(T key)
        {
            lock(_connections)
            {
                if (_connections.TryGetValue(key, out var connectionId))
                {
                    _connections.Remove(key);
                }
            }
        }
    }
}
