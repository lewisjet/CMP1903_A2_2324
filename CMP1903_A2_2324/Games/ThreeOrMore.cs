using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using CMP1903_A2_2324.Interfaces;

namespace CMP1903_A2_2324.Games
{
    internal class ThreeOrMore : Game, IStatisticsTrackable
    {
        public Dictionary<int, int> Points { get; } = new Dictionary<int, int>();

        public override string Name => "Three or More";

        public ThreeOrMore(bool playAgainstComputer = true) : base(5, playAgainstComputer)
        {
            Points.Add(1, 0);
            Points.Add(2, 0);
        }

        private string FormatRolls() => string.Join(", ", Dice.Select(i => i.CurrentValue.ToString()));

        private bool CalculateComputerTwoOfAKind(IEnumerable<int> diceValues)
        {
            Console.WriteLine($"The computer rolled {FormatRolls()}, which contains a pair of two.");

            var shouldRerollAll = diceValues.Max() < 3;

            Thread.Sleep(1000);

            if (shouldRerollAll)
                Console.WriteLine("The computer decided to reroll all dice.");
            else
                Console.WriteLine("The computer decided to reroll the remaining dice.");

            Console.WriteLine();

            return shouldRerollAll;
        }

        private bool GetTwoOfKindInput()
        {
            Console.WriteLine($"You rolled {FormatRolls()}, which contains a pair of two.");
            Console.WriteLine($"Would you like to roll all your dice again, or only the three remaining dice?");
            Console.Write($"Type \"all\" to roll all dice again, or type anything else to only roll the remaining: ");
            var input = Console.ReadLine();
            Console.WriteLine();
            return input != null && input.ToLower() == "all";
        }

        private int HandleTwoOfKind(IEnumerable<int> diceValues, int valueTwoOf, int playerNumber, bool isComputer)
        {

            var shouldGlobalRoll = (isComputer && CalculateComputerTwoOfAKind(diceValues)) 
                || (!isComputer && GetTwoOfKindInput());

            if (shouldGlobalRoll)
            {
                foreach (var die in Dice) die.Roll();
            }
            else foreach(var die in Dice)
                if (die.CurrentValue != valueTwoOf)
                    die.Roll();
            
            return AddPointsToAward(playerNumber, isComputer);
        }

        private int AddPointsToAward(int playerNumber, bool isComputer)
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

                return HandleTwoOfKind(dieValues, index + 1, playerNumber, isComputer);

            }

            var totalPointsGiven = 0;
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
                totalPointsGiven += pointsToAward;
            }
            return totalPointsGiven;
        }
        
        private int RollDiceAndAddPoints(int playerNumber, bool isComputer)
        {
            foreach (var die in Dice) die.Roll();

            return AddPointsToAward(playerNumber, isComputer);
        }
        
        protected override bool CompleteComputerTurn(int playerNumber)
        {
            Console.WriteLine($"======== Player {playerNumber}'s Turn [Computer] ========");
            var newPoints = RollDiceAndAddPoints(playerNumber, true);

            Console.WriteLine($"The computer rolled: {FormatRolls()}, bringing them to {Points[playerNumber]} points.");
            Console.WriteLine();
            Thread.Sleep(1000);

            InvokeTurnCompleted(playerNumber, newPoints, Points[playerNumber]);
            
            return Points[playerNumber] >= 20;
        }
        
        protected override bool CompletePlayerTurn(int playerNumber)
        {
            Console.WriteLine($"======== Player {playerNumber}'s Turn [Player] ========");
            var newPoints =  RollDiceAndAddPoints(playerNumber, false);
            
            Console.WriteLine($"Player {playerNumber} rolled: {FormatRolls()}, bringing them up to {Points[playerNumber]} points.");
            Console.WriteLine("Press any key to continue.");
            Console.WriteLine();
            Console.ReadKey();

            InvokeTurnCompleted(playerNumber, newPoints, Points[playerNumber]);

            return Points[playerNumber] >= 20;
        }

        public override void PlayGame()
        {
            for (int i = 0; i < Points.Count; i++) Points[i] = 0;
            base.PlayGame();
            GameStatistics.AddNewGame(this);
        }
    }
}