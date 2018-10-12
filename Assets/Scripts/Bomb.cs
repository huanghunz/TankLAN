using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Networking;

public class Bomb : BulletBase {
    
    public GameObject BombEffect;
   
    private float _exposeTime = 3f;

    private List<GameObject> _playerInRange;
    private MeshRenderer _bombRange;

    void Start()
    {
        Debug.Assert(this.BombEffect != null, "Please assign bomb effect object.");

        _playerInRange = new List<GameObject>();
        Transform range = this.transform.Find("BombRange");
        Debug.Assert(range != null, "Fail to find object of name 'BombRange'");
        _bombRange = range.GetComponent<MeshRenderer>();


        TankUtility.Utility.Instance.AnimateScale(this.gameObject, Vector3.one * 4f, _exposeTime, delegate
        {
            this.Expose();
        });

        TankUtility.Utility.Instance.AnimateAlpha(_bombRange, 0.5f, _exposeTime);
        //StartCoroutine(this.UpdateRange(_exposeTime, Vector3.one * 4f));

        //Invoke("Expose", _exposeTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag != "Player") return;

        _playerInRange.Add(other.gameObject);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag != "Player") return;
        _playerInRange.Remove(other.gameObject);
    }

    void Expose()
    {
        GameObject e = Instantiate(this.Explosion, this.transform.position, Quaternion.identity);
        e.AddComponent<NetworkIdentity>();
        e.AddComponent<NetworkTransform>();

        foreach(GameObject player in _playerInRange)
        {
            Vector3 force = player.transform.position - this.transform.position;
            player.GetComponent<Rigidbody>().AddForce(force.normalized * 1500f);
            player.GetComponent<LocalPlayer>().AddDamage(-this.BulletDamage);
        }

        Destroy(this.gameObject, 0.1f);
        Destroy(e, 2f);
    }

    private IEnumerator UpdateRange(float time, Vector3 targetScale)
    {
        float timeStep = 0;
        while (timeStep < time)
        {
            timeStep += Time.deltaTime/time;
            Color oldColor = _bombRange.material.color;
            Color newColor = new Color(oldColor.r, oldColor.g, oldColor.b, 0.5f * timeStep);
            _bombRange.material.SetColor("_Color", newColor);

            this.transform.localScale = Vector3.Lerp(this.transform.localScale, targetScale, timeStep);

            yield return new WaitForEndOfFrame();
        }
    }
}
