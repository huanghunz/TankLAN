using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Prototype.NetworkLobby;

public class NetworkLobbyHook : LobbyHook {

    public override void OnLobbyServerSceneLoadedForPlayer(NetworkManager manager, GameObject lobbyPlayer, GameObject gamePlayer) {
        LobbyPlayer lobby = lobbyPlayer.GetComponent<LobbyPlayer>();
        LocalPlayer player = gamePlayer.GetComponent<LocalPlayer>();

        //player.PlayerName = lobby.playerName;
        //player.PlayerPrefab = LobbyManager.s_Singleton.spawnPrefabs[lobby.playerModelId];

        //Debug.Log("HOOKKKK?? : " + player.PlayerPrefab + " lobby.playerModelId: " + lobby.playerModelId
        //    + " model name: " + LobbyManager.s_Singleton.spawnPrefabs[lobby.playerModelId].name);

        //Debug.Log("lobby model id: " + LobbyManager.s_Singleton._playerPrefabIdx + " nameL " + lobby.playerModelName);



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
