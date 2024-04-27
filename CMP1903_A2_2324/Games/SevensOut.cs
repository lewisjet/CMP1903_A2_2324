using System;
using System.Collections.Generic;
using System.Threading;
using CMP1903_A2_2324.Interfaces;

namespace CMP1903_A2_2324.Games
{
    /// <summary>
    /// Implementation of the game Sevens Out.
    /// The class inherits from Game and implements IStatisticsTrackable, letting it use the Statistics class.
    /// </summary>
    internal class SevensOut : Game, IStatisticsTrackable
    {
        /// <summary>
        /// Added to implement IStatisticsTrackable. Holds key-value pairs, connecting the Player ID and number of points
        /// they have scored.
        /// </summary>
        public Dictionary<int, int> Points { get; } = new Dictionary<int, int>();

        /// <summary>
        /// The name of the game is Sevens Out.
        /// </summary>
        protected override string Name => "Sevens Out";

        /// <summary>
        /// The constructor for the SevensOut class. Calls the base Game constructor, defining the number of dice to two.
        /// </summary>
        /// <param name="playAgainstComputer">If the game should be played against a computer.</param>
        public SevensOut(bool playAgainstComputer = true) : base(2, playAgainstComputer)
        {
            // Add the first and secondary player numbers into the points dictionary.
            Points.Add(FirstPlayerNumber, 0);
            Points.Add(SecondaryPlayerNumber, 0);
        }

        /// <summary>
        /// Rolls the two dice, calculating the points they will give.
        /// </summary>
        /// <returns>The points given from the two dice rolls. This is equal to seven if the game should end.</returns>
        private int RollAndSumDice()
        {
            // Define the total points
            var total = 0;
            
            // As points double on a double, the previous roll and if every roll has been the same so far should be kept.
            var previousRoll = -1;
            var allRollsSame = true;
            
            // For every die instantiated,
            foreach (var die in Dice)
            {
                // Roll the die.
                var roll = die.Roll();
                
                // Add the value of the roll to the total points.
                total += roll;
                
                // If this isn't the first roll, and it isn't the same as the last roll, then not every roll is the same.
                if (previousRoll != -1 && previousRoll != roll)
                    allRollsSame = false;
                
                // Set the previous roll to this, so the loop can iterate.
                previousRoll = roll;
            }

            // If every roll was the same, double the total.
            if (allRollsSame) total *= 2;

            // Return the total.
            return total;
        }

        /// <summary>
        /// Prints the results of the turn to the console
        /// </summary>
        /// <param name="playerNumber">The Player ID for whose turn it is</param>
        /// <param name="total">The total points gained in the turn</param>
        /// <param name="isComputer">If the player is a computer or not</param>
        private void OutputTurnResults(int playerNumber, int total, bool isComputer)
        {
            // Write whose player's turn it is, and if they are human or not.
            Console.WriteLine();
            Console.WriteLine($"======== Player {playerNumber}'s Turn {(isComputer ? "[Computer]" : "[Human]")} ========");
            
            // Output what the dice rolls were.
            for (var i = 0; i < Dice.Count; i++)
            {
                Console.WriteLine($"Die {i + 1}: {Dice[i].CurrentValue}");
            }

            // Output the total score.
            Console.WriteLine($"Total: {total}");
            
            // Output if it is Sevens Out, and therefore, the end of the game.
            if(total == 7) Console.WriteLine("Sevens out!");
        }
        
        /// <summary>
        /// The common code for completing a turn in SevensOut.
        /// </summary>
        /// <param name="playerNumber">The Player ID of whose turn it is.</param>
        /// <param name="isComputer">If the player is a computer or not.</param>
        /// <returns>If the game should terminate.</returns>
        private bool CompleteTurn(int playerNumber, bool isComputer)
        {
            // Roll all the dice and find the points gained in the turn.
            var total = RollAndSumDice();
            
            // Print the results of the turn to the user.
            OutputTurnResults(playerNumber, total, isComputer);
            
            // If it is SevensOut, then the game should terminate.
            if (total == 7) return true;
            
            // Else, give the player the points they earned.
            Points[playerNumber] += total;
            
            // Invoke the event handler for a turn being completed, so other classes can react.
            InvokeTurnCompleted(playerNumber, total, Points[playerNumber]);
            
            // Return that the game should continue.
            return false;
        }

        /// <summary>
        /// Plays a turn of Sevens Out for a computer.
        /// </summary>
        /// <param name="playerNumber">The ID of the Computer Player.</param>
        /// <returns>If the game should end, or not.</returns>
        protected override bool CompleteComputerTurn(int playerNumber)
        {
            // Run the common algorithm for sevens out, for a computer.
            var complete = CompleteTurn(playerNumber, true);
            
            // Wait for 1.5 seconds
            Thread.Sleep(1500);
            
            // If the common algorithm wants the game to end, end it.
            return complete;
        }

        /// <summary>
        /// Plays a turn of Sevens Out for a human.
        /// </summary>
        /// <param name="playerNumber">The ID of the Human Player.</param>
        /// <returns>If the game should end, or not.</returns>
        protected override bool CompletePlayerTurn(int playerNumber)
        {
            // Run the common algorithm for sevens out, for a human.
            var complete = CompleteTurn(playerNumber, false);
            
            // Prompt the user to acknowledge the end of their turn.
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
            
            // If the common algorithm wants the game to end, end it.
            return complete;
        }

        /// <summary>
        /// Plays a game of Sevens Out, whilst resetting points at the start and adding statistics at the end.
        /// </summary>
        public override void PlayGame()
        {
            // Reset all points to zero.
            for(var i = 0; i < Points.Count; i++) Points[i] = 0;
            
            // Run the base algorithm to play the game, in the Game class.
            base.PlayGame();
            
            // Add this game to the statistics.
            GameStatistics.AddNewGame(this);
        }
    }
}