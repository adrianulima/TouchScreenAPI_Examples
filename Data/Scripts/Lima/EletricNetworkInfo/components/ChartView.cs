using VRageMath;
using Lima.API;
using System.Collections.Generic;
using System;
using VRage.Game.GUI.TextPanel;

namespace Lima2
{
  public class ChartView : FancyView
  {
    private List<float> _consumptionHistory = new List<float>();
    private List<float> _maxConsumptionHistory = new List<float>();
    private List<float> _productionHistory = new List<float>();
    private List<float> _maxProductionHistory = new List<float>();

    public int maxSamples = 1000;

    private FancyChart _chart;
    private FancyView _chartView;
    private FancyView _legendsView;
    private FancyView _labelsWrapper;
    private List<float[]> _dataSets;
    private LegendItem[] _legends;
    private FancyLabel[] _labels;

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
      var intervalSwitcher = new FancySwitch(new string[] { "30s", "1m", "10m", "30m", "1h" }, 0, (int v) =>
      {

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
      _legendsView.SetPadding(new Vector4(8, 4, 8, 4));
      _legendsView.SetScale(new Vector2(1, 0));
      _legendsView.SetPixels(new Vector2(0, 20));
      AddChild(_legendsView);

      _legends = new LegendItem[4];
      _legends[0] = new LegendItem("Consumption", Color.OrangeRed);
      _legendsView.AddChild(_legends[0]);
      _legends[1] = new LegendItem("Max Consum.", Color.SlateGray);
      _legendsView.AddChild(_legends[1]);
      _legends[2] = new LegendItem("Production", Color.DarkGreen);
      _legendsView.AddChild(_legends[2]);
      _legends[3] = new LegendItem("Max Prod.", Color.DarkSlateGray);
      _legendsView.AddChild(_legends[3]);
    }

    public void UpdateValues(float consumption, float maxConsumption, float production, float maxProduction)
    {
      _consumptionHistory.Add(consumption);
      _maxConsumptionHistory.Add(maxConsumption);
      _productionHistory.Add(production);
      _maxProductionHistory.Add(maxProduction);

      TrimHistoryLimit();

      UpdateChartDataSets();
    }

    private void UpdateChartDataSets()
    {
      var source = _consumptionHistory.ToArray();
      _dataSets[3] = source;

      source = _maxConsumptionHistory.ToArray();
      _dataSets[2] = source;

      source = _productionHistory.ToArray();
      _dataSets[1] = source;

      source = _maxProductionHistory.ToArray();
      _dataSets[0] = source;
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
    }
  }
}