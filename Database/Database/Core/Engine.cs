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

        public async void insert(TABLE type, List<string> data)
        {
            switch(type)
            {
                case TABLE.DRINKS:
                    DrinkInfo info = new DrinkInfo();
                    info.Id = data.ElementAt(0);
                    info.Gin = data.ElementAt(1);
                    info.Tonic = data.ElementAt(2);
                    info.Garnish = data.ElementAt(3);
                    info.Description = data.ElementAt(4);
                    break;

                case TABLE.RATING:
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

        public async void list_all(bool type)
        {
            var collection = type ? database.GetCollection<BsonDocument>("drinks") : database.GetCollection<BsonDocument>("ratings");
            using (IAsyncCursor<BsonDocument> cursor = await collection.FindAsync(new BsonDocument()))
            {
                print_info("listing all " + (type ? "drinks" : "ratings"));
                while (await cursor.MoveNextAsync())
                {
                    IEnumerable<BsonDocument> batch = cursor.Current;
                    foreach (BsonDocument document in batch)
                    {
                        List<string> document_info = get_document_info(document, type);
                        foreach (string s in document_info)
                            Console.WriteLine(s);

                        Console.WriteLine();
                    }
                }
            }
        }

        public async void list_one()
        {

        }

        public async void search(string[] keys)
        {

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
