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

            engine.list_all(true);
            engine.list_all(false);

            var hendrick = new List<string>
            {
                "Daniel",
                "Gordon's Dry",
                "Schweppes",
                "Cucumber",
                "An iconic and luxurious combination"
            };

            await engine.insert(Engine.TABLE.DRINKS, hendrick);

            var menu = new Menu();
            menu.Show();
            Console.Read();
        }
    }
}
