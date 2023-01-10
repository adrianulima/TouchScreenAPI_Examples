using System;
using VRageMath;
using Lima.API;
using VRage.Game.GUI.TextPanel;

namespace Lima
{
  public class EntityItem : TouchView
  {
    public string Title;
    public int Count;
    public float Value;
    public float MaxValue;

    private TouchView _titleView;
    private TouchLabel _titleLabel;
    private TouchLabel _countLabel;
    private TouchProgressBar _progressBar;

    public EntityItem(string title, Color TextColor) : base(ViewDirection.Column)
    {
      Title = title;
      SetStyles();
      CreateElements(TextColor);
    }

    private void SetStyles()
    {
      Padding = new Vector4(2);
    }

    private void CreateElements(Color TextColor)
    {
      _titleView = new TouchView(ViewDirection.Row);
      _titleView.Scale = new Vector2(1, 0);
      _titleView.Pixels = new Vector2(0, 14);
      AddChild(_titleView);

      _titleLabel = new TouchLabel(Title, 0.4f, TextAlignment.LEFT);
      _titleLabel.TextColor = TextColor;
      _titleView.AddChild(_titleLabel);

      _countLabel = new TouchLabel("0", 0.4f, TextAlignment.RIGHT);
      _countLabel.TextColor = TextColor;
      _countLabel.Scale = new Vector2(0, 1);
      _countLabel.Pixels = new Vector2(10, 0);
      _titleView.AddChild(_countLabel);

      _progressBar = new TouchProgressBar(0, MaxValue);
      _progressBar.Scale = new Vector2(1, 0);
      _progressBar.Pixels = new Vector2(0, 16);
      _progressBar.Label.FontSize = 0.35f;
      AddChild(_progressBar);
    }

    public void UpdateValues()
    {
      _titleLabel.Text = Title;

      var countText = Count.ToString();

      if (_countLabel.Text != countText)
      {
        _countLabel.Text = countText;
        var px = (App?.Theme.MeasureStringInPixels(countText, App.Theme.Font, 0.4f).X ?? 0) + 2;
        _countLabel.Pixels = new Vector2(px, 0);
      }

      var sv = ElectricNetworkInfoApp.PowerFormat(Value);
      _progressBar.Label.Text = sv;
      _progressBar.MaxValue = MaxValue;
      _progressBar.Value = Value;
    }
  }
}