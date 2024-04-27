using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using CMP1903_A2_2324.Interfaces;

namespace CMP1903_A2_2324.Games
{
    /// <summary>
    /// Implementation of the game Three Or More.
    /// The class inherits from Game and implements IStatisticsTrackable, letting it use the Statistics class.
    /// </summary>
    internal class ThreeOrMore : Game, IStatisticsTrackable
    {
        /// <summary>
        /// Added to implement IStatisticsTrackable. Holds key-value pairs, connecting the Player ID and number of points
        /// they have scored.
        /// </summary>
        public Dictionary<int, int> Points { get; } = new Dictionary<int, int>();

        /// <summary>
        /// The name of the game is Three or More.
        /// </summary>
        protected override string Name => "Three or More";

        /// <summary>
        /// The constructor for the ThreeOrMore class. Calls the base Game constructor, defining the number of dice to five.
        /// </summary>
        /// <param name="playAgainstComputer">If the game should be played against a computer.</param>
        public ThreeOrMore(bool playAgainstComputer = true) : base(5, playAgainstComputer)
        {
            // Add the first and secondary player numbers into the points dictionary.
            Points.Add(1, 0);
            Points.Add(2, 0);
        }

        /// <summary>
        /// Formats the rolls into a human-readable format.
        /// </summary>
        /// <returns>A string of every roll, comma seperated.</returns>
        private string FormatRolls() => string.Join(", ", Dice.Select(i => i.CurrentValue.ToString()));

        /// <summary>
        /// Calculates if a computer player should reroll its dice, or only the non-two-of-a-kinds.
        /// </summary>
        /// <param name="diceValues">An array of magnitudes for how many of each number were rolled.</param>
        /// <returns>If all the dice should be rerolled, or not.</returns>
        private bool CalculateComputerTwoOfAKind(IEnumerable<int> diceValues)
        {
            // Declare that the computer got two of a kind, and therefore must make a decision.
            Console.WriteLine($"The computer rolled {FormatRolls()}, which contains a pair of two.");

            // If the magnitude of the most rolled number is less than three, then all dice should be rerolled.
            var shouldRerollAll = diceValues.Max() < 3;
            
            // Wait a second to give the illusion of the computer thinking.
            Thread.Sleep(1000);

            // Declare what the computer did.
            if (shouldRerollAll)
                Console.WriteLine("The computer decided to reroll all dice.");
            else
                Console.WriteLine("The computer decided to reroll the remaining dice.");

            // Create an empty line.
            Console.WriteLine();

            // Return if the values should be rerolled or not.
            return shouldRerollAll;
        }

        /// <summary>
        /// Prompt the user for if they would either like to reroll all their dice, or only some of them.
        /// </summary>
        /// <returns>If the user would like to reroll all their dice or not.</returns>
        private bool GetTwoOfKindInput()
        {
            // Prompt the user what they should do.
            Console.WriteLine($"You rolled {FormatRolls()}, which contains a pair of two.");
            Console.WriteLine($"Would you like to roll all your dice again, or only the three remaining dice?");
            Console.Write($"Type \"all\" to roll all dice again, or type anything else to only roll the remaining: ");
            
            // Get an input from the user, creating a new line afterward.
            var input = Console.ReadLine();
            Console.WriteLine();
            
            // If the user typed "all", then all the dice should be rerolled. Else, only some should be.
            return input != null && input.ToLower() == "all";
        }

        /// <summary>
        /// Handles if two of the same roll is gotten.
        /// </summary>
        /// <param name="diceValues">An array of magnitudes for how many of each number were rolled.</param>
        /// <param name="valueTwoOf">The number rolled two of.</param>
        /// <param name="playerNumber">The Player ID of whose turn it is.</param>
        /// <param name="isComputer">If the turn is for a computer.</param>
        /// <returns>The new number of points to award.</returns>
        private int HandleTwoOfKind(IEnumerable<int> diceValues, int valueTwoOf, int playerNumber, bool isComputer)
        {

            // Finds if all dice should be rerolled, depending on if it is a computer or human. 
            var rerollAllDice = (isComputer && CalculateComputerTwoOfAKind(diceValues)) 
                || (!isComputer && GetTwoOfKindInput());

            // If all dice should be rerolled, roll them.
            if (rerollAllDice)
                foreach (var die in Dice)
                    die.Roll();
            
            // Else, only reroll the dice which two of weren't rolled.
            else foreach (var die in Dice.Where(die => die.CurrentValue != valueTwoOf))
                die.Roll();
            
            // Run the AddPointsToAward method to calculate if this method should be run again, or if points can be awarded.
            return AddPointsToAward(playerNumber, isComputer);
        }

