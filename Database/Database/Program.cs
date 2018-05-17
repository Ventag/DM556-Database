using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Database.Model;
using Database.Core;
using MongoDB.Driver;
using MongoDB.Bson;
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

            /*var menu = new Menu(engine);
            menu.display();*/


            DrinkInfo drink = new DrinkInfo
            {
                Id = "GinTonicGarnish",
                UserId = "Fred",
                Gin = "Gin",
                Tonic = "Tonic",
                Garnish = "Garnish",
                Description = "Description"
            };

            engine.insert_data<DrinkInfo>("drinks", drink);

            Console.Read();
        }
    }
}
