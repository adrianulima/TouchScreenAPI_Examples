using System;
using Lima.API;
using Sandbox.ModAPI;
using VRage.Game.Components;
using VRage.Utils;

namespace Lima2
{
  [MySessionComponentDescriptor(MyUpdateOrder.NoUpdate)]
  public class GameSession : MySessionComponentBase
  {
    public TouchScreenAPI Api { get; private set; }
    public static GameSession Instance;

    public override void LoadData()
    {
      if (MyAPIGateway.Utilities.IsDedicated)
        return;

      Instance = this;
      Api = new TouchScreenAPI();
      Api.Load();
    }

    protected override void UnloadData()
    {
      Api?.Unload();
      Instance = null;
    }
  }
}