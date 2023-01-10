using System;
using Sandbox.Game.Entities;
using Sandbox.Game.GameSystems.TextSurfaceScripts;
using Sandbox.ModAPI;
using VRage.Game;
using VRage.Game.ModAPI;
using VRage.ModAPI;
using VRage.Utils;
using VRageMath;

namespace Lima
{
  [MyTextSurfaceScript("TouchUI_Sample", "TouchUI Sample")]
  public class UiSampleTSS : MyTSSCommon
  {
    public override ScriptUpdate NeedsUpdate => ScriptUpdate.Update10;

    IMyCubeBlock _block;
    IMyTerminalBlock _terminalBlock;
    IMyTextSurface _surface;

    SampleApp _app;

    bool _init = false;
    int ticks = 0;

    public UiSampleTSS(IMyTextSurface surface, IMyCubeBlock block, Vector2 size) : base(surface, block, size)
    {
      _block = block;
      _surface = surface;
      _terminalBlock = (IMyTerminalBlock)block;

      surface.ScriptBackgroundColor = Color.Black;
      Surface.ScriptForegroundColor = Color.SteelBlue;
    }

    public void Init()
    {
      if (!GameSession.Instance.Api.IsReady)
        return;

      if (_init)
        return;
      _init = true;

      _app = new SampleApp();
      _app.InitApp(this.Block as MyCubeBlock, this.Surface as IMyTextSurface);
      _app.CreateElements();
      _app.Theme.Scale = Math.Min(Math.Max(this.Surface.SurfaceSize.Y / 256, 0.4f), 1);

      // Sandbox.Game.MyVisualScriptLogicProvider.SendChatMessage($"Init: {_app}", "TouchSampleTSS");

      _terminalBlock.OnMarkForClose += BlockMarkedForClose;
    }

    public override void Dispose()
    {
      base.Dispose();

      _app?.ForceDispose();
      _terminalBlock.OnMarkForClose -= BlockMarkedForClose;
    }

    void BlockMarkedForClose(IMyEntity ent)
    {
      Dispose();
    }

    public override void Run()
    {
      try
      {
        if (!_init && ticks++ < (6 * 2)) // 2 secs
          return;

        Init();

        if (_app == null)
          return;

        base.Run();

        using (var frame = m_surface.DrawFrame())
        {
          _app.ForceUpdate();
          frame.AddRange(_app.GetSprites());
          frame.Dispose();
        }
      }
      catch (Exception e)
      {
        _app = null;
        MyLog.Default.WriteLineAndConsole($"{e.Message}\n{e.StackTrace}");

        if (MyAPIGateway.Session?.Player != null)
          MyAPIGateway.Utilities.ShowNotification($"[ ERROR: {GetType().FullName}: {e.Message} ]", 5000, MyFontEnum.Red);
      }
    }
  }
}