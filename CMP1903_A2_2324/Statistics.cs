using CMP1903_A2_2324.Interfaces;
using System;
using System.Collections.Generic;

namespace CMP1903_A2_2324
{
    internal class Statistics
    {
        private int highScore;
        private int lowestScore;

        public int HighScore { get => highScore; private set => highScore = value > highScore ? value : highScore; }
        public int LowestScore { get => lowestScore; private set => lowestScore = value < highScore ? value : highScore; }
        public int TimesPlayed { get; private set; }
        public int TimesPlayedWithPartner { get; private set; }
        public int TimesPlayedWithComputer { get; private set; }
        public Dictionary<int, int> TimesWonByPlayer { get; } = new Dictionary<int, int>();

        public void AddNewGame(IStatisticsTrackable statisticsData)
        {
            var currentWinningPlayer = -1;
            var currentWinningPlayerScore = -1;
            foreach (var player in statisticsData.Points)
            {
                if (!TimesWonByPlayer.ContainsKey(player.Key)) TimesWonByPlayer.Add(player.Key, 0);

                if (player.Value > currentWinningPlayerScore)
                {
                    currentWinningPlayer = player.Key;
                    currentWinningPlayerScore = player.Value;
                }

                HighScore = player.Value;
                LowestScore = player.Value;
            }

            TimesWonByPlayer[currentWinningPlayer]++;
            TimesPlayed++;

            if (statisticsData.PlayingAgainstComputer) TimesPlayedWithComputer++;
            else TimesPlayedWithPartner++;
        }

        public void PrintStatistics()
        {
            Console.WriteLine($"High Score: {HighScore}");
            Console.WriteLine($"Lowest Score: {LowestScore}");

            Console.WriteLine();

            Console.WriteLine($"Number of Times Played: {TimesPlayed}");
            Console.WriteLine($"Number of Times Played with a Partner: {TimesPlayedWithPartner}");
            Console.WriteLine($"Number of Times Played with a Computer: {TimesPlayedWithComputer}");

            foreach(var player in TimesWonByPlayer)
                Console.WriteLine($"Number of Times Won by Player {player.Key}: {player.Value}");
        }
    }
}
