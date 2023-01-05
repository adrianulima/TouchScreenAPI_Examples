using System;
using System.Collections.Generic;
using Lima.API;
using Sandbox.Game.EntityComponents;
using Sandbox.ModAPI;
using VRage.Game.Components;
using VRage.Game.ModAPI;

namespace Lima
{
  [MySessionComponentDescriptor(MyUpdateOrder.AfterSimulation)]
  public class GameSession : MySessionComponentBase
  {
    public TouchScreenAPI Api { get; private set; }
    public static GameSession Instance;

    private List<ElectricNetworkManager> _electricManagers;
    public BlockStorageHandler BlockHandler;
    public FileStorageHandler FileHandler;
    public NetworkHandler<BlockStorageContent> NetHandler;

    public override void LoadData()
    {
      BlockHandler = new BlockStorageHandler();
      NetHandler = new NetworkHandler<BlockStorageContent>();
      NetHandler.Init(); // Init on server and clients

      if (MyAPIGateway.Utilities.IsDedicated)
      {
        NetHandler.MessageReceivedEvent += BlockContentReceivedServer;
        return;
      }

      Instance = this;
      FileHandler = new FileStorageHandler();
      Api = new TouchScreenAPI();
      Api.Load();
    }

    private void BlockContentReceivedServer(BlockStorageContent blockContent)
    {
      var block = MyAPIGateway.Entities.GetEntityById(blockContent.BlockId) as IMyCubeBlock;

      if (block != null)
        BlockHandler.SaveBlockContent(block, blockContent);
    }

    public ElectricNetworkManager GetElectricManagerForBlock(IMyCubeBlock lcdBlock)
    {
      if (MyAPIGateway.Utilities.IsDedicated)
        return null;

      if (_electricManagers != null)
      {
        foreach (var manager in _electricManagers)
          if (manager.AddBlockIfSameGrid(lcdBlock))
            return manager;
      }
      else
        _electricManagers = new List<ElectricNetworkManager>();

      var newManager = new ElectricNetworkManager(lcdBlock);
      _electricManagers.Add(newManager);
      return newManager;
    }

    public void RemoveManagerFromBlock(IMyCubeBlock lcdBlock)
    {
      if (_electricManagers != null)
      {
        foreach (var manager in _electricManagers)
          if (manager.RemoveBlockAndCount(lcdBlock))
          {
            manager.Dispose();
            _electricManagers.Remove(manager);
            return;
          }
      }
    }

    protected override void UnloadData()
    {
      if (_electricManagers != null)
        foreach (var manager in _electricManagers)
          manager.Dispose();

      GameSession.Instance.NetHandler.MessageReceivedEvent -= BlockContentReceivedServer;
      NetHandler?.Dispose();
      Api?.Unload();
      Instance = null;
      _electricManagers = null;
    }

    public override void UpdateAfterSimulation()
    {
      if (_electricManagers != null)
        foreach (var manager in _electricManagers)
          manager.Update();
    }

    public override void SaveData()
    {
      if (FileHandler != null)
        FileHandler.Save(_electricManagers);
    }
  }
}