        /// <summary>
        /// Calculates the number of points to award from a set of rolls, awarding them to the relevant player.
        /// </summary>
        /// <param name="playerNumber">The Player ID of whose turn it is.</param>
        /// <param name="isComputer">If the turn is for a computer.</param>
        /// <returns>The total points awarded.</returns>
        private int AddPointsToAward(int playerNumber, bool isComputer)
        {
            // Rearrange the dice into an int array, where the index is the roll of the dice minus one
            // and the value is the number of times that roll came up.
            var dieValues = new int[Die.SidesOnDie];
            foreach (var die in Dice)
            {
                var roll = die.CurrentValue;
                dieValues[roll - 1]++;
            }

            // Iterate six times
            for (var index = 0; index < dieValues.Length; index++)
            {
                // Find how many times the roll `index + 1` was rolled.
                var value = dieValues[index];
                
                // If it wasn't twice, then continue iterating.
                if (value != 2) continue;

                // Else, handle there being two of the same dice roll, creating a recursive loop.
                return HandleTwoOfKind(dieValues, index + 1, playerNumber, isComputer);

            }

            // Set a variable to store the number of points awarded, overall.
            var totalPointsGiven = 0;
            
            // Iterate six times,
            foreach (var value in dieValues)
            {
                // Set a variable to store the number of points awarded, for this dice value.
                var pointsToAward = 0;
                
                // Assign the appropriate number of points for how many of the value was rolled.
                switch (value)
                {
                    case 5:
                        pointsToAward = 12;
                        break;
                    case 4:
                        pointsToAward = 6;
                        break;
                    case 3:
                        pointsToAward = 3;
                        break;
                }

                // Add the points to the player
                Points[playerNumber] += pointsToAward;
                
                // Add the points to the running total of points awarded.
                totalPointsGiven += pointsToAward;
            }
            
            // Return the number of points awarded.
            return totalPointsGiven;
        }
        
        /// <summary>
        /// Rolls all the game dice, calculating its score and awarding the points to the player.
        /// </summary>
        /// <param name="playerNumber">The Player ID of whose turn it is.</param>
        /// <param name="isComputer">If the turn is for a computer.</param>
        /// <returns>The number of points earned in the turn.</returns>
        private int RollDiceAndAddPoints(int playerNumber, bool isComputer)
        {
            // Roll every dice
            foreach (var die in Dice) die.Roll();

            // Calculate the score of the rolls and add it to the player.
            return AddPointsToAward(playerNumber, isComputer);
        }
        
        /// <summary>
        /// Plays a turn of Three or More for a computer.
        /// </summary>
        /// <param name="playerNumber">The ID of the Computer Player.</param>
        /// <returns>If the game should end, or not.</returns>
        protected override bool CompleteComputerTurn(int playerNumber)
        {
            // Declare whose turn it is, and roll their dice / add their points.
            Console.WriteLine($"======== Player {playerNumber}'s Turn [Computer] ========");
            var newPoints = RollDiceAndAddPoints(playerNumber, true);

            // Say what the final rolls were, and how many points it brings the player up to.
            Console.WriteLine($"The computer rolled: {FormatRolls()}, bringing them to {Points[playerNumber]} points.");
            Console.WriteLine();
            
            // Wait a second to give the illusion of the computer doing something.
            Thread.Sleep(1000);

            // Invoke the event handler for a turn being completed, so other classes can react.
            InvokeTurnCompleted(playerNumber, newPoints, Points[playerNumber]);
            
            // If 20 or more points have been scored, then the game should end.
            return Points[playerNumber] >= 20;
        }
        
        /// <summary>
        /// Plays a turn of Three or More for a human.
        /// </summary>
        /// <param name="playerNumber">The ID of the Human Player.</param>
        /// <returns>If the game should end, or not.</returns>
        protected override bool CompletePlayerTurn(int playerNumber)
        {
            // Declare whose turn it is, and roll their dice / add their points.
            Console.WriteLine($"======== Player {playerNumber}'s Turn [Player] ========");
            var newPoints =  RollDiceAndAddPoints(playerNumber, false);
            
            // Say what the final rolls were, and how many points it brings the player up to.
            Console.WriteLine($"Player {playerNumber} rolled: {FormatRolls()}, bringing them up to {Points[playerNumber]} points.");
            
            // Prompt the player to acknowledge their turn is over.
            Console.WriteLine("Press any key to continue.");
            Console.WriteLine();
            Console.ReadKey();

            // Invoke the event handler for a turn being completed, so other classes can react.
            InvokeTurnCompleted(playerNumber, newPoints, Points[playerNumber]);

            // If 20 or more points have been scored, then the game should end.
            return Points[playerNumber] >= 20;
        }

        /// <summary>
        /// Plays a game of Three or More, whilst resetting points at the start and adding statistics at the end.
        /// </summary>
        /// <exception cref="StackOverflowException">Happens if a player rolls two of the same dice too many times.</exception>
        public override void PlayGame()
        {
            // Attempt to play the game as normal.
            try
            {
                // Firstly, reset the points from the last game.
                for (var i = 0; i < Points.Count; i++) Points[i] = 0;
                
                // Play the game, as defined in the parent class.
                base.PlayGame();
                
                // Add the game to statistics, to be recorded.
                GameStatistics.AddNewGame(this);
            }
            
            // If a Stack Overflow Exception occurs,
            catch (StackOverflowException)
            {
                // Rewrite the error and give it to the user.
                throw new StackOverflowException(
                    "Two dice of the same value were rolled so many times that it caused a Stack Overflow.");
            }
        }
    }
}