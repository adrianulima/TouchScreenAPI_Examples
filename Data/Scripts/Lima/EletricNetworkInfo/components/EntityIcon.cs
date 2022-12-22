using Lima.API;
using VRage.Game.GUI.TextPanel;

namespace Lima2
{
  public class EntityIcon : FancyCustomElement
  {
    public string Image;

    private MySprite _imageSprite;

    public EntityIcon(string image) : base()
    {
      Image = image;

      CreateElements();
    }

    private void CreateElements()
    {
      _imageSprite = new MySprite()
      {
        Type = SpriteType.TEXTURE,
        Data = Image,
        RotationOrScale = 0
      };

      GetSprites().Add(_imageSprite);
    }
  }
}