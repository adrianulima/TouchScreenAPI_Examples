using VRageMath;
using Lima.API;
using System.Collections.Generic;
using System;
using VRage.Game.GUI.TextPanel;
using System.Linq;

namespace Lima2
{
  public class ChartView : FancyView
  {
    private List<float> _consumptionHistory = new List<float>();
    private List<float> _maxConsumptionHistory = new List<float>();
    private List<float> _productionHistory = new List<float>();
    private List<float> _maxProductionHistory = new List<float>();
    private List<float> _batteryOutputHistory = new List<float>();
    private List<float> _batteryMaxOutputHistory = new List<float>();

    public int maxSamples = 1800; // 30 min
    private int _skip = 1;

    private FancyChart _chart;
    private FancyView _chartView;
    private FancyView _legendsView;
    private FancyView _labelsWrapper;
    private List<float[]> _dataSets;
    private LegendItem[] _legends;
    private FancyLabel[] _labels;

    public bool BatteryOutputAsProduction = false;

    public ChartView() : base(ViewDirection.Column)
    {
      CreateElements();
    }

    public void SetChartColors(Color bgColor, Color color)
    {
      _chartView.SetBorderColor(bgColor);
      _chartView.SetBorder(new Vector4(1));
      _legendsView.SetBgColor(bgColor);

      foreach (var leg in _legends)
        leg.UpdateTitleColor(color);

      var min = _chart.GetMinValue();
      if (min != float.MaxValue)
      {
        var len = _labels.Length;
        var interval = (_chart.GetMaxValue() - min) / (len - 1);
        if (interval > 0)
        {
          for (int i = 0; i < len; i++)
          {
            _labels[i].SetEnabled(true);
            _labels[i].SetTextColor(color);
            _labels[i].SetText($"{ElectricNetworkInfoApp.PowerFormat(min + interval * (len - 1 - i), "0")}");
          }

          var gap = (_labelsWrapper.GetSize().Y - (len * _labels[0].GetSize().Y)) / (len - 1);
          _labelsWrapper.SetGap((int)gap);
        }
      }
    }

    private void CreateElements()
    {
      var intervalSwitcher = new FancySwitch(new string[] { "30s", "1m", "5m", "10m", "30m" }, 0, (int v) =>
      {
        switch (v)
        {
          case 0:
            _skip = 1;
            break;
          case 1:
            _skip = 2; // 60 / 30;
            break;
          case 2:
            _skip = 10; // (5 * 60) / 30;
            break;
          case 3:
            _skip = 20; // (10 * 60) / 30;
            break;
          case 4:
            _skip = 60; // (30 * 60) / 30;
            break;
          default:
            _skip = 1;
            break;
        }
        UpdateChartDataSets();
      });
      AddChild(intervalSwitcher);

      _chartView = new FancyView(ViewDirection.Row);
      _chartView.SetPadding(new Vector4(4));
      AddChild(_chartView);

      _labelsWrapper = new FancyView(ViewDirection.Column);
      _labelsWrapper.SetScale(new Vector2(0, 1));
      _labelsWrapper.SetPixels(new Vector2(50, 1));
      _chartView.AddChild(_labelsWrapper);

      _labels = new FancyLabel[5];
      _labels[0] = new FancyLabel("1000 MW");
      _labels[0].SetFontSize(0.4f);
      _labels[0].SetEnabled(false);
      _labels[0].SetAlignment(TextAlignment.RIGHT);
      _labelsWrapper.AddChild(_labels[0]);
      _labels[1] = new FancyLabel("1000 MW");
      _labels[1].SetFontSize(0.4f);
      _labels[1].SetEnabled(false);
      _labels[1].SetAlignment(TextAlignment.RIGHT);
      _labelsWrapper.AddChild(_labels[1]);
      _labels[2] = new FancyLabel("1000 MW");
      _labels[2].SetFontSize(0.4f);
      _labels[2].SetEnabled(false);
      _labels[2].SetAlignment(TextAlignment.RIGHT);
      _labelsWrapper.AddChild(_labels[2]);
      _labels[3] = new FancyLabel("1000 MW");
      _labels[3].SetFontSize(0.4f);
      _labels[3].SetEnabled(false);
      _labels[3].SetAlignment(TextAlignment.RIGHT);
      _labelsWrapper.AddChild(_labels[3]);
      _labels[4] = new FancyLabel("1000 MW");
      _labels[4].SetFontSize(0.4f);
      _labels[4].SetEnabled(false);
      _labels[4].SetAlignment(TextAlignment.RIGHT);
      _labelsWrapper.AddChild(_labels[4]);

      var chartWrapper = new FancyView(ViewDirection.Row);
      chartWrapper.SetMargin(new Vector4(4, 8, 0, 8));
      _chartView.AddChild(chartWrapper);

      var intervals = 30;
      _chart = new FancyChart(intervals);
      _dataSets = _chart.GetDataSets();
      _dataSets.Add(null);
      _dataSets.Add(null);
      _dataSets.Add(null);
      _dataSets.Add(null);
      _chart.GetDataColors().Add(Color.DarkSlateGray);
      _chart.GetDataColors().Add(Color.DarkGreen);
      _chart.GetDataColors().Add(Color.SlateGray);
      _chart.GetDataColors().Add(Color.OrangeRed);
      _chart.SetGridHorizontalLines(5);
      _chart.SetGridVerticalLines(13);
      chartWrapper.AddChild(_chart);

      _legendsView = new FancyView(ViewDirection.Row);
      _legendsView.SetPadding(new Vector4(8, 2, 4, 2));
      _legendsView.SetScale(new Vector2(1, 0));
      _legendsView.SetPixels(new Vector2(0, 20));
      AddChild(_legendsView);

      _legends = new LegendItem[4];
      _legends[0] = new LegendItem("Consumption", Color.OrangeRed);
      _legends[0].SetMargin(Vector4.UnitY * 2);
      _legendsView.AddChild(_legends[0]);
      _legends[1] = new LegendItem("Max Consum.", Color.SlateGray);
      _legends[1].SetMargin(Vector4.UnitY * 2);
      _legendsView.AddChild(_legends[1]);
      _legends[2] = new LegendItem("Production", Color.DarkGreen);
      _legends[2].SetMargin(Vector4.UnitY * 2);
      _legendsView.AddChild(_legends[2]);
      _legends[3] = new LegendItem("Max Prod.", Color.DarkSlateGray);
      _legends[3].SetMargin(Vector4.UnitY * 2);
      _legendsView.AddChild(_legends[3]);

      var checkboxLabel = new FancyLabel("Battery", 0.4f, TextAlignment.RIGHT);
      checkboxLabel.SetMargin(new Vector4(0, 2, 4, 0));
      checkboxLabel.SetScale(new Vector2(0.5f, 0));
      _legendsView.AddChild(checkboxLabel);
      var checkbox = new FancyCheckbox((bool v) =>
      {
        BatteryOutputAsProduction = v;
        UpdateChartDataSets();
      }, BatteryOutputAsProduction);
      checkbox.SetPixels(new Vector2(16));
      _legendsView.AddChild(checkbox);
    }

