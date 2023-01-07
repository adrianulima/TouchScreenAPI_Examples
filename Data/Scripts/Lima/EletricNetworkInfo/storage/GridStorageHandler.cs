using System;
using ProtoBuf;
using Sandbox.Game.EntityComponents;
using Sandbox.ModAPI;
using VRage.Game.ModAPI;
using VRage.Utils;

namespace Lima
{
  [ProtoContract(UseProtoMembersOnly = true)]
  public class GridStorageContent : NetworkMessage
  {
    [ProtoMember(1)]
    public long GridId;

    [ProtoMember(2)]
    public ElectricNetworkManager.PowerStats[] History_0;
    [ProtoMember(3)]
    public ElectricNetworkManager.PowerStats[] History_1;
    [ProtoMember(4)]
    public ElectricNetworkManager.PowerStats[] History_2;
    [ProtoMember(5)]
    public ElectricNetworkManager.PowerStats[] History_3;
    [ProtoMember(6)]
    public ElectricNetworkManager.PowerStats[] History_4;

    public GridStorageContent() { }
  }

  public class GridStorageHandler
  {
    protected readonly Guid StorageGuid = new Guid("AD71A300-ED4F-4075-BB00-0A51EB910132");

    public GridStorageContent LoadGridContent(IMyCubeGrid grid)
    {
      if (grid.Storage == null)
        return null;

      string rawData;
      if (!grid.Storage.TryGetValue(StorageGuid, out rawData))
        return null;

      try
      {
        return MyAPIGateway.Utilities.SerializeFromBinary<GridStorageContent>(Convert.FromBase64String(rawData));
      }
      catch (Exception e)
      {
        MyLog.Default.WriteLineAndConsole($"{e.Message}\n{e.StackTrace}");
      }

      return null;
    }

    public void SaveGridContent(IMyCubeGrid grid, GridStorageContent gridContent)
    {
      if (grid.Storage == null)
        grid.Storage = new MyModStorageComponent();

      grid.Storage.SetValue(StorageGuid, Convert.ToBase64String(MyAPIGateway.Utilities.SerializeToBinary(gridContent)));
    }
  }
}