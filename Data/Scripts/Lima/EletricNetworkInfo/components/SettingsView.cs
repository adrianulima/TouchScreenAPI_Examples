using VRageMath;
using Lima.API;

namespace Lima
{
  public class SettingsView : FancyScrollView
  {
    public SettingsView()
    {
      Direction = ViewDirection.Column;
    }

    public void CreateElements()
    {
      Padding = new Vector4(4);
    }
  }
}