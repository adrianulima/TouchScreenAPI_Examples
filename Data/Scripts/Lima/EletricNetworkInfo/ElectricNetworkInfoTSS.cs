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
  [MyTextSurfaceScript("TouchUI_ElectricNetworkInfo", "Electric Network Info")]
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

      // surface.ScriptBackgroundColor = Color.Black;
      // Surface.ScriptForegroundColor = Color.SteelBlue;
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

      _app = new ElectricNetworkInfoApp(electricManager, SaveConfigAction);
      _app.InitApp(this.Block as MyCubeBlock, this.Surface as IMyTextSurface);
      _app.CreateElements();
      _app.Theme.Scale = Math.Min(Math.Max(Math.Min(this.Surface.SurfaceSize.X, this.Surface.SurfaceSize.Y) / 512, 0.4f), 2);

      _app.Cursor.Scale = _app.Theme.Scale;

      var appContent = GameSession.Instance.BlockHandler.LoadAppContent(_block, _surface.Name);
      if (appContent != null)
        _app.ApplySettings(appContent.GetValueOrDefault());

      GameSession.Instance.NetBlockHandler.MessageReceivedEvent += OnBlockContentReceived;
      _terminalBlock.OnMarkForClose += BlockMarkedForClose;
    }

    private bool IsOwnerOrFactionShare()
    {
      var player = MyAPIGateway.Session.Player;
      var relation = (_block.OwnerId > 0 ? player.GetRelationTo(_block.OwnerId) : MyRelationsBetweenPlayerAndBlock.NoOwnership);

      return relation == MyRelationsBetweenPlayerAndBlock.Owner || relation == MyRelationsBetweenPlayerAndBlock.FactionShare;
    }

    private void SaveConfigAction()
    {
      var appContent = new AppContent()
      {
        SurfaceName = _surface.Name,
        Layout = _app.WindowBarButtons.CurrentLayout,
        ChartIntervalIndex = _app.OverviewPanel.ChartPanel.ChartIntervalIndex,
        BatteryChartEnabled = _app.OverviewPanel.ChartPanel.BatteryOutputAsProduction,
        ChartDataColors = _app.OverviewPanel.ChartPanel.DataColors
      };

      var blockContent = GameSession.Instance.BlockHandler.SaveAppContent(_block, appContent);
      if (MyAPIGateway.Multiplayer.MultiplayerActive && blockContent != null)
      {
        blockContent.NetworkId = MyAPIGateway.Session.Player.SteamUserId;
        GameSession.Instance.NetBlockHandler.Broadcast(blockContent);
      }
    }

    private void OnBlockContentReceived(BlockStorageContent blockContent)
    {
      if (blockContent.BlockId != _block.EntityId)
        return;

      var appContent = blockContent.GetAppContent(_surface.Name);
      if (appContent != null)
        _app.ApplySettings(appContent.GetValueOrDefault());
    }

    public override void Dispose()
    {
      base.Dispose();

      GameSession.Instance.RemoveManagerFromBlock(_block);

      _app?.Dispose();
      _terminalBlock.OnMarkForClose -= BlockMarkedForClose;
      GameSession.Instance.NetBlockHandler.MessageReceivedEvent -= OnBlockContentReceived;
    }

    private void BlockMarkedForClose(IMyEntity ent)
    {
      Dispose();
    }

    public override void Run()
    {
      // Sandbox.Game.MyVisualScriptLogicProvider.SendChatMessage($"Run", "SampleApp");
      try
      {
        if (!_init && ticks++ < (6 * 2)) // 2 secs
          return;

        if (!IsOwnerOrFactionShare())
          return;

        if (!_init)
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
        _app?.Dispose();
        _app = null;
        MyLog.Default.WriteLineAndConsole($"{e.Message}\n{e.StackTrace}");

        if (MyAPIGateway.Session?.Player != null)
          MyAPIGateway.Utilities.ShowNotification($"[ ERROR: {GetType().FullName}: {e.Message} ]", 5000, MyFontEnum.Red);
      }
    }
  }
}