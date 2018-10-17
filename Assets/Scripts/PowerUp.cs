using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using Random = UnityEngine.Random;

public class PowerUp : NetworkBehaviour
{
    public GameObject PowerUpPrefab;
    public int NumberOfItems = 5;

    public int _numPickedItems = 0;

    //private float RESPAWN_TIMER = 10f;
    //private float _respawnTimer = 0;

    private List<PowerUpItem> _powerupItems;

    Dictionary<LocalPlayer, Dictionary<string, float>> _playerPowerUp;

    private void Awake()
    {
        _playerPowerUp = new Dictionary<LocalPlayer, Dictionary<string, float>>();
     //   _respawnTimer = RESPAWN_TIMER;
    }

    private void Update()
    {
        //_respawnTimer -= Time.deltaTime;
        //if (_respawnTimer <= 0)
        //{
        //    _respawnTimer = RESPAWN_TIMER;
        //    this.SpawnPowerupItems(this.NumberOfItems);
        //}


        if (_playerPowerUp.Count == 0) return;

        int count = _playerPowerUp.Count;
        for (int i = 0; i < count; i++)
        {
            LocalPlayer playerKey = _playerPowerUp.Keys.ElementAt(i);
            int typeCount = _playerPowerUp[playerKey].Count;
            for(int j = 0; j < typeCount; ++j)
            {
                string typeKey = _playerPowerUp[playerKey].Keys.ElementAt(j);

                if (_playerPowerUp[playerKey][typeKey] == 0f)
                {
                    continue;
                }

                _playerPowerUp[playerKey][typeKey] -= Time.deltaTime;
                if (_playerPowerUp[playerKey][typeKey] < 0f)
                {
                    this.OnPowerUpEnd(playerKey, typeKey);
                    _playerPowerUp[playerKey][typeKey] = 0;
                }
            }
        }
    }

    //public override void OnStartServer()
    public void SpawnPowerupItems(int num)
    {
        this.NumberOfItems = num;
        _powerupItems = new List<PowerUpItem>();

        int numTypes = Enum.GetNames(typeof(PowerUpItem.Types)).Length;
        for (int i = 0; i < num; i++)
        {
            var item = (GameObject)Instantiate(PowerUpPrefab, Vector3.zero, Quaternion.identity);
            Vector3 pos = GameController.GetUniqueSpawnPosition();
            item.transform.position = new Vector3(pos.x, 10f, pos.z);
            var powerUpitem = item.GetComponent<PowerUpItem>();
            powerUpitem.PowerUpType = Random.Range(0, numTypes);
            NetworkServer.Spawn(item);

            _powerupItems.Add(powerUpitem);

            powerUpitem.OnTriggerEntered += this.OnPlayerTriggered;
            powerUpitem.OnDestorySelf += this.OnItemDestory;

            Vector3 targetPos = new Vector3(pos.x, 1.25f, pos.z);

            TankUtility.Utility.Instance.AnimateMove(item, targetPos, Random.Range(0.2f, 0.5f));
        }
    }


    void OnItemDestory(PowerUpItem item)
    {
        item.OnTriggerEntered -= this.OnPlayerTriggered;
        item.OnDestorySelf -= this.OnItemDestory;
    }


    private void OnDestroy()
    {
        if (_powerupItems == null) return;

        foreach (PowerUpItem item in _powerupItems)
        {
            if (item == null)
            {
                continue;
            }
            item.OnTriggerEntered -= this.OnPlayerTriggered;
            item.OnDestorySelf -= this.OnItemDestory;
        }
    }

    private void OnPlayerTriggered(GameObject player, PowerUpItem.Types type)
    {

        string typeStr = type.ToString();
        LocalPlayer localPlayer = player.GetComponent<LocalPlayer>();
        switch (type)
        {
            case PowerUpItem.Types.AddHealth:
                localPlayer.HealthValue = Mathf.Min(100, localPlayer.HealthValue + 5);
                break;
            case PowerUpItem.Types.Invisible:
                // Should use shader on the player visible objects
                localPlayer.SetVisibility(false);
                break;
            case PowerUpItem.Types.TopDownView:
                localPlayer.EnableMap = true;
                break;
        }

        if (!_playerPowerUp.ContainsKey(localPlayer))
        {
            _playerPowerUp.Add(localPlayer, new Dictionary<string, float>());
            
        }
        if (!_playerPowerUp[localPlayer].ContainsKey(typeStr))
        {
            _playerPowerUp[localPlayer].Add(typeStr, 0);
        }

        _playerPowerUp[localPlayer][typeStr] += 5f;
    }

    private void OnPowerUpEnd(LocalPlayer player, string type)
    {
        switch (type)
        {
            case "TopDownView":
                player.EnableMap = false;
                break;
            case "Invisible":
                player.SetVisibility(true);
                break;
        }
    }
}