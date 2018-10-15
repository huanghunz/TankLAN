using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Prototype.NetworkLobby;

public class NetworkLobbyHook : LobbyHook {

    public override void OnLobbyServerSceneLoadedForPlayer(NetworkManager manager, GameObject lobbyPlayer, GameObject gamePlayer) {
        LobbyPlayer lobby = lobbyPlayer.GetComponent<LobbyPlayer>();
        LocalPlayer player = gamePlayer.GetComponent<LocalPlayer>();

        player.PlayerName = lobby.playerName;
        
       
        //GameObject playerPrefab = manager.spawnPrefabs[lobby.playerModelIdx];
        //player.PlayerPrefab = playerPrefab;

        //NetworkServer.ReplacePlayerForConnection
    }
    //public override GameObject OnLobbyServerCreateGamePlayer(
    //                NetworkConnection conn, short playerControllerId)
    //{
    //    GameObject _temp = (GameObject)GameObject.Instantiate(
    //        spawnPrefabs[playerPrefabIndex],
    //        startPositions[conn.connectionId].position,
    //        Quaternion.identity);

    //    NetworkServer.AddPlayerForConnection(conn, _temp, playerControllerId);
    //    return _temp;
    //}

}
