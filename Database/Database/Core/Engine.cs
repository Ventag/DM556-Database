using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Database.Model;
using MongoDB.Driver;
using MongoDB.Bson;
using Newtonsoft.Json;
using MongoDB.Bson.Serialization;

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
                        Helpfull = 0,
                        Unhelpfull = 0
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

        public async Task list_all_of_type(bool type)
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
                        if (type)
                        {
                            var drink = (BsonSerializer.Deserialize<DrinkInfo>(document));
                            Console.WriteLine(); 
                            Console.WriteLine("User:        " + drink.UserId); 
                            Console.WriteLine("Gin:         " + drink.Gin); 
                            Console.WriteLine("Tonic:       " + drink.Tonic); 
                            Console.WriteLine("Garnish:     " + drink.Garnish); 
                            Console.WriteLine("Description: " + drink.Description); 
                        }
                        else
                        {
                            var rating = (BsonSerializer.Deserialize<RatingInfo>(document));
                            Console.WriteLine(); 
                            Console.WriteLine("User:      " + rating.UserId); 
                            Console.WriteLine("DrinkID:   " + rating.DrinkId); 
                            Console.WriteLine("Rating:    " + rating.Rating); 
                            Console.WriteLine("Comment:   " + rating.Comment); 
                            Console.WriteLine("Helpful:   " + rating.Helpfull); 
                            Console.WriteLine("Unhelpful: " + rating.Unhelpfull); 
                        }
                    }
                }
            }
        }

        public async Task list_one_type(TABLE type, List<object> objects)
        {
            switch(type)
            {
                case TABLE.DRINKS:
                    foreach(var obj in objects)
                    {
                        var drink = (DrinkInfo)obj;
                        Console.WriteLine(); 
                        Console.WriteLine("User:        " + drink.UserId); 
                        Console.WriteLine("Gin:         " + drink.Gin); 
                        Console.WriteLine("Tonic:       " + drink.Tonic); 
                        Console.WriteLine("Garnish:     " + drink.Garnish); 
                        Console.WriteLine("Description: " + drink.Description); 
                    }
                    break;

                case TABLE.RATING:
                    foreach (var obj in objects)
                    {
                        var rating = (RatingInfo)obj;
                        Console.WriteLine(); 
                        Console.WriteLine("User:      " + rating.UserId); 
                        Console.WriteLine("DrinkID:   " + rating.DrinkId); 
                        Console.WriteLine("Comment:   " + rating.Comment); 
                        Console.WriteLine("Helpful:   " + rating.Helpfull); 
                        Console.WriteLine("Unhelpful: " + rating.Unhelpfull); 
                    }
                    break;

                case TABLE.USER:
                    foreach (var obj in objects)
                    {
                        var user = (UserInfo)obj;
                        Console.WriteLine(); 
                        Console.WriteLine("User: " + user.Id); 
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
                    var drink_return = drink_collection.Find(x => x.Id == key).ToList();
                    return new List<object>(drink_return);

                case TABLE.RATING:
                    var rating_collection = database.GetCollection<RatingInfo>("ratings");
                    var rating_return = rating_collection.Find(x => x.UserId == key).ToList();
                    return new List<object>(rating_return);

                case TABLE.USER:
                    var user_collection = database.GetCollection<UserInfo>("users");
                    var user_return = user_collection.Find(x => x.Id == key).ToList();
                    return new List<object>(user_return);

                default:
                    print_error("OBI-WAN");
                    break;
            }

            return new List<object>();
        }

        public List<object> search(TABLE type, string key, string subkey)
        {
            switch (type)
            {
                case TABLE.DRINKS:
                    var drink_collection = database.GetCollection<DrinkInfo>("drinks");
                    List<DrinkInfo> drink_return = null;

                    if(key == "gin")
                        drink_return = drink_collection.Find(x => x.Gin == subkey).ToList();
                    else if(key == "tonic")
                        drink_return = drink_collection.Find(x => x.Tonic == subkey).ToList();
                    else if(key == "garnish")
                        drink_return = drink_collection.Find(x => x.Garnish == subkey).ToList();
                    return new List<object>(drink_return);

                default:
                    print_error("OBI-WAN");
                    break;
            }

            return new List<object>();
        }

        public List<RatingInfo> get_all_ratings()
        {
            var rating_collection = database.GetCollection<RatingInfo>("ratings");
            var rating_return = rating_collection.Find(_ => true).ToList();
            return new List<RatingInfo>(rating_return);
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
    }
}