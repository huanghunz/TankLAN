using System.Collections;
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
    private static List<Vector3> s_spawnPositionInUse;
    private static bool s_isMazeReady = false;

    [SyncVar(hook = "ServerMazeReady")]
    private bool serverSizeMazeReady = false;

    private SetupMaze _maze;
    private PowerUp _powerUpItems;
    private bool _isClientSetup;

    private void ServerMazeReady(bool isReady)
    {
        serverSizeMazeReady = isReady;
    }

    public static Vector3 GetUniqueSpawnPosition()
    {
        if (!s_isMazeReady)
        {
            Debug.LogWarning("Maze creation is not ready");
            return Vector3.zero;
        }

        if (s_spawnPositions.Count == 0)
        {
            for(int i = 0; i < s_spawnPositionInUse.Count; ++i)
            {
                s_spawnPositions.Add(s_spawnPositionInUse[i]);
            }
            s_spawnPositionInUse.Clear();
        }

        if (s_spawnPositions.Count == 0)
        {
            Debug.LogWarning("s_spawnPositions.Count == 0");
            return Vector3.zero;
        }

        Vector3 pos = s_spawnPositions[Random.Range(0, s_spawnPositions.Count)];
        s_spawnPositions.Remove(pos);
        s_spawnPositionInUse.Add(pos);
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

    private void Update()
    {
        if (!isServer && serverSizeMazeReady && !_isClientSetup)
        {
            _isClientSetup = true;
            
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

            InitialStatus();
        }
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

        int numPowerUp = Mathf.FloorToInt(_maze.Size.x * _maze.Size.z / 3f);
        _powerUpItems.SpawnPowerupItems(numPowerUp);
        serverSizeMazeReady = true;
    }

    public static void InitialStatus()
    {
        var positions = FindObjectsOfType<NetworkStartPosition>();
        s_spawnPositions = new List<Vector3>();
        s_spawnPositionInUse = new List<Vector3>();
        foreach (var position in positions)
        {
            s_spawnPositions.Add(position.transform.position);
        }
        s_isMazeReady = true;
    }
}
