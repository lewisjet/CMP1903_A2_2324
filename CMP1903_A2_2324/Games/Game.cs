using System;
using System.Collections.Generic;
using System.Linq;
using CMP1903_A2_2324;

namespace CMP1903_A2_2324.Games
{
    /// <summary>
    /// An abstract class, defining what a game should include and how one plays.
    /// </summary>
    internal abstract class Game
    {
        /// <summary>
        /// The ID for the first player
        /// </summary>
        protected const int FirstPlayerNumber = 1;
        
        /// <summary>
        /// The ID for the second player
        /// </summary>
        protected const int SecondaryPlayerNumber = 2;
        
        /// <summary>
        /// A list of dice, which can be rolled in the game.
        /// </summary>
        protected List<Die> Dice { get; }
        
        /// <summary>
        /// Holds an instance of the statistics class, allowing for some games to track their play.
        /// </summary>
        protected Statistics GameStatistics { get; } = new Statistics();
        
        /// <summary>
        /// The name of the game.
        /// </summary>
        protected abstract string Name { get; }

        /// <summary>
        /// If the game should be played against a computer or a human.
        /// </summary>
        public bool PlayingAgainstComputer { get; set; }

        /// <summary>
        /// If other classes would like to run code when every turn is finished, they may implement a TurnCompletedHandler.
        /// </summary>
        public delegate void TurnCompletedHandler(int player, int gainedScore, int newScore, int[] diceRolls);
        
        /// <summary>
        /// Allows other classes to listen for when a turn is completed, invoking their TurnCompletedHandlers.
        /// </summary>
        public event TurnCompletedHandler OnTurnCompleted;

        /// <summary>
        /// Plays a turn of the game, for a computer.
        /// </summary>
        /// <param name="playerNumber">The player ID whose turn is being played.</param>
        /// <returns>If the game is finished after the turn completed.</returns>
        protected abstract bool CompleteComputerTurn(int playerNumber);
        
        /// <summary>
        /// Plays a turn of the game, for a human.
        /// </summary>
        /// <param name="playerNumber">The player ID whose turn is being played.</param>
        /// <returns>If the game is finished after the turn completed.</returns>
        protected abstract bool CompletePlayerTurn(int playerNumber);

        /// <summary>
        /// So the children of the game class can invoke the event handler, a method has been made for them to call.
        /// </summary>
        /// <param name="player">The player ID of whose turn was just completed.</param>
        /// <param name="gainedScore">If the game uses a scoring system, what the change of score was on the turn.</param>
        /// <param name="newScore">If the game uses a scoring system, what the total score is now for the player.</param>
        protected void InvokeTurnCompleted(int player, int gainedScore, int newScore) => 
            OnTurnCompleted?.Invoke(player, gainedScore, newScore, Dice.Select(i => i.CurrentValue).ToArray());

        /// <summary>
        /// Draws a menu, asking the user to choose a game from a selection, given.
        /// </summary>
        /// <param name="games">The selection of games to ask the user what they would like to play.</param>
        /// <returns>The game the user selected.</returns>
        public static Game PromptUserForGame(IEnumerable<Game> games)
        {
            // So the games can be enumerated, convert them to an array.
            var gamesArr = games.ToArray();

            // Tell the user what game options are available.
            Console.WriteLine("Please press the number corresponding to the game you would like to play.");
            for(int i = 0; i < gamesArr.Length; i++)
            {
                Console.WriteLine($"{i + 1}) {gamesArr[i].Name}");
            }

            // Until the user gives a valid response,
            while (true)
            {
                // Get a keypress from the user, on its own line.
                var keypress = Console.ReadKey();
                Console.WriteLine();

                // If the keypress corresponds with one of the games, return the game.
                for (int i = 0; i < gamesArr.Length; i++) 
                {
                    if(keypress.KeyChar.ToString() == (i + 1).ToString())
                        return gamesArr[i];
                }
                
                // Else, return an error message.
                Console.WriteLine("Your input was invalid. Please press a valid number key.");
            }
        }

        /// <summary>
        /// Exposes the PrintStatistics method on the Statistics class to other classes.
        /// </summary>
        public void PrintStatistics() => GameStatistics.PrintStatistics();

        /// <summary>
        /// Plays a two-player game until completion, alternating between player and computer turns if required.
        /// </summary>
        public virtual void PlayGame()
        {
            // Loop until the game is completed.
            while (true)
            {
                // Play the human player's turn. If it wins the game, break out of the method.
                if(CompletePlayerTurn(FirstPlayerNumber)) return;
            
                // Next play the computer or partner's turn. If it wins the game, break out of the method.
                if(PlayingAgainstComputer ?
                       CompleteComputerTurn(SecondaryPlayerNumber) :
                       CompletePlayerTurn(SecondaryPlayerNumber))
                    return;
            }
        }

        /// <summary>
        /// A constructor for the game class, instantiating its dice and setting if it should be played against a computer.
        /// </summary>
        /// <param name="numberOfDice">The number of dice to use for the game.</param>
        /// <param name="playAgainstComputer">If the game should be played against a computer.</param>
        protected Game(int numberOfDice, bool playAgainstComputer)
        {
            // Initialize the PlayingAgainstComputer property.
            PlayingAgainstComputer = playAgainstComputer;

            // Instantiate the dice list, populating it with n dice.
            Dice = new List<Die>();
            for(int i = 0; i < numberOfDice; i++) Dice.Add(new Die());
        }
    }
}