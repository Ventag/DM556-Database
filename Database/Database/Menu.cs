using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Database.Model;

namespace Database
{
    public class Menu
    {
        private string current_user;
        private List<string> path;
        private List<string> options;
        private Core.Engine engine;
        string input;

        public Menu(Core.Engine eng)
        {
            path = new List<string>();
            options = new List<string>();
            engine = eng;
            input = "";

            path.Add("main page");
        }

        public async Task display()
        {
            while(true)
            {
                Console.Clear();
                if(current_user == null)
                {
                    await create_user_or_login();
                }
                else
                {
                    main_menu();
                }
            }
        }

        private async Task create_user_or_login()
        {
            reset();
            options.Add("login");
            options.Add("create new user");
            list_options();
            input = Console.ReadLine();
            Console.WriteLine("choice: " + int.Parse(input));
            switch(int.Parse(input))
            {
                case 1:
                    login();
                    break;
                case 2:
                    await create_user();
                    break;
                default:
                    break;
            }
        }

        private async Task create_user()
        {
            reset();
            options.Add("enter new user");
            list_options();
            input = Console.ReadLine();

            if (input == "0")
                display();

            if(does_user_exist(input))
            {
                Console.WriteLine("user already exists");
                System.Threading.Thread.Sleep(500);
                create_user();
            }
            List<string> data = new List<string>();
            data.Add(input);
            await engine.insert(Core.Engine.TABLE.USER, data);
            Console.WriteLine("user \"{0}\" has been created", input);
            current_user = input;
            sleep(500);
        }

        private void login()
        {
            reset();
            header("root > login");
            options.Add("enter username");
            list_options();
           
            input = Console.ReadLine();

            if (input == "0")
                display();

            if (does_user_exist(input))
            {
                current_user = input;
                Console.WriteLine("logged in as {0}", current_user);
            }
            else
            {
                Console.WriteLine("user doesn't exist");
                sleep(500);
                login();
            }

            sleep(500);
        }

        private async Task main_menu()
        {
            reset();
            header("root > main");
            options.Add("create new drink");
            options.Add("rate a drink");
            options.Add("search");
            options.Add("list all ratings");
            options.Add("list my drinks");
            options.Add("list my ratings");
            list_options();
            input = Console.ReadLine();

            switch(int.Parse(input))
            {
                case 0:
                    logout();
                    break;
                case 1:
                    create_drink();
                    break;
                case 2:
                    rate_drink();
                    break;
                case 3:
                    search();
                    break;
                case 4:
                    list_all_ratings();
                    break;
                case 5:
                    list_my_drinks();
                    break;
                case 6:
                    list_my_ratings();
                    break;
                default:
                    break;
            }
        }

        private async Task create_drink()
        {
            reset();
            header("root > main > create_drink");
            options.Add("enter a gin");
            list_options();
            string gin = Console.ReadLine();
            options.Clear();

            if (gin == "0")
                display();
            
            options.Add("enter a tonic");
            list_options();
            string tonic = Console.ReadLine();
            options.Clear();

            options.Add("enter a garnish");
            list_options();
            string garnish = Console.ReadLine();
            options.Clear();

            options.Add("enter a description");
            list_options();
            string description = Console.ReadLine();
            options.Clear();

            Console.WriteLine("adding drink to database");

            List<string> data = new List<string>();
            data.Add(current_user);
            data.Add(gin);
            data.Add(tonic);
            data.Add(garnish);
            data.Add(description);
            await engine.insert(Core.Engine.TABLE.DRINKS, data);

            sleep(500);
        }

        private async Task rate_drink()
        {
            reset();
            header("root > main > rate_drink");
            options.Add("enter drink-id that you wish to rate");
            list_options();
            string drink_id = Console.ReadLine();

            if (drink_id == "0")
                display();

            if (engine.search(Core.Engine.TABLE.DRINKS, drink_id).Count < 0)
            {
                Console.WriteLine("drink does not exist");
                sleep(1000);
                display();
            }

            options.Clear();
            options.Add("enter your rating (number between 1 - 5, anything below or above will be clamped)");
            list_options();
            string rating = Console.ReadLine();
            if (int.Parse(rating) > 5)
                rating = "5";
            else if (int.Parse(rating) < 1)
                rating = "1";
            options.Clear();

            options.Add("enter a comment");
            list_options();
            string comment = Console.ReadLine();
            options.Clear();

            List<string> data = new List<string>();
            data.Add(current_user);
            data.Add(drink_id);
            data.Add(rating);
            data.Add(comment);
            engine.insert(Core.Engine.TABLE.RATING, data);
        }

