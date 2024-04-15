using System;
using System.Collections.Generic;
using System.Threading;
using CMP1903_A2_2324.Interfaces;

namespace CMP1903_A2_2324.Games
{
    internal class SevensOut : Game, IStatisticsTrackable
    {
        public Dictionary<int, int> Points { get; } = new Dictionary<int, int>();

        public override string Name => "Sevens Out";

        public SevensOut(bool playAgainstComputer) : base(2, playAgainstComputer)
        {
            Points.Add(1, 0);
            Points.Add(2, 0);
        }

        private int RollAndSumDice()
        {
            var total = 0;
            var previousRoll = -1;
            var allRollsSame = true;
            
            foreach (var die in Dice)
            {
                var roll = die.Roll();
                
                total += roll;
                
                if (previousRoll != -1 && previousRoll != roll)
                    allRollsSame = false;
                
                previousRoll = roll;
            }

            if (allRollsSame) total *= 2;

            return total;
        }

        private void OutputTurnResults(int playerNumber, int total, bool isComputer)
        {
            Console.WriteLine();
            Console.WriteLine($"======== Player {playerNumber}'s Turn {(isComputer ? "[Computer]" : "[Human]")} ========");
            for (var i = 0; i < Dice.Count; i++)
            {
                Console.WriteLine($"Die {i + 1}: {Dice[i].CurrentValue}");
            }

            Console.WriteLine($"Total: {total}");
            if(total == 7) Console.WriteLine("Sevens out!");
        }
        
        private bool CompleteTurn(int playerNumber, bool isComputer)
        {
            var total = RollAndSumDice();
            OutputTurnResults(playerNumber, total, isComputer);
            if (total == 7) return true;
            Points[playerNumber] += total;
            return false;
        }

        protected override bool CompleteComputerTurn(int playerNumber)
        {
            var complete = CompleteTurn(playerNumber, true);
            Thread.Sleep(1500);
            return complete;
        }

        protected override bool CompletePlayerTurn(int playerNumber)
        {
            var complete = CompleteTurn(playerNumber, false);
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
            return complete;
        }

        public override void PlayGame()
        {
            for(int i = 0; i < Points.Count; i++) Points[i] = 0;
            
            base.PlayGame();
            GameStatistics.AddNewGame(this);
        }
    }
}