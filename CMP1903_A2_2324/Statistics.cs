using CMP1903_A2_2324.Interfaces;
using System;
using System.Collections.Generic;

namespace CMP1903_A2_2324
{
    /// <summary>
    /// Contains the statistics for the Game class, and methods to extract statistics from the end-state of a game.
    /// </summary>
    internal class Statistics
    {
        // Holds the highest and lowest score, accessible through properties.
        private int highScore;
        private int lowestScore;

        /// <summary>
        /// Allows the high score to be retrieved and modified, with it only being updated if it is truly the highest score given.
        /// </summary>
        public int HighScore { get => highScore; private set => highScore = value > highScore ? value : highScore; }

        /// <summary>
        /// Allows the lowest score to be retrieved and modified, with it only being updated if it is truly the lowest score given.
        /// </summary>
        public int LowestScore { get => lowestScore; private set => lowestScore = value < highScore ? value : highScore; }

        /// <summary>
        /// An auto property, holding how many times the AddNewGame method has been run.
        /// </summary>
        public int TimesPlayed { get; private set; }

        /// <summary>
        /// An auto property, holding how many games have been played with a partner.
        /// </summary>
        public int TimesPlayedWithPartner { get; private set; }

        /// <summary>
        /// An auto property, holding how many games have been played against a computer.
        /// </summary>
        public int TimesPlayedWithComputer { get; private set; }

        /// <summary>
        /// An auto-property dictionary, relating the player number to the times won..
        /// </summary>
        public Dictionary<int, int> TimesWonByPlayer { get; } = new Dictionary<int, int>();

        /// <summary>
        /// Adds a new game to the statistics record. Should be run after a game has finished.
        /// </summary>
        /// <param name="statisticsData">A class with points and the option for multiplayer.</param>
        public void AddNewGame(IStatisticsTrackable statisticsData)
        {
            // To find what player won, the current winning player ID and score should be held.
            var currentWinningPlayer = -1;
            var currentWinningPlayerScore = -1;

            // For each player who was playing,
            foreach (var player in statisticsData.Points)
            {
                // If a key doesn't exist for them in the TimesWonByPlayer dictionary, add one with no wins.
                if (!TimesWonByPlayer.ContainsKey(player.Key)) TimesWonByPlayer.Add(player.Key, 0);

                // If the player is in the lead,
                if (player.Value > currentWinningPlayerScore)
                {
                    // Set them as the current winning player.
                    currentWinningPlayer = player.Key;
                    currentWinningPlayerScore = player.Value;
                }

                // Update the highest and lowest scores. As the properties validate themselves, no checks are required.
                HighScore = player.Value;
                LowestScore = player.Value;
            }

            // Enumerate the winner's score in the dictionary
            TimesWonByPlayer[currentWinningPlayer]++;

            // Log that a game has been played (i.e., this method has been executed).
            TimesPlayed++;

            // Log if the game was played against a human or partner.
            if (statisticsData.PlayingAgainstComputer) TimesPlayedWithComputer++;
            else TimesPlayedWithPartner++;
        }

        /// <summary>
        /// Outputs all the held statistics to the console.
        /// </summary>
        public void PrintStatistics()
        {
            // Output the high and low scores
            Console.WriteLine($"High Score: {HighScore}");
            Console.WriteLine($"Lowest Score: {LowestScore}");

            Console.WriteLine();

            // Output the statistics about times played
            Console.WriteLine($"Number of Times Played: {TimesPlayed}");
            Console.WriteLine($"Number of Times Played with a Partner: {TimesPlayedWithPartner}");
            Console.WriteLine($"Number of Times Played with a Computer: {TimesPlayedWithComputer}");

            // For every player that ever played the game, output their number of wins.
            foreach(var player in TimesWonByPlayer)
                Console.WriteLine($"Number of Times Won by Player {player.Key}: {player.Value}");
        }
    }
}
