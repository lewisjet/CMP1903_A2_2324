using System;
using System.Threading;
using CMP1903_A2_2324.Interfaces;

namespace CMP1903_A2_2324.Games
{
    internal class SevensOut : Game, IPointsTracking
    {
        public int[] Points { get; } = new int[2];

        public SevensOut(bool playAgainstComputer) : base(2, playAgainstComputer)
        {
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
            Console.WriteLine($"======== Player {playerNumber + 1}'s Turn {(isComputer ? "[Computer]" : "[Human]")} ========");
            for (var i = 0; i < Dice.Length; i++)
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
    }
}