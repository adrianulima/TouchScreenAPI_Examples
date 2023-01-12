using System.Collections.Generic;
using Lima.API;
using Sandbox.ModAPI;
using VRage.Game;
using VRage.Game.Components;
using VRage.Game.ModAPI;

namespace Lima
{
  [MySessionComponentDescriptor(MyUpdateOrder.NoUpdate)]
  public class GameSession : MySessionComponentBase
  {
    public TouchUiKit Api { get; private set; }
    public static GameSession Instance;

    public override void LoadData()
    {

      if (MyAPIGateway.Utilities.IsDedicated)
        return;

      // Only for clients
      Instance = this;
      Api = new TouchUiKit();
      Api.Load();
    }
  }
}