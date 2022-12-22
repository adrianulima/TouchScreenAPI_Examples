using System;
using Sandbox.Game.Entities;
using Sandbox.Game.GameSystems.TextSurfaceScripts;
using Sandbox.ModAPI;
using VRage.Game;
using VRage.Game.ModAPI;
using VRage.ModAPI;
using VRage.Utils;
using VRageMath;

namespace Lima2
{
  [MyTextSurfaceScript("FancyUI_ElectricNetworkInfo", "Electric Network Info")]
  public class ElectricNetworkInfoTSS : MyTSSCommon
  {
    public override ScriptUpdate NeedsUpdate => ScriptUpdate.Update10;

    private ElectricNetworkManager _electricMan = new ElectricNetworkManager();

    private IMyCubeBlock _block;
    private IMyTerminalBlock _terminalBlock;
    private IMyTextSurface _surface;

    private ElectricNetworkInfoApp _app;

    bool _init = false;
    int ticks = 0;

    public ElectricNetworkInfoTSS(IMyTextSurface surface, IMyCubeBlock block, Vector2 size) : base(surface, block, size)
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

      _electricMan.Init(_block);

      _app = new ElectricNetworkInfoApp(_electricMan);
      _app.InitApp(this.Block as MyCubeBlock, this.Surface as IMyTextSurface);
      _app.CreateElements();
      _app.InitElements();
      _app.GetTheme().SetScale(Math.Min(Math.Max(this.Surface.SurfaceSize.Y / 256, 0.4f), 1));

      _terminalBlock.OnMarkForClose += BlockMarkedForClose;
    }

    public override void Dispose()
    {
      base.Dispose();

      _electricMan.Dispose();
      _app?.Dispose();
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
          if (_electricMan.Update())
            _app.Update();
          frame.AddRange(_app.GetSprites());
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