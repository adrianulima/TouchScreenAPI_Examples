using System;
using VRageMath;
using Lima.API;
using VRage.Game.GUI.TextPanel;

namespace Lima2
{
  public class StatusView : FancyView
  {
    public string Title;
    public float Value;
    public float MaxValue;

    private FancyLabel _titleLabel;
    private FancyProgressBar _progressBar;

    public StatusView(string title, float maxValue = 100, float value = 0) : base(FancyView.ViewDirection.Column)
    {
      Title = title;
      Value = value;
      MaxValue = maxValue;

      CreateElements();
    }

    private void CreateElements()
    {
      _titleLabel = new FancyLabel(Title, 0.4f, TextAlignment.CENTER);
      AddChild(_titleLabel);

      _progressBar = new FancyProgressBar(0, MaxValue, false);
      _progressBar.SetLabelScale(0.4f);
      _progressBar.SetLabelAlignment(TextAlignment.RIGHT);
      AddChild(_progressBar);
    }

    public void UpdateValues()
    {
      var sv = ElectricNetworkInfoApp.PowerFormat(Value);
      var smv = ElectricNetworkInfoApp.PowerFormat(MaxValue);

      _progressBar.SetLabel($"{sv} / {smv}");
      _progressBar.SetValue(Value);
      _progressBar.SetMaxValue(MaxValue);
    }
  }
}