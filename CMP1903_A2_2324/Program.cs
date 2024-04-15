using System;
using CMP1903_A2_2324.Games;

namespace CMP1903_A2_2324
{
    internal static class Program
    {
        /*
         * In C#, it is seen as bad practice to instantiate the Random class multiple times.
         * This is because of both performance reasons and risking the same number being given multiple times in a row.
         *
         * To mitigate this, I have made RandomInstance its own property.
         * 
         * Cited: https://learn.microsoft.com/en-us/dotnet/fundamentals/runtime-libraries/system-random#avoid-multiple-instantiations
         */
        public static Random RandomInstance { get; } = new Random();

        private static Game[] games = new Game[]
                {
                    new SevensOut(false),
                    new ThreeOrMore(false)
                };

        private static bool PromptUserForPartnerPlay()
        {
            Console.WriteLine("Would you like to play with another person, or against the computer?");
            Console.WriteLine("Please press '1' if you would like to play with a partner, or '2' to play with a computer.");
            
            while (true)
            {
                var keypress = Console.ReadKey();
                Console.WriteLine();

                switch (keypress.KeyChar)
                {
                    case '1':
                        return true;
                    case '2':
                        return false;
                    default:
                        Console.WriteLine("Your input was invalid. Please press '1' or '2'.");
                        break;
                }
            }
        }
        
        private static void Main()
        {
            do
            {
                var game = Game.PromptUserForGame(games);
            
                game.PlayingAgainstComputer = !PromptUserForPartnerPlay();
                game.PlayGame();
                Console.WriteLine("Would you like to play another game? If so, press 'y'. To exit, press any other key.");
            } while (Console.ReadKey().KeyChar.ToString().ToLower() == "y");
            
            Console.WriteLine();

            foreach(var game in games) 
            {
                Console.WriteLine($"======== {game.Name} Statistics ========");
                game.PrintStatistics();
                Console.WriteLine();
            }

            Console.ReadKey();
        }
    }
}
