using VRageMath;
using Lima.API;
using VRage;
using System;

namespace Lima
{
  public class OverviewPanel : FancyView
  {
    public Action OnChangeConfig;

    public OverviewPanel(Action onChangeConfig)
    {
      OnChangeConfig = onChangeConfig;
    }

    public StatusView ConsumptionStatus { get; private set; }
    public StatusView ProductionStatus { get; private set; }
    public StatusView BatteryOutputStatus { get; private set; }
    public BatteryStorageView BatteryStorageView { get; private set; }
    public ChartView ChartPanel { get; private set; }

    public void CreateElements(PowerStatsHistory history)
    {
      Padding = new Vector4(4);
      Gap = 4;

      // Bars
      var barsPanel = new FancyView(ViewDirection.Row);
      barsPanel.Pixels = new Vector2(0, 32 * 0.4f + 24);
      barsPanel.Scale = new Vector2(1, 0);
      barsPanel.Gap = 4;
      AddChild(barsPanel);

      ConsumptionStatus = new StatusView("CONSUMPTION");
      barsPanel.AddChild(ConsumptionStatus);
      ProductionStatus = new StatusView("PRODUCTION");
      barsPanel.AddChild(ProductionStatus);
      BatteryOutputStatus = new StatusView("BATTERY OUTPUT");
      barsPanel.AddChild(BatteryOutputStatus);

      var batteryAndChartPanel = new FancyView(ViewDirection.Row);
      batteryAndChartPanel.Gap = 4;
      AddChild(batteryAndChartPanel);

      // Chart Panel
      string[] intervalNames = new string[history.Intervals.Length];
      for (int i = 0; i < intervalNames.Length; i++)
        intervalNames[i] = history.Intervals[i].Item1;
      ChartPanel = new ChartView(OnChangeConfig, intervalNames);
      batteryAndChartPanel.AddChild(ChartPanel);

      // Battery Storage bar
      BatteryStorageView = new BatteryStorageView();
      batteryAndChartPanel.AddChild(BatteryStorageView);
    }

    public void ApplySettings(AppContent content, ElectricNetworkManager electricMan)
    {
      ChartPanel.UpdateValues(electricMan.History);
      ChartPanel.BatteryOutputAsProduction = content.BatteryChartEnabled;
      ChartPanel.ChartIntervalIndex = content.ChartIntervalIndex;

      if (content.ChartDataColors != null)
      {
        for (int i = 0; i < 4; i++)
          ChartPanel.DataColors[i] = content.ChartDataColors[i];
        ChartPanel.ApplyColors();
      }
    }

    public void UpdateValues(ElectricNetworkManager electricMan)
    {
      ConsumptionStatus.Value = electricMan.CurrentPowerStats.Consumption;
      ConsumptionStatus.MaxValue = electricMan.CurrentPowerStats.MaxConsumption;
      ProductionStatus.Value = electricMan.CurrentPowerStats.Production;
      ProductionStatus.MaxValue = electricMan.CurrentPowerStats.MaxProduction;
      BatteryOutputStatus.Value = electricMan.CurrentPowerStats.BatteryOutput;
      BatteryOutputStatus.MaxValue = electricMan.CurrentPowerStats.BatteryMaxOutput;
      BatteryStorageView.Value = electricMan.CurrentBatteryStats.BatteryCharge;
      BatteryStorageView.MaxValue = electricMan.CurrentBatteryStats.BatteryMaxCharge;
      BatteryStorageView.HoursLeft = electricMan.CurrentBatteryStats.BatteryHoursLeft;
      BatteryStorageView.UpdateOverloadStatus(electricMan.CurrentBatteryStats.EnergyState == MyResourceStateEnum.OverloadBlackout);

      var inputRatio = electricMan.CurrentBatteryStats.BatteryInput / electricMan.CurrentBatteryStats.BatteryMaxInput;
      BatteryStorageView.InputRatio = float.IsNaN(inputRatio) ? 0f : inputRatio;
      var outputRatio = electricMan.CurrentPowerStats.BatteryOutput / electricMan.CurrentPowerStats.BatteryMaxOutput;
      BatteryStorageView.OutputRatio = float.IsNaN(outputRatio) ? 0f : outputRatio;

      ConsumptionStatus.UpdateValues();
      ProductionStatus.UpdateValues();
      BatteryOutputStatus.UpdateValues();
      BatteryStorageView.UpdateValues();
      ChartPanel.UpdateValues(electricMan.History);
    }

    public void Dispose()
    {
      OnChangeConfig = null;
    }
  }
}