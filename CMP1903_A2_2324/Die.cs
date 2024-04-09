using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMP1903_A1_2324
{
    internal class Die
    {
        // Declare a constant integer, defining how many sides the die has.
        private const int SidesOnDie = 6;
        
        /*
         * The Die class should contain one property to hold the current die value,
         * and one method that rolls the die, returns and integer and takes no parameters.
         */
        
        /// <summary>
        /// _currentValue holds the result of the last die roll. If the die hasn't been rolled, 0 will be given.
        /// </summary>
        private int _currentValue;
        
        /// <summary>
        /// CurrentValue is a property, exposing _currentValue publicly whilst disallowing it's modification
        /// unless by the class itself or its children.
        /// </summary>
        public int CurrentValue
        {
            // Returns _currentValue when the property is accessed
            get => _currentValue;
            
            // If the property is modified by itself or it's children, _currentValue is modified instead.
            protected set => _currentValue = value; 
        }

        /// <summary>
        /// Picks a random number between 1 and the number of sides on the dice (i.e. 6).
        /// This number is then written to the current die value and is returned.
        /// </summary>
        /// <returns>The new current die value.</returns>
        public int Roll()
        {
            // Select a random number between 1 (inclusive) and 7 (exclusive)
            var newDieValue = Program.RandomInstance.Next(1, SidesOnDie + 1);
            
            // Write the new die value to the _currentValue
            _currentValue = newDieValue;
            
            // Return the new die value
            return newDieValue;
        }
    }
}
