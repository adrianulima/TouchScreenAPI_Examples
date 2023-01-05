using System;
using VRageMath;
using Sandbox.Game.Entities;
using Sandbox.ModAPI;
using VRage.Game;
using VRage.Game.ModAPI;
using System.Collections.Generic;
using Sandbox.Game.EntityComponents;
using VRage;
using SpaceEngineers.Game.ModAPI;

namespace Lima
{
  public class ElectricNetworkManager
  {
    public struct PowerStats
    {
      public float Consumption;
      public float MaxConsumption;
      public float Production;
      public float MaxProduction;
      public float BatteryOutput;
      public float BatteryMaxOutput;
    }

    public struct BatteryStats
    {
      public float BatteryInput;
      public float BatteryMaxInput;
      public float BatteryCharge;
      public float BatteryMaxCharge;
      public float BatteryHoursLeft;
      public MyResourceStateEnum EnergyState;
    }

    private readonly List<IMyCubeBlock> _lcdBlocks;
    private MyDefinitionId _electricityId = MyResourceDistributorComponent.ElectricityId;

    public const int TicksPerSecond = (int)((MyEngineConstants.UPDATE_STEPS_PER_MINUTE / 60));
    private int _tick = TicksPerSecond - 1;
    public event Action UpdateEvent;

    public PowerStats CurrentPowerStats = new PowerStats();
    public BatteryStats CurrentBatteryStats = new BatteryStats();

    private readonly int _maxHistory = 1800; // 30 min
    public readonly List<PowerStats> PowerStatsHistory = new List<PowerStats>();

    private readonly List<IMyCubeGrid> _grids = new List<IMyCubeGrid>();

    private readonly List<MyCubeBlock> _inputList = new List<MyCubeBlock>();
    private readonly List<MyCubeBlock> _outputList = new List<MyCubeBlock>();
    private readonly List<MyCubeBlock> _thrustersList = new List<MyCubeBlock>();

    public readonly Dictionary<string, Vector2> ProductionBlocks = new Dictionary<string, Vector2>();
    public readonly Dictionary<string, Vector2> ConsumptionBlocks = new Dictionary<string, Vector2>();

    public ElectricNetworkManager(IMyCubeBlock lcdBlock)
    {
      _lcdBlocks = new List<IMyCubeBlock>() { lcdBlock };
      HandleGrid(lcdBlock.CubeGrid);

      LoadAppContent();
    }

    public FileStorageHandler.ManagerContent GenerateManagerContent()
    {
      return new FileStorageHandler.ManagerContent()
      {
        GridId = _lcdBlocks[0].CubeGrid.EntityId,
        PowerStatsHistory = PowerStatsHistory
      };
    }

    public void LoadAppContent()
    {
      var loadContent = GameSession.Instance.FileHandler.GetManagerContent(_lcdBlocks[0].CubeGrid.EntityId);
      if (loadContent != null)
      {
        var content = loadContent.GetValueOrDefault();

        PowerStatsHistory.AddRange(content.PowerStatsHistory);
      }
    }

    public void Dispose()
    {
      Clear();
      PowerStatsHistory.Clear();
      _lcdBlocks.Clear();
      UpdateEvent = null;
    }

    public void Clear()
    {
      _grids.Clear();
      _inputList.Clear();
      _outputList.Clear();
      _thrustersList.Clear();
      ProductionBlocks.Clear();
      ConsumptionBlocks.Clear();

      foreach (MyCubeGrid grid in _grids)
      {
        grid.OnBlockAdded -= OnBlockAddedToGrid;
        grid.OnBlockRemoved -= OnBlockRemovedFromGrid;
        grid.OnConnectionChanged -= OnConnectGrid;
      }
    }

    private void HandleGrid(IMyCubeGrid cubeGrid)
    {
      MyAPIGateway.GridGroups.GetGroup(cubeGrid, GridLinkTypeEnum.Physical, _grids);

      foreach (MyCubeGrid grid in _grids)
      {
        grid.OnBlockAdded -= OnBlockAddedToGrid;
        grid.OnBlockRemoved -= OnBlockRemovedFromGrid;
        grid.OnConnectionChanged -= OnConnectGrid;
        grid.OnBlockAdded += OnBlockAddedToGrid;
        grid.OnBlockRemoved += OnBlockRemovedFromGrid;
        grid.OnConnectionChanged += OnConnectGrid;

        foreach (MyCubeBlock block in grid.GetFatBlocks())
        {
          HandleBlock(block);
        }
      }
    }

    private void HandleBlock(MyCubeBlock block)
    {
      if (block is IMyBatteryBlock)
      {
        _inputList.Add(block);
      }
      else
      {
        var thruster = block as MyThrust;
        if (thruster != null && thruster.FuelDefinition.Id == _electricityId)
        {
          _thrustersList.Add(block);
        }
        else
        {
          MyResourceSinkComponent sink = block.Components?.Get<MyResourceSinkComponent>();
          if (sink != null && sink.AcceptedResources.IndexOf(_electricityId) != -1)
          {
            _inputList.Add(block);
          }
        }
      }

      var source = block.Components?.Get<MyResourceSourceComponent>();
      if (source != null && source.ResourceTypes.IndexOf(_electricityId) != -1)
      {
        _outputList.Add(block);
      }
    }

