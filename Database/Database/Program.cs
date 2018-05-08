using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Database.Model;
using Database.Core;
using MongoDB.Driver;
using MongoDB.Bson;
using Newtonsoft.Json;

namespace Database
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var engine = new Database.Core.Engine();
            engine.init();

            engine.list_all(true);
            engine.list_all(false);

            var test = new List<string>
            {
                "Simon",
                "Johhny Walker",
                "Red Label",
                "",
                "Johhny Walker"
            };
            Console.Read();

            var menu = new Menu();
            menu.Show();
        }
    }
}
