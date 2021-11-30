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
    FancyCursor _cursor;

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
      if (!SampleSession.Instance.Api.IsReady)
        return;

      if (_init)
        return;
      _init = true;

      _screen = new TouchScreen(SampleSession.Instance.Api.CreateTouchScreen(_block, _surface));
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
        Position = new Vector2(50, 100),
        Size = new Vector2(100, 100)
      };
      _squareHandler = new ClickHandler();
      _squareHandler.SetHitArea(new Vector4(50, 50, 150, 150));

      _terminalBlock.OnMarkForClose += BlockMarkedForClose;
    }

    public override void Dispose()
    {
      base.Dispose();

      SampleSession.Instance.Api.RemoveTouchScreen(_block, _surface);
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
        _square.Color = Color.Red;
        _custom = !_custom;
      }
      else if (_squareHandler.IsMouseOver())
        _square.Color = Color.BlueViolet;
      else
        _square.Color = Color.RoyalBlue;

      if (_custom)
        _customCursor.Position = _screen.GetCursorPosition();
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