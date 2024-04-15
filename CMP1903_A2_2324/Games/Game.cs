using System;
using System.Collections.Generic;
using System.Linq;
using CMP1903_A2_2324;

namespace CMP1903_A2_2324.Games
{
    internal abstract class Game
    {
        protected const int FirstPlayerNumber = 1;
        protected const int SecondaryPlayerNumber = 2;
        
        protected List<Die> Dice { get; private set; }
        protected Statistics GameStatistics { get; } = new Statistics();
        public abstract string Name { get; }

        public bool PlayingAgainstComputer { get; set; }

        protected abstract bool CompleteComputerTurn(int playerNumber);
        protected abstract bool CompletePlayerTurn(int playerNumber);

        public static Game PromptUserForGame(IEnumerable<Game> games)
        {
            var gamesArr = games.ToArray();

            Console.WriteLine("Please press the number corresponding to the game you would like to play.");
            for(int i = 0; i < gamesArr.Length; i++)
            {
                Console.WriteLine($"{i + 1}) {gamesArr[i].Name}");
            }

            while (true)
            {
                var keypress = Console.ReadKey();
                Console.WriteLine();

                for (int i = 0; i < gamesArr.Length; i++) 
                {
                    if(keypress.KeyChar.ToString() == (i + 1).ToString())
                        return gamesArr[i];
                }
                
                Console.WriteLine("Your input was invalid. Please press a valid number key.");
            }
        }

        public virtual void PrintStatistics() => GameStatistics.PrintStatistics();

        public virtual void PlayGame()
        {
            while (true)
            {
                if(CompletePlayerTurn(FirstPlayerNumber)) return;
            
                if(PlayingAgainstComputer ?
                       CompleteComputerTurn(SecondaryPlayerNumber) :
                       CompletePlayerTurn(SecondaryPlayerNumber))
                    return;
            }
        }

        protected Game(int numberOfDice, bool playAgainstComputer)
        {
            PlayingAgainstComputer = playAgainstComputer;

            Dice = new List<Die>();
            for(int i = 0; i < numberOfDice; i++) Dice.Add(new Die());
        }
    }
}