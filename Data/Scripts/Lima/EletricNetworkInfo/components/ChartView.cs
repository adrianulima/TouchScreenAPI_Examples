using VRageMath;
using Lima.API;
using System.Collections.Generic;
using System;

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
    private FancyView _chartContainer;
    private List<float[]> _dataSets;

    public ChartView() : base(FancyView.ViewDirection.Column)
    {
      CreateElements();
    }

    public void SetChartContainerBorder(Color color)
    {
      _chartContainer.SetBorderColor(color);
      _chartContainer.SetBorder(new Vector4(1));
    }

    private void CreateElements()
    {
      var intervalSwitcher = new FancySwitch(new string[] { "30s", "1m", "10m", "30m", "1h" }, 0, (int v) =>
      {

      });
      AddChild(intervalSwitcher);

      _chartContainer = new FancyView();
      _chartContainer.SetPadding(new Vector4(4));
      AddChild(_chartContainer);

      var intervals = 30;
      _chart = new FancyChart(intervals);
      _dataSets = _chart.GetDataSets();
      _dataSets.Add(null);
      _dataSets.Add(null);
      _dataSets.Add(null);
      _dataSets.Add(null);
      _chart.GetDataColors().Add(Color.SlateGray);
      _chart.GetDataColors().Add(Color.DarkViolet);
      _chart.GetDataColors().Add(Color.DarkGreen);
      _chart.GetDataColors().Add(Color.OrangeRed);
      _chartContainer.AddChild(_chart);
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

      source = _productionHistory.ToArray();
      _dataSets[2] = source;

      source = _maxConsumptionHistory.ToArray();
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