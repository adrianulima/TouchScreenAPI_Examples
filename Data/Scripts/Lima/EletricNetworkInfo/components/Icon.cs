using Lima.API;
using VRage.Game.GUI.TextPanel;
using VRageMath;

namespace Lima2
{
  public class Icon : FancyEmptyElement
  {
    private string _image;
    private Vector2 _size;

    public Icon(string image, Vector2 size) : base()
    {
      _image = image;
      _size = size;

      SetPixels(size);
      SetScale(Vector2.Zero);

      RegisterUpdate(Update);
    }

    private void Update()
    {
      var imageSprite = new MySprite()
      {
        Type = SpriteType.TEXTURE,
        Data = _image,
        RotationOrScale = 0,
        Size = _size,
        Position = GetPosition()
      };

      GetSprites().Clear();
      GetSprites().Add(imageSprite);
    }
  }
}