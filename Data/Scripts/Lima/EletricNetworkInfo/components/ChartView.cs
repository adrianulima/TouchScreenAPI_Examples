using VRageMath;
using Lima.API;
using System.Collections.Generic;
using System;
using VRage.Game.GUI.TextPanel;

namespace Lima
{
  public class ChartView : FancyView
  {
    private PowerStatsHistory _history;

    private FancyChart _chart;
    private FancyView _chartView;
    private FancyView _legendsView;
    private FancyView _labelsWrapper;
    private List<float[]> _dataSets;
    private LegendItem[] _legends;
    private FancyLabel[] _labels;
    private FancySwitch _intervalSwitcher;
    private FancyCheckbox _batteryCheckbox;

    private bool _batteryOutputAsProduction = true;
    public bool BatteryOutputAsProduction
    {
      get { return _batteryOutputAsProduction; }
      set
      {
        if (_batteryOutputAsProduction == value)
          return;
        _batteryOutputAsProduction = value;
        _batteryCheckbox.Value = _batteryOutputAsProduction;
        UpdateChartDataSets();
        OnChangeConfig();
      }
    }

    private int _chartIntervalIndex = 0;
    public int ChartIntervalIndex
    {
      get { return _chartIntervalIndex; }
      set
      {
        if (_chartIntervalIndex == value)
          return;
        _chartIntervalIndex = value;
        _intervalSwitcher.Index = _chartIntervalIndex;
        UpdateChartDataSets();
        OnChangeConfig();
      }
    }

    public Action OnChangeConfig;

    public ChartView(Action onChangeConfig, string[] intervalNames) : base(ViewDirection.Column)
    {
      OnChangeConfig = onChangeConfig;

      CreateElements(intervalNames);

      RegisterUpdate(Update);
    }

    private void Update()
    {
      var bgColor = App.Theme.GetMainColorDarker(2);
      var color = App.Theme.MainColor;

      _chartView.BorderColor = bgColor;
      _chartView.Border = new Vector4(1);
      _legendsView.BgColor = bgColor;

      foreach (var leg in _legends)
        leg.UpdateTitleColor(color);

      var min = _chart.MinValue;
      if (min != float.MaxValue)
      {
        var len = _labels.Length;
        var interval = (_chart.MaxValue - min) / (len - 1);
        if (interval > 0)
        {
          for (int i = 0; i < len; i++)
          {
            _labels[i].Enabled = true;
            _labels[i].TextColor = color;
            _labels[i].Text = $"{ElectricNetworkInfoApp.PowerFormat(min + interval * (len - 1 - i), "0")}";
          }

          var gap = (_labelsWrapper.GetSize().Y - (len * _labels[0].GetSize().Y)) / (len - 1);
          _labelsWrapper.Gap = (int)gap;
        }
      }
    }

