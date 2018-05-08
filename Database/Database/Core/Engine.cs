using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Database.Model;
using MongoDB.Driver;
using MongoDB.Bson;
using Newtonsoft.Json;

namespace Database.Core
{
    class Engine
    {
        static MongoClient client;
        static IMongoDatabase database;

        public enum TABLE
        {
            DRINKS,
            RATING,
            USER
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

        public async Task insert(TABLE type, List<string> data)
        {
            print_info("inserting data into database");
            switch(type)
            {
                case TABLE.DRINKS:
                    DrinkInfo drink = new DrinkInfo
                    {
                        UserId = data.ElementAt(0),
                        Gin = data.ElementAt(1),
                        Tonic = data.ElementAt(2),
                        Garnish = data.ElementAt(3),
                        Description = data.ElementAt(4),
                        Id = data.ElementAt(1) + data.ElementAt(2) + data.ElementAt(3)
                    };

                    var drink_collection = database.GetCollection<DrinkInfo>("drinks");

                    if (search(TABLE.DRINKS, drink.Id))
                    {
                        print_error("drink already exists");
                        return;
                    }

                    await drink_collection.InsertOneAsync(drink);
                    break;

                case TABLE.RATING:
                    RatingInfo rating = new RatingInfo
                    {
                        UserId = data.ElementAt(0),
                        DrinkId = data.ElementAt(1),
                        Rating = int.Parse(data.ElementAt(2)),
                        Comment = data.ElementAt(3),
                        Helpfull = int.Parse(data.ElementAt(4)),
                        Unhelpfull = int.Parse(data.ElementAt(5))
                    };
                    
                    var rating_collection = database.GetCollection<RatingInfo>("ratings");
                    await rating_collection.InsertOneAsync(rating);
                    break;

                case TABLE.USER:

                    break;

                default:
                    print_error("OBI-WAN");
                    break;
            }
        }

        public async void delete()
        {

        }

        private List<string> get_document_info(BsonDocument doc, bool type)
        {
            List<string> result = new List<string>();

            if(type)
            {
                var info = JsonConvert.DeserializeObject<DrinkInfo>(doc.ToJson());
                result.Add("Gin: " + info.Gin);
                result.Add("Tonic: " + info.Tonic);
                result.Add("Garnish: " + info.Garnish);
                result.Add("Description: " + info.Description);
            }
            else
            {
                var info = JsonConvert.DeserializeObject<RatingInfo>(doc.ToJson());
                result.Add("User: " + info.UserId);
                result.Add("Drink ID: " + info.DrinkId);
                result.Add("Rating: " + info.Rating);
                result.Add("User comment: " + info.Comment);
                result.Add("Helpful: " + info.Helpfull);
                result.Add("Unhelpful: " + info.Unhelpfull);
            }

            return result;
        }

        public async Task list_all(bool type)
        {
            var collection = type ? database.GetCollection<BsonDocument>("drinks") : database.GetCollection<BsonDocument>("ratings");
            using (var cursor = await collection.FindAsync(new BsonDocument()))
            {
                print_info("listing all " + (type ? "drinks" : "ratings"));
                while (await cursor.MoveNextAsync())
                {
                    var batch = cursor.Current;
                    foreach (BsonDocument document in batch)
                    {
                        var document_info = get_document_info(document, type);
                        foreach (string s in document_info)
                            Console.WriteLine(s);

                        Console.WriteLine();
                    }
                }
            }
        }

        public async Task list_one()
        {

        }

        public bool search(TABLE type, string key)
        {
            switch (type)
            {
                case TABLE.DRINKS:
                    var drink_collection = database.GetCollection<DrinkInfo>("drinks");
                    if (drink_collection.Find(x => x.Id == key).Any())
                        return true;
                    
                    break;

                case TABLE.RATING:
                    break;

                case TABLE.USER:
                    break;

                default:
                    print_error("OBI-WAN");
                    break;
            }

            return false;
        }

        private void print_ok(string msg)
        {
            Console.Write("[");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write(" ok ");
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("] ");
            Console.Write(msg + "\n");
        }

        private void print_info(string msg)
        {
            Console.Write("[");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("info");
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("] ");
            Console.Write(msg + "\n");
        }

        private void print_error(string msg)
        {
            Console.Write("[");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("erro");
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("] ");
            Console.Write(msg + "\n");
        }
    }
}
