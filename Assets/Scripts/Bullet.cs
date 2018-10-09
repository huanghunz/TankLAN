using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Bullet : NetworkBehaviour {

    public GameObject Explosion;

    [ClientRpc]
    public void RpcCreateExplosion()
    {
        GameObject e = Instantiate(this.Explosion, this.transform.position, Quaternion.identity);
        //e.transform.parent = this.transform;
        e.AddComponent<NetworkIdentity>();
        e.AddComponent<NetworkTransform>();
        //NetworkServer.Spawn(e);
        Destroy(this.gameObject, 0.1f);
        Destroy(e, 0.3f);
    }

    [Command]
    private void CmdOnCollisionEnter()
    {
        this.RpcCreateExplosion();
    }

    private void Start()
    {
        Debug.Assert(this.Explosion != null, "Please assign bullet explosion object.");
    }

    void OnCollisionEnter(Collision collision)
    {
        GameObject e = Instantiate(this.Explosion, this.transform.position, Quaternion.identity);
        //e.transform.parent = this.transform;
        e.AddComponent<NetworkIdentity>();
        e.AddComponent<NetworkTransform>();
        //NetworkServer.Spawn(e);
        Destroy(this.gameObject, 0.1f);
        Destroy(e, 0.3f);
    }
}
