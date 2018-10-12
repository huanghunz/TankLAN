using System;
using UnityEngine;

public class PowerUpItem : MonoBehaviour {

    public delegate void TriggerEntered(GameObject go, Types type);
    public event TriggerEntered OnTriggerEntered;

    public delegate void DestroySelf(PowerUpItem item);
    public event DestroySelf OnDestorySelf;

    public enum Types
    {
        AddHealth,
        TopDownView,
        Invisible
    }

    public GameObject[] PowerUpVisial;
    public GameObject PowerUpBox;

    public Types PowerUpType;

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

        if (this.OnTriggerEntered != null)
        {
            this.OnTriggerEntered(other.gameObject, this.PowerUpType);
        }

        GameObject typeVisual = this.PowerUpVisial[(int)this.PowerUpType];
        typeVisual.SetActive(true);
        typeVisual.transform.LookAt(other.transform);

        TankUtility.Utility.Instance.AnimateScale(this.PowerUpBox, Vector3.zero, 0.25f);

        Vector3 targetPosLocal = new Vector3(typeVisual.transform.localPosition.x,
                                        typeVisual.transform.localPosition.y + 2,
                                        typeVisual.transform.localPosition.z);

        TankUtility.Utility.Instance.AnimateMoveLocalWithDelay(0.1f, typeVisual, targetPosLocal, 0.5f);

        TankUtility.Utility.Instance.AnimateScaleWithDelay(0.1f, typeVisual, Vector3.zero, 0.5f,
                                                      delegate
                                                      {
                                                          this.gameObject.SetActive(false);
                                                      });
    }

    private void OnDestroy()
    {
        if (this.OnDestorySelf != null)
        {
            this.OnDestorySelf(this);
        }
    }

}
