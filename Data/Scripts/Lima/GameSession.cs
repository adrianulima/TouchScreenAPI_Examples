using System.Collections.Generic;
using Lima.API;
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

    public override void LoadData()
    {
      if (MyAPIGateway.Utilities.IsDedicated)
        return;

      Instance = this;
      Api = new TouchScreenAPI();
      Api.Load();
    }

    public ElectricNetworkManager GetElectricManagerForBlock(IMyCubeBlock lcdBlock)
    {
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
  }
}