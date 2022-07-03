using System;
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
                Console.SetWindowSize(Console.LargestWindowWidth, Console.LargestWindowHeight);  //33,137
                Console.CancelKeyPress += new ConsoleCancelEventHandler(MyHandler);
                Menu();
            }
            catch (Exception ex)
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Red;
                var message = ex.Message;
                CenterText(message, 1);
                Console.WriteLine(ex.Message);
                Thread.Sleep(6000);
                Console.ForegroundColor = ConsoleColor.Gray;
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

                if (pressedKey.Key == ConsoleKey.D1 || pressedKey.Key == ConsoleKey.D2 || pressedKey.Key == ConsoleKey.D3)
                {
                    a = pressedKey;
                    PrintMenuOption(a.KeyChar - 48);
                }
                else if (pressedKey.Key != ConsoleKey.Enter)
                {
                    PrintMenuOption();
                    a = default;
                }
            }
            while (pressedKey.Key != ConsoleKey.Enter);

            if (a == default)
            {
                throw new Exception("Не сте избрали опция.");
            }
            if (a.Key == ConsoleKey.D1)
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
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.CursorVisible = true;
                Console.Clear();
                //Console.WriteLine(db.DbPath); //data baza papka  / C:\Users\User\AppData\Local\blogging.db
                var newUser = new User();
                Console.WriteLine("Въведете потребителско име:");
                string username = Console.ReadLine();
                var a = db.Users.Where(user => user.Username == username).ToArray();
                if (Regex.IsMatch(username, "^[A-Za-z0-9_]{5,35}$") == false)
                {
                    throw new Exception($"Потробителското име трябва да съдържа от 5 до 35 символа и да е с букви, цифри или долна черта.");
                }
                else if (a.Length > 0)
                {
                    Console.WriteLine();
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"Потребителското име е заето.");
                    Thread.Sleep(1500);
                    CreateAccount();
                }
                newUser.Username = username;

                Console.WriteLine("Въведете парола:");               
                string password = PasswordCover(0, 3, " ");               
                newUser.Password = Crypt(password);

                Console.WriteLine("Въведете име:");
                newUser.Names = Console.ReadLine();

                db.Users.Add(newUser);
                db.SaveChanges();

                // var user = db.Users.AsEnumerable().LastOrDefault();

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
            using (var db = new ChatContext())
            {
                Console.CursorVisible = true;
                Console.Clear();

                string f = "Потребител:";

                CenterText(f, 5, 1);
                Console.WriteLine(f);
                CenterText(f, 5, 2);
                Console.WriteLine("".PadRight(f.Length, '_'));

                CenterText(f, 5, 4);
                Console.WriteLine("Парола:");
                CenterText(f, 5, 5);
                Console.WriteLine("".PadRight(f.Length, '_'));

                CenterText(f, 5, 2);
                string username = UsernameInput((Console.WindowWidth - f.Length) / 2, 16,"_");
                
                
                CenterText(f, 5, 5);
                string password = PasswordCover((Console.WindowWidth - f.Length) / 2, 19, "_");
                
                var a = db.Users.Where(user => user.Username == username && user.Password == Crypt(password)).ToArray();

                if (a.Length >= 1)
                {
                    Console.Clear();
                    Console.CursorVisible = false;
                    string fText = $" Влязохте в акаунта си.";
                    CenterText(fText, 1);
                    Console.WriteLine(fText);
                    Thread.Sleep(2000);
                    string welcome = $"Добре дошли, {a[0].Names}!";

                    int b = fText.Length > welcome.Length ? fText.Length : welcome.Length;
                    CenterText(welcome, 1);
                    Console.WriteLine(welcome.PadRight(b).PadLeft(b));

                    Thread.Sleep(5000);

                    Account(a[0]);
                }
                else
                {
                    Console.Clear();
                    PrintLoginMistakeOption();

                    ConsoleKeyInfo click = default;
                    ConsoleKeyInfo pressedKey;

                    do
                    {
                        Console.SetCursorPosition(0, 0);
                        Console.ForegroundColor = ConsoleColor.Black;
                        Console.CursorVisible = false;

                        pressedKey = Console.ReadKey();
                        Console.ForegroundColor = ConsoleColor.Gray;

                        if(pressedKey.Key == ConsoleKey.D1 || pressedKey.Key == ConsoleKey.Backspace)
                        {
                            click = pressedKey;
                            PrintLoginMistakeOption(click.KeyChar - 48);
                        }
                        else if (pressedKey.Key != ConsoleKey.Enter)
                        {
                            PrintLoginMistakeOption();
                            click = default;
                        }
                    }
                    while (pressedKey.Key != ConsoleKey.Enter);

                    if (a == default)
                    {
                        throw new Exception("Не сте избрали опция.");
                    }
                    if (click.Key == ConsoleKey.D1)
                    {
                        ForgotenPassword();
                    }
                    if (click.Key == ConsoleKey.Backspace)
                    {
                        Console.WriteLine();
                        LogInAccount();
                    }
                }
            }
        }
        public static void Account(User user)
        {
            using (var db = new ChatContext())
            {
                Console.Clear();
                Console.SetCursorPosition(0, 0);
                Console.WriteLine(user.Username);
                Console.SetCursorPosition(0,31); //130,31
                Console.WriteLine($"{DateTime.Today}");
                                                
                PrintAccountOptions();       
                ConsoleKeyInfo a = default;
                ConsoleKeyInfo pressedKey;

                do
                {
                    Console.SetCursorPosition(13, 0);
                    Console.ForegroundColor = ConsoleColor.Black;
                    Console.CursorVisible = false;

                    pressedKey = Console.ReadKey();
                    Console.ForegroundColor = ConsoleColor.Gray;

                    if (pressedKey.Key == ConsoleKey.D1 || pressedKey.Key == ConsoleKey.D2 || pressedKey.Key == ConsoleKey.D0)
                    {
                        a = pressedKey;
                        PrintAccountOptions(a.KeyChar - 48);
                    }
                    else if(pressedKey.Key != ConsoleKey.Enter)
                    {
                        PrintAccountOptions();
                        a = default;
                    }
                }
                while (pressedKey.Key != ConsoleKey.Enter);

                if(a == default)
                {
                    throw new Exception("Не сте избрали опция.");
                }
                if(a.Key == ConsoleKey.D0)
                {
                    ChangePassword();
                }
            }
        }
        public static void ForgotenPassword()
        {
            using (var db = new ChatContext())
            {
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.CursorVisible = true;
                Console.Clear();
                Console.WriteLine("Въведете потребителскотo си име:");
                string username = Console.ReadLine();
                Console.WriteLine("Въведете вашето ID:");
                int id = int.Parse(Console.ReadLine());

                var a = db.Users.Where(user => user.Username == username && user.Id == id).ToArray();
                if (a.Length == 0)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine();
                    Console.WriteLine("Бъркате нещо.");
                    Thread.Sleep(2000);
                    ForgotenPassword();
                }
                else
                {
                    Console.WriteLine($"Вашата парола е: {Decrypt(a[0].Password)}");
                    Thread.Sleep(8000);
                    Menu();
                    
                }
            }
        }
        public static void ChangePassword()
        {
            using (var db = new ChatContext())
            {
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.CursorVisible = true;
                Console.Clear();
                Console.WriteLine("Смяна на парола");
                Console.WriteLine("_______________");
                Console.WriteLine();
                Console.WriteLine("Потребител:");
                string username = Console.ReadLine();
                Console.WriteLine("ID:");
                int id = int.Parse(Console.ReadLine());
                Console.WriteLine("Стара парола:");

                string oldPassword = PasswordCover(0, 8, " ");
                

                var loggedUser = db.Users.Where(user => user.Username == username && user.Password == Crypt(oldPassword) && user.Id == id).ToArray();
                if (loggedUser.Length != 0)
                {
                    Console.WriteLine("Нова парола:");
                    string newPassword = PasswordCover(0, 10, " ");
                   
                    if (newPassword == oldPassword)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Старата и новата парола съвпадат.");
                        Thread.Sleep(3000);
                        ChangePassword();                       
                    }

                    loggedUser[0].Password = Crypt(newPassword);
                    db.SaveChanges();
                    Console.WriteLine();
                    Console.WriteLine("Успешно променихте паролата си.");
                    Thread.Sleep(5000);
                    Menu();
                }
                else
                {
                    Console.WriteLine("Бъркате нещо.");
                    Thread.Sleep(5000);
                    ChangePassword();
                }
            }
        }

        public static void CenterText(string text, int height, int rows = 0)
        {
            Console.SetCursorPosition((Console.WindowWidth - text.Length) / 2, (Console.WindowHeight - height) / 2 + rows);
        }
        protected static void MyHandler(object sender, ConsoleCancelEventArgs args)
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
        public static void PrintAccountOptions(int a = -1)
        {
            string optionOne = "1) Морски шах";
            string optionTwo = "2) Бесеница";
            string passChange = "0) Смяна на паролата";

            var defaultColor = ConsoleColor.Gray;
            var selectedColor = ConsoleColor.Cyan;

            Console.ForegroundColor = a == 1 ? selectedColor: defaultColor;
            CenterText(optionOne, 5, 1);
            Console.WriteLine(optionOne);

            Console.ForegroundColor = a == 2 ? selectedColor : defaultColor;
            CenterText(optionOne, 5, 2);
            Console.WriteLine(optionTwo);

            Console.ForegroundColor = a == 0 ? selectedColor : defaultColor;
            CenterText(optionOne, 5, 3);
            Console.WriteLine(passChange);

            Console.ForegroundColor = defaultColor;
        }
        public static void PrintLoginMistakeOption(int a = -1)
        {
            string optionOne = "1) Забравих паролата си.";
            string optionTwo = "Backspace) Опитай пак.";

            var defaultColor = ConsoleColor.Gray;
            var selectedColor = ConsoleColor.Cyan;

            string text = "Бъркате потребителско име или парола.";
            CenterText(text, 4, 1);
            Console.WriteLine(text);

            CenterText(text, 4, 2);
            Console.WriteLine("-------------------------------------");

            Console.ForegroundColor = a == 1 ? selectedColor : defaultColor;
            CenterText(text, 5, 3);
            Console.WriteLine(optionOne);

            Console.ForegroundColor = a == -40 ? selectedColor : defaultColor;
            CenterText(text, 5, 4);
            Console.WriteLine(optionTwo);
        }
       
        public static string PasswordCover(int left, int top, string deleteIndex)
        {         
            string password = null;
            
            while (true)
            {
                var key = Console.ReadKey();
                if (key.Key == ConsoleKey.Enter)
                {
                    Console.WriteLine();
                    break;
                }
                if (key.Key == ConsoleKey.Backspace && password.Length > 0)
                {
                    password = password.Remove(password.Length - 1, 1);
                    left--;
                    Console.SetCursorPosition(left, top);
                    Console.Write(deleteIndex);
                }
                else if (key.Key == ConsoleKey.Backspace && password.Length == 0)
                {
                    Console.SetCursorPosition(left, top);
                    continue;
                }
                else
                {
                    password += key.KeyChar;
                    Console.SetCursorPosition(left, top);
                    Console.Write('*');
                    left++;
                }
            }
            return password;
        }
        public static string UsernameInput(int left, int top, string deleteIndex)
        {
            string username = null;
            while (true)
            {
                var key = Console.ReadKey();
                int count = 0;
                if (key.Key == ConsoleKey.Enter)
                {
                    break;
                }
                if (key.Key == ConsoleKey.Backspace && username.Length > 0)
                {
                    username = username.Remove(username.Length - 1, 1);
                    left--;
                    Console.SetCursorPosition(left, top);
                    Console.Write(deleteIndex);
                }
                else if(key.Key == ConsoleKey.Backspace && username.Length == 0)
                {
                    Console.SetCursorPosition(left, top);
                    continue;
                }
                else
                {
                    username += key.KeyChar;
                    if (count == 0)
                    {
                        Console.SetCursorPosition(left+1, top);
                        count++;
                        
                    }
                    else
                    {
                        Console.SetCursorPosition(left, top);
                        
                    }
                    left++;
                }
            }
                return username;
        }
        public static string Crypt(string pass)
        {
            char[] chars = pass.ToCharArray();
            char[] result = new char[chars.Length];
            int counter = 0;
            foreach (char c in chars)
            {
                int a = c;
                result[counter] = (char)(a + 6);
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
