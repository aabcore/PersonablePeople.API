using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PersonablePeople.API.Models
{
    public class DatabaseSettings
    {
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
        public string DatabaseUserName { get; set; }
        public string DatabasePassword { get; set; }
        public string LeadsCollectionName { get; set; }
        public string UsersCollectionName { get; set; }
        public string ContactsCollectionName { get; set; }
    }
}
