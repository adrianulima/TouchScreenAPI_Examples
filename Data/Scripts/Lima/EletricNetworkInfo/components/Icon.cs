using Lima.API;
using VRage.Game.GUI.TextPanel;
using VRageMath;

namespace Lima
{
  public class Icon : FancyEmptyElement
  {
    public string SpriteImage;
    public Vector2 SpriteSize;
    public float SpriteRotation;
    public Color? SpriteColor;

    public Icon(string image, Vector2 size, float rotation = 0, Color? color = null)
    {
      SpriteImage = image;
      SpriteSize = size;
      SpriteRotation = rotation;
      SpriteColor = color;

      Pixels = size;
      Scale = Vector2.Zero;

      RegisterUpdate(Update);
    }

    private void Update()
    {
      var imageSprite = new MySprite()
      {
        Type = SpriteType.TEXTURE,
        Data = SpriteImage,
        RotationOrScale = SpriteRotation,
        Color = SpriteColor ?? App.Theme.WhiteColor,
        Size = SpriteSize,
        Position = Position
      };

      GetSprites().Clear();
      GetSprites().Add(imageSprite);
    }
  }
}