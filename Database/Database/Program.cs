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

            await engine.rate_helpfullness("Daniel1", "Simon", true);
            
            var menu = new Menu();
            menu.Show();
            Console.Read();
        }
    }
}
