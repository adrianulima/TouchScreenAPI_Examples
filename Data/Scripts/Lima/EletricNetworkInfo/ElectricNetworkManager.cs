using System;
using VRageMath;
using Lima.API;
using VRage.Game.GUI.TextPanel;
using Sandbox.Game.Entities;
using Sandbox.Game.GameSystems.TextSurfaceScripts;
using Sandbox.ModAPI;
using VRage.Game;
using VRage.Game.ModAPI;
using VRage.ModAPI;
using VRage.Utils;
using System.Collections.Generic;
using Sandbox.Game.EntityComponents;
using VRage;
using Sandbox.Definitions;
using Sandbox.Game.GameSystems;
using SpaceEngineers.Game.ModAPI;

namespace Lima2
{
  public class ElectricNetworkManager
  {
    private IMyCubeBlock _block;
    private bool _init = false;

    private MyDefinitionId _elec = MyResourceDistributorComponent.ElectricityId;

    public const int TicksPerSecond = (int)((MyEngineConstants.UPDATE_STEPS_PER_MINUTE / 60) / 10);
    private int _tick = 0;

    public float Consumption { get; private set; }
    public float MaxConsumption { get; private set; }

    public float Production { get; private set; }
    public float MaxProduction { get; private set; }

    public float BatteryCharge { get; private set; }
    public float BatteryMaxCharge { get; private set; }
    public int BatteryCount { get; private set; }
    public int IsBatteryCharging { get; private set; }

    private readonly List<IMyCubeGrid> _grids = new List<IMyCubeGrid>();

    private readonly List<MyCubeBlock> _inputList = new List<MyCubeBlock>();
    private readonly List<MyCubeBlock> _outputList = new List<MyCubeBlock>();
    private readonly List<MyCubeBlock> _thrustersList = new List<MyCubeBlock>();

    public Dictionary<string, Vector2> ProductionBlocks = new Dictionary<string, Vector2>();
    public Dictionary<string, Vector2> ConsumptionBlocks = new Dictionary<string, Vector2>();

    public ElectricNetworkManager()
    {
    }

    public void Dispose()
    {
      _grids.Clear();
      _inputList.Clear();
      _outputList.Clear();
      _thrustersList.Clear();
      ProductionBlocks.Clear();
      ConsumptionBlocks.Clear();
    }

    public void Init(IMyCubeBlock lcdBlock)
    {
      _init = true;
      _block = lcdBlock;

      MyAPIGateway.GridGroups.GetGroup(lcdBlock.CubeGrid, GridLinkTypeEnum.Electrical, _grids);

      foreach (MyCubeGrid grid in _grids)
      {
        grid.OnBlockAdded += OnBlockAddedToGrid;
        grid.OnBlockRemoved += OnBlockRemovedFromGrid;

        foreach (MyCubeBlock block in grid.GetFatBlocks())
        {
          HandleBlock(block);
        }
      }
      Sandbox.Game.MyVisualScriptLogicProvider.SendChatMessage($"_inputList: {_inputList.Count}", "Electric");
      Sandbox.Game.MyVisualScriptLogicProvider.SendChatMessage($"_outputList: {_outputList.Count}", "Electric");
      Sandbox.Game.MyVisualScriptLogicProvider.SendChatMessage($"_thrustersList: {_thrustersList.Count}", "Electric");
    }

    private void HandleBlock(MyCubeBlock block)
    {
      if (block is IMyBatteryBlock)
      {
        _inputList.Add(block);
      }
      else if (block is MyGyro)
      {
        // TODO: fix gyro
        _inputList.Add(block);
      }
      else
      {
        var thruster = block as MyThrust;
        if (thruster != null && thruster.FuelDefinition.Id == _elec)
        {
          _thrustersList.Add(block);
        }
        else
        {
          MyResourceSinkComponent sink = block.Components?.Get<MyResourceSinkComponent>();
          if (sink != null && sink.AcceptedResources.IndexOf(_elec) != -1)
          {
            _inputList.Add(block);
          }
        }
      }

      var source = block.Components?.Get<MyResourceSourceComponent>();
      if (source != null && source.ResourceTypes.IndexOf(_elec) != -1)
      {
        _outputList.Add(block);
      }
    }

