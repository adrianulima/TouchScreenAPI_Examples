using System;
using Lima.API;
using Sandbox.ModAPI;
using VRage.Game.Components;

namespace Lima
{
  [MySessionComponentDescriptor(MyUpdateOrder.AfterSimulation)]
  public class GameExampleSession : MySessionComponentBase
  {
    public TouchUiKit Api { get; private set; }
    public static GameExampleSession Instance;

    public override void LoadData()
    {
      if (MyAPIGateway.Utilities.IsDedicated)
        return;

      // Only for clients
      Instance = this;
      Api = new TouchUiKit();
      Api.Load();
    }

    protected override void UnloadData()
    {
      Api?.Unload();
      Instance = null;
    }
  }
}