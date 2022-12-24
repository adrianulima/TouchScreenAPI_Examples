using Lima.API;
using VRage.Game.GUI.TextPanel;
using VRageMath;

namespace Lima2
{
  public class Icon : FancyEmptyElement
  {
    public string SpriteImage;
    public Vector2 SpriteSize;
    public float SpriteRotation;
    public Color? SpriteColor;

    public Icon(string image, Vector2 size, float rotation = 0, Color? color = null) : base()
    {
      SpriteImage = image;
      SpriteSize = size;
      SpriteRotation = rotation;
      SpriteColor = color;

      SetPixels(size);
      SetScale(Vector2.Zero);

      RegisterUpdate(Update);
    }

    private void Update()
    {
      var imageSprite = new MySprite()
      {
        Type = SpriteType.TEXTURE,
        Data = SpriteImage,
        RotationOrScale = SpriteRotation,
        Color = SpriteColor ?? GetApp().GetTheme().GetColorWhite(),
        Size = SpriteSize * GetApp().GetTheme().GetScale(),
        Position = GetPosition()
      };

      GetSprites().Clear();
      GetSprites().Add(imageSprite);
    }
  }
}