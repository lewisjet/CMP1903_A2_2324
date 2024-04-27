using CMP1903_A2_2324.Games;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMP1903_A2_2324
{
    /// <summary>
    /// Handles testing for inheritors of the Game class.
    /// </summary>
    internal class Testing : IDisposable
    {
        private const string DefaultLogFileName = "testing_results.log";

        /// <summary>
        /// The path where the log file is saved to.
        /// </summary>
        public string LogPath => Path.Combine(Environment.CurrentDirectory, DefaultLogFileName);
        
        /// <summary>
        /// Holds the game instance, instantiated for testing.
        /// </summary>
        private Game GameInstance { get; }
        
        /// <summary>
        /// So the results of the test can be written to a file, a file stream is stored in the class.
        /// </summary>
        private FileStream Stream { get; }

        /// <summary>
        /// Holds if all the tests passed, on the last run through.
        /// </summary>
        public bool TestPassed { get; private set; } = true;

        /// <summary>
        /// Initializes the Testing class, instantiating a game of the type specified.
        /// </summary>
        /// <param name="gameType">The type of game to instantiate.</param>
        /// <exception cref="InvalidCastException">If the game cannot be tested as it does not inherit the Game class, an exception is thrown.</exception>
        /// <exception cref="MissingMethodException">To instantiate the game, a constructor taking one boolean argument should be given. If this is not provided, an exception is thrown.</exception>
        public Testing(Type gameType)
        {
            try
            {
                // Try to instantiate the game instance from the type given, with playing against a computer set to true.
                GameInstance = (Game)Activator.CreateInstance(gameType, new object[] { true });

                // Try to create a file stream for appending logs to.
                Stream = new FileStream(LogPath, FileMode.Append);
            }

            // If the class is not a game, throw an exception.
            catch (InvalidCastException)
            {
                throw new InvalidCastException(
                    "The specified class could not be tested, as it does not derive from the Game class.");
            }

            // If the class cannot be constructed, throw an exception.
            catch (MissingMethodException)
            {
                throw new MissingMethodException(
                    "The specified game could not be tested, as it does not have a constructor taking one argument.");
            }

            // If the file stream creation fails, throw an exception.
            catch (IOException)
            {
                throw new IOException("The testing log file could not be created or written to.");
            }
        }

        /// <summary>
        /// Runs the game and the tests which should be conducted for it.
        /// </summary>
        /// <exception cref="NotImplementedException">If the game is not supported, an exception is thrown.</exception>
        public void RunTests()
        {
            // If the game is recognised, rerun the method with the correct event handler.
            if (GameInstance.GetType() == typeof(SevensOut)) RunTests(OnSevensOutTurn);
            else if (GameInstance.GetType() == typeof(ThreeOrMore)) RunTests(OnThreeOrMore);

            // If it isn't recognised, throw an exception.
            else throw new NotImplementedException("The specified game could not be tested, as its tests have not been implemented.");
        }

        /// <summary>
        /// Runs the game, and the tests specified by the TurnCompletedHandler.
        /// </summary>
        /// <param name="handler">The handler to use when an in-game turn ends, to test the game.</param>
        private void RunTests(Game.TurnCompletedHandler handler)
        {
            // Give instructions on how to cooperate with the testing process.
            Console.WriteLine("Please play the game as you would if you were playing against a computer. The tests will run automatically.");

            // Tell the game to call the specified handler when the turn is completed.
            GameInstance.OnTurnCompleted += handler;

            // Play the game.
            GameInstance.PlayGame();
        }

        /// <summary>
        /// Appends a log to the log file.
        /// </summary>
        /// <param name="message">The log to add to the file.</param>
        private void WriteToLogFile(string message)
        {
            // Create a byte array, containing what should be written to the file.
            var log = Encoding.UTF8.GetBytes($"\n{DateTime.Now}: {message}");
            
            // Append that byte array to the file stream.
            Stream.Write(log, 0, log.Length);
        }

        /// <summary>
        /// Runs Debug.Assert on a condition, setting TestPassed to false if the test failed.
        /// The result of the test is logged into the file stream.
        /// </summary>
        /// <param name="condition">Is true if the test passed.</param>
        /// <param name="message">The message to show on a test failure.</param>
        /// <returns>If the condition is true or false.</returns>
        private bool TestCondition(bool condition, string message)
        {
            // Make a popup showing the failure if the condition is not met.
            Debug.Assert(condition, message);

            // If the test hasn't already failed, set it to the condition's value.
            TestPassed = TestPassed && condition;

            WriteToLogFile($"[{(condition ? "PASSED" : "FAILED")}] {message}");

            // Return the condition.
            return condition;
        }

        /// <summary>
        /// The TurnCompletedHandler for when SevensOut is being tested.
        /// </summary>
        /// <param name="playerNumber">The player's number, used for tracking who's turn it is.</param>
        /// <param name="gainedScore">The points the player got this turn.</param>
        /// <param name="score">The total points gained by the player this game. Currently unused by the method.</param>
        /// <param name="diceRolls">The dice rolls the player last rolled.</param>
        private void OnSevensOutTurn(int playerNumber, int gainedScore, int score, int[] diceRolls)
        {
            WriteToLogFile($"Testing Player {playerNumber}s turn, in Sevens Out.");
            
            // Test that the game broke when it was meant to.
            TestCondition(gainedScore != 7, "The game terminates with a score of seven.");

            // Test that the game is using two dice. This is here to prevent an IndexOutOfRangeException.
            if(!TestCondition(diceRolls.Length == 2, "The game is using two dice")) return;

            // Sum the total dice rolls and validate that the score given is accurate
            var sum = diceRolls[0] + diceRolls[1];
            TestCondition((diceRolls[0] == diceRolls[1] ? sum * 2 : sum) == gainedScore, "The correct score is being given");
        }

        /// <summary>
        /// The TurnCompletedHandler for when ThreeOrMore is being tested.
        /// </summary>
        /// <param name="playerNumber">The player's number, used for tracking who's turn it is.</param>
        /// <param name="gainedScore">The points the player got this turn.</param>
        /// <param name="score">The total points gained by the player this game.</param>
        /// <param name="diceRolls">The dice rolls the player last rolled.</param>
        private void OnThreeOrMore(int playerNumber, int gainedScore, int score, int[] diceRolls)
        {
            WriteToLogFile($"Testing Player {playerNumber}s turn, in Three or More.");
            
            // Check that the right number of dice are being used.
            TestCondition(diceRolls.Length == 5, "The game is using five dice");

            // Sum how many of each number were rolled, compiling the results into a dictionary.
            var rollTotals = new Dictionary<int, int>();
            foreach( var roll in diceRolls)
            {
                if(!rollTotals.ContainsKey(roll)) rollTotals.Add(roll, 1);
                else rollTotals[roll]++;
            }

            // From the sums of what was rolled, recalculate the score awarded.
            var validatedScore = 0;
            foreach( var qty in rollTotals.Values)
            {
                switch (qty)
                {
                    case 3: validatedScore += 3; continue;
                    case 4: validatedScore += 6; continue;
                    case 5: validatedScore += 12; continue;
                }
            }

            // Test that both scores match.
            TestCondition(validatedScore == gainedScore, "The points gained is correct.");

            // Check that the game hasn't continued longer than it should have.
            TestCondition(score - gainedScore < 20, "The game terminates after 20 points are scored.");

            // Check that the score is added correctly to the gained score.
            TestCondition(gainedScore - score <= 0, "The score is added correctly to the player.");

        }

        /// <summary>
        /// A deconstructor for the testing class. Runs the method to close the file stream.
        /// </summary>
        ~Testing()
        {
            // Close the file stream.
            Dispose();
        }

        /// <summary>
        /// Implements the IDisposable Interface.
        /// When the Testing class is disposed, the log file stream is closed with it. 
        /// </summary>
        public void Dispose()
        {
            // Close the file stream, used for logging. C# can finalize everything else automatically.
            Stream?.Close();
        }
    }
}