    public void UpdateValues(float consumption, float maxConsumption, float production, float maxProduction, float batteryOutput, float batteryMaxOutput)
    {
      _consumptionHistory.Add(consumption);
      _maxConsumptionHistory.Add(maxConsumption);
      _productionHistory.Add(production);
      _maxProductionHistory.Add(maxProduction);
      _batteryOutputHistory.Add(batteryOutput);
      _batteryMaxOutputHistory.Add(batteryMaxOutput);

      TrimHistoryLimit();

      // Prevent calling these methods every second if they won't give different results
      if (_skip == 1 || _maxProductionHistory.Count % _skip == 0)
        UpdateChartDataSets();
    }

    private void UpdateChartDataSets()
    {
      var count = 30;
      if (BatteryOutputAsProduction)
      {
        _dataSets[0] = SumLists(
          TakeOneEvery(TakeLast(_maxProductionHistory, count * _skip), _skip),
          TakeOneEvery(TakeLast(_batteryMaxOutputHistory, count * _skip), _skip)
        ).ToArray();
        _dataSets[1] = SumLists(
          TakeOneEvery(TakeLast(_productionHistory, count * _skip), _skip),
          TakeOneEvery(TakeLast(_batteryOutputHistory, count * _skip), _skip)
        ).ToArray();
      }
      else
      {
        _dataSets[0] = TakeOneEvery(TakeLast(_maxProductionHistory, count * _skip), _skip).ToArray();
        _dataSets[1] = TakeOneEvery(TakeLast(_productionHistory, count * _skip), _skip).ToArray();
      }
      _dataSets[3] = TakeOneEvery(TakeLast(_consumptionHistory, count * _skip), _skip).ToArray();
      _dataSets[2] = TakeOneEvery(TakeLast(_maxConsumptionHistory, count * _skip), _skip).ToArray();
    }

    private IEnumerable<float> TakeLast(IEnumerable<float> list, int n)
    {
      return list.Skip(Math.Max(0, list.Count() - n));
    }

    private IEnumerable<float> TakeOneEvery(IEnumerable<float> list, int n)
    {
      if (n <= 1)
        return list;
      return list.Where((val, i) => i % n == 0);
    }

    private IEnumerable<float> SumLists(IEnumerable<float> listA, IEnumerable<float> listB)
    {
      return listA.Zip(listB, (x, y) => x + y);
    }

    private void TrimHistoryLimit()
    {
      while (_consumptionHistory.Count > maxSamples)
        _consumptionHistory.RemoveAt(0);
      while (_maxConsumptionHistory.Count > maxSamples)
        _maxConsumptionHistory.RemoveAt(0);
      while (_productionHistory.Count > maxSamples)
        _productionHistory.RemoveAt(0);
      while (_maxProductionHistory.Count > maxSamples)
        _maxProductionHistory.RemoveAt(0);
      while (_batteryOutputHistory.Count > maxSamples)
        _batteryOutputHistory.RemoveAt(0);
      while (_batteryMaxOutputHistory.Count > maxSamples)
        _batteryMaxOutputHistory.RemoveAt(0);
    }
  }
}