using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.UI;

public class LocalPlayer : NetworkBehaviour {

    public GameObject Explosion;

    public GameObject PlayerPrefab;

    private GameObject _playerModel;
    private GameObject _playerGhost;

    private PlayerHUD _HUD;

    private bool _isPlayerSpawned;
    
    [SyncVar (hook = "OnChangeName")]
	public string PlayerName = "player";

    [SyncVar (hook = "OnChangeHealth")]
    public int HealthValue = 100;

    [SyncVar(hook = "OnChangeVisibilty")]
    public bool Visible = true;


    [Command]
    public void CmdChangeName(string newName)
    {
        PlayerName = newName;
        _HUD.UpdateName(PlayerName);
    }

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
        //if (!isLocalPlayer) return;
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

    void OnChangeVisibilty(bool visible)
    {
        Visible = visible;
        if (isLocalPlayer)
        {
            _HUD.SetVisible(true);
            _playerModel.SetActive(visible);
            _playerGhost.SetActive(!visible);
            //if (!visible && _isUsingOriginalMaterials)
            //{
            //    this.SwapMaterials(false);
            //    _isUsingOriginalMaterials = false;
            //}
            //if (visible && !_isUsingOriginalMaterials)
            //{
            //    this.SwapMaterials(true);
            //    _isUsingOriginalMaterials = true;
            //}
        }
        else
        {
            _HUD.SetVisible(visible);
            _playerModel.SetActive(visible);
        }
    }

    void OnChangeName (string n)
    {
        PlayerName = n;
        _HUD.UpdateName(PlayerName);
    }

    private void SetupPlayer()
    {
        if (isLocalPlayer)
        {
            // Server is ready.
            this.SetControllability(true);
            CameraFollow360.player = this.gameObject.transform;
            
            this.transform.position = GameController.GetUniqueSpawnPosition();
        }

        this.SetVisibility(true);
        _playerModel.SetActive(true);
        this.OnPropertyChange();
    }

    public void SetVisibility(bool visible)
    {
        Visible = visible;
    }

    private void SetControllability(bool control)
    {
        GetComponent<TankPlayerController>().enabled = control;
    }

    void Awake()
    {
        Debug.Assert(transform.Find("Model") != null, "Fail to find object called 'Model'");
        _playerModel = transform.Find("Model").gameObject;

         _HUD = this.GetComponent<PlayerHUD>();
        Debug.Assert(_HUD != null, "Fail to find component PlayerHUD");

        Debug.Assert(transform.Find("Ghost") != null, "Fail to find object called 'Ghost'");
        _playerGhost = transform.Find("Ghost").gameObject;

        _playerModel.SetActive(false);
        _playerGhost.SetActive(false);
        this.SetVisibility(false);
        this.SetControllability(false);
    }

    public override void OnStartLocalPlayer()
    {
       // Debug.Log("on start local player");
       //// this.IsLocalPlayer = isLocalPlayer;
       // //GameObject actual = Instantiate(this.PlayerPrefab, Vector3.zero, Quaternion.identity);
       // //actual.transform.SetParent(this.transform);
        _HUD.IsLocalPlayer = isLocalPlayer;

        base.OnStartLocalPlayer();
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
