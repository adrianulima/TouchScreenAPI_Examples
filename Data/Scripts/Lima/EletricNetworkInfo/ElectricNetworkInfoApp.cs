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
    public StatusView BatteryStatus { get; private set; }
    public EntityListView ProductionList { get; private set; }
    public EntityListView ConsumptionList { get; private set; }

    public void CreateElements()
    {
      var windowBar = new FancyWindowBar("Electric Network Info");
      AddChild(windowBar);

      var window = new FancyView();
      window.SetPadding(new Vector4(4));
      window.SetGap(8);
      AddChild(window);

      // Bars
      var barsPanel = new FancyView(FancyView.ViewDirection.Row);
      barsPanel.SetPixels(new Vector2(0, 16 + 24 + 1));
      barsPanel.SetScale(new Vector2(1, 0));
      barsPanel.SetGap(8);
      window.AddChild(barsPanel);

      ConsumptionStatus = new StatusView("Consumption");
      barsPanel.AddChild(ConsumptionStatus);
      ProductionStatus = new StatusView("Production");
      barsPanel.AddChild(ProductionStatus);
      BatteryStatus = new StatusView("Battery Charge");
      barsPanel.AddChild(BatteryStatus);

      // Time Inter Switcher
      var intervalSwitcher = new FancySwitch(new string[] { "5s", "30s", "1m", "10m", "30m", "1h" }, 0, (int v) =>
      {

      });
      window.AddChild(intervalSwitcher);

      // Chart Panel
      var chartConsumption = new ChartView();
      window.AddChild(chartConsumption);

      // Entities Panel
      var entitiesPanel = new FancyView(FancyView.ViewDirection.Row);
      entitiesPanel.SetGap(4);
      window.AddChild(entitiesPanel);

      var color = GetTheme().GetColorMain();
      ConsumptionList = new EntityListView("Consumption", color, 3);
      ConsumptionList.SetScale(new Vector2(1, 1));
      entitiesPanel.AddChild(ConsumptionList);

      ProductionList = new EntityListView("Production", color, 1);
      ProductionList.SetScale(new Vector2(0.33f, 1));
      entitiesPanel.AddChild(ProductionList);
    }

    public void Update()
    {
      ConsumptionStatus.Value = _electricMan.Consumption;
      ConsumptionStatus.MaxValue = _electricMan.MaxConsumption;
      ProductionStatus.Value = _electricMan.Production;
      ProductionStatus.MaxValue = _electricMan.MaxProduction;
      BatteryStatus.Value = _electricMan.BatteryCharge;
      BatteryStatus.MaxValue = _electricMan.BatteryMaxCharge;

      ConsumptionStatus.UpdateValues();
      ProductionStatus.UpdateValues();
      BatteryStatus.UpdateValues();

      var color = GetTheme().GetColorMain();

      ProductionList.ScrollViewBgColor = new Color(color * 0.05f, 1);
      ProductionList.RemoveAllChildren();

      var productionList = _electricMan.ProductionBlocks.ToList();
      productionList.Sort((pair1, pair2) => pair2.Value.Y.CompareTo(pair1.Value.Y));
      foreach (var item in productionList)
      {
        var entity = new EntityItem(item.Key, color);
        entity.Count = (int)item.Value.X;
        entity.Value = item.Value.Y;
        entity.MaxValue = _electricMan.Production;
        entity.UpdateValues();
        entity.SetBgColor(new Color(color * 0.3f, 1));
        ProductionList.AddItem(entity);
      }

      ConsumptionList.ScrollViewBgColor = new Color(color * 0.05f, 1);
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