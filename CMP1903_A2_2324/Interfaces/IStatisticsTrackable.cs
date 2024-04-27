using System.Collections.Generic;

namespace CMP1903_A2_2324.Interfaces
{
    /// <summary>
    /// An interface, implemented by games so their statistics can be tracked by the Statistics class.
    /// </summary>
    public interface IStatisticsTrackable
    {
        /// <summary>
        /// A dictionary, correlating Player IDs to the number of points the player earned.
        /// </summary>
        Dictionary<int, int> Points { get; }
        
        /// <summary>
        /// If the game is against a human or computer.
        /// </summary>
        bool PlayingAgainstComputer { get; }
    }
}