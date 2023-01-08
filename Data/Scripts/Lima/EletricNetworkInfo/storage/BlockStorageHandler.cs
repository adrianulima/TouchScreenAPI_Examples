using System;
using System.Collections.Generic;
using ProtoBuf;
using Sandbox.Game.EntityComponents;
using Sandbox.ModAPI;
using VRage.Game.ModAPI;
using VRage.Utils;
using VRageMath;

namespace Lima
{
  [ProtoContract(UseProtoMembersOnly = true)]
  public class BlockStorageContent : NetworkMessage
  {
    [ProtoMember(1)]
    public List<AppContent> Apps = new List<AppContent>();

    [ProtoMember(2)]
    public long BlockId;

    public BlockStorageContent() { }

    public AppContent? GetAppContent(string surfaceName)
    {
      foreach (var app in Apps)
      {
        if (app.SurfaceName == surfaceName)
          return app;
      }
      return null;
    }

    public void AddOrUpdateAppContent(AppContent appContent)
    {
      int index = Apps.FindIndex(app => app.SurfaceName == appContent.SurfaceName);
      if (index != -1)
        Apps[index] = appContent;
      else
        Apps.Add(appContent);
    }
  }

  [ProtoContract(UseProtoMembersOnly = true)]
  public struct AppContent
  {
    [ProtoMember(1)]
    public string SurfaceName;

    [ProtoMember(2)]
    public int Layout;

    [ProtoMember(3)]
    public int ChartIntervalIndex;

    [ProtoMember(4)]
    public bool BatteryChartEnabled;

    [ProtoMember(5)]
    public List<Color> ChartDataColors;
  }

  public class BlockStorageHandler
  {
    protected readonly Guid StorageGuid = new Guid("AD71A300-ED4F-4075-BB00-0A51EB910132");

    public AppContent? LoadAppContent(IMyCubeBlock block, string surfaceName)
    {
      if (block.Storage == null)
        return null;

      string rawData;
      if (!block.Storage.TryGetValue(StorageGuid, out rawData))
        return null;

      try
      {
        var blockContent = MyAPIGateway.Utilities.SerializeFromBinary<BlockStorageContent>(Convert.FromBase64String(rawData));
        if (blockContent != null)
          return blockContent.GetAppContent(surfaceName);
      }
      catch (Exception e)
      {
        MyLog.Default.WriteLineAndConsole($"{e.Message}\n{e.StackTrace}");
      }

      return null;
    }

    public BlockStorageContent SaveAppContent(IMyCubeBlock block, AppContent appContent)
    {
      BlockStorageContent blockContent = null;
      string rawData;
      if (block.Storage == null)
      {
        block.Storage = new MyModStorageComponent();
        blockContent = new BlockStorageContent();
        blockContent.NetworkId = MyAPIGateway.Session.Player.SteamUserId;
        blockContent.BlockId = block.EntityId;
      }
      else if (block.Storage.TryGetValue(StorageGuid, out rawData))
        blockContent = MyAPIGateway.Utilities.SerializeFromBinary<BlockStorageContent>(Convert.FromBase64String(rawData));

      if (blockContent != null)
      {
        blockContent.AddOrUpdateAppContent(appContent);
        block.Storage.SetValue(StorageGuid, Convert.ToBase64String(MyAPIGateway.Utilities.SerializeToBinary(blockContent)));
      }

      return blockContent;
    }

    public void SaveBlockContent(IMyCubeBlock block, BlockStorageContent blockContent)
    {
      if (block.Storage == null)
        block.Storage = new MyModStorageComponent();

      block.Storage.SetValue(StorageGuid, Convert.ToBase64String(MyAPIGateway.Utilities.SerializeToBinary(blockContent)));
    }
  }
}