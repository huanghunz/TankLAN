using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using MyMaze;

// message formmat
public class MessageTypes
{
    // Message ID
    public const short PlayerPrefabSelect = MsgType.Highest + 2;
    
    public class PlayerPrefabMessage : MessageBase
    {
        public short controllerID;
        public short prefabIndex;
    }
}

public class CustomNetworkManager : NetworkManager {

    public short PlayerPrefabIndex;

    public override void OnStartServer()
    {
        Debug.Log("OnStartServer");
        NetworkServer.RegisterHandler(MessageTypes.PlayerPrefabSelect, this.OnResponsePrefab);
        base.OnStartServer();
    }

    public override void OnClientConnect(NetworkConnection conn)
    {
        Debug.Log("OnClientConnect");
        client.RegisterHandler(MessageTypes.PlayerPrefabSelect, this.OnRequestPrefab);
        base.OnClientConnect(conn);
    }
    
    public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
    {
        Debug.Log("On server add player");
        MessageTypes.PlayerPrefabMessage msg = new MessageTypes.PlayerPrefabMessage();
        msg.controllerID = playerControllerId;
        NetworkServer.SendToClient(conn.connectionId, MessageTypes.PlayerPrefabSelect, msg);
        //base.OnServerAddPlayer(conn, playerControllerId, extraMessageReader);
    }

    private void OnResponsePrefab(NetworkMessage netMsg)
    {
        Debug.Log("On response prefabs");
        MessageTypes.PlayerPrefabMessage msg = netMsg.ReadMessage<MessageTypes.PlayerPrefabMessage>();

        // Update the player model
        this.playerPrefab = spawnPrefabs[msg.prefabIndex];
        base.OnServerAddPlayer(netMsg.conn, msg.controllerID);
    }

    private void OnRequestPrefab(NetworkMessage netMsg)
    {
        Debug.Log("On request prefabs");
        MessageTypes.PlayerPrefabMessage msg = new MessageTypes.PlayerPrefabMessage();
        msg.controllerID = netMsg.ReadMessage<MessageTypes.PlayerPrefabMessage>().controllerID;
        msg.prefabIndex = this.PlayerPrefabIndex;
        client.Send(MessageTypes.PlayerPrefabSelect, msg);
    }
}
