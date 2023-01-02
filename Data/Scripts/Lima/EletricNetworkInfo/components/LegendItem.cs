using VRageMath;
using Lima.API;
using VRage.Game.GUI.TextPanel;

namespace Lima
{
  public class LegendItem : FancyView
  {
    public string Title;
    public float Value;

    private Color _color;

    private FancyLabel _titleLabel;
    private FancyView _square;

    public LegendItem(string title, Color color, float value = 0) : base(ViewDirection.Row)
    {
      Title = title;
      Value = value;
      _color = color;

      CreateElements();
    }

    public void UpdateTitleColor(Color color)
    {
      _titleLabel.TextColor = color;
    }

    private void CreateElements()
    {
      Gap = 4;

      _square = new FancyView();
      _square.BgColor = _color;
      _square.Margin = Vector4.UnitY * 2;
      _square.Pixels = new Vector2(8, 8);
      _square.Scale = new Vector2(0, 0);
      AddChild(_square);

      _titleLabel = new FancyLabel(Title, 0.4f, TextAlignment.CENTER);
      _titleLabel.Alignment = TextAlignment.LEFT;
      AddChild(_titleLabel);
    }

    public void UpdateValues()
    {
      var sv = ElectricNetworkInfoApp.PowerFormat(Value);

      _titleLabel.Text = sv;
    }
  }
}