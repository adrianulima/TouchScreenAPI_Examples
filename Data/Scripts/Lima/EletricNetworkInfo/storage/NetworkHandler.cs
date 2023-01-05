using System;
using System.Collections.Generic;
using ProtoBuf;
using Sandbox.ModAPI;
using VRage.Game.ModAPI;
using VRage.Utils;

namespace Lima
{
  [ProtoInclude(2, typeof(BlockStorageContent))]
  [ProtoContract(UseProtoMembersOnly = true)]
  public abstract class NetworkMessage
  {
    [ProtoMember(1)]
    public ulong NetworkId;
  }

  public class NetworkHandler<T> where T : NetworkMessage
  {
    // TODO: Replace by mod id
    private const ushort _channel = 041414;

    private List<IMyPlayer> _players = null;

    public event Action<T> MessageReceivedEvent;

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
      // MyLog.Default.WriteLineAndConsole($"====== HandleMessage isFromServer {isFromServer}");
      try
      {
        var message = MyAPIGateway.Utilities.SerializeFromBinary<T>(rawData);
        // MyLog.Default.WriteLineAndConsole($"====== HandleMessage {message.NetworkId}");
        if (!isFromServer && MyAPIGateway.Multiplayer.IsServer)
          ForwardToPlayers(message, rawData);

        MessageReceivedEvent?.Invoke(message);
      }
      catch (Exception e)
      {
        MyLog.Default.WriteLineAndConsole($"{e.Message}\n{e.StackTrace}");
      }
    }

    int count = 0;
    private void ForwardToPlayers(T message, byte[] rawData)
    {
      if (count > 10)
        return;
      count += 1;

      // MyLog.Default.WriteLineAndConsole($"====== ForwardToPlayers {count}");

      if (_players == null)
        _players = new List<IMyPlayer>(MyAPIGateway.Session.SessionSettings.MaxPlayers);
      else
        _players.Clear();

      MyAPIGateway.Players.GetPlayers(_players);

      // MyLog.Default.WriteLineAndConsole($"====== _players.Count {_players.Count}");
      // MyLog.Default.WriteLineAndConsole($"====== ServerId {MyAPIGateway.Multiplayer.ServerId}");
      // MyLog.Default.WriteLineAndConsole($"====== message.NetworkId {message.NetworkId}");

      foreach (var player in _players)
      {
        if (player.IsBot || player.SteamUserId == MyAPIGateway.Multiplayer.ServerId || player.SteamUserId == message.NetworkId)
          continue;

        // MyLog.Default.WriteLineAndConsole($"====== Sending to {player.SteamUserId}");
        MyAPIGateway.Multiplayer.SendMessageTo(_channel, rawData, player.SteamUserId);
      }

      _players.Clear();
    }
  }
}