using System;
using VRageMath;
using Lima.API;
using VRage.Game.GUI.TextPanel;

namespace Lima
{
  public class EntityItem : FancyView
  {
    public string Title;
    public int Count;
    public float Value;
    public float MaxValue;

    private FancyView _titleView;
    private FancyLabel _titleLabel;
    private FancyLabel _countLabel;
    private FancyProgressBar _progressBar;

    public EntityItem(string title) : base(ViewDirection.Column)
    {
      Title = title;
      SetStyles();
      CreateElements();

      RegisterUpdate(Update);
    }

    private void Update()
    {
      BgColor = App.Theme.GetMainColorDarker(4);

      _titleLabel.TextColor = App.Theme.MainColor;
      _countLabel.TextColor = App.Theme.MainColor;
    }

    private void SetStyles()
    {
      Padding = new Vector4(2);
    }

    private void CreateElements()
    {
      _titleView = new FancyView(ViewDirection.Row);
      _titleView.Scale = new Vector2(1, 0);
      _titleView.Pixels = new Vector2(0, 14);
      AddChild(_titleView);

      _titleLabel = new FancyLabel(Title, 0.4f, TextAlignment.LEFT);
      _titleView.AddChild(_titleLabel);

      _countLabel = new FancyLabel("0", 0.4f, TextAlignment.RIGHT);
      _titleView.AddChild(_countLabel);

      _progressBar = new FancyProgressBar(0, MaxValue, false);
      _progressBar.Scale = new Vector2(1, 0);
      _progressBar.Pixels = new Vector2(0, 16);
      _progressBar.LabelScale = 0.35f;
      AddChild(_progressBar);
    }

    public void UpdateValues()
    {
      _titleLabel.Text = Title;
      _countLabel.Text = Count.ToString();

      var sv = ElectricNetworkInfoApp.PowerFormat(Value);

      _progressBar.Label = sv;
      _progressBar.MaxValue = MaxValue;
      _progressBar.Value = Value;
    }
  }
}