        private async Task search()
        {
            reset();
            header("root > main > search");

            options.Add("enter type of ingredient (gin, tonic, garnish)");
            list_options();
            
            string ingredient = Console.ReadLine();

            if (ingredient == "0")
                display();

            List<string> whitelist = new List<string> { "gin", "tonic", "garnish"};
            if (!whitelist.Contains(ingredient))
                search();

            options.Clear();
            options.Add("enter an item to search for");
            list_options();
            input = Console.ReadLine();
            
            List<object> objects = engine.search(Core.Engine.TABLE.DRINKS, ingredient, input);

            if (objects.Count < 1)
            {
                Console.WriteLine("\nno drinks found with the given ingredient");
                Console.Read();
                search();
            }

            Console.WriteLine("found [" + objects.Count + "] amount of drinks containing " + input);
            Console.WriteLine("drink id's of drinks containing item listed below\n");

            int counter = 1;
            foreach (var o in objects)
            {
                var drink = (DrinkInfo)o;
                Console.WriteLine("> " + counter + ": " + drink.Id);
            }
            Console.Read();
        }

        private async Task list_all_ratings()
        {
            reset();
            header("root > main > list_all_ratings");
            var ratings = engine.get_all_ratings();
            ratings.Sort(delegate (RatingInfo x, RatingInfo y)
            {
                return y.Helpfull.CompareTo(x.Helpfull);
            });

            if(ratings.Count < 1)
            {
                engine.print_info("there are no ratings");
                Console.Read();
                display();
            }

            int counter = 1;
            foreach(var rating in ratings)
            {
                Console.WriteLine("");
                Console.WriteLine("rating number: " + counter);
                Console.WriteLine("user:      " + rating.UserId);
                Console.WriteLine("drink id:  " + rating.DrinkId);
                Console.WriteLine("rating:    " + rating.Rating);
                Console.WriteLine("comment:   " + rating.Comment);
                Console.WriteLine("helpful:   " + rating.Helpfull);
                Console.WriteLine("unhelpful: " + rating.Unhelpfull);
                counter++;
            }

            options.Add("enter a rating number to rate a drink");
            list_options();

            int drink = int.Parse(Console.ReadLine());

            if (drink == 0)
            {
                display();
                return;
            }

            if(drink > counter)
            {
                engine.print_error("that rating number does not exist, click any key to return");
                Console.Read();
                list_all_ratings();
            }

            options.Clear();
            options.Add("type h to rate it helpful, or u for unhelpful");
            list_options();

            input = Console.ReadLine().ToLower();
            List<string> whitelist = new List<string> { "h", "u"};

            if(!whitelist.Contains(input))
            {
                engine.print_error("input not understood, click any key to return");
                Console.Read();
                list_all_ratings();
            }

            engine.rate_helpfullness(ratings[drink - 1].UserId, ratings[drink - 1].DrinkId, input == "h" ? true : false);

            sleep(500);
        }

        private async Task list_my_drinks()
        {
            reset();
            var objects = engine.search(Core.Engine.TABLE.DRINKS, "user", current_user);
            header("root > main > list_my_drinks");

            if (objects.Count < 1)
            {
                engine.print_info("user has no registered drinks");
                Console.Read();
                display();
            }

            foreach (var o in objects)
            {
                var drink = (DrinkInfo)o;
                Console.WriteLine("");
                Console.WriteLine("drink id: " + drink.Id);
                Console.WriteLine("gin:      " + drink.Gin);
                Console.WriteLine("tonic:    " + drink.Tonic);
                Console.WriteLine("garnish:  " + drink.Garnish);
            }
            
            Console.Read();
        }

        private async Task list_my_ratings()
        {
            reset();
            var objects = engine.search(Core.Engine.TABLE.RATING, current_user);
            header("root > main > list_my_ratings");

            if (objects.Count < 1)
            {
                engine.print_info("user has no registered ratings");
                Console.Read();
                display();
            }

            foreach (var o in objects)
            {
                var rating = (RatingInfo)o;
                Console.WriteLine("");
                Console.WriteLine("drink id: " + rating.DrinkId);
                Console.WriteLine("rating:   " + rating.Rating);
                Console.WriteLine("helpful:  " + rating.Helpfull);
                Console.WriteLine("garnish:  " + rating.Unhelpfull);
            }

            Console.Read();
        }

        private void logout()
        {
            reset();
            options.Clear();
            Console.WriteLine("are you sure you wish to log out? (y/n)");

            input = Console.ReadLine();

            switch(input)
            {
                case "y":
                    current_user = null;
                    Console.WriteLine("\nLogged out\n");
                    sleep(500);
                    create_user_or_login();
                    break;
                case "n":
                    display();
                    break;
                default:
                    Console.WriteLine("input not understood");
                    sleep(500);
                    logout();
                    break;
            }

        }

        private void list_options()
        {
            int count = 0;

            foreach( var o in options )
            {
                Console.WriteLine(count + ". " + o);
                count++;
            }
        }

        private bool does_user_exist(string user)
        {
            if (engine.search(Core.Engine.TABLE.USER, user).Count > 0)
                return true;

            return false;
        }

        private void reset()
        {
            input = "";
            options.Clear();
            Console.Clear();

            options.Add("go back");
        }

        private void sleep(int time)
        {
            System.Threading.Thread.Sleep(time);
        }

        private void header(string path)
        {
            Console.WriteLine("=============================================================");
            Console.WriteLine("User: " + current_user);
            Console.WriteLine("Path: " + path);
            Console.WriteLine("=============================================================");
        }
    }
}