using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Database.Model;
using MongoDB.Driver;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using System.Linq.Expressions;

namespace Database.Core
{
    public class Engine
    {
        static MongoClient client;
        static IMongoDatabase database;

        public enum TABLE
        {
            DRINKS,
            RATING,
            USER,
            TEST
        };

        public void init()
        {
            client = new MongoClient("mongodb://root:root@ds111410.mlab.com:11410/gintonic");
            database = client.GetDatabase("gintonic");

            var drinks = database.GetCollection<BsonDocument>("drinks");

            var count = drinks.Count(new BsonDocument());
            if (count >= 0)
                print_ok("connected to database");
            else
                print_error("couldn't connect to database");
        }

        public async Task insert_data<T>(string collection_name, T data)
        {
            var collection = database.GetCollection<T>(collection_name);

            switch(collection_name)
            {
                case "drinks":
                    print_sql_drink(data);
                    break;
                case "ratings":
                    print_sql_rating(data);
                    break;
                case "users":
                    print_sql_user(data);
                    break;
            }

            collection.InsertOne(data);
            print_info("INSERT " + data.GetType() + " INTO " + collection_name);
        }

        public List<T> get_data<T>(string collection_name, Expression<Func<T, bool>> filter)
        {
            var collection = database.GetCollection<T>(collection_name);
            return collection.Find(filter).ToList<T>();
        }

        public void print_ok(string msg)
        {
            Console.Write("[");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write(" ok ");
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("] ");
            Console.Write(msg + "\n");
        }

        public void print_info(string msg)
        {
            Console.Write("[");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("info");
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("] ");
            Console.Write(msg + "\n");
        }

        public void print_error(string msg)
        {
            Console.Write("[");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("erro");
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("] ");
            Console.Write(msg + "\n");
        }

        private void print_sql_drink(object data)
        {
            var d = data as DrinkInfo;
            Console.WriteLine($"INSERT INTO drinks VALUES ('{d.Id}', '{d.UserId}', '{d.Gin}', '{d.Tonic}', '{d.Garnish}', '{d.Description}'");
        }

        private void print_sql_rating(object data)
        {
            var d = data as RatingInfo;
            Console.WriteLine($"INSERT INTO ratings VALUES ('{d.Id}', '{d.UserId}', '{d.DrinkId}', {d.Rating}, '{d.Comment}', '{d.Helpfull}', '{d.Unhelpfull}')");
        }

        private void print_sql_user(object data)
        {
            var d = data as UserInfo;
            Console.WriteLine($"INSERT INTO users VALUES ('{d.Id}')");
        }
    }
}