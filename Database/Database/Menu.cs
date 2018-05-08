using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Database.Model;
using MongoDB.Driver;

namespace Database
{
    public class Menu
    {
        private string _currentUser;

        public Menu()
        {
        }

        public void Show()
        {
            CreateUserOrLogin();
        }

        private void CreateUserOrLogin()
        {
            Console.WriteLine("1. Login");
            Console.WriteLine("2. Create new user");

            while (true)
            {
                int.TryParse(Console.ReadLine(), out var choice);
                switch (choice)
                {
                    case 1:
                        Login();
                        break;
                    case 2:
                        CreateUser();
                        break;
                    default:
                        Console.WriteLine("You have to choose a valid option.");
                        break;
                }
            }
        }

        private void CreateUser()
        {
            Console.Write("Choose a username: ");

            var chosenUsername = Console.ReadLine();

            if (UserNameExists(chosenUsername))
            {
                Console.WriteLine($"\nThe username {chosenUsername} is already taken, please choose a new");
                CreateUser();
            }

            InsertUserName();
            Console.WriteLine($"\nThe user {chosenUsername} has been created\n");
            CreateUserOrLogin();
        }

        private bool UserNameExists(string readLine)
        {
            return true;
        }

        private void InsertUserName()
        {
            return;
        }

        private void Login()
        {
            Console.Write("Username: ");

            var username = Console.ReadLine();

            if (!UserNameExists(username))
            {
                Console.WriteLine($"\nThe username {username} does not exist, please try again");
                Login();
            }

            _currentUser = username;
            Console.WriteLine($"\nLogged in as {username}\n");
            Options();
        }

        private void Options()
        {
            Console.WriteLine("1. Create new gin, tonic or ganish combination");
            Console.WriteLine("2. Create new gin, tonic or ganish combination");
            Console.WriteLine("3. Create new gin, tonic or ganish combination");
            Console.WriteLine("4. Logout");

            int.TryParse(Console.ReadLine(), out var chosenOption);

            switch (chosenOption)
            {
                case 1:
                    break;
                case 2:
                    break;
                case 3:
                    break;
                case 4:
                    Logout();
                    break;
            }
        }

        private void Logout()
        {
            _currentUser = null;
            Console.WriteLine("\nLogged out\n");
            CreateUserOrLogin();
        }
    }
}
