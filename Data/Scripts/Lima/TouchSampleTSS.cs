using System;
using Sandbox.Game.GameSystems.TextSurfaceScripts;
using Sandbox.ModAPI;
using VRage.Game;
using VRage.Game.GUI.TextPanel;
using VRage.Game.ModAPI;
using VRage.ModAPI;
using VRage.Utils;
using VRageMath;
using Lima.API;

namespace Lima2
{
  [MyTextSurfaceScript("Touch_Sample", "Touch Sample")]
  public class TouchSampleTSS : MyTSSCommon
  {
    public override ScriptUpdate NeedsUpdate => ScriptUpdate.Update10;

    IMyCubeBlock _block;
    IMyTerminalBlock _terminalBlock;
    IMyTextSurface _surface;

    TouchScreen _screen;
    FancyCursor _cursor;

    bool _custom = false;
    MySprite _customCursor;
    MySprite _square;
    ClickHandler _squareHandler;

    MySprite _circle1;
    MySprite _circle2;
    MySprite _circle3;

    int n = 60;
    int n2 = 6;
    MySprite[] _circle;
    private int _rotation = 0;
    private float _pitch = 0.35f;

    private float[] _lati;
    private float[] _long;
    private float _radius = 150;
    private Vector2 _center = new Vector2(512 / 2, 512 / 2);

    bool _init = false;
    int ticks = 0;

    public TouchSampleTSS(IMyTextSurface surface, IMyCubeBlock block, Vector2 size) : base(surface, block, size)
    {
      _block = block;
      _surface = surface;
      _terminalBlock = (IMyTerminalBlock)block;

      surface.ScriptBackgroundColor = Color.Black;
      Surface.ScriptForegroundColor = Color.RoyalBlue;
    }

    public void Init()
    {
      if (!GameSession.Instance.Api.IsReady)
        return;

      if (_init)
        return;
      _init = true;

      _screen = new TouchScreen(GameSession.Instance.Api.CreateTouchScreen(_block, _surface));
      _cursor = new FancyCursor(_screen);

      _customCursor = new MySprite()
      {
        Type = SpriteType.TEXTURE,
        Data = "Circle",
        RotationOrScale = 0,
        Color = Color.White,
        Size = new Vector2(10, 10)
      };

      _square = new MySprite()
      {
        Type = SpriteType.TEXTURE,
        Data = "SquareSimple",
        RotationOrScale = 0,
        Color = Color.RoyalBlue,
        Position = new Vector2(0, 25),
        Size = new Vector2(50, 50)
      };
      _squareHandler = new ClickHandler();
      _squareHandler.SetHitArea(new Vector4(0, 0, 50, 50));


      _circle1 = new MySprite()
      {
        Type = SpriteType.TEXTURE,
        Data = "Circle",
        RotationOrScale = 0,
        Color = new Color(Color.RoyalBlue, 0.1f),
        Position = new Vector2(512 / 2 - 300 / 2, 512 / 2),
        Size = new Vector2(300, 300)
      };

      _circle2 = new MySprite()
      {
        Type = SpriteType.TEXTURE,
        Data = "Circle",
        // RotationOrScale = -0.655f,
        RotationOrScale = 0,
        Color = new Color(Color.RoyalBlue, 0.3f),
        Position = new Vector2(512 / 2 - 270 / 2, 512 / 2 - 29),
        Size = new Vector2(270, 240)
      };

      _circle3 = new MySprite()
      {
        Type = SpriteType.TEXTURE,
        Data = "Circle",
        RotationOrScale = 0,
        Color = new Color(Color.RoyalBlue, 0.6f),
        Position = new Vector2(512 / 2 - 70 / 2, 512 / 2 - 80),
        Size = new Vector2(70, 60)
      };


      _lati = new float[n];
      _long = new float[n];
      _circle = new MySprite[n];

      var div = (n / n2);
      var pi_lat = (float)Math.PI / (div + 1);
      var pi_long = (float)Math.PI / n2;

      for (int i = 0; i < div; i++)
      {
        for (int j = 0; j < n2; j++)
        {
          _lati[i * n2 + j] = i * pi_lat + pi_lat / 2;
          _long[i * n2 + j] = j * pi_long * 2;
          _circle[i * n2 + j] = new MySprite()
          {
            Type = SpriteType.TEXTURE,
            Data = "Circle",
            RotationOrScale = 0,
            Color = Color.DarkBlue,
            Position = new Vector2(256, 256),
            Size = new Vector2(10, 10)
          };
        }
      }

      _terminalBlock.OnMarkForClose += BlockMarkedForClose;
    }

