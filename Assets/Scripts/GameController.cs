﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class GameController : NetworkBehaviour {

    public static bool IsMazeReady
    {
        get
        {
            return s_isMazeReady;
        }
    }
    
    private static List<Vector3> s_spawnPositions;
    private static bool s_isMazeReady = false;

    private SetupMaze _maze;
    private PowerUp _powerUpItems;

    public static Vector3 GetUniqueSpawnPosition()
    {
        if (!s_isMazeReady)
        {
            Debug.LogWarning("Maze creation is not ready");
            return Vector3.zero;
        }
        
        Vector3 pos = s_spawnPositions[Random.Range(0, s_spawnPositions.Count)];
        s_spawnPositions.Remove(pos);
        return pos;
    }

    public override void OnStartServer()
    {
        if (isServer)
        {
            _maze = FindObjectOfType<SetupMaze>();
            if (_maze != null)
            {
                _maze.CreateMaze();
                _maze.OnFinishGeneration += this.OnMazeReady;
            }
        }
    }

    public override void OnStartClient()
    {
        if (isServer)
        {
            return;
        }

        InitialStatus();
        _maze = FindObjectOfType<SetupMaze>();
        var mazeParts = GameObject.FindGameObjectsWithTag("MazeComponent");
       
        if (mazeParts == null)
        {
            return;
        }

        foreach (var mp in mazeParts)
        {
            mp.transform.localScale = Vector3.one * _maze.FinalScale;
        }
    }

    public override void OnStartLocalPlayer()
    {
        Debug.Log("start local player");
    }

    private void OnDestroy()
    {
        if (_maze != null)
        {
            _maze.OnFinishGeneration -= this.OnMazeReady;
        }
    }

    private void OnMazeReady()
    {
        InitialStatus();

        _powerUpItems = FindObjectOfType<PowerUp>();
        _powerUpItems.SpawnPowerupItems();
    }

    public static void InitialStatus()
    {
        var positions = FindObjectsOfType<NetworkStartPosition>();
        s_spawnPositions = new List<Vector3>();
        foreach (var position in positions)
        {
            s_spawnPositions.Add(position.transform.position);
        }
        s_isMazeReady = true;
    }
}
