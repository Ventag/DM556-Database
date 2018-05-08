using System;
using System.Collections.Generic;
using Database.Model;
using MongoDB.Driver;
using MongoDB.Bson;
using Newtonsoft.Json;

namespace Database
{
    class Program
    {
        static void Main(string[] args)
        {
            var client = new MongoClient("mongodb://root:root@ds111410.mlab.com:11410/gintonic");
            var database = client.GetDatabase("gintonic");

            var collection = database.GetCollection<BsonDocument>("drinks");

            var document = collection.Find(new BsonDocument()).First();

            Console.WriteLine(document.ToJson());

            var drink = JsonConvert.DeserializeObject<DrinkInfo>(document.ToJson());

            Console.Read();
        }
    }
}
