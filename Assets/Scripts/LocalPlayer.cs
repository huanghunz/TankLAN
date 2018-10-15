using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.UI;

public class LocalPlayer : NetworkBehaviour {

    public GameObject Explosion;

    public GameObject PlayerPrefab;

    private GameObject _playerModel;
    private Renderer[] _renderers;

    private PlayerHUD _HUD;

    public Material InvisibleVisual;
    private Material[] _materialsOriginal;

    private bool _isPlayerSpawned;
    private bool _isUsingOriginalMaterials = true;

    public bool IsLocalPlayer;
    
    [SyncVar (hook = "OnChangeName")]
	public string PlayerName = "player";

    [SyncVar (hook = "OnChangeHealth")]
    public int HealthValue = 100;

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
        if (!IsLocalPlayer) return;
        this.transform.position = GameController.GetUniqueSpawnPosition();
    }
    
    public void AddDamage(int damage)
    {
        if (!IsLocalPlayer) return;
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

    private void SetupPlayer()
    {
        if (IsLocalPlayer)
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

    public void SetVisibility(bool visibleForOther)
    {
        if (IsLocalPlayer)
        {
            _HUD.SetVisible(true);
            if (!visibleForOther && _isUsingOriginalMaterials)
            {
                this.SwapMaterials(false);
                _isUsingOriginalMaterials = false;
            }
            if (visibleForOther && !_isUsingOriginalMaterials)
            {
                this.SwapMaterials(true);
                _isUsingOriginalMaterials = true;
            }
        }
        else
        {
            _playerModel.SetActive(visibleForOther);
            _HUD.SetVisible(visibleForOther);
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
        GetComponent<TankPlayerController>().enabled = control;
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

        _playerModel.SetActive(false);
        this.SetVisibility(false);
        this.SetControllability(false);
    }

    public override void OnStartLocalPlayer()
    {
        this.IsLocalPlayer = isLocalPlayer;
        //GameObject actual = Instantiate(this.PlayerPrefab, Vector3.zero, Quaternion.identity);
        //actual.transform.SetParent(this.transform);
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
