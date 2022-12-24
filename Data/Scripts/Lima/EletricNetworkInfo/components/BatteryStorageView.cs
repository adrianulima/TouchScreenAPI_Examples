using System;
using VRageMath;
using Lima.API;
using VRage.Game.GUI.TextPanel;

namespace Lima2
{
  public class BatteryStorageView : FancyView
  {
    public float Value;
    public float MaxValue;

    private FancyProgressBar _progressBar;
    private Icon _icon;

    public BatteryStorageView(float maxValue = 100, float value = 0) : base(FancyView.ViewDirection.Column)
    {
      Value = value;
      MaxValue = maxValue;

      SetStyles();
      CreateElements();

      RegisterUpdate(Update);
    }

    private void SetStyles()
    {
      SetPixels(new Vector2(44, 1));
      SetScale(new Vector2(0, 1));
    }

    private void CreateElements()
    {
      _progressBar = new FancyProgressBar(0, MaxValue, false, true);
      _progressBar.SetPixels(new Vector2(44, 0));
      _progressBar.SetLabelScale(0.6f);
      _progressBar.SetLabelAlignment(TextAlignment.CENTER);
      AddChild(_progressBar);

      _icon = new Icon("IconEnergy", new Vector2(44, 44));
      _icon.SetAbsolute(true);
      AddChild(_icon);
    }

    public void UpdateValues()
    {
      _progressBar.SetLabel($"{((Value / MaxValue) * 100).ToString("0")}%");
      _progressBar.SetValue(Value);
      _progressBar.SetMaxValue(MaxValue);
    }

    private void Update()
    {
      _icon.SetPosition(GetPosition() + new Vector2(0, GetSize().Y / 2));
    }
  }
}