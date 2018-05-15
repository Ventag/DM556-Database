using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Database.Model;
using Database.Core;
using MongoDB.Driver;
using MongoDB.Bson;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace Database
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Start();
        }

        public static async Task Start()
        {
            var engine = new Database.Core.Engine();
            engine.init();

            List<string> data = new List<string>
            {
                "Daniel",
                "Simon",
                "0",
                "He stupeed",
                "1",
                "-1"
            };

            await engine.insert(Engine.TABLE.RATING, data);

            await engine.rate_helpfullness("Daniel1", "Simon", true);
            //search(TABLE.RATING, rating.DrinkId).Count() > 0)
            //await engine.list_one(Engine.TABLE.RATING, engine.search(Engine.TABLE.RATING, "user"));
            
            //var menu = new Menu();
            //menu.Show();
            Console.Read();
        }
    }
}
