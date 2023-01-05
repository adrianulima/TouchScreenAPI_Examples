using System;
using VRage;


namespace Lima
{
  using Interval = MyTuple<string, int, ElectricNetworkManager.PowerStats[]>;

  public class PowerStatsHistory
  {
    public readonly int Count;

    public Interval[] Intervals { get; private set; }
    public bool[] UpdatedLastIndex { get; private set; }

    public PowerStatsHistory(int samples = 30)
    {
      Count = samples;

      Intervals = new Interval[5] {
        new Interval("30s", 30 / Count, new ElectricNetworkManager.PowerStats[Count]),
        new Interval("1m", 60 / Count, new ElectricNetworkManager.PowerStats[Count]),
        new Interval("5m", (5 * 60) / Count, new ElectricNetworkManager.PowerStats[Count]),
        new Interval("10m", (10 * 60) / Count, new ElectricNetworkManager.PowerStats[Count]),
        new Interval("30m", (30 * 60) / Count, new ElectricNetworkManager.PowerStats[Count])
      };
      UpdatedLastIndex = new bool[] { false, false, false, false, false };
    }

    public void Dispose()
    {
      Intervals = null;
      UpdatedLastIndex = null;
    }

    int _index = 0;
    public void Add(ElectricNetworkManager.PowerStats powerStats)
    {
      for (int i = 0; i < Intervals.Length; i++)
        if (UpdatedLastIndex[i] = _index % Intervals[i].Item2 == 0)
          ShiftAndAdd(Intervals[i].Item3, powerStats);

      _index++;
      // TODO: Reset index
    }

    private void ShiftAndAdd(ElectricNetworkManager.PowerStats[] array, ElectricNetworkManager.PowerStats powerStats)
    {
      var len = array.Length;
      Array.Copy(array, 1, array, 0, len - 1);
      array[len - 1] = powerStats;
      // var len = array.Length;
      // Array.Copy(array, 0, array, 1, len - 1);
      // array[0] = powerStats;
    }
  }
}