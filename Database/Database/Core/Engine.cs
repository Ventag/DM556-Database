using System;
using System.Collections.Generic;
using System.Linq;
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

                    if (search(TABLE.DRINKS, drink.Id).Count() > 0)
                    {
                        print_error("drink already exists");
                        return;
                    }

                    print_info("adding drink to database");
                    drink_collection.InsertOne(drink);
                    break;

                case TABLE.RATING:
                    RatingInfo rating = new RatingInfo
                    {
                        Id = ObjectId.GenerateNewId(),
                        UserId = data.ElementAt(0),
                        DrinkId = data.ElementAt(1),
                        Rating = int.Parse(data.ElementAt(2)),
                        Comment = data.ElementAt(3),
                        Helpfull = int.Parse(data.ElementAt(4)),
                        Unhelpfull = int.Parse(data.ElementAt(5))
                    };

                    var rating_collection = database.GetCollection<RatingInfo>("ratings");
                    var rating_result = search(TABLE.RATING, rating.UserId);
            
                    foreach(var r in rating_result)
                    {
                        var res = (RatingInfo)r;
                        if(res.DrinkId == rating.DrinkId)
                        {
                            print_error("rating for drink by user already exists");
                            return;
                        }
                    }

                    print_info("adding rating to database");
             
                    rating_collection.InsertOne(rating);
                    break;

                case TABLE.USER:
                    UserInfo user = new UserInfo
                    {
                        Id = data.ElementAt(0)
                    };

                    var user_collection = database.GetCollection<UserInfo>("users");
                    if(search(TABLE.USER, user.Id).Count() > 0)
                    {
                        print_error("user already exists");
                        return;
                    }

                    print_info("adding user to database");
                    user_collection.InsertOne(user);
                    break;

                default:
                    print_error("OBI-WAN");
                    break;
            }
        }

        public async void delete(TABLE type, List<string> keys)
        {
            switch(type)
            {
                case TABLE.DRINKS:
                    var drink_collection = database.GetCollection<DrinkInfo>("drinks");
                    drink_collection.DeleteOne(x => x.Id == keys.ElementAt(0));
                    print_info("deleted drink by id \"" + keys.ElementAt(0) + "\"");
                    break;

                case TABLE.RATING:
                    var rating_collection = database.GetCollection<RatingInfo>("ratings");
                    rating_collection.DeleteOne(x => x.UserId == keys.ElementAt(0) && x.DrinkId == keys.ElementAt(1));
                    print_info("deleted rating for drinkid \"" + keys.ElementAt(1) + "\" by \"" + keys.ElementAt(0) + "\"");
                    break;

                case TABLE.USER:
                    var user_collection = database.GetCollection<UserInfo>("users");
                    user_collection.DeleteOne(x => x.Id == keys.ElementAt(0));
                    print_info("deleted user \"" + keys.ElementAt(0) + "\"");
                    break;

                default:
                    print_error("OBI-WAN");
                    break;
            }
        }

        public async Task rate_helpfullness(string user, string drinkid, bool helpful)
        {
            var rating_collection = database.GetCollection<RatingInfo>("ratings");
            var filter = Builders<RatingInfo>.Filter.Eq(s => s.UserId, user) & Builders<RatingInfo>.Filter.Eq(s => s.DrinkId, drinkid);
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

        public async Task list_one(TABLE type, List<object> objects)
        {
            switch(type)
            {
                case TABLE.DRINKS:
                    foreach(var obj in objects)
                    {
                        var drink = (DrinkInfo)obj;
                        Console.WriteLine(drink.UserId);
                        Console.WriteLine(drink.Gin);
                        Console.WriteLine(drink.Tonic);
                        Console.WriteLine(drink.Garnish);
                        Console.WriteLine(drink.Description);
                    }
                    break;

                case TABLE.RATING:
                    foreach (var obj in objects)
                    {
                        var rating = (RatingInfo)obj;
                        Console.WriteLine(rating.UserId);
                        Console.WriteLine(rating.DrinkId);
                        Console.WriteLine(rating.Comment);
                        Console.WriteLine(rating.Helpfull);
                        Console.WriteLine(rating.Unhelpfull);
                    }
                    break;

                case TABLE.USER:
                    foreach (var obj in objects)
                    {
                        var user = (UserInfo)obj;
                        Console.WriteLine(user.Id);
                    }
                    break;

                default:
                    break;
            }
        }

        public List<object> search(TABLE type, string key)
        {
            switch (type)
            {
                case TABLE.DRINKS:
                    var drink_collection = database.GetCollection<DrinkInfo>("drinks");
                    var drink_return = drink_collection.Find(x => x.Id == key).FirstOrDefault<DrinkInfo>();
                    List<object> drink_list = new List<object>();
                    drink_list.Add(drink_return);
                    return drink_list;

                case TABLE.RATING:
                    var rating_collection = database.GetCollection<RatingInfo>("ratings");
                    var rating_return = rating_collection.Find(x => x.UserId == key).ToList();
                    return new List<object>(rating_return);

                case TABLE.USER:
                    var user_collection = database.GetCollection<UserInfo>("users");
                    var user_return = user_collection.Find(x => x.Id == key).FirstOrDefault<UserInfo>();
                    List<object> user_list = new List<object>();
                    user_list.Add(user_return);
                    break;

                default:
                    print_error("OBI-WAN");
                    break;
            }

            return new List<object>();
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