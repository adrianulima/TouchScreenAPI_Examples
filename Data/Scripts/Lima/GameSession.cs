using System;
using Lima.API;
using Sandbox.ModAPI;
using VRage.Game.Components;
using VRage.Utils;

namespace Lima2
{
  [MySessionComponentDescriptor(MyUpdateOrder.AfterSimulation)]
  public class GameSession : MySessionComponentBase
  {
    public TouchScreenAPI Api { get; private set; }
    public static GameSession Instance;

    public ElectricNetworkManager ElectricMan;

    public override void LoadData()
    {
      if (MyAPIGateway.Utilities.IsDedicated)
        return;

      Instance = this;
      Api = new TouchScreenAPI();
      ElectricMan = new ElectricNetworkManager();
      Api.Load();
    }

    protected override void UnloadData()
    {
      ElectricMan.Dispose();

      Api?.Unload();
      Instance = null;
    }

    public override void UpdateAfterSimulation()
    {
      if (ElectricMan == null)
        return;

      ElectricMan.Update();
    }
  }
}