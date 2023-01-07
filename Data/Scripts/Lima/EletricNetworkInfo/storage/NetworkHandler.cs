using System;
using System.Collections.Generic;
using ProtoBuf;
using Sandbox.ModAPI;
using VRage.Game.ModAPI;
using VRage.Utils;

namespace Lima
{
  [ProtoInclude(2, typeof(BlockStorageContent))]
  [ProtoInclude(3, typeof(GridStorageContent))]
  [ProtoContract(UseProtoMembersOnly = true)]
  public abstract class NetworkMessage
  {
    [ProtoMember(1)]
    public ulong NetworkId;

    public NetworkMessage() { }
  }

  public class NetworkHandler<T> where T : NetworkMessage
  {
    private ushort _channel;

    private List<IMyPlayer> _players = null;

    public event Action<T> MessageReceivedEvent;

    public bool AutoBroadcastEnabled = true;

    public NetworkHandler(ushort channel)
    {
      _channel = channel;
    }

    public void Init()
    {
      MyAPIGateway.Multiplayer.RegisterSecureMessageHandler(_channel, HandleMessage);
    }

    public void Dispose()
    {
      MyAPIGateway.Multiplayer.UnregisterSecureMessageHandler(_channel, HandleMessage);
    }

    public void Broadcast(T message)
    {
      var bytes = MyAPIGateway.Utilities.SerializeToBinary(message);
      if (MyAPIGateway.Multiplayer.IsServer)
        ForwardToPlayers(message, bytes);
      else
        MyAPIGateway.Multiplayer.SendMessageToServer(_channel, bytes);
    }

    private void HandleMessage(ushort handler, byte[] rawData, ulong id, bool isFromServer)
    {
      try
      {
        var message = MyAPIGateway.Utilities.SerializeFromBinary<T>(rawData);
        if (!isFromServer && MyAPIGateway.Multiplayer.IsServer && AutoBroadcastEnabled)
          ForwardToPlayers(message, rawData);

        MessageReceivedEvent?.Invoke(message);
      }
      catch (Exception e)
      {
        MyLog.Default.WriteLineAndConsole($"{e.Message}\n{e.StackTrace}");
      }
    }

    private void ForwardToPlayers(T message, byte[] rawData)
    {
      if (_players == null)
        _players = new List<IMyPlayer>(MyAPIGateway.Session.SessionSettings.MaxPlayers);
      else
        _players.Clear();

      MyAPIGateway.Players.GetPlayers(_players);

      foreach (var player in _players)
      {
        if (player.IsBot || player.SteamUserId == MyAPIGateway.Multiplayer.ServerId || player.SteamUserId == message.NetworkId)
          continue;

        MyAPIGateway.Multiplayer.SendMessageTo(_channel, rawData, player.SteamUserId);
      }

      _players.Clear();
    }
  }
}