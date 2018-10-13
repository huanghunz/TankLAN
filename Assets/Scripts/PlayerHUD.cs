using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHUD : MonoBehaviour {

    public Text NamePrefab;
    public Transform NamePos;
    
    public Slider HealthPrefab;
    public Transform HealthPos;

    public Slider BulletCountPrefab;
    public Transform BulletCountPos;



    public bool IsLocalPlayer;

    private GameObject _canvas;
    private Text _nameLabel;
    private Slider _healthBar;
    private Slider _bulletCount;

    private static readonly Vector3 OFF_SCREEN_POS = new Vector3(-1000, -1000, 0);

    private void Awake()
    {
        // All UI should be a child of a canvas
        GameObject canvas = GameObject.FindWithTag("MainCanvas");
        _nameLabel = Instantiate(NamePrefab, Vector3.zero, Quaternion.identity) as Text;
        _nameLabel.transform.SetParent(canvas.transform);
        _healthBar = Instantiate(HealthPrefab, Vector3.zero, Quaternion.identity) as Slider;
        _healthBar.transform.SetParent(canvas.transform);

        _bulletCount = Instantiate(BulletCountPrefab, Vector3.zero, Quaternion.identity) as Slider;
        _bulletCount.transform.SetParent(canvas.transform);
    }
    
    private void Update()
    {
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

                if (this.IsLocalPlayer)
                {
                    Vector3 bulletCountPos = Camera.main.WorldToScreenPoint(BulletCountPos.position);
                    _bulletCount.transform.position = bulletCountPos;
                }
                else
                {
                    _bulletCount.transform.position = OFF_SCREEN_POS;
                }
            }
            else //otherwise draw it WAY off the screen
            {
                _nameLabel.transform.position = OFF_SCREEN_POS;
                _healthBar.transform.position = OFF_SCREEN_POS;
                _bulletCount.transform.position = OFF_SCREEN_POS;
            }
        }
    }


    void OnDestroy()
    {
        if (_nameLabel != null && _healthBar != null && _bulletCount != null)
        {
            Destroy(_nameLabel.gameObject);
            Destroy(_healthBar.gameObject);
            Destroy(_bulletCount.gameObject);
        }
    }

    public void SetVisible(bool visible)
    {
        _nameLabel.gameObject.SetActive(visible);
        _healthBar.gameObject.SetActive(visible);
        _bulletCount.gameObject.SetActive(visible);
    }

    public void UpdateHealth(int value)
    {
        _healthBar.value = value;
    }

    public void UpdateName(string name)
    {
        _nameLabel.text = name;
    }

    public void UpdateBulletCount(int count)
    {
        _bulletCount.value = count;
    }
}
