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
            var engine = new Engine();
            engine.init();


            //engine.list_one(Engine.TABLE.RATING, engine.search(Engine.TABLE.RATING, "Daniel"));
            //engine.list_one(Engine.TABLE.USER, engine.search(Engine.TABLE.USER, "Daniel"));
            
            var menu = new Menu(engine);
            menu.display();
            Console.Read();
        }
    }
}