    private void OnConnectGrid(MyCubeGrid grid, GridLinkTypeEnum linkType)
    {
      if (linkType == GridLinkTypeEnum.Electrical)
      {
        Clear();
        HandleGrid(_lcdBlocks[0].CubeGrid);
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

    public bool AddBlockIfSameGrid(IMyCubeBlock lcdBlock)
    {
      if (_lcdBlocks[0].CubeGrid == lcdBlock.CubeGrid)
      {
        if (!_lcdBlocks.Contains(lcdBlock))
          _lcdBlocks.Add(lcdBlock);

        return true;
      }
      return false;
    }

    public bool RemoveBlockAndCount(IMyCubeBlock lcdBlock)
    {
      if (_grids.Contains(lcdBlock.CubeGrid))
      {
        if (_lcdBlocks.Contains(lcdBlock))
        {
          _lcdBlocks.Remove(lcdBlock);
          return _lcdBlocks.Count == 0;
        }
      }
      return false;
    }

    private void UpdatePowerDict(Dictionary<string, Vector2> dict, MyCubeBlock block, float power)
    {
      Vector2 count = Vector2.Zero;
      if (dict.TryGetValue(block.DefinitionDisplayNameText, out count))
        dict[block.DefinitionDisplayNameText] = new Vector2(count.X + 1, count.Y + power);
      else
        dict.Add(block.DefinitionDisplayNameText, new Vector2(1, power));
    }

    private void UpdateDistributiorStatus()
    {
      MyResourceDistributorComponent distributor = _lcdBlocks[0].CubeGrid.ResourceDistributor as MyResourceDistributorComponent;
      var grid = _lcdBlocks[0].CubeGrid as MyCubeGrid;
      CurrentBatteryStats.BatteryHoursLeft = distributor.RemainingFuelTimeByType(MyResourceDistributorComponent.ElectricityId, grid: grid);
      CurrentBatteryStats.EnergyState = distributor.ResourceStateByType(MyResourceDistributorComponent.ElectricityId, grid: grid);
    }

    public void Update()
    {
      _tick++;
      if (_tick % (TicksPerSecond * 1) != 0)// 1 second
        return;
      _tick = 0;

      CurrentPowerStats = new PowerStats();
      CurrentBatteryStats = new BatteryStats();

      UpdateDistributiorStatus();

      ProductionBlocks.Clear();
      ConsumptionBlocks.Clear();

      foreach (MyCubeBlock block in _thrustersList)
      {
        MyThrust thrust = block as MyThrust;
        var cons = thrust.MinPowerConsumption + thrust.CurrentStrength * (thrust.MaxPowerConsumption - thrust.MinPowerConsumption);
        CurrentPowerStats.Consumption += cons;
        CurrentPowerStats.MaxConsumption += cons;

        UpdatePowerDict(ConsumptionBlocks, block, cons);
      }

      foreach (MyCubeBlock block in _inputList)
      {
        MyResourceSinkComponent sink = block.Components?.Get<MyResourceSinkComponent>();
        if (sink != null)
        {
          var cons = sink.CurrentInputByType(_electricityId);
          CurrentPowerStats.Consumption += cons;

          IMyBatteryBlock battery = block as IMyBatteryBlock;
          if (battery != null)
          {
            CurrentBatteryStats.BatteryCharge += battery.CurrentStoredPower;
            CurrentBatteryStats.BatteryMaxCharge += battery.MaxStoredPower;

            if (battery.ChargeMode == Sandbox.ModAPI.Ingame.ChargeMode.Recharge)
              CurrentPowerStats.MaxConsumption += sink.MaxRequiredInputByType(_electricityId);
            else
              CurrentPowerStats.MaxConsumption += cons;

            CurrentBatteryStats.BatteryInput = cons;
            CurrentBatteryStats.BatteryMaxInput = sink.MaxRequiredInputByType(_electricityId);
          }
          else
          {
            if (block is IMyBeacon)
              CurrentPowerStats.MaxConsumption += Math.Max(cons, sink.MaxRequiredInputByType(_electricityId) / 1000f);
            else if (block is IMyRadioAntenna)
              CurrentPowerStats.MaxConsumption += Math.Max(cons, sink.MaxRequiredInputByType(_electricityId) * 100f);
            else if (block is IMyMedicalRoom)
              CurrentPowerStats.MaxConsumption += Math.Max(cons, sink.MaxRequiredInputByType(_electricityId) / 100f);
            else
              CurrentPowerStats.MaxConsumption += Math.Max(cons, sink.MaxRequiredInputByType(_electricityId));
          }

          UpdatePowerDict(ConsumptionBlocks, block, cons);
        }
      }

      foreach (MyCubeBlock block in _outputList)
      {
        var source = block.Components?.Get<MyResourceSourceComponent>();
        if (source != null)
        {
          var prod = source.CurrentOutputByType(_electricityId);
          if (block is IMyBatteryBlock)
          {
            CurrentPowerStats.BatteryOutput += prod;
            CurrentPowerStats.BatteryMaxOutput += source.MaxOutputByType(_electricityId);
          }
          else
          {
            CurrentPowerStats.Production += prod;
            CurrentPowerStats.MaxProduction += source.MaxOutputByType(_electricityId);
          }
          UpdatePowerDict(ProductionBlocks, block, prod);
        }
      }

      PowerStatsHistory.Add(CurrentPowerStats);

      UpdateEvent?.Invoke();
    }

    private void TrimHistoryLimit()
    {
      while (PowerStatsHistory.Count > _maxHistory)
        PowerStatsHistory.RemoveAt(0);
    }
  }
}