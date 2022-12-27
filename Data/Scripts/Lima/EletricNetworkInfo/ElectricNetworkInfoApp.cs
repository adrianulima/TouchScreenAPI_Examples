using System;
using System.Collections.Generic;
using VRageMath;
using Lima.API;
using System.Linq;
using VRage;
using VRage.Utils;
using System.Text;

namespace Lima
{
  public class ElectricNetworkInfoApp : FancyApp
  {
    private ElectricNetworkManager _electricMan;
    private int _lastUpdate = -1;

    public ElectricNetworkInfoApp(ElectricNetworkManager electricManager)
    {
      _electricMan = electricManager;
    }

    public StatusView ConsumptionStatus { get; private set; }
    public StatusView ProductionStatus { get; private set; }
    public StatusView BatteryOutputStatus { get; private set; }
    public BatteryStorageView BatteryStorageView { get; private set; }
    public EntityListView ProductionList { get; private set; }
    public EntityListView ConsumptionList { get; private set; }
    public ChartView Charts { get; private set; }

    public void CreateElements()
    {
      var windowBar = new FancyWindowBar("Electric Network Info");
      AddChild(windowBar);

      var windowBarsAndChard = new FancyView();
      windowBarsAndChard.Padding = new Vector4(4);
      windowBarsAndChard.Gap = 4;
      AddChild(windowBarsAndChard);

      // Bars
      var barsPanel = new FancyView(FancyView.ViewDirection.Row);
      barsPanel.Pixels = new Vector2(0, 32 * 0.4f + 24);
      barsPanel.Scale = new Vector2(1, 0);
      barsPanel.Gap = 4;
      windowBarsAndChard.AddChild(barsPanel);

      ConsumptionStatus = new StatusView("CONSUMPTION");
      barsPanel.AddChild(ConsumptionStatus);
      ProductionStatus = new StatusView("PRODUCTION");
      barsPanel.AddChild(ProductionStatus);
      BatteryOutputStatus = new StatusView("BATTERY OUTPUT");
      barsPanel.AddChild(BatteryOutputStatus);

      var batteryAndChartPanel = new FancyView(FancyView.ViewDirection.Row);
      batteryAndChartPanel.Gap = 4;
      windowBarsAndChard.AddChild(batteryAndChartPanel);

      // Chart Panel
      Charts = new ChartView();
      batteryAndChartPanel.AddChild(Charts);

      // Battery Storage bar
      BatteryStorageView = new BatteryStorageView();
      batteryAndChartPanel.AddChild(BatteryStorageView);

      // Entities Panel
      var entitiesPanel = new FancyView(FancyView.ViewDirection.Row);
      entitiesPanel.Padding = new Vector4(4);
      entitiesPanel.Gap = 4;
      AddChild(entitiesPanel);

      var bgColor = Theme.GetMainColorDarker(1);

      ConsumptionList = new EntityListView("INPUT", 3);
      ConsumptionList.SetScrollViewBgColor(bgColor);
      ConsumptionList.Scale = new Vector2(3, 1);
      entitiesPanel.AddChild(ConsumptionList);

      ProductionList = new EntityListView("OUTPUT", 1);
      ProductionList.SetScrollViewBgColor(bgColor);
      ProductionList.Scale = new Vector2(1, 1);
      entitiesPanel.AddChild(ProductionList);
    }

    public void ApplySettings(FileHandler.AppContent content)
    {
      Charts.BatteryOutputAsProduction = content.BatteryChartEnabled;
      Charts.ChartIntervalIndex = content.ChartIntervalIndex;
    }

