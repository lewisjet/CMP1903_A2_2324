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

        private static Game PromptUserForGame(bool playAgainstComputer)
        {
            Console.WriteLine("Please press the number corresponding to the game you would like to play.");
            Console.WriteLine("1) Sevens Out");
            Console.WriteLine("2) Three or More");

            while (true)
            {
                var keypress = Console.ReadKey();
                Console.WriteLine();
                
                switch (keypress.KeyChar)
                {
                    case '1':
                        return new SevensOut(playAgainstComputer);
                    case '2':
                        return new ThreeOrMore(playAgainstComputer);
                    default:
                        Console.WriteLine("Your input was invalid. Please press '1' or '2'.");
                        break;
                }
            }
        }

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
            var game = PromptUserForGame(!PromptUserForPartnerPlay());
            game.PlayGame();
        }
    }
}
