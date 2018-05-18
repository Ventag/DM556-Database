using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Database.Model;
using MongoDB.Driver;
using MongoDB.Bson;
using System.Linq.Expressions;
using System.Data.Linq;

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
                    print_sql_drink("ins", data);
                    break;
                case "ratings":
                    print_sql_rating("ins", data);
                    break;
                case "users":
                    print_sql_user("ins", data);
                    break;
            }

            collection.InsertOne(data);
            print_info("INSERT " + data.GetType() + " INTO " + collection_name);
        }

        public async Task<List<T>> get_data<T>(string collection_name, Expression<Func<T, bool>> filter)
        {
            var collection = database.GetCollection<T>(collection_name);
            switch (collection_name)
            {
                case "drinks":
                    print_sql_drink("sel", filter);
                    break;
                case "ratings":
                    print_sql_rating("sel", filter);
                    break;
                case "users":
                    print_sql_user("sel", filter);
                    break;
            }

            return collection.Find(filter).ToList<T>();
        }

        public async Task rate_helpfullness(string ratingid, bool helpful)
        {
            var rating_collection = database.GetCollection<RatingInfo>("ratings");
            var filter = Builders<RatingInfo>.Filter.Eq(s => s.Id, ratingid);
            UpdateDefinition<RatingInfo> update = null;
            if(helpful)
                update = Builders<RatingInfo>.Update.Inc(s => s.Helpfull, 1);
            else
                update = Builders<RatingInfo>.Update.Inc(s => s.Unhelpfull, 1);
            var result = rating_collection.UpdateOne(filter, update);

            if (result.ModifiedCount > 0)
                print_ok("helpful rating registered");
            else
                print_error("couldn't register helpfull rating");
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

        private void print_sql_drink(string tag, object data)
        {
            var d = data as DrinkInfo;
            switch (tag)
            {
                case "ins":
                    print_info($"INSERT INTO drinks VALUES ('{d.Id}', '{d.UserId}', '{d.Gin}', '{d.Tonic}', '{d.Garnish}', '{d.Description}'");
                    break;
                case "sel":
                    print_info($"SELECT FROM drinks WHERE '{data.ToString()}'");
                    break;
                default:
                    print_info("wrong tag");
                    break;
            }
        }

        private void print_sql_rating(string tag, object data)
        {
            var d = data as RatingInfo; switch (tag)
            {
                case "ins":
                    print_info($"INSERT INTO ratings VALUES ('{d.Id}', '{d.UserId}', '{d.DrinkId}', {d.Rating}, '{d.Comment}', '{d.Helpfull}', '{d.Unhelpfull}')");
                    break;
                case "sel":
                    print_info($"SELECT FROM ratings WHERE '{data.ToString()}'");
                    break;
                default:
                    print_info("wrong tag");
                    break;
            }
        }

        private void print_sql_user(string tag, object data)
        {
            var d = data as UserInfo;
            switch (tag)
            {
                case "ins":
                    print_info($"INSERT INTO users VALUES ('{d.Id}')");
                    break;
                case "sel":
                    print_info($"SELECT FROM users WHERE '{data.ToString()}'");
                    break;
                default:
                    print_info("wrong tag");
                    break;
            }
        }
    }
}