    public void UpdateValues()
    {
      if (_electricMan.Updatecount <= _lastUpdate)
        return;
      _lastUpdate = _electricMan.Updatecount;

      ConsumptionStatus.Value = _electricMan.CurrentPowerStats.Consumption;
      ConsumptionStatus.MaxValue = _electricMan.CurrentPowerStats.MaxConsumption;
      ProductionStatus.Value = _electricMan.CurrentPowerStats.Production;
      ProductionStatus.MaxValue = _electricMan.CurrentPowerStats.MaxProduction;
      BatteryOutputStatus.Value = _electricMan.CurrentPowerStats.BatteryOutput;
      BatteryOutputStatus.MaxValue = _electricMan.CurrentPowerStats.BatteryMaxOutput;
      BatteryStorageView.Value = _electricMan.CurrentBatteryStats.BatteryCharge;
      BatteryStorageView.MaxValue = _electricMan.CurrentBatteryStats.BatteryMaxCharge;
      BatteryStorageView.HoursLeft = _electricMan.CurrentBatteryStats.BatteryHoursLeft;
      BatteryStorageView.UpdateOverloadStatus(_electricMan.CurrentBatteryStats.EnergyState == MyResourceStateEnum.OverloadBlackout);

      var inputRatio = _electricMan.CurrentBatteryStats.BatteryInput / _electricMan.CurrentBatteryStats.BatteryMaxInput;
      BatteryStorageView.InputRatio = float.IsNaN(inputRatio) ? 0f : inputRatio;
      var outputRatio = _electricMan.CurrentPowerStats.BatteryOutput / _electricMan.CurrentPowerStats.BatteryMaxOutput;
      BatteryStorageView.OutputRatio = float.IsNaN(outputRatio) ? 0f : outputRatio;

      ConsumptionStatus.UpdateValues();
      ProductionStatus.UpdateValues();
      BatteryOutputStatus.UpdateValues();
      BatteryStorageView.UpdateValues();
      Charts.UpdateValues(_electricMan.PowerStatsHistory);

      var bgColor = Theme.GetMainColorDarker(1);

      ProductionList.SetScrollViewBgColor(bgColor);
      ProductionList.RemoveAllChildren();

      var productionList = _electricMan.ProductionBlocks.ToList();
      productionList.Sort((pair1, pair2) => pair2.Value.Y.CompareTo(pair1.Value.Y));
      foreach (var item in productionList)
      {
        var entity = new EntityItem(item.Key);
        entity.Count = (int)item.Value.X;
        entity.Value = item.Value.Y;
        entity.MaxValue = _electricMan.CurrentPowerStats.Production + _electricMan.CurrentPowerStats.BatteryOutput;
        entity.UpdateValues();
        ProductionList.AddItem(entity);
      }
      ProductionList.FillLastView();

      ConsumptionList.SetScrollViewBgColor(bgColor);
      ConsumptionList.RemoveAllChildren();

      var consumptionList = _electricMan.ConsumptionBlocks.ToList();
      consumptionList.Sort((pair1, pair2) => pair2.Value.Y.CompareTo(pair1.Value.Y));
      foreach (var item in consumptionList)
      {
        var entity = new EntityItem(item.Key);
        entity.Count = (int)item.Value.X;
        entity.Value = item.Value.Y;
        entity.MaxValue = _electricMan.CurrentPowerStats.Consumption;
        entity.UpdateValues();
        ConsumptionList.AddItem(entity);
      }
      ConsumptionList.FillLastView();
    }

    public void Dispose()
    {
      ForceDispose();
      ProductionList.Dispose();
      ConsumptionList.Dispose();
    }

    public static string PowerFormat(float MW, string decimals = "0.##")
    {
      if (MW >= 1000000000000)
        return $"{MW.ToString("E2")} MW";
      if (MW >= 1000000000)
        return $"{(MW / 1000000000).ToString(decimals)} PW";
      if (MW >= 1000000)
        return $"{(MW / 1000000).ToString(decimals)} TW";
      if (MW >= 1000)
        return $"{(MW / 1000).ToString(decimals)} GW";
      if (MW >= 1)
        return $"{MW.ToString(decimals)} MW";
      if (MW >= 0.001)
        return $"{(MW * 1000f).ToString(decimals)} kW";
      return $"{(MW * 1000000f).ToString(decimals)} W";
    }

    private readonly static StringBuilder _str = new StringBuilder();
    public static string HoursFormat(float hours, string decimals = "0.##")
    {
      if (hours > 24 * 365)
        return "1 year +";

      _str.Clear();
      MyValueFormatter.AppendTimeInBestUnit(hours * 3600f, _str);
      return _str.ToString();
    }
  }
}