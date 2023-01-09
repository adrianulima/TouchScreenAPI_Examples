using System;
using VRageMath;
using Lima.API;
using VRage.Game.GUI.TextPanel;

namespace Lima
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

    public BatteryStorageView(float maxValue = 100, float value = 0) : base(ViewDirection.Column)
    {
      Value = value;
      MaxValue = maxValue;

      CreateElements();
      SetStyles();

      RegisterUpdate(Update);
    }

    private void SetStyles()
    {
      Scale = new Vector2(0, 1);

      _isOverloadBlackout = true;
      UpdateOverloadStatus(false);
    }

    public void UpdateOverloadStatus(bool overload)
    {
      if (_isOverloadBlackout != overload)
      {
        _isOverloadBlackout = overload;
        if (_isOverloadBlackout)
        {
          Pixels = new Vector2(_width + 2, 1);
          Border = new Vector4(1);
          BorderColor = Color.Red;
          _overloadView.Enabled = true;
        }
        else
        {
          Pixels = new Vector2(_width, 1);
          Border = new Vector4(0);
          _overloadView.Enabled = false;
        }
      }
    }

    private void CreateElements()
    {
      _progressBar = new FancyProgressBar(0, MaxValue, true);
      _progressBar.Pixels = new Vector2(_width, 0);
      _progressBar.Scale = new Vector2(0, 1);
      _progressBar.Label.FontSize = 0.6f;
      _progressBar.Label.Alignment = TextAlignment.CENTER;
      AddChild(_progressBar);

      _icon = new Icon("IconEnergy", new Vector2(_width));
      _icon.Absolute = true;
      AddChild(_icon);

      _inputArrow = new Icon("Triangle", new Vector2(20, 12));// MathHelper.Pi);
      _inputArrow.Absolute = true;
      _inputArrow.Enabled = false;
      AddChild(_inputArrow);

      _outputArrow = new Icon("Triangle", new Vector2(20, 12), MathHelper.Pi);
      _outputArrow.Absolute = true;
      _outputArrow.Enabled = false;
      AddChild(_outputArrow);

      _overloadView = new FancyView();
      _overloadView.BgColor = Color.Red;
      _overloadView.Padding = Vector4.UnitY * 1;
      _overloadView.Pixels = new Vector2(0, 12);
      _overloadView.Scale = new Vector2(1, 0);
      AddChild(_overloadView);

      _overloadLabel = new FancyLabel("OVERLOAD", 0.35f, TextAlignment.CENTER);
      _overloadView.AddChild(_overloadLabel);

      _timeLeftView = new FancyView();
      _timeLeftView.Padding = Vector4.UnitY * 1;
      _timeLeftView.Pixels = new Vector2(0, 14);
      // _timeLeftView.SetScale(new Vector2(1, 0));
      _timeLeftView.Absolute = true;
      AddChild(_timeLeftView);

      _timeLeftLabel = new FancyLabel("00.0h", 0.4f, TextAlignment.CENTER);
      _timeLeftView.AddChild(_timeLeftLabel);
    }

    public void UpdateValues()
    {
      var percentage = (Value / MaxValue);
      _progressBar.Label.Text = $"{(float.IsNaN(percentage) ? 0f : (percentage * 100)).ToString("0")}%";
      _progressBar.MaxValue = MaxValue;
      _progressBar.Value = Value;

      _timeLeftLabel.Text = ElectricNetworkInfoApp.HoursFormat(HoursLeft);
    }

    int _tick = 0;
    private void Update()
    {
      var scale = App.Theme.Scale;
      var size = GetSize();
      var pos = Position;
      var mainColor = App.Theme.MainColor;

      _timeLeftView.Position = pos - new Vector2(size.X / 2, 0);
      _timeLeftView.Pixels = new Vector2(size.X / scale, 14);

      var iconPos = pos + new Vector2(0, size.Y / 2);
      _icon.Position = iconPos;
      _icon.SpriteSize = new Vector2(size.X);

      _tick++;
      if (_tick > 6)
        _tick = 0;

      var anim = (float)_tick / 6;

      if (InputRatio > 0)
      {
        var animInput = anim * InputRatio;
        _inputArrow.Enabled = true;
        _inputArrow.SpriteSize = new Vector2(20, 12) * scale;
        _inputArrow.Position = iconPos + new Vector2(size.X / 2 - (_inputArrow.SpriteSize.X / 2), animInput * -(_icon.SpriteSize.Y));
        _inputArrow.SpriteColor = new Color(mainColor, animInput * 1f);
      }
      else
        _inputArrow.Enabled = false;

      if (OutputRatio > 0)
      {
        var animOutput = anim * OutputRatio;
        _outputArrow.Enabled = true;
        _outputArrow.SpriteSize = new Vector2(20, 12) * scale;
        _outputArrow.Position = iconPos + new Vector2(size.X / 2 - (_inputArrow.SpriteSize.X / 2), animOutput * (_icon.SpriteSize.Y));
        _outputArrow.SpriteColor = new Color(mainColor, animOutput * 1f);
      }
      else
        _outputArrow.Enabled = false;
    }
  }
}