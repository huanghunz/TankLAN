using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ControlBase : NetworkBehaviour
{

    public string PlayerNameBase = "Hello World.";
    public GameObject PlayerPrefab;

    

    private bool _created;


	
    private void Update()
    {
        if (!_created && this.PlayerPrefab != null)
        {
            GameObject actual = Instantiate(this.PlayerPrefab);
            actual.GetComponent<LocalPlayer>().PlayerName = this.PlayerNameBase;
            _created = true;
            actual.GetComponent<LocalPlayer>().IsLocalPlayer = isLocalPlayer;
        }
    }
}
