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
    private FancyView _overloadView;
    private FancyLabel _overloadLabel;
    private FancyView _timeLeftView;
    private FancyLabel _timeLeftLabel;

    private bool _isOverloadBlackout = false;
    public float HoursLeft = 0;

    private int _width = 48;

    public BatteryStorageView(float maxValue = 100, float value = 0) : base(FancyView.ViewDirection.Column)
    {
      Value = value;
      MaxValue = maxValue;

      CreateElements();
      SetStyles();

      RegisterUpdate(Update);
    }

    private void SetStyles()
    {
      SetScale(new Vector2(0, 1));

      _isOverloadBlackout = true;
      UpdateOverloadStyle(false);
    }

    public void UpdateOverloadStyle(bool overload)
    {
      if (_isOverloadBlackout != overload)
      {
        _isOverloadBlackout = overload;
        if (_isOverloadBlackout)
        {
          SetPixels(new Vector2(_width + 2, 1));
          SetBorder(new Vector4(1));
          SetBorderColor(Color.Red);
          _overloadView.SetEnabled(true);
        }
        else
        {
          SetPixels(new Vector2(_width, 1));
          SetBorder(new Vector4(0));
          _overloadView.SetEnabled(false);
        }
      }
    }

    private void CreateElements()
    {
      _progressBar = new FancyProgressBar(0, MaxValue, false, true);
      _progressBar.SetPixels(new Vector2(_width, 0));
      _progressBar.SetLabelScale(0.6f);
      _progressBar.SetLabelAlignment(TextAlignment.CENTER);
      AddChild(_progressBar);

      _icon = new Icon("IconEnergy", new Vector2(_width));
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

      _overloadView = new FancyView();
      _overloadView.SetBgColor(Color.Red);
      _overloadView.SetPadding(Vector4.UnitY * 1);
      _overloadView.SetPixels(new Vector2(0, 12));
      _overloadView.SetScale(new Vector2(1, 0));
      AddChild(_overloadView);

      _overloadLabel = new FancyLabel("OVERLOAD", 0.35f, TextAlignment.CENTER);
      _overloadView.AddChild(_overloadLabel);

      _timeLeftView = new FancyView();
      _timeLeftView.SetPadding(Vector4.UnitY * 1);
      _timeLeftView.SetPixels(new Vector2(0, 14));
      // _timeLeftView.SetScale(new Vector2(1, 0));
      _timeLeftView.SetAbsolute(true);
      AddChild(_timeLeftView);

      _timeLeftLabel = new FancyLabel("00.0h", 0.4f, TextAlignment.CENTER);
      _timeLeftView.AddChild(_timeLeftLabel);
    }

    public void UpdateValues()
    {
      _progressBar.SetLabel($"{((Value / MaxValue) * 100).ToString("0")}%");
      _progressBar.SetValue(Value);
      _progressBar.SetMaxValue(MaxValue);

      _timeLeftLabel.SetText(ElectricNetworkInfoApp.HoursFormat(HoursLeft));
    }

    int _tick = 0;
    private void Update()
    {
      var scale = GetApp().GetTheme().GetScale();
      var size = GetSize();
      var pos = GetPosition();
      var mainColor = GetApp().GetTheme().GetColorMain();

      // _timeLeftView.SetBgColor(GetApp().GetTheme().GetColorMainDarker(20));
      _timeLeftView.SetPosition(pos - new Vector2(size.X / 2, 0));
      _timeLeftView.SetPixels(new Vector2(size.X, 14));

      var iconPos = pos + new Vector2(0, size.Y / 2);
      _icon.SetPosition(iconPos);
      _icon.SpriteSize = new Vector2(size.X);

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
        _inputArrow.SpriteColor = new Color(mainColor, animInput * 1f);
      }
      else
        _inputArrow.SetEnabled(false);

      if (OutputRatio > 0)
      {
        var animOutput = anim * OutputRatio;
        _outputArrow.SetEnabled(true);
        _outputArrow.SpriteSize = new Vector2(20, 12) * scale;
        _outputArrow.SetPosition(iconPos + new Vector2(size.X / 2 - scale * (_inputArrow.SpriteSize.X / 2), animOutput * (_icon.SpriteSize.Y)));
        _outputArrow.SpriteColor = new Color(mainColor, animOutput * 1f);
      }
      else
        _outputArrow.SetEnabled(false);
    }
  }
}