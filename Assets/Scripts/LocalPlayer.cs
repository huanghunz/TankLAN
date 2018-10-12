using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.UI;

public class LocalPlayer : NetworkBehaviour {

    public GameObject Explosion;
    
	//private string _textboxname = "";
	//private string _colourboxname = "";

    private GameObject _playerModel;
    private Renderer[] _renderers;

    private PlayerHUD _HUD;

    public Material InvisibleVisual;
    private Material[] _materialsOriginal;

    private bool _isPlayerSpawned;
    private bool _isUsingOriginalMaterials = true;
    
    [SyncVar (hook = "OnChangeName")]
	public string PlayerName = "player";

	//[SyncVar (hook = "OnChangeColour")]
	//public string PlayerVisibility = "#ffffff";

    [SyncVar (hook = "OnChangeHealth")]
    public int HealthValue = 100;

    [Command]
    public void CmdChangeName(string newName)
    {
        PlayerName = newName;
        _HUD.UpdateName(PlayerName);
    }

    //[Command]
    //public void CmdChangeColour(string newColour)
    //{
    //    PlayerVisiblity = newColour;
        
    //    foreach (Renderer r in _renderers)
    //    {
    //        r.material.SetColor("_Color", ColorFromHex(PlayerVisiblity));
    //    }
    //}

    [Command]
    public void CmdChangeHealth(int hitValue)
    {
        HealthValue += hitValue;
        _HUD.UpdateHealth(HealthValue);

        if (HealthValue <= 0)
        {
            GameObject e = Instantiate(this.Explosion, this.transform.position, Quaternion.identity);
            NetworkServer.Spawn(e);
            this.GetComponent<Rigidbody>().velocity = Vector3.zero;
            this.RpcRespawn();
            this.HealthValue = 100;
            Destroy(e, 3f);
        }
    }

    [ClientRpc]
    public void RpcRespawn()
    {
        if (!isLocalPlayer) return;
        this.transform.position = GameController.GetUniqueSpawnPosition();
    }
    
    public void AddDamage(int damage)
    {
        if (!isLocalPlayer) return;
        this.CmdChangeHealth(damage);
    }

    private void OnPropertyChange()
    {
       // this.OnChangeColour(this.PlayerVisiblity);
        this.OnChangeName(this.PlayerName);
        this.OnChangeHealth(this.HealthValue);
    }

    void OnChangeHealth(int n)
    {
        HealthValue = n;
        _HUD.UpdateHealth(HealthValue);
    }

    void OnChangeName (string n)
    {
        PlayerName = n;
        _HUD.UpdateName(PlayerName);
    }

  //  void OnChangeColour (string n)
  //  {
  //      PlayerVisiblity = n;
		//Renderer[] rends = GetComponentsInChildren<Renderer>( );

  //      foreach( Renderer r in rends )
  //      {
  //       	if(r.gameObject.name == "BODY")
  //          	r.material.SetColor("_Color", ColorFromHex(PlayerVisiblity));
  //      }
  //  }
	

	//void OnGUI()
	//{
	//	if(isLocalPlayer)
	//	{
	//		_textboxname = GUI.TextField (new Rect (25, 15, 100, 25), _textboxname);
 //           if (GUI.Button(new Rect(130, 15, 35, 25), "Set"))
 //           {
 //               Debug.Log("CALL cmd change name");
 //               CmdChangeName(_textboxname);
 //           }

	//		_colourboxname = GUI.TextField (new Rect (170, 15, 100, 25), _colourboxname);
	//		if(GUI.Button(new Rect(275,15,35,25),"Set"))
	//			CmdChangeColour(_colourboxname);
	//	}
	//}


	//Credit for this method: from http://answers.unity3d.com/questions/812240/convert-hex-int-to-colorcolor32.html
	//hex for testing green: 04BF3404  red: 9F121204  blue: 221E9004
	//Color ColorFromHex(string hex)
	//{
	//	hex = hex.Replace ("0x", "");
 //       hex = hex.Replace ("#", "");
 //       byte a = 255;
 //       byte r = byte.Parse(hex.Substring(0,2), System.Globalization.NumberStyles.HexNumber);
 //       byte g = byte.Parse(hex.Substring(2,2), System.Globalization.NumberStyles.HexNumber);
 //       byte b = byte.Parse(hex.Substring(4,2), System.Globalization.NumberStyles.HexNumber);
 //       if(hex.Length == 8)
 //       {
 //            a = byte.Parse(hex.Substring(4,2), System.Globalization.NumberStyles.HexNumber);
 //       }
 //       return new Color32(r,g,b,a);
 //   }

    private void SetupPlayer()
    {
        if (isLocalPlayer)
        {
            // Server is ready.
            this.SetVisibility(true);
            this.SetControllability(true);
            CameraFollow360.player = this.gameObject.transform;
            
            this.transform.position = GameController.GetUniqueSpawnPosition();
        }
        else
        {
            this.SetVisibility(true);
        }

      //  Debug.Log("Manual hook change name");
        this.OnPropertyChange();
    }

    public void SetVisibility(bool visibleForAll, bool visibleLocal = true)
    {
        _playerModel.SetActive(visibleForAll);
        _HUD.SetVisible(visibleForAll);

        if (!visibleForAll && visibleLocal && isLocalPlayer && _isUsingOriginalMaterials)
        {
            _playerModel.SetActive(true);
            _HUD.SetVisible(true);
            this.SwapMaterials(false);
        }
        if (visibleForAll && !_isUsingOriginalMaterials)
        {
            this.SwapMaterials(true);
        }
    }

    private void SwapMaterials(bool useOriginal)
    {
        if (useOriginal)
        {
            for (int i = 0; i < _renderers.Length; ++i)
            {
                _renderers[i].material = _materialsOriginal[i];
            }

            _isUsingOriginalMaterials = true;
        }
        else
        {
            for (int i = 0; i < _renderers.Length; ++i)
            {
                _renderers[i].material = this.InvisibleVisual;
            }
            _isUsingOriginalMaterials = false;
        }
    }

    private void SetControllability(bool control)
    {
        GetComponent<PlayerController>().enabled = control;
    }

    void Awake()
    {
        _playerModel = transform.Find("Model").gameObject;
        Debug.Assert(_playerModel != null, "Fail to find object called 'Model'");
        _renderers = _playerModel.GetComponentsInChildren<Renderer>();

        _materialsOriginal = new Material[_renderers.Length];
        for (int i = 0; i < _renderers.Length; ++i)
        {
            _materialsOriginal[i] = new Material(_renderers[i].material);
        }

        _HUD = this.GetComponent<PlayerHUD>();

        this.SetVisibility(false, true);
        this.SetControllability(false);
    }

    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();

        _HUD.IsLocalPlayer = isLocalPlayer;
    }

    void Update()
	{
        if (!_isPlayerSpawned && GameController.IsMazeReady)
        {
            _isPlayerSpawned = true;
            _playerModel.SetActive(true);
            this.SetupPlayer();
        }
	}
}
