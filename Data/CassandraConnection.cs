using Cassandra;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gestión_Hotelera.Data
{
    public static class CassandraConnection
    {
        private static ISession _session;

        public static ISession GetSession()
        {
            if (_session == null)
            {
                var clusterIp = ConfigurationManager.AppSettings["Cluster"];
                var keyspace = ConfigurationManager.AppSettings["KeySpace"];

                var cluster = Cluster.Builder()
                    .AddContactPoint(clusterIp)
                    .Build();

                _session = cluster.Connect(keyspace);
            }

            return _session;
        }
    }
}
