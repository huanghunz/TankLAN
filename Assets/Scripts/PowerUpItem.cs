using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpItem : MonoBehaviour {

    public delegate void TriggerEntered(GameObject go, Types type);
    public event TriggerEntered OnTriggerEntered;

    public enum Types
    {
        AddHealth,
        TopDownView,
        Invisible
    }

    public GameObject[] PowerUpVisial;
    public GameObject PowerUpBox;

    public Types PowerUpType;

    private bool _isTriggered;

    private void Awake()
    {
        if (PowerUpVisial.Length != Enum.GetNames(typeof(PowerUpItem.Types)).Length)
        {
            Debug.LogError("Powerup prefabs has not matched the length of power up types");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag != "Player") return;
        
        _isTriggered = true;

        if (this.OnTriggerEntered != null)
        {
            this.OnTriggerEntered(other.gameObject, this.PowerUpType);
        }

        GameObject typeVisual = this.PowerUpVisial[(int)this.PowerUpType];
        typeVisual.SetActive(true);

        TankUtility.Utility.Instance.AnimateScale(this.PowerUpBox, Vector3.zero, 0.25f, delegate
        {
           
            Debug.Log("visual name: " + typeVisual.name);
            TankUtility.Utility.Instance.AnimateScale(typeVisual, Vector3.zero, 0.75f,
                                                      delegate {
                                                          this.gameObject.SetActive(false);
                                                      });
        });

       
    }

    void FixedUpdate () {
        transform.Rotate(Vector3.up, Time.fixedDeltaTime * 45f);

        //if (_isTriggered)
        //{
        //    transform.localScale = Vector3.Lerp(transform.localScale, Vector3.zero, Time.fixedDeltaTime);
        //    if (transform.localScale == Vector3.zero)
        //    {
        //        this.gameObject.SetActive(false);
        //    }
        //}
	}
}
