using System;
using System.Collections.Generic;
using VRageMath;
using Lima.API;
using System.Linq;
using VRage;
using VRage.Utils;
using System.Text;

namespace Lima2
{
  public class ElectricNetworkInfoApp : FancyApp
  {
    private ElectricNetworkManager _electricMan = new ElectricNetworkManager();

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
      windowBarsAndChard.SetPadding(new Vector4(4));
      windowBarsAndChard.SetGap(4);
      AddChild(windowBarsAndChard);

      // Bars
      var barsPanel = new FancyView(FancyView.ViewDirection.Row);
      barsPanel.SetPixels(new Vector2(0, 32 * 0.4f + 24));
      barsPanel.SetScale(new Vector2(1, 0));
      barsPanel.SetGap(4);
      windowBarsAndChard.AddChild(barsPanel);

      ConsumptionStatus = new StatusView("CONSUMPTION");
      barsPanel.AddChild(ConsumptionStatus);
      ProductionStatus = new StatusView("PRODUCTION");
      barsPanel.AddChild(ProductionStatus);
      BatteryOutputStatus = new StatusView("BATTERY OUTPUT");
      barsPanel.AddChild(BatteryOutputStatus);

      var batteryAndChartPanel = new FancyView(FancyView.ViewDirection.Row);
      batteryAndChartPanel.SetGap(4);
      windowBarsAndChard.AddChild(batteryAndChartPanel);

      // Chart Panel
      Charts = new ChartView();
      Charts.SetChartColors(GetTheme().GetColorMainDarker(30), GetTheme().GetColorMain());
      batteryAndChartPanel.AddChild(Charts);

      // Battery Storage bar
      BatteryStorageView = new BatteryStorageView();
      batteryAndChartPanel.AddChild(BatteryStorageView);

      // Entities Panel
      var entitiesPanel = new FancyView(FancyView.ViewDirection.Row);
      entitiesPanel.SetPadding(new Vector4(4));
      entitiesPanel.SetGap(4);
      AddChild(entitiesPanel);

      var bgColor = GetTheme().GetColorMainDarker(10);

      ConsumptionList = new EntityListView("INPUT", 3);
      ConsumptionList.SetScrollViewBgColor(bgColor);
      ConsumptionList.SetScale(new Vector2(3, 1));
      entitiesPanel.AddChild(ConsumptionList);

      ProductionList = new EntityListView("OUTPUT", 1);
      ProductionList.SetScrollViewBgColor(bgColor);
      ProductionList.SetScale(new Vector2(1, 1));
      entitiesPanel.AddChild(ProductionList);
    }

    public void Update()
    {
      ConsumptionStatus.Value = _electricMan.Consumption;
      ConsumptionStatus.MaxValue = _electricMan.MaxConsumption;
      ProductionStatus.Value = _electricMan.Production;
      ProductionStatus.MaxValue = _electricMan.MaxProduction;
      BatteryOutputStatus.Value = _electricMan.BatteryOutput;
      BatteryOutputStatus.MaxValue = _electricMan.BatteryMaxOutput;
      BatteryStorageView.Value = _electricMan.BatteryCharge;
      BatteryStorageView.MaxValue = _electricMan.BatteryMaxCharge;
      BatteryStorageView.HoursLeft = _electricMan.BatteryHoursLeft;
      BatteryStorageView.UpdateOverloadStyle(_electricMan.EnergyState == MyResourceStateEnum.OverloadBlackout);

      var inputRatio = _electricMan.BatteryInput / _electricMan.BatteryMaxInput;
      BatteryStorageView.InputRatio = float.IsNaN(inputRatio) ? 0f : inputRatio;
      var outputRatio = _electricMan.BatteryOutput / _electricMan.BatteryMaxOutput;
      BatteryStorageView.OutputRatio = float.IsNaN(outputRatio) ? 0f : outputRatio;

      ConsumptionStatus.UpdateValues();
      ProductionStatus.UpdateValues();
      BatteryOutputStatus.UpdateValues();
      BatteryStorageView.UpdateValues();

      Charts.SetChartColors(GetTheme().GetColorMainDarker(20), GetTheme().GetColorMain());
      Charts.UpdateValues(
        _electricMan.Consumption,
        _electricMan.MaxConsumption,
        _electricMan.Production,
        _electricMan.MaxProduction,
        _electricMan.BatteryOutput,
        _electricMan.BatteryMaxOutput
      );

      var bgColor = GetTheme().GetColorMainDarker(10);

      ProductionList.SetScrollViewBgColor(bgColor);
      ProductionList.RemoveAllChildren();

      var productionList = _electricMan.ProductionBlocks.ToList();
      productionList.Sort((pair1, pair2) => pair2.Value.Y.CompareTo(pair1.Value.Y));
      foreach (var item in productionList)
      {
        var entity = new EntityItem(item.Key);
        entity.Count = (int)item.Value.X;
        entity.Value = item.Value.Y;
        entity.MaxValue = _electricMan.Production + _electricMan.BatteryOutput;
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
        entity.MaxValue = _electricMan.Consumption;
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
        return "1 year";

      _str.Clear();
      MyValueFormatter.AppendTimeInBestUnit(hours * 3600f, _str);
      return _str.ToString();
    }
  }
}