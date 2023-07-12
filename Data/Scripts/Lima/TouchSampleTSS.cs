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

namespace Lima
{
  [MyTextSurfaceScript("Touch_Sample", "Touch Sample")]
  public class TouchSampleTSS : MyTSSCommon
  {
    public override ScriptUpdate NeedsUpdate => ScriptUpdate.Update10;

    IMyCubeBlock _block;
    IMyTerminalBlock _terminalBlock;
    IMyTextSurface _surface;

    TouchScreen _screen;
    Cursor _cursor;

    bool _custom = false;
    MySprite _customCursor;
    MySprite _square;
    ClickHandler _squareHandler;

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
      if (!GameExampleSession.Instance.Api.IsReady)
        return;

      if (_init)
        return;
      _init = true;

      _screen = new TouchScreen(GameExampleSession.Instance.Api.CreateTouchScreen(_block, _surface));
      _cursor = new Cursor(_screen);

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
        Data = "Circle",
        RotationOrScale = 0,
        Color = Surface.ScriptForegroundColor,
        Position = new Vector2(0, 75),
        Size = new Vector2(150, 150)
      };
      _squareHandler = new ClickHandler();
      _squareHandler.HitArea = new Vector4(0, 0, 150, 150);

      _terminalBlock.OnMarkForClose += BlockMarkedForClose;
    }

    public override void Dispose()
    {
      base.Dispose();

      GameExampleSession.Instance.Api.RemoveTouchScreen(_block, _surface);
      _screen?.ForceDispose();
      _cursor?.ForceDispose();
      _terminalBlock.OnMarkForClose -= BlockMarkedForClose;
    }

    void BlockMarkedForClose(IMyEntity ent)
    {
      Dispose();
    }

    private void UpdateSprites()
    {
      _squareHandler.Update(_screen);

      if (_squareHandler.Mouse1.IsPressed)
      {
        _square.Color = Color.White;
        _custom = !_custom;
      }
      else if (_squareHandler.Mouse1.IsOver)
        _square.Color = Color.BlueViolet;
      else
        _square.Color = Color.RoyalBlue;

      if (_custom)
        _customCursor.Position = _screen.CursorPosition;
    }

    int ClampAngle(int angle)
    {
      angle = angle % 360;
      if (angle < 0) angle += 360;
      return angle;
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

          if (_custom)
            frame.Add(_customCursor);
          else
            frame.AddRange(_cursor.GetSprites());
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