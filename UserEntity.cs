using System;
using System.Collections.Generic;
using System.Text;

namespace Example_Cosmos_DB_Cassandra_API
{
    public class UserEntity
    {
        public int UserID { get; set; }
        public String Username { get; set; }
        public String City { get; set; }

        public UserEntity(int user_id, String user_name, String user_bcity)
        {
            this.UserID = user_id;
            this.Username = user_name;
            this.City = user_bcity;
        }

        public override String ToString()
        {
            return String.Format(" {0} | {1} | {2} ", UserID, Username, City);
        }
    }
}
