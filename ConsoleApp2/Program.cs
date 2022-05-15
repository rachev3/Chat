﻿using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;

namespace ConsoleApp2
{
    internal class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Console.Title = "Chat";
                Console.SetWindowSize(Console.LargestWindowWidth, Console.LargestWindowHeight);
                Console.CancelKeyPress += new ConsoleCancelEventHandler(myHandler);
                Menu();
            }
            catch (Exception ex)
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Red;
                var message = ex.Message;
                CenterText(message, 1);
                Console.WriteLine(ex.Message);
                Thread.Sleep(10000);
                Console.ForegroundColor= ConsoleColor.Gray;
                Menu();

            }
           
        }

        public static void Menu()
        {
            Console.Clear();

            PrintMenuOption();

            ConsoleKeyInfo a = default;
            ConsoleKeyInfo pressedKey;

            do
            {
                Console.SetCursorPosition(0, 0);
                Console.ForegroundColor = ConsoleColor.Black;
                Console.CursorVisible = false;

                pressedKey = Console.ReadKey();
                Console.ForegroundColor = ConsoleColor.Gray;
                //a = Console.ReadKey();
                if(pressedKey.Key == ConsoleKey.D1 || pressedKey.Key == ConsoleKey.D2 || pressedKey.Key == ConsoleKey.D3)
                {
                    a = pressedKey;
                    PrintMenuOption(a.KeyChar-48);
                }
                else if (pressedKey.Key != ConsoleKey.Enter)
                {
                    PrintMenuOption();
                    a = default;
                }
            }
            while (pressedKey.Key != ConsoleKey.Enter);
            //Console.WriteLine("Излязох.");
            if(a == default)
            {
                throw new Exception("Не сте избрали опция.");
            }
            if(a.Key == ConsoleKey.D1)
            {
                CreateAccount();
            }
            if (a.Key == ConsoleKey.D2)
            {
                LogInAccount();
            }
            if (a.Key == ConsoleKey.D3)
            {
                ForgotenPassword();
            }
        }

        public static void CreateAccount()
        {
            using (var db = new ChatContext())
            {
                Console.Clear();
               // Console.WriteLine(db.DbPath); //data baza papka
                var newUser = new User();
                Console.WriteLine("Въведете потребителско име:");
                string username =  Console.ReadLine();
                if (Regex.IsMatch(username, "^[A-Za-z0-9_]{5,35}$") == false)
                {
                    throw new Exception($"Потробителското име трябва да съдържа от 5 до 35 символа и да е с бъкви, цифри или долна черта.");
                }
                newUser.Username = username;
                Console.WriteLine("Въведете парола:");
                string password = null;
                int countChar = 0;
                while (true)
                {
                    
                    var key = Console.ReadKey();
                    if (key.Key == ConsoleKey.Enter)
                    {
                        Console.WriteLine();
                        break;
                    }
                    if(key.Key == ConsoleKey.Backspace && password.Length > 0)
                    {
                        password = password.Remove(password.Length - 1,1);
                        countChar--;               
                        Console.SetCursorPosition(countChar, 3);
                        Console.Write(' ');
                    }
                    else
                    {
                        password += key.KeyChar;
                        Console.SetCursorPosition(countChar, 3);                        
                        Console.Write('*');
                        countChar++;                   
                    }
                }
                newUser.Password = Crypt(password);               
                Console.WriteLine("Въведете име:");
                newUser.Names = Console.ReadLine();
                db.Users.Add(newUser);
                db.SaveChanges();

                // var user = db.Users.AsEnumerable().LastOrDefault();

                // Console.WriteLine(newUser.Id);
                Console.Clear();
                string congrat = $"Успешно създадохте вашият акаунт. Вашият идентификационен номер е: {newUser.Id}";
                CenterText(congrat, 1);
                Console.WriteLine(congrat);
                Thread.Sleep(5000);
                Menu();
            }
        }
        public static void LogInAccount()
        {
             using(var db = new ChatContext())
            {
                Console.Clear();

                string f = "Потребител:";

                CenterText(f, 5, 1);
                Console.WriteLine(f);
                CenterText(f, 5, 2);
                Console.WriteLine("".PadRight(f.Length,'_'));

                CenterText(f, 5, 4);
                Console.WriteLine("Парола:");
                CenterText(f, 5, 5);
                Console.WriteLine("".PadRight(f.Length, '_'));
                CenterText(f, 5, 2);

                string username = Console.ReadLine();
                CenterText(f, 5, 5);
                string password = Console.ReadLine();
               
                
                var a = db.Users.Where(user => user.Username == username && user.Password == Crypt(password)).ToArray();
               
                if(a.Length >= 1)
                {
                    Console.Clear();
                    Console.CursorVisible = false;
                    string fText = "Влязохте в акаунта си.";
                    CenterText(fText, 1);
                    Console.WriteLine(fText);
                    Thread.Sleep(2000);
                    //Console.Clear();
                    string welcome = $"Добре дошли, {a[0].Names}!";
                    
                    int b = fText.Length > welcome.Length ? fText.Length : welcome.Length;                  
                    CenterText(welcome, 1);
                    Console.WriteLine(welcome.PadRight(b));

                    Thread.Sleep(5000);
                    Menu();

                }
                else
                {
                    Console.Clear();

                    string text = "Бъркате потребителско име или парола.";
                    CenterText(text, 4, 1);
                    Console.WriteLine(text);

                    CenterText(text, 4, 2);
                    Console.WriteLine("-------------------------------------");

                    CenterText(text, 4, 3);
                    Console.WriteLine("2) Забравих паролата си.");

                    CenterText(text, 4, 4);
                    Console.WriteLine("Backspace) Опитай пак.");
                    
                    ConsoleKeyInfo click;

                    do
                    {
                        click = Console.ReadKey();
                    }
                    while (click.Key != ConsoleKey.Backspace && click.Key != ConsoleKey.D2);

                    if(click.Key == ConsoleKey.D2)
                    {
                        ForgotenPassword();
                    }
                    if(click.Key == ConsoleKey.Backspace)
                    {
                        Console.WriteLine();
                        LogInAccount();
                    }
                }
           }   
        }
        public static void ForgotenPassword()
        {
            using (var db = new ChatContext())
            {
                Console.Clear();
                Console.WriteLine("Въведете потребителскот си име:");
                string username = Console.ReadLine();
                Console.WriteLine("Въведете вашето ID:");
                int id = int.Parse(Console.ReadLine());

                var a = db.Users.Where(user => user.Username == username && user.Id == id).ToArray();
                if (a.Length == 0)
                {
                    Console.WriteLine();
                    Console.WriteLine("Объркахте нещо.");
                }
                else
                {
                    Console.WriteLine($"Вашата парола е: {Decrypt(a[0].Password)}");
                    Thread.Sleep(5000);
                    Menu();
                }
            }
        }

        public static void CenterText(string text, int height, int rows = 0)
        {
            Console.SetCursorPosition((Console.WindowWidth - text.Length) / 2, (Console.WindowHeight - height) / 2 + rows);
        }
        protected static void myHandler(object sender, ConsoleCancelEventArgs args)
        {
            Console.Clear();
            string a = "Довиждане!";
            CenterText(a, 1);
            Console.WriteLine(a);   
            args.Cancel = true;
            Thread.Sleep(4000);
            Environment.Exit(0);
        }
        public static void PrintMenuOption(int a = -1)
        {
            string optionOne = "1) Създаване на потребител";
            string optionTwo = "2) Вход на потребител";
            string optionThree = "3) Забравена парола";

            var defaultColor = ConsoleColor.Gray;
            var selectedColor = ConsoleColor.Cyan;

            Console.ForegroundColor = a == 1 ? selectedColor : defaultColor;
            CenterText(optionOne, 5, 1);
            Console.WriteLine(optionOne);

            Console.ForegroundColor = a == 2 ? selectedColor : defaultColor;
            CenterText(optionOne, 5, 2);
            Console.WriteLine(optionTwo);

            Console.ForegroundColor = a == 3 ? selectedColor : defaultColor;
            CenterText(optionOne, 5, 3);
            Console.WriteLine(optionThree);

            Console.ForegroundColor = defaultColor;
        }
        public static string Crypt(string pass)
        {
            char[] chars = pass.ToCharArray();
            char[] result = new char[chars.Length];
            int counter = 0;
            foreach(char c in chars)
            {
                int a = c;
                result[counter] = (char)(a+6);
                counter++;
            }
            return new string(result);
        }
        public static string Decrypt(string pass)
        {
            char[] chars = pass.ToCharArray();
            char[] result = new char[chars.Length];
            int counter = 0;
            foreach (char c in chars)
            {
                int a = c;
                result[counter] = (char)(a - 6);
                counter++;
            }
            return new string(result);
        }
    }
}