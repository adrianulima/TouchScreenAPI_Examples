using Lima.API;
using Sandbox.ModAPI;
using VRage.Game.Components;

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