using System;

namespace Golf
{
    class Program
    {
       
        static void Main(string[] args)
        { 
            TheGame game = new TheGame();
            Console.WriteLine("Welcome to Augusta Golf Course!\nHome of the 2021 Masters.");
            game.StartGame();
            Environment.Exit(0);
        }
    }
}
