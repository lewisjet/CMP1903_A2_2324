using System;

namespace CMP1903_A1_2324
{
    internal static class Program
    {
        /*
         * In C#, it is seen as bad practice to instantiate the Random class multiple times.
         * This is because of both performance reasons, as well as risking the same number being given multiple times in a row.
         *
         * To mitigate this, I have made RandomInstance its own property.
         * 
         * Cited: https://learn.microsoft.com/en-us/dotnet/fundamentals/runtime-libraries/system-random#avoid-multiple-instantiations
         */
        public static Random RandomInstance { get; } = new Random();

        private static void Main(string[] args)
        {
            /*
             * Create a Game object and call its methods.
             * Create a Testing object to verify the output and operation of the other classes.
             */
            
            // Instantiate a new instance of the game class
            var game = new Game();
            
            // Roll each dice in the game until the user tells the program not to.
            try
            {
                
                // The RollDiceContinuously method calls every function in the Game class, bar the SumAllDice method.
                game.RollDiceContinuously();
                
                // So every function is used in the Main method, the last sum calculated will be displayed at the end.
                Console.WriteLine($"Final sum of dice of the game: {game.SumAllDice()}");
            }
            
            // If the game has no dice and therefore nothing can be outputted, the exception will be caught.
            catch (GameUsesNoDiceException)
            {
                Console.WriteLine("The game has no dice which can be rolled.");
                Console.WriteLine("Because of this, the game will now terminate.");
            }
            
            // Regardless of if the game threw an error, test it.
            finally
            {
                // the Testing class is instantiated to ensure that everything is working correctly.
                var testingClass = new Testing(game);
            
                // Every test in the testing class will be run.
                testingClass.RunTestingSuite();
            }
            
        }
    }
}
