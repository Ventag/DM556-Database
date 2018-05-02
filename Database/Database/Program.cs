using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Driver;
using MongoDB.Bson;

namespace Database
{
    class Program
    {
        static void Main(string[] args)
        {
            var client = new MongoClient("mongodb://root:root@ds111410.mlab.com:11410/gintonic");
            var database = client.GetDatabase("gintonic");

            //Console.Write(client.ListDatabases());

            Console.WriteLine(database.ListCollections());

            Console.WriteLine("you are mongo gay");
            Console.Read();
        }
    }
}
