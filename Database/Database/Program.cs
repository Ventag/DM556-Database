using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Driver;

namespace Database
{
    class Program
    {
        static void Main(string[] args)
        {
            var client = new MongoClient("mongodb://root:<root>@dm556-shard-00-00-kjfjr.mongodb.net:27017,dm556-shard-00-01-kjfjr.mongodb.net:27017,dm556-shard-00-02-kjfjr.mongodb.net:27017/test?ssl=true&replicaSet=DM556-shard-0&authSource=admin");
            var database = client.GetDatabase("gintonic");

            //Console.Write(client.ListDatabases());

            Console.WriteLine(database.ListCollections());

            Console.WriteLine("you are mongo gay");
            Console.Read();
        }
    }
}