    private void OnBlockAddedToGrid(IMySlimBlock slimBlock)
    {
      MyCubeBlock block = slimBlock.FatBlock as MyCubeBlock;
      if (block == null)
        return;
      HandleBlock(block);
    }

    private void OnBlockRemovedFromGrid(IMySlimBlock slimBlock)
    {
      MyCubeBlock block = slimBlock.FatBlock as MyCubeBlock;
      if (block == null)
        return;

      _inputList.Remove(block);
      _outputList.Remove(block);
      _thrustersList.Remove(block);
    }

    private void UpdatePowerDict(Dictionary<string, Vector2> dict, MyCubeBlock block, float power)
    {
      Vector2 count = Vector2.Zero;
      if (dict.TryGetValue(block.DefinitionDisplayNameText, out count))
        dict[block.DefinitionDisplayNameText] = new Vector2(count.X + 1, count.Y + power);
      else
        dict.Add(block.DefinitionDisplayNameText, new Vector2(1, power));
    }

    public bool Update()
    {
      if (!_init) return false;

      _tick++;
      if (_tick % (TicksPerSecond * 5) != 0)// 5 seconds
        return false;

      MaxConsumption = 0;
      Consumption = 0;
      Production = 0;
      MaxProduction = 0;
      BatteryCharge = 0;
      BatteryMaxCharge = 0;

      ProductionBlocks.Clear();
      ConsumptionBlocks.Clear();

      foreach (MyCubeBlock block in _thrustersList)
      {
        MyThrust thrust = block as MyThrust;
        var cons = thrust.MinPowerConsumption + thrust.CurrentStrength * (thrust.MaxPowerConsumption - thrust.MinPowerConsumption);
        Consumption += cons;
        MaxConsumption += cons;

        UpdatePowerDict(ConsumptionBlocks, block, cons);
      }

      foreach (MyCubeBlock block in _inputList)
      {
        MyResourceSinkComponent sink = block.Components?.Get<MyResourceSinkComponent>();
        if (sink != null)
        {
          var cons = sink.CurrentInputByType(_elec);
          Consumption += cons;

          IMyBatteryBlock battery = block as IMyBatteryBlock;
          if (battery != null)
          {
            // TotalStoring += battery.CurrentInput - battery.CurrentOutput;
            BatteryCharge += battery.CurrentStoredPower;
            BatteryMaxCharge += battery.MaxStoredPower;

            if (battery.ChargeMode == Sandbox.ModAPI.Ingame.ChargeMode.Recharge)
              MaxConsumption += sink.MaxRequiredInputByType(_elec);
            else
              MaxConsumption += cons;
          }
          else
          {
            if (block is IMyBeacon)
              MaxConsumption += Math.Max(cons, sink.MaxRequiredInputByType(_elec) / 1000f);
            else if (block is IMyRadioAntenna)
              MaxConsumption += Math.Max(cons, sink.MaxRequiredInputByType(_elec) * 100f);
            else if (block is IMyMedicalRoom)
              MaxConsumption += Math.Max(cons, sink.MaxRequiredInputByType(_elec) / 100f);
            else
              MaxConsumption += Math.Max(cons, sink.MaxRequiredInputByType(_elec));
          }


          UpdatePowerDict(ConsumptionBlocks, block, cons);
        }
      }

      foreach (MyCubeBlock block in _outputList)
      {
        var source = block.Components?.Get<MyResourceSourceComponent>();
        if (source != null)
        {
          var prod = source.CurrentOutputByType(_elec);
          Production += prod;
          MaxProduction += source.MaxOutputByType(_elec);

          UpdatePowerDict(ProductionBlocks, block, prod);
        }
      }

      // Sandbox.Game.MyVisualScriptLogicProvider.SendChatMessage($"{block.DefinitionDisplayNameText} {Math.Max(cons, sink.MaxRequiredInputByType(_elec))}", "Electric");
      // Sandbox.Game.MyVisualScriptLogicProvider.SendChatMessage($"{Consumption}/{MaxConsumption}", "Electric");
      // Sandbox.Game.MyVisualScriptLogicProvider.SendChatMessage($"{Production}/{MaxProduction}", "Electric");
      return true;
    }
  }
}