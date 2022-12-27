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
  [MyTextSurfaceScript("FancyUI_ElectricNetworkInfo", "Electric Network Info")]
  public class ElectricNetworkInfoTSS : MyTSSCommon
  {
    public override ScriptUpdate NeedsUpdate => ScriptUpdate.Update10;

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

      // Default Blue
      // surface.ScriptBackgroundColor = new Color(0, 88, 155);
      // Surface.ScriptForegroundColor = new Color(179, 237, 255);

      // Beige
      // surface.ScriptBackgroundColor = new Color(140, 120, 95);
      // Surface.ScriptForegroundColor = new Color(230, 220, 210);

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

      var electricManager = GameSession.Instance.GetElectricManagerForBlock(_block);
      if (electricManager == null)
        return;

      _app = new ElectricNetworkInfoApp(electricManager);
      _app.InitApp(this.Block as MyCubeBlock, this.Surface as IMyTextSurface);
      _app.CreateElements();
      _app.InitElements();
      _app.Theme.Scale = Math.Min(Math.Max(this.Surface.SurfaceSize.Y / 256, 0.4f), 1);

      GameSession.Instance.Handler.AddActiveTSS(this);
      LoadAppContent();

      _terminalBlock.OnMarkForClose += BlockMarkedForClose;
    }

    public FileHandler.AppContent GenerateAppContent()
    {
      return new FileHandler.AppContent()
      {
        BlockId = _block.EntityId,
        SurfaceName = _surface.Name,
        ScriptBackgroundColor = _surface.ScriptBackgroundColor.PackedValue,
        ScriptForegroundColor = _surface.ScriptForegroundColor.PackedValue,
        BatteryChartEnabled = _app.Charts.BatteryOutputAsProduction,
        ChartIntervalIndex = _app.Charts.ChartIntervalIndex
      };
    }

    public void LoadAppContent()
    {
      var loadContent = GameSession.Instance.Handler.GetAppContent(_block.EntityId, _surface.Name);
      if (loadContent != null)
      {
        var content = loadContent.GetValueOrDefault();

        _surface.ScriptBackgroundColor = new Color(content.ScriptBackgroundColor);
        _surface.ScriptForegroundColor = new Color(content.ScriptForegroundColor);

        _app.ApplySettings(content);
      }
    }

    public override void Dispose()
    {
      base.Dispose();

      GameSession.Instance.RemoveManagerFromBlock(_block);
      GameSession.Instance.Handler.RemoveActiveTSS(this);

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

        if (!_init)
          Init();

        if (_app == null)
          return;

        base.Run();

        using (var frame = m_surface.DrawFrame())
        {
          _app.UpdateValues();
          frame.AddRange(_app.Sprites);
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