    public override void Dispose()
    {
      base.Dispose();

      GameSession.Instance.Api.RemoveTouchScreen(_block, _surface);
      _screen?.Dispose();
      _cursor?.Dispose();
      _terminalBlock.OnMarkForClose -= BlockMarkedForClose;
    }

    void BlockMarkedForClose(IMyEntity ent)
    {
      Dispose();
    }

    private void UpdateSprites()
    {
      _squareHandler.UpdateStatus(_screen);

      if (_squareHandler.IsMousePressed())
      {
        _square.Color = Color.White;
        _custom = !_custom;
      }
      else if (_squareHandler.IsMouseOver())
        _square.Color = Color.BlueViolet;
      else
        _square.Color = Color.RoyalBlue;

      if (_custom)
        _customCursor.Position = _screen.GetCursorPosition();

      _rotation += 1;
      if (_rotation >= 360)
        _rotation = 0;

      // var moveY = (_cursor.GetPosition().Y / 512) * Math.PI;
      // var radi_rotation = (_cursor.GetPosition().X / 512) * Math.PI;

      var radi_rotation = _rotation * (Math.PI / 180);

      for (int i = 0; i < n; i++)
      {
        var z = _radius * Math.Sin(_lati[i]) * Math.Cos(_long[i] + radi_rotation);
        var x = _radius * Math.Sin(_lati[i]) * Math.Sin(_long[i] + radi_rotation);
        var y = _radius * Math.Cos(_lati[i]);


        var sz = (float)(-z / _radius);
        y -= sz * 30;

        var size = new Vector2(2, 2) + new Vector2(3, 3) * sz;
        _circle[i].Size = size;
        _circle[i].Position = _center + new Vector2((float)x, -(float)y);
        _circle[i].Position -= Vector2.UnitX * (size.X / 2);

        _circle[i].RotationOrScale = sz > 0 ? 1 : -1;
      }

      // Sandbox.Game.MyVisualScriptLogicProvider.SendChatMessage($"{(_cursor.GetPosition().Y / 512) * Math.PI}", "SampleApp");
    }

    public override void Run()
    {
      try
      {
        if (!_init && ticks++ < (6 * 2)) // 2 secs
          return;

        Init();

        if (_screen == null)
          return;

        UpdateSprites();

        base.Run();

        using (var frame = m_surface.DrawFrame())
        {
          frame.Add(_square);
          frame.Add(_circle1);
          frame.Add(_circle2);
          // frame.Add(_circle3);

          for (int i = 0; i < n; i++)
          {
            if (_circle[i].RotationOrScale >= 0)
              frame.Add(_circle[i]);
          }

          if (_custom)
            frame.Add(_customCursor);
          else
            frame.AddRange(_cursor.GetSprites());
          frame.Dispose();
        }
      }
      catch (Exception e)
      {
        MyLog.Default.WriteLineAndConsole($"{e.Message}\n{e.StackTrace}");

        if (MyAPIGateway.Session?.Player != null)
          MyAPIGateway.Utilities.ShowNotification($"[ ERROR: {GetType().FullName}: {e.Message} ]", 5000, MyFontEnum.Red);
      }
    }
  }
}