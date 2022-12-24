using System;
using System.Collections.Generic;
using VRageMath;
using Lima.API;
using System.Linq;

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

      ConsumptionStatus = new StatusView("Consumption");
      barsPanel.AddChild(ConsumptionStatus);
      ProductionStatus = new StatusView("Production");
      barsPanel.AddChild(ProductionStatus);
      BatteryOutputStatus = new StatusView("Battery Output");
      barsPanel.AddChild(BatteryOutputStatus);

      var batteryAndChartPanel = new FancyView(FancyView.ViewDirection.Row);
      batteryAndChartPanel.SetGap(4);
      windowBarsAndChard.AddChild(batteryAndChartPanel);

      // Chart Panel
      Charts = new ChartView();
      Charts.SetChartContainerBorder(GetTheme().GetColorMainDarker(20));
      batteryAndChartPanel.AddChild(Charts);

      // Battery Storage bar
      BatteryStorageView = new BatteryStorageView();
      batteryAndChartPanel.AddChild(BatteryStorageView);

      // Entities Panel
      var entitiesPanel = new FancyView(FancyView.ViewDirection.Row);
      entitiesPanel.SetPadding(new Vector4(4));
      entitiesPanel.SetGap(4);
      AddChild(entitiesPanel);

      var color = GetTheme().GetColorMain();

      ConsumptionList = new EntityListView("Input", 3);
      ConsumptionList.SetScrollViewBgColor(new Color(color * 0.05f, 1));
      ConsumptionList.SetScale(new Vector2(3, 1));
      entitiesPanel.AddChild(ConsumptionList);

      ProductionList = new EntityListView("Output", 1);
      ProductionList.SetScrollViewBgColor(new Color(color * 0.05f, 1));
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

      ConsumptionStatus.UpdateValues();
      ProductionStatus.UpdateValues();
      BatteryOutputStatus.UpdateValues();
      BatteryStorageView.UpdateValues();

      Charts.UpdateValues(_electricMan.Consumption, _electricMan.MaxConsumption, _electricMan.Production, _electricMan.MaxProduction);

      var color = GetTheme().GetColorMain();

      ProductionList.SetScrollViewBgColor(new Color(color * 0.05f, 1));
      ProductionList.RemoveAllChildren();

      var productionList = _electricMan.ProductionBlocks.ToList();
      productionList.Sort((pair1, pair2) => pair2.Value.Y.CompareTo(pair1.Value.Y));
      foreach (var item in productionList)
      {
        var entity = new EntityItem(item.Key, color);
        entity.Count = (int)item.Value.X;
        entity.Value = item.Value.Y;
        entity.MaxValue = _electricMan.Production + _electricMan.BatteryOutput;
        entity.UpdateValues();
        entity.SetBgColor(new Color(color * 0.3f, 1));
        ProductionList.AddItem(entity);
      }

      ConsumptionList.SetScrollViewBgColor(new Color(color * 0.05f, 1));
      ConsumptionList.RemoveAllChildren();

      var consumptionList = _electricMan.ConsumptionBlocks.ToList();
      consumptionList.Sort((pair1, pair2) => pair2.Value.Y.CompareTo(pair1.Value.Y));
      foreach (var item in consumptionList)
      {
        var entity = new EntityItem(item.Key, color);
        entity.Count = (int)item.Value.X;
        entity.Value = item.Value.Y;
        entity.MaxValue = _electricMan.Consumption;
        entity.UpdateValues();
        entity.SetBgColor(new Color(color * 0.3f, 1));
        ConsumptionList.AddItem(entity);
      }

      Charts.SetChartContainerBorder(GetTheme().GetColorMainDarker(20));
    }

    public void Dispose()
    {
      ForceDispose();
      ProductionList.Dispose();
      ConsumptionList.Dispose();
    }

    public static string PowerFormat(float MW)
    {
      if (MW >= 1000000000000)
        return $"{MW.ToString("E2")} MW";
      if (MW >= 1000000000)
        return $"{(MW / 1000000000).ToString("0.##")} PW";
      if (MW >= 1000000)
        return $"{(MW / 1000000).ToString("0.##")} TW";
      if (MW >= 1000)
        return $"{(MW / 1000).ToString("0.##")} GW";
      if (MW >= 1)
        return $"{MW.ToString("0.##")} MW";
      if (MW >= 0.001)
        return $"{(MW * 1000f).ToString("0.##")} kW";
      return $"{(MW * 1000000f).ToString("0.##")} W";
    }

    public static string PowerStorageFormat(float MWh)
    {
      return $"{MWh} h";
    }
  }
}