using VRageMath;
using Lima.API;

namespace Lima2
{
  public class ChartView : FancyView
  {

    public ChartView() : base(FancyView.ViewDirection.Column)
    {
      SetStyles();
      CreateElements();
    }

    private void SetStyles()
    {
      SetPixels(new Vector2(0, 120));
      SetScale(new Vector2(1, 0));
    }

    private void CreateElements()
    {
      var chart = new FancyView(FancyView.ViewDirection.Row, Color.RoyalBlue);
      AddChild(chart);
    }
  }
}