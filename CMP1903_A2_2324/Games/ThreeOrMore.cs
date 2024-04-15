using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using CMP1903_A2_2324.Interfaces;

namespace CMP1903_A2_2324.Games
{
    internal class ThreeOrMore : Game, IPointsTracking
    {
        public int[] Points { get; } = new int[2];
        
        public ThreeOrMore(bool playAgainstComputer) : base(5, playAgainstComputer)
        {
        }

        private bool GetTwoOfKindInput()
        {
            Console.WriteLine($"You rolled {string.Join(", ", Dice.Select(i => i.CurrentValue.ToString()))}, which contains a pair of two.");
            Console.WriteLine($"Would you like to roll all your dice again, or only the three remaining dice?");
            Console.Write($"Type \"all\" to roll all dice again, or type anything else to only roll the remaining: ");
            var input = Console.ReadLine();
            return input != null && input.ToLower() == "all";
        }

        private void HandleTwoOfKind(IEnumerable<int> diceValues, int valueTwoOf, int playerNumber, bool isComputer)
        {
            var shouldGlobalRoll = (isComputer && diceValues.Max() < 3) || (!isComputer && GetTwoOfKindInput());

            if (shouldGlobalRoll)
            {
                foreach (var die in Dice) die.Roll();
            }
            else foreach(var die in Dice)
                if (die.CurrentValue != valueTwoOf)
                    die.Roll();
            
            AddPointsToAward(playerNumber, true);
        }

        private void AddPointsToAward(int playerNumber, bool isComputer)
        {
            var dieValues = new int[Die.SidesOnDie];
            foreach (var die in Dice)
            {
                var roll = die.CurrentValue;
                dieValues[roll - 1]++;
            }

            for (var index = 0; index < dieValues.Length; index++)
            {
                var value = dieValues[index];
                if (value != 2) continue;
                
                HandleTwoOfKind(dieValues, index + 1, playerNumber, isComputer);
                return;

            }

            foreach (var value in dieValues)
            {
                var pointsToAward = 0;
                
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

                Points[playerNumber] += pointsToAward;
            }
        }
        
        private void RollDice(int playerNumber, bool isComputer)
        {
            foreach (var die in Dice) die.Roll();

            AddPointsToAward(playerNumber, isComputer);
        }
        
        protected override bool CompleteComputerTurn(int playerNumber)
        {
            Console.WriteLine($"======== Player {playerNumber + 1}'s Turn [Computer] ========");
            RollDice(playerNumber, true);

            Console.WriteLine($"The computer rolled: {string.Join(", ", Dice.Select(i => i.CurrentValue.ToString()))}, bringing them to {Points[playerNumber]} points.");
            Console.WriteLine();
            Thread.Sleep(1000);
            
            return Points[playerNumber] >= 20;
        }
        
        protected override bool CompletePlayerTurn(int playerNumber)
        {
            Console.WriteLine($"======== Player {playerNumber + 1}'s Turn [Player] ========");
            RollDice(playerNumber, false);
            
            Console.WriteLine($"Player {playerNumber + 1} rolled: {string.Join(", ", Dice.Select(i => i.CurrentValue.ToString()))}, bringing them up to {Points[playerNumber]} points.");
            Console.WriteLine("Press any key to continue.");
            Console.WriteLine();
            Console.ReadKey();
            
            return Points[playerNumber] >= 20;
        }

    }
}