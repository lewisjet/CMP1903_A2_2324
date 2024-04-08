using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMP1903_A1_2324
{
    internal class Testing
    {
        /*
         * This class should test the Game and the Die class.
         * Create a Game object, call the methods and compare their output to expected output.
         * Create a Die object and call its method.
         * Use debug.assert() to make the comparisons and tests.
         */

        // Since testing is done on the game, a readonly variable to store the Game class in has been made.
        private readonly Game _gameTested;

        /// <summary>
        /// The constructor for the Testing class
        /// </summary>
        /// <param name="gameTested">The game to test</param>
        public Testing(Game gameTested)
        {
            // Sets the _gameTested variable to the game which is being tested by the class.
            _gameTested = gameTested;
        }

        /// <summary>
        /// Validates that both the RollAllDice and SumAllDice functions work as intended.
        /// This is done by finding the sum of the dice rolls given, and the sum of the dice rolls stored
        /// and comparing the two in an assert function.
        /// </summary>
        private void ValidateSumOfDice()
        {
            // Run the roll all dice function, to get an enumerable of each die result.
            var diceRolls = _gameTested.RollAllDice();
            
            // Find the sum of all the dice rolls, gotten from the RollAllDice function.
            var sumOne = diceRolls.Sum();
            
            // Get the sum of all dice rolls from the SumAllDice function.
            var sumTwo = _gameTested.SumAllDice();

            // Asset that both of the sums are the same.
            Debug.Assert(sumOne == sumTwo);
        }

        /// <summary>
        /// Creates a die instance, rolls it, and checks that its result is the same as the die's current value.
        /// The method also checks that the value is between 1 and 6.
        /// This method has been made static since it does not rely on the game class.
        /// </summary>
        private static void CheckDiceRollAsIntended()
        {
            // Instantiate a new Die instance.
            var die = new Die();
            
            // Roll the new die,storing the result in a variable.
            var result = die.Roll();
            
            // Assert that the result equals the current value of the die.
            Debug.Assert(result == die.CurrentValue);
            
            // Check that the value is between 1 and 6.
            Debug.Assert(result > 0 && result < 7);
        }

        /// <summary>
        /// Rolls every dice in the game instance, checking that each dice value is between 1 and 6.
        /// </summary>
        private void CheckThatGameRollsAreWithinRange()
        {
            // Roll every dice in the game, getting their results
            var rolls = _gameTested.RollAllDice();
            
            // For each dice rolled, assert that the roll is between 1 and 6.
            foreach (var roll in rolls)
            {
                Debug.Assert(roll > 0 && roll < 7);
            }
        }

        /// <summary>
        /// Runs each of the testing functions in the class, whilst providing error handling.
        /// </summary>
        public void RunTestingSuite()
        {
            try
            {
                // Checks that the dice work as intended.
                CheckDiceRollAsIntended();

                // Checks that all rolls from the Game class are between 1 and 6.
                CheckThatGameRollsAreWithinRange();

                // Checks that the Game class can roll and sum correctly.
                ValidateSumOfDice();
            }
            catch (GameUsesNoDiceException)
            {
                Debug.Fail("The game has no dice which can be rolled.");
            }
        }
    }
}
