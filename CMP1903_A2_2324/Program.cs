using System;
using CMP1903_A2_2324.Enums;
using CMP1903_A2_2324.Games;

namespace CMP1903_A2_2324
{
    /// <summary>
    /// A class required for the program to execute.
    /// Contains the methods required to execute the games/tests, and control its flow.
    /// </summary>
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

        /// <summary>
        /// The games which can be played.
        /// </summary>
        private static Game[] games = new Game[]
                {
                    new SevensOut(false),
                    new ThreeOrMore(false)
                };

        /// <summary>
        /// Asks the user to select what "state" to run the program in.
        /// This is used to find if the user would like to test the game, look at its statistics, or play it against a computer/partner.
        /// </summary>
        /// <returns>What state the user would like the program in.</returns>
        private static ProgramStateType PromptUserForProgramState()
        {
            // If the program is in release, the option to test should be unavailable.
            // Prompt the user to input a number, corresponding to an enum value.
#if !DEBUG
            Console.WriteLine("Would you like to play with another person, against the computer, or view game statistics?");
            Console.WriteLine("Please press '1' if you would like to play with a partner, '2' to play with a computer, or '3' to view game statistics.");
#else
            Console.WriteLine("Would you like to play with another person, against the computer, view game statistics, or test the game?");
            Console.WriteLine("Please press '1' if you would like to play with a partner, '2' to play with a computer, '3' to view game statistics, or '4' to test the game.");
#endif            
            // Keep asking the user to input a character, corresponsing to a state type, until they input something valid.
            // When they do, return what they say.
            while (true)
            {
                var keypress = Console.ReadKey();
                Console.WriteLine();

                switch (keypress.KeyChar)
                {
                    case '1':
                        return ProgramStateType.PartnerPlay;

                    case '2':
                        return ProgramStateType.ComputerPlay;

                    case '3':
                        return ProgramStateType.Statistics;
#if DEBUG
                    case '4':
                        return ProgramStateType.Testing;
#endif
                    default:
#if DEBUG
                        Console.WriteLine("Your input was invalid. Please press '1', '2', '3' or '4'.");

#else
                        Console.WriteLine("Your input was invalid. Please press '1', '2' or '3'.");
#endif                        
                        break;
                }
            }
        }

        /// <summary>
        /// Draws the menus for the game, and runs the game/tests from what was said by the user.
        /// </summary>
        /// <exception cref="NotImplementedException">The method throws a NotImplementedException is given an unjown ProgramStateType.</exception>
        private static void DrawGameMenu()
        {
            do
            {
                // Draws a menu, asking the user for what game to use.
                var game = Game.PromptUserForGame(games);

                // Draws a menu, asking the user what to do with the game.
                var programState = PromptUserForProgramState();

                // If the user would like to see the game's statistics, show it to them.
                if (programState == ProgramStateType.Statistics)
                {
                    Console.WriteLine();
                    game.PrintStatistics();
                }

                // Else, if the player would like to test the game (they must be in DEBUG for this to be available),
                else if (programState == ProgramStateType.Testing)
                {
                    // Create a testing class for the type of game specified by the user
                    var testingClass = new Testing(game.GetType());

                    // Run all the tests for the game
                    testingClass.RunTests();

                    // Tell the user if all the tests passed or failed.
                    if (testingClass.TestPassed) Console.WriteLine("All tests passed successfully.");
                    else Console.WriteLine("Some tests failed during execution.");

                }

                // Else, if the user asked to play the game, set the game to playing against the computer (or not).
                else if (programState == ProgramStateType.PartnerPlay || programState == ProgramStateType.ComputerPlay)
                {
                    game.PlayingAgainstComputer = programState != ProgramStateType.PartnerPlay;

                    // The game should also execute.
                    game.PlayGame();
                }

                // If none of the above were selected, then throw an Exception.
                else throw new NotImplementedException("The game was given a ProgramStateType which has not been implemented.");

                // Prompt the user to press 'y' if they would like to play again.
                Console.WriteLine("Would you like to play another game? If so, press 'y'. To exit, press any other key.");

            // Loop the game if the player presses 'y'
            } while (Console.ReadKey().KeyChar.ToString().ToLower() == "y");
        }
        
        /// <summary>
        /// Displays an exception to the user in a friendly way, asking the user if they would like to restart the program.
        /// </summary>
        /// <param name="err">The error thrown</param>
        /// <returns>If the user would like to exit.</returns>
        private static bool HandleException(Exception err)
        {
            // Display the error message in red.
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine();
            Console.WriteLine("An unexpected error occured!");
            Console.WriteLine(err.Message);
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.White;

            // Ask the user if they would like to restart the program.
            Console.WriteLine("Would you like restart the program? If so, press 'y'. To exit, press any other key.");

            // If they don't, return true. Else, return false.
            if (Console.ReadKey().KeyChar.ToString().ToLower() != "y") return true;
            Console.WriteLine();
            return false;
        }
        
        /// <summary>
        /// The first method to run in the program.
        /// </summary>
        private static void Main()
        {
            while (true)
            {
                try
                {
                    // Attempt to draw a game menu and play/test the games
                    DrawGameMenu();

                    // If all succeds, exit the program.
                    break;
                }
                catch (Exception err)
                {
                    // Else, show the error to the user and check if they would like to restart the application.
                    if (!HandleException(err)) break;
                }
            }
        }
    }
}
