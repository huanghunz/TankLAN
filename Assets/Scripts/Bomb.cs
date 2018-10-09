using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Networking;

public class Bomb : NetworkBehaviour {

    public GameObject Explosion;
    public GameObject BombEffect;

    public Transform BombEffectPos;
    private GameObject _bombEffect;

    private float _exposeTime = 3f;

    private List<GameObject> _playerInRange;
    private MeshRenderer _bombRange;

    void Start()
    {
        Debug.Assert(this.Explosion != null, "Please assign explosion object.");
        Debug.Assert(this.BombEffect != null, "Please assign bomb effect object.");

        _bombEffect = Instantiate(this.BombEffect, this.BombEffectPos.position, Quaternion.identity);
        _bombEffect.transform.parent = this.BombEffectPos;
        _bombEffect.AddComponent<NetworkIdentity>();
        _bombEffect.AddComponent<NetworkTransform>();
        NetworkServer.Spawn(_bombEffect);

        _playerInRange = new List<GameObject>();
        Transform range = this.transform.Find("BombRange");
        Debug.Assert(range != null, "Fail to find object of name 'BombRange'");
        _bombRange = range.GetComponent<MeshRenderer>();

        StartCoroutine(this.UpdateRange(_exposeTime, Vector3.one * 0.8f));

        Invoke("ExposeAfterSeconds", _exposeTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("on trigeer ender" + other.gameObject.name) ;
        if (other.gameObject.tag != "Player") return;

        _playerInRange.Add(other.gameObject);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag != "Player") return;
        _playerInRange.Remove(other.gameObject);
    }

    void ExposeAfterSeconds()
    {
        GameObject e = Instantiate(this.Explosion, this.transform.position, Quaternion.identity);
        e.AddComponent<NetworkIdentity>();
        e.AddComponent<NetworkTransform>();
        NetworkServer.Spawn(e);

        foreach(GameObject player in _playerInRange)
        {
            player.GetComponent<LocalPlayer>().AddDamage(-5);
        }

        Destroy(this.gameObject, 0.1f);
        Destroy(e, 2f);
    }

    

    private IEnumerator UpdateRange(float time, Vector3 targetScale)
    {
        float timeStep = 0;
        while (timeStep < 1)
        {
            timeStep += Time.deltaTime/time;
            Color oldColor = _bombRange.material.color;
            Color newColor = new Color(oldColor.r, oldColor.g, oldColor.b, 0.5f * timeStep);
            _bombRange.material.SetColor("_Color", newColor);

            _bombRange.transform.localScale = Vector3.Lerp(_bombRange.transform.localScale, targetScale, timeStep);

            yield return new WaitForEndOfFrame();
        }

        //var currentPos = transform.position;
        //var t = 0f;
        //while (t < 1)
        //{
        //    t += Time.deltaTime / timeToMove;
        //    transform.position = Vector3.Lerp(currentPos, position, t);
        //    yield return null;
        //}
    }
}
