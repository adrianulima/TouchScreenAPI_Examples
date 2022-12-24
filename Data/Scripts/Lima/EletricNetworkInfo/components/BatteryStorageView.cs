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

    public float InputRatio;
    public float OutputRatio;

    private FancyProgressBar _progressBar;
    private Icon _icon;

    private Icon _inputArrow;
    private Icon _outputArrow;

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

      _icon = new Icon("IconEnergy", new Vector2(44));
      _icon.SetAbsolute(true);
      AddChild(_icon);

      _inputArrow = new Icon("Triangle", new Vector2(20, 12));// MathHelper.Pi);
      _inputArrow.SetAbsolute(true);
      _inputArrow.SetEnabled(false);
      AddChild(_inputArrow);

      _outputArrow = new Icon("Triangle", new Vector2(20, 12), MathHelper.Pi);
      _outputArrow.SetAbsolute(true);
      _outputArrow.SetEnabled(false);
      AddChild(_outputArrow);
    }

    public void UpdateValues()
    {
      _progressBar.SetLabel($"{((Value / MaxValue) * 100).ToString("0")}%");
      _progressBar.SetValue(Value);
      _progressBar.SetMaxValue(MaxValue);
    }

    int _tick = 0;
    private void Update()
    {
      var scale = GetApp().GetTheme().GetScale();
      var size = GetSize();

      var iconPos = GetPosition() + new Vector2(0, size.Y / 2);
      _icon.SetPosition(iconPos);
      _icon.SpriteSize = new Vector2(44 * scale);

      _tick++;
      if (_tick > 6)
        _tick = 0;

      var anim = (float)_tick / 6;

      if (InputRatio > 0)
      {
        var animInput = anim * InputRatio;
        _inputArrow.SetEnabled(true);
        _inputArrow.SpriteSize = new Vector2(20, 12) * scale;
        _inputArrow.SetPosition(iconPos + new Vector2(size.X / 2 - scale * (_inputArrow.SpriteSize.X / 2), animInput * -(_icon.SpriteSize.Y)));
        _inputArrow.SpriteColor = new Color(GetApp().GetTheme().GetColorMain(), animInput * 1f);
      }
      else
        _inputArrow.SetEnabled(false);


      if (OutputRatio > 0)
      {
        var animOutput = anim * OutputRatio;
        _outputArrow.SetEnabled(true);
        _outputArrow.SpriteSize = new Vector2(20, 12) * scale;
        _outputArrow.SetPosition(iconPos + new Vector2(size.X / 2 - scale * (_inputArrow.SpriteSize.X / 2), animOutput * (_icon.SpriteSize.Y)));
        _outputArrow.SpriteColor = new Color(GetApp().GetTheme().GetColorMain(), animOutput * 1f);
      }
      else
        _outputArrow.SetEnabled(false);
    }
  }
}