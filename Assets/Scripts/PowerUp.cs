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

    private List<PowerUpItem> _powerupItems;

    Dictionary<LocalPlayer, Dictionary<string, float>> _playerPowerUp;

    private void Awake()
    {
        _playerPowerUp = new Dictionary<LocalPlayer, Dictionary<string, float>>();
    }

    private void Update()
    {
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

        //foreach (LocalPlayer player in _playerPowerUp.Keys)
        //{
        //    var keys = _playerPowerUp[player].Keys;
        //    foreach (string type in keys)
        //    {
        //        if (_playerPowerUp[player][type] == 0f)
        //        {
        //            continue;
        //        }

        //           _playerPowerUp[player][type] -= Time.deltaTime;
        //        if (_playerPowerUp[player][type] < 0f)
        //        {
        //            this.OnPowerUpEnd(player, type);
        //            _playerPowerUp[player][type] = 0;
        //        }
        //    }
        //}
    }

    //public override void OnStartServer()
    public void SpawnPowerupItems()
    {
        _powerupItems = new List<PowerUpItem>();

        int numTypes = Enum.GetNames(typeof(PowerUpItem.Types)).Length;
        for (int i = 0; i < NumberOfItems; i++)
        {
            var item = (GameObject)Instantiate(PowerUpPrefab, Vector3.zero, Quaternion.identity);
            Vector3 pos = GameController.GetUniqueSpawnPosition();
            item.transform.position = new Vector3(pos.x, 10f, pos.z);
            NetworkServer.Spawn(item);

            _powerupItems.Add(item.GetComponent<PowerUpItem>());
            _powerupItems[i].PowerUpType = (PowerUpItem.Types)Random.Range(0, numTypes);
            _powerupItems[i].OnTriggerEntered += this.OnPlayerTriggered;

            Vector3 targetPos = new Vector3(pos.x, 1.25f, pos.z);
            StartCoroutine(this.MoveTo(item, targetPos, Random.Range(20,50)));
        }
    }

    private void OnDestroy()
    {
        if (_playerPowerUp == null) return;

        foreach(PowerUpItem item in _powerupItems)
        {
            item.OnTriggerEntered -= this.OnPlayerTriggered;
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
                TopDownViewCamera.ENABLE_VIEW = true;
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
                TopDownViewCamera.ENABLE_VIEW = false;
                break;
            case "Invisible":
                player.SetVisibility(true);
                break;
        }
    }

    private IEnumerator MoveTo(GameObject go, Vector3 targetPos, float speed)
    {
        var delay = new WaitForEndOfFrame();
        
        while (go.transform.position != targetPos)
        {
            go.transform.position = Vector3.MoveTowards(go.transform.position, targetPos, Time.deltaTime * speed);

            yield return delay;
        }
    }
}