using Lima.API;
using Sandbox.ModAPI;
using VRage.Game.Components;

namespace Lima
{
  [MySessionComponentDescriptor(MyUpdateOrder.NoUpdate)]
  public class SampleSession : MySessionComponentBase
  {
    public TouchScreenAPI Api { get; private set; }
    public static SampleSession Instance;

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