using System;
using VRageMath;
using Lima.API;
using VRage.Game.GUI.TextPanel;

namespace Lima2
{
  public class LegendItem : FancyView
  {
    public string Title;
    public float Value;

    private Color _color;

    private FancyLabel _titleLabel;
    private FancyView _square;

    public LegendItem(string title, Color color, float value = 0) : base(FancyView.ViewDirection.Row)
    {
      Title = title;
      Value = value;
      _color = color;

      CreateElements();
    }

    public void UpdateTitleColor(Color color)
    {
      _titleLabel.SetTextColor(color);
    }

    private void CreateElements()
    {
      SetGap(4);

      _square = new FancyView();
      _square.SetBgColor(_color);
      _square.SetMargin(Vector4.UnitY * 2);
      _square.SetPixels(new Vector2(8, 10));
      _square.SetScale(new Vector2(0, 0));
      AddChild(_square);

      _titleLabel = new FancyLabel(Title, 0.4f, TextAlignment.CENTER);
      _titleLabel.SetAlignment(TextAlignment.LEFT);
      AddChild(_titleLabel);
    }

    public void UpdateValues()
    {
      var sv = ElectricNetworkInfoApp.PowerFormat(Value);

      _titleLabel.SetText($"{sv}");
    }
  }
}