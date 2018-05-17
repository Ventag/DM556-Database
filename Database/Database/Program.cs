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

            var menu = new Menu(engine);
            menu.display();


            DrinkInfo drink = new DrinkInfo
            {
                Id = "GordonSchweppesLime",
                UserId = "Fred",
                Gin = "Gordon",
                Tonic = "Schweppes",
                Garnish = "Lime",
                Description = "Description"
            };

            //engine.insert_data<DrinkInfo>("drinks", drink);


            /*RatingInfo rating = new RatingInfo
            {
                Id = "FredGordonSchweppesLime",
                UserId = "Fred",
                DrinkId = "GordonSchweppesLime",
                Rating = 5,
                Comment = "I like this drink",
                Helpfull = 0,
                Unhelpfull = 0
            };
            engine.insert_data<RatingInfo>("ratings", rating);
            rating = new RatingInfo
            {
                Id = "FredGordonSchweppesLemon",
                UserId = "Fred",
                DrinkId = "GordonSchweppesLemon",
                Rating = 5,
                Comment = "I like this drink alot!",
                Helpfull = 0,
                Unhelpfull = 0
            };
            engine.insert_data<RatingInfo>("ratings", rating);
            rating = new RatingInfo
            {
                Id = "SimonGordonSchweppesLemon",
                UserId = "Simon",
                DrinkId = "GordonSchweppesLemon",
                Rating = 5,
                Comment = "I prefer Red Label...",
                Helpfull = 0,
                Unhelpfull = 0
            };
            engine.insert_data<RatingInfo>("ratings", rating);*/

            /*var items = engine.get_data<RatingInfo>("ratings", t => t.DrinkId.Contains("GordonSchweppes"));
            foreach(var i in items)
            {
                Console.WriteLine(i.DrinkId);
                Console.WriteLine(i.UserId);
                Console.WriteLine(i.Rating);
                Console.WriteLine(i.Comment);
                Console.WriteLine();
            }*/

            /*var objects = engine.get_data<DrinkInfo>("drinks", t => true);
            foreach (var obj in objects)
                Console.WriteLine(obj.UserId);*/

            Console.Read();
        }
    }
}
