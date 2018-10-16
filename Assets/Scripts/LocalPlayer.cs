using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.UI;

public partial class LocalPlayer : NetworkBehaviour {

    // Local Player
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
        if (!_isPlayerSpawned) return;
        HealthValue = n;
        _HUD.UpdateHealth(HealthValue);
    }

    void OnChangeVisibilty(bool visible)
    {
        if (!_isPlayerSpawned) return;

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

        if (_HUD != null)
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
        _enableControl = control;
    }


    //public override void OnStartLocalPlayer()
    //{
       

    //    base.OnStartLocalPlayer();
    //}

    void Update()
	{
        if (!_isPlayerSpawned && GameController.IsMazeReady)
        {
            if (this.PlayerPrefab == null)
            {
                Debug.LogError("nul lplayer prefab");
                return;
            }

            GameObject actual = Instantiate(this.PlayerPrefab);
            actual.transform.SetParent(this.transform);
            actual.transform.localPosition = Vector3.zero;
            actual.transform.localRotation = Quaternion.identity;

            _HUD = actual.GetComponent<PlayerHUD>();
            Debug.Assert(_HUD != null, "Fail to find component PlayerHUD");
            _HUD.IsLocalPlayer = isLocalPlayer;
            _HUD.UpdateName(PlayerName);

            Debug.Assert(actual.transform.Find("Model") != null, "Fail to find object called 'Model'");
            _playerModel = actual.transform.Find("Model").gameObject;

            Debug.Assert(actual.transform.Find("Ghost") != null, "Fail to find object called 'Ghost'");
            _playerGhost = actual.transform.Find("Ghost").gameObject;

            _playerModel.SetActive(false);
            _playerGhost.SetActive(false);
            this.SetVisibility(false);
            this.SetControllability(false);

            //Fire Attribute setup
            ModelData data = actual.GetComponent<ModelData>();
            this.BulletForce = data.BulletForces;
            this.BulletPrefab = data.BulletPrefab;
            this.BulletSpwanPoint = data.BulletSpawnPositions;
            this.NumBullerPerShooting = data.NumBulletPerShooting;
            this.MaxNumBullet = data.MaxNumBullet;

            // Movement setup
            _rb = this.GetComponent<Rigidbody>();

            _isPlayerSpawned = true;
            _playerModel.SetActive(true);
            this.SetupPlayer();
        }

        if (_isPlayerSpawned)
        this.UpdateFireControl();
	}
}
