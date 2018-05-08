using System;
using System.Collections.Generic;
using Database.Model;
using Database.Core;
using MongoDB.Driver;
using MongoDB.Bson;
using Newtonsoft.Json;

namespace Database
{
    class Program
    {
        static void Main(string[] args)
        {
            var engine = new Database.Core.Engine();
            engine.init();

            engine.list_all(true);
            engine.list_all(false);

            List<string> test = new List<string>();
            test.Add("Simon");
            test.Add("Johhny Walker");
            test.Add("Red Label");
            test.Add("Splash of lemon");
            test.Add("Sure to grant a night you'll never remember");

            engine.insert(Engine.TABLE.DRINKS, test);
            Console.Read();
        }
    }
}
