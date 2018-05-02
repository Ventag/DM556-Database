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
            var client = new MongoClient("mongodb://root(admin):root@ds111410.mlab.com:11410/gintonic");
            Console.WriteLine("dbgs");
            var database = client.GetDatabase("gintonic");
            

            Console.Write("you are mongo gay\n");
            Console.Read();
        }
    }
}
