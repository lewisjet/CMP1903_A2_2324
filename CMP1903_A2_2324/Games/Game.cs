using System;
using System.Collections.Generic;
using System.Linq;
using CMP1903_A2_2324;

namespace CMP1903_A2_2324.Games
{
    internal abstract class Game
    {
        protected const int FirstPlayerNumber = 0;
        protected const int SecondaryPlayerNumber = 1;
        
        protected Die[] Dice { get; private set; }

        public bool PlayingAgainstComputer { get; private set; }

        protected abstract bool CompleteComputerTurn(int playerNumber);
        protected abstract bool CompletePlayerTurn(int playerNumber);

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
            Dice = new Die[numberOfDice].Select(_ => new Die()).ToArray();
        }
    }
}