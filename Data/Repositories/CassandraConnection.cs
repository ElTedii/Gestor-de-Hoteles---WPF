using Cassandra;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gestión_Hotelera.Data.Repositories
{
    public sealed class CassandraConnection
    {
        private static readonly Lazy<ISession> _session = new Lazy<ISession>(() =>
        {
            var clusterConfig = ConfigurationManager.AppSettings["Cluster"]; // ej: "localhost"
            var keyspace = ConfigurationManager.AppSettings["KeySpace"];    // ej: "hotel_ks"

            var cluster = Cluster.Builder()
                                 .AddContactPoint(clusterConfig)
                                 .Build();

            return cluster.Connect(keyspace);
        });

        public static ISession Session => _session.Value;
    }
}
