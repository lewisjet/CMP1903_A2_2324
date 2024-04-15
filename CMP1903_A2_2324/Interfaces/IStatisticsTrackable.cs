using System.Collections.Generic;

namespace CMP1903_A2_2324.Interfaces
{
    public interface IStatisticsTrackable
    {
        Dictionary<int, int> Points { get; }
        bool PlayingAgainstComputer { get; set; }
    }
}