using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.UI;

public class LocalPlayer : NetworkBehaviour {

    public bool IsVisible
    {
        get;
        private set;
    }

    public float InvisibleTimer
    {
        get;
        set;
    }

	public Text NamePrefab;
	public Transform NamePos;

    public Slider HealthPrefab;
    public GameObject Explosion;
    

	private string _textboxname = "";
	private string _colourboxname = "";
    private Text _nameLabel;
    private Slider _healthBar;
    private GameObject _playerModel;
    private MeshRenderer[] _renderers;

    private bool _isPlayerSpawned;


    

    [SyncVar (hook = "OnChangeName")]
	public string PlayerName = "player";

	[SyncVar (hook = "OnChangeColour")]
	public string PlayerColour = "#ffffff";

    [SyncVar (hook = "OnChangeHealth")]
    public int HealthValue = 100;

    [Command]
    public void CmdChangeName(string newName)
    {
        PlayerName = newName;
        _nameLabel.text = PlayerName;
    }

    [Command]
    public void CmdChangeColour(string newColour)
    {
        PlayerColour = newColour;
        
        foreach (Renderer r in _renderers)
        {
            r.material.SetColor("_Color", ColorFromHex(PlayerColour));
        }
    }

    [Command]
    public void CmdChangeHealth(int hitValue)
    {
        HealthValue += hitValue;
        _healthBar.value = HealthValue;

        if (_healthBar.value <= 0)
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
        this.CmdChangeHealth(damage);
    }

    private void OnPropertyChange()
    {
        this.OnChangeColour(this.PlayerColour);
        this.OnChangeName(this.PlayerName);
        this.OnChangeHealth(this.HealthValue);
    }

    void OnChangeHealth(int n)
    {
        HealthValue = n;
        _healthBar.value = HealthValue;
    }

    void OnChangeName (string n)
    {
       // Debug.Log("trigger hook change name");
        PlayerName = n;
		_nameLabel.text = PlayerName;
    }

    void OnChangeColour (string n)
    {
        PlayerColour = n;
		Renderer[] rends = GetComponentsInChildren<Renderer>( );

        foreach( Renderer r in rends )
        {
         	if(r.gameObject.name == "BODY")
            	r.material.SetColor("_Color", ColorFromHex(PlayerColour));
        }
    }
	

	void OnGUI()
	{
		if(isLocalPlayer)
		{
			_textboxname = GUI.TextField (new Rect (25, 15, 100, 25), _textboxname);
            if (GUI.Button(new Rect(130, 15, 35, 25), "Set"))
            {
                Debug.Log("CALL cmd change name");
                CmdChangeName(_textboxname);
            }

			_colourboxname = GUI.TextField (new Rect (170, 15, 100, 25), _colourboxname);
			if(GUI.Button(new Rect(275,15,35,25),"Set"))
				CmdChangeColour(_colourboxname);
		}
	}


	//Credit for this method: from http://answers.unity3d.com/questions/812240/convert-hex-int-to-colorcolor32.html
	//hex for testing green: 04BF3404  red: 9F121204  blue: 221E9004
	Color ColorFromHex(string hex)
	{
		hex = hex.Replace ("0x", "");
        hex = hex.Replace ("#", "");
        byte a = 255;
        byte r = byte.Parse(hex.Substring(0,2), System.Globalization.NumberStyles.HexNumber);
        byte g = byte.Parse(hex.Substring(2,2), System.Globalization.NumberStyles.HexNumber);
        byte b = byte.Parse(hex.Substring(4,2), System.Globalization.NumberStyles.HexNumber);
        if(hex.Length == 8)
        {
             a = byte.Parse(hex.Substring(4,2), System.Globalization.NumberStyles.HexNumber);
        }
        return new Color32(r,g,b,a);
    }

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

    public void SetVisibility(bool visible)
    {
        this.IsVisible = visible;
        //_playerModel.SetActive(visible);
        _nameLabel.gameObject.SetActive(visible);
        _healthBar.gameObject.SetActive(visible);

        TankUtility.Utility.Instance.
            AnimateAlpha(_renderers, visible ? 1 : 0, 0.5f);
    }

    private void SetControllability(bool control)
    {
        GetComponent<PlayerController>().enabled = control;
    }

    void Awake()
    {
        _playerModel = transform.Find("Model").gameObject;
        Debug.Assert(_playerModel != null, "Fail to find object called 'Model'");
        _renderers = _playerModel.GetComponentsInChildren<MeshRenderer>();
        if (_renderers == null) Debug.Log("null r");
        // All UI should be a child of a canvas
        GameObject canvas = GameObject.FindWithTag("MainCanvas");
        _nameLabel = Instantiate(NamePrefab, Vector3.zero, Quaternion.identity) as Text;
        _nameLabel.transform.SetParent(canvas.transform);
        _healthBar = Instantiate(HealthPrefab, Vector3.zero, Quaternion.identity) as Slider;
        _healthBar.transform.SetParent(canvas.transform);

        _playerModel.SetActive(false);

        this.SetVisibility(false);
        this.SetControllability(false);
    }

    void OnDestroy()
	{
        if (_nameLabel != null && _healthBar != null)
        {
            Destroy(_nameLabel.gameObject);
            Destroy(_healthBar.gameObject);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (isLocalPlayer && collision.gameObject.tag == "Bullet")
        {
            this.CmdChangeHealth(-5);
        }
    }

    void Update()
	{
        if (!_isPlayerSpawned && GameController.IsMazeReady)
        {
            _isPlayerSpawned = true;
            _playerModel.SetActive(true);
            this.SetupPlayer();
        }

        if (this.InvisibleTimer > 0)
        {
            this.InvisibleTimer -= Time.deltaTime;
            this.SetVisibility(false);
        }

        //determine if the object is inside the camera's viewing volume
        if (_nameLabel != null)
        {
            Vector3 viewportPoint = Camera.main.WorldToViewportPoint(this.transform.position);
            bool onViewport = viewportPoint.z > 0 && viewportPoint.x > 0 &&
                            viewportPoint.x < 1 && viewportPoint.y > 0 && viewportPoint.y < 1;
            //if it is on screen draw its label attached to is name position
            if (onViewport)
            {
                Vector3 nameLabelPos = Camera.main.WorldToScreenPoint(NamePos.position);
                _nameLabel.transform.position = nameLabelPos;
                _healthBar.transform.position = nameLabelPos + new Vector3(0, 15, 0);
            }
            else //otherwise draw it WAY off the screen
            {
                _nameLabel.transform.position = new Vector3(-1000, -1000, 0);
                _healthBar.transform.position = new Vector3(-1000, -1000, 0);
            }
        }
	}
}
