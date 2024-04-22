using System.Collections.Generic;

namespace CMP1903_A2_2324.Interfaces
{
    /// <summary>
    /// An interface, implemented by games so their statistics can be tracked by the Statistics class.
    /// </summary>
    public interface IStatisticsTrackable
    {
        Dictionary<int, int> Points { get; }
        bool PlayingAgainstComputer { get; set; }
    }
}