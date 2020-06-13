using Cassandra;
using Cassandra.Mapping;
using System;
using System.Net.Security;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;

namespace Example_Cosmos_DB_Cassandra_API
{
    class Program
    {
        private const string Username = "angelpe-example-cosmos-db-cassandra-api";
        private const string Password = "<PRIMARY KEY>";
        private const string CassandraContactPoint = "angelpe-example-cosmos-db-cassandra-api.cassandra.cosmos.azure.com";
        private static int CassandraPort = 10350;

        public static void Main(string[] args)
        {
            // Connect to cassandra cluster  (Cassandra API on Azure Cosmos DB supports only TLSv1.2)
            var options = new Cassandra.SSLOptions(SslProtocols.Tls12, true, ValidateServerCertificate);
            options.SetHostNameResolver((ipAddress) => CassandraContactPoint);
            Cluster cluster = Cluster.Builder().WithCredentials(Username, Password).WithPort(CassandraPort).AddContactPoint(CassandraContactPoint).WithSSL(options).Build();
            ISession session = cluster.Connect();

            // Creating KeySpace and table
            session.Execute("DROP KEYSPACE IF EXISTS uprofile");
            session.Execute("CREATE KEYSPACE uprofile WITH REPLICATION = { 'class' : 'NetworkTopologyStrategy', 'datacenter1' : 1 };");
            Console.WriteLine(String.Format("created keyspace uprofile"));
            session.Execute("CREATE TABLE IF NOT EXISTS uprofile.user (user_id int PRIMARY KEY, user_name text, user_bcity text)");
            Console.WriteLine(String.Format("created table user"));

            session = cluster.Connect("uprofile");
            IMapper mapper = new Mapper(session);

            // Inserting Data into user table
            mapper.Insert<UserEntity>(new UserEntity(1, "LyubovK", "Dubai"));
            mapper.Insert<UserEntity>(new UserEntity(2, "JiriK", "Toronto"));
            mapper.Insert<UserEntity>(new UserEntity(3, "IvanH", "Mumbai"));
            mapper.Insert<UserEntity>(new UserEntity(4, "LiliyaB", "Seattle"));
            mapper.Insert<UserEntity>(new UserEntity(5, "JindrichH", "Buenos Aires"));
            Console.WriteLine("Inserted data into user table");

            Console.WriteLine("Select ALL");
            Console.WriteLine("-------------------------------");
            foreach (UserEntity user in mapper.Fetch<UserEntity>("Select * from user"))
            {
                Console.WriteLine(user);
            }

            Console.WriteLine("Getting by id 3");
            Console.WriteLine("-------------------------------");
            UserEntity userId3 = mapper.FirstOrDefault<UserEntity>("Select * from user where user_id = ?", 3);
            Console.WriteLine(userId3);

            // Clean up of Table and KeySpace
            session.Execute("DROP table user");
            session.Execute("DROP KEYSPACE uprofile");

            // Wait for enter key before exiting  
            Console.ReadLine();
        }

        public static bool ValidateServerCertificate(
            object sender,
            X509Certificate certificate,
            X509Chain chain,
            SslPolicyErrors sslPolicyErrors)
        {
            if (sslPolicyErrors == SslPolicyErrors.None)
                return true;

            Console.WriteLine("Certificate error: {0}", sslPolicyErrors);
            // Do not allow this client to communicate with unauthenticated servers.
            return false;
        }
    }
}