    private void CreateElements(string[] intervalNames)
    {
      _intervalSwitcher = new FancySwitch(intervalNames, _chartIntervalIndex, (int v) =>
      {
        ChartIntervalIndex = v;
      });
      AddChild(_intervalSwitcher);

      _chartView = new FancyView(ViewDirection.Row);
      _chartView.Padding = new Vector4(4);
      AddChild(_chartView);

      _labelsWrapper = new FancyView(ViewDirection.Column);
      _labelsWrapper.Scale = new Vector2(0, 1);
      _labelsWrapper.Pixels = new Vector2(50, 0);
      _chartView.AddChild(_labelsWrapper);

      _labels = new FancyLabel[5];
      _labels[0] = new FancyLabel("1000 MW");
      _labels[0].FontSize = 0.4f;
      _labels[0].Enabled = false;
      _labels[0].Alignment = TextAlignment.RIGHT;
      _labelsWrapper.AddChild(_labels[0]);
      _labels[1] = new FancyLabel("1000 MW");
      _labels[1].FontSize = 0.4f;
      _labels[1].Enabled = false;
      _labels[1].Alignment = TextAlignment.RIGHT;
      _labelsWrapper.AddChild(_labels[1]);
      _labels[2] = new FancyLabel("1000 MW");
      _labels[2].FontSize = 0.4f;
      _labels[2].Enabled = false;
      _labels[2].Alignment = TextAlignment.RIGHT;
      _labelsWrapper.AddChild(_labels[2]);
      _labels[3] = new FancyLabel("1000 MW");
      _labels[3].FontSize = 0.4f;
      _labels[3].Enabled = false;
      _labels[3].Alignment = TextAlignment.RIGHT;
      _labelsWrapper.AddChild(_labels[3]);
      _labels[4] = new FancyLabel("1000 MW");
      _labels[4].FontSize = 0.4f;
      _labels[4].Enabled = false;
      _labels[4].Alignment = TextAlignment.RIGHT;
      _labelsWrapper.AddChild(_labels[4]);

      var chartWrapper = new FancyView(ViewDirection.Row);
      chartWrapper.Padding = new Vector4(4, 4, 0, 4);
      _chartView.AddChild(chartWrapper);

      var intervals = 30;
      _chart = new FancyChart(intervals);
      _dataSets = _chart.DataSets;
      _dataSets.Add(null);
      _dataSets.Add(null);
      _dataSets.Add(null);
      _dataSets.Add(null);
      _chart.DataColors.Add(Color.FloralWhite);
      _chart.DataColors.Add(Color.DarkViolet);
      _chart.DataColors.Add(Color.SlateGray);
      _chart.DataColors.Add(Color.OrangeRed);
      _chart.GridHorizontalLines = 5;
      _chart.GridVerticalLines = 13;
      chartWrapper.AddChild(_chart);

      _legendsView = new FancyView(ViewDirection.Row);
      _legendsView.Padding = new Vector4(8, 2, 4, 2);
      _legendsView.Scale = new Vector2(1, 0);
      _legendsView.Pixels = new Vector2(0, 20);
      AddChild(_legendsView);

      _legends = new LegendItem[4];
      _legends[0] = new LegendItem("Consumption", Color.OrangeRed);
      _legends[0].Margin = Vector4.UnitY * 2;
      _legendsView.AddChild(_legends[0]);
      _legends[1] = new LegendItem("Max Consum.", Color.SlateGray);
      _legends[1].Margin = Vector4.UnitY * 2;
      _legendsView.AddChild(_legends[1]);
      _legends[2] = new LegendItem("Production", Color.DarkViolet);
      _legends[2].Margin = Vector4.UnitY * 2;
      _legendsView.AddChild(_legends[2]);
      _legends[3] = new LegendItem("Capacity", Color.FloralWhite);
      _legends[3].Margin = Vector4.UnitY * 2;
      _legendsView.AddChild(_legends[3]);

      var checkboxLabel = new FancyLabel("Battery", 0.4f, TextAlignment.RIGHT);
      checkboxLabel.Margin = new Vector4(0, 2, 4, 0);
      checkboxLabel.Scale = new Vector2(0.5f, 0);
      _legendsView.AddChild(checkboxLabel);
      _batteryCheckbox = new FancyCheckbox((bool v) =>
      {
        BatteryOutputAsProduction = v;
        UpdateChartDataSets();
      }, BatteryOutputAsProduction);
      _batteryCheckbox.Pixels = new Vector2(16);
      _legendsView.AddChild(_batteryCheckbox);
    }

    public void UpdateValues(PowerStatsHistory history)
    {
      _history = history;

      // Prevent calling these methods every second if they won't give different results
      if (_history.UpdatedLastIndex[ChartIntervalIndex])
        UpdateChartDataSets();
    }

    private void UpdateChartDataSets()
    {
      if (_history == null) return;

      var subSet = _history.Intervals[ChartIntervalIndex].Item3;
      var len = subSet.Length;

      _dataSets[0] = new float[len];
      _dataSets[1] = new float[len];
      _dataSets[2] = new float[len];
      _dataSets[3] = new float[len];
      for (int i = 0; i < len; i++)
      {
        if (BatteryOutputAsProduction)
        {
          _dataSets[0][i] = subSet[i].MaxProduction + subSet[i].BatteryMaxOutput;
          _dataSets[1][i] = subSet[i].Production + subSet[i].BatteryOutput;
        }
        else
        {
          _dataSets[0][i] = subSet[i].MaxProduction;
          _dataSets[1][i] = subSet[i].Production;
        }
        _dataSets[2][i] = subSet[i].MaxConsumption;
        _dataSets[3][i] = subSet[i].Consumption;
      }
    }
  }
}