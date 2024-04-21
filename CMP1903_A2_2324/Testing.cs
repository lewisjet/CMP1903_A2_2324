using CMP1903_A2_2324.Games;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMP1903_A2_2324
{
    internal class Testing
    {
        Game GameInstance { get; }
        public bool TestPassed { get; private set; } = true;

        public Testing(Type gameType)
        {
            try
            {
                GameInstance = (Game) Activator.CreateInstance(gameType, new object[] { true });
            }
            catch (InvalidCastException)
            {
                throw new InvalidCastException("The specified class could not be tested, as it does not derive from the Game class.");
            }
            catch (MissingMethodException)
            {
                throw new Exception("The specified game could not be tested, as it does not have a constructor taking one argument.");
            }
        }

        public void RunTests()
        {
            if (GameInstance.GetType() == typeof(SevensOut)) RunTests(OnSevensOutTurn);
            else if (GameInstance.GetType() == typeof(ThreeOrMore)) RunTests(OnThreeOrMore);
            else throw new NotImplementedException("The specified game could not be tested, as its tests have not been implemented.");
        }

        private void RunTests(Game.TurnCompletedHandler handler)
        {
            Console.WriteLine("Please play the game as you would if you were playing against a computer. The tests will run automatically.");
            GameInstance.OnTurnCompleted += handler;

            GameInstance.PlayGame();
        }

        private bool TestCondition(bool condition, string message)
        {
            Debug.Assert(condition, message);
            TestPassed = TestPassed && condition;
            return condition;
        }

        private void OnSevensOutTurn(int playerNumber, int gainedScore, int score, int[] diceRolls)
        {
            TestCondition(gainedScore != 7, "The game terminates with a score of seven.");
            if(!TestCondition(diceRolls.Length == 2, "The game is using two dice")) return;

            var sum = diceRolls[0] + diceRolls[1];
            TestCondition((diceRolls[0] == diceRolls[1] ? sum * 2 : sum) == gainedScore, "The correct score is being given");

        }

        private void OnThreeOrMore(int playerNumber, int gainedScore, int score, int[] diceRolls)
        {
            TestCondition(diceRolls.Length == 5, "The game is using five dice");

            var rollTotals = new Dictionary<int, int>();
            foreach( var roll in diceRolls)
            {
                if(!rollTotals.ContainsKey(roll)) rollTotals.Add(roll, 1);
                else rollTotals[roll]++;
            }

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

            TestCondition(validatedScore == gainedScore, "The points gained is correct.");
            TestCondition(score - gainedScore < 20, "The game terminates after 20 points are scored.");
            TestCondition(gainedScore - score <= 0, "The score is added correctly to the player.");

        }
    }
}
