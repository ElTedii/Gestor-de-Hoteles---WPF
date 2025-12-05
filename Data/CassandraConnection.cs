using Cassandra;
using System.Configuration;

namespace Gestión_Hotelera.Data
{
    public static class CassandraConnection
    {
        private static ISession _session;

        public static ISession GetSession()
        {
            if (_session != null)
                return _session;

            // Lee de appSettings y aplica defaults si vienen nulos/vacíos
            var clusterIp = ConfigurationManager.AppSettings["Cluster"];
            var keyspace = ConfigurationManager.AppSettings["KeySpace"];

            if (string.IsNullOrWhiteSpace(clusterIp))
                clusterIp = "localhost";       // default seguro

            if (string.IsNullOrWhiteSpace(keyspace))
                keyspace = "hotel_ks";         // tu keyspace

            var cluster = Cluster.Builder()
                                 .AddContactPoint(clusterIp)
                                 .Build();

            _session = cluster.Connect(keyspace);
            return _session;
        }
    }
}