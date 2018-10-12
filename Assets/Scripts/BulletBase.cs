using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class BulletBase : NetworkBehaviour {

    public GameObject Explosion;
    public int BulletDamage = 5;

    void Awake()
    {
        Debug.Assert(this.Explosion != null, "Please assign bullet explosion object.");
    }

    [ClientRpc]
    public void RpcCreateExplosion()
    {
        GameObject e = Instantiate(this.Explosion, this.transform.position, Quaternion.identity);
        //e.transform.parent = this.transform;
        e.AddComponent<NetworkIdentity>();
        e.AddComponent<NetworkTransform>();

        Destroy(this.gameObject, 0.1f);
        Destroy(e, 0.3f);
    }

    [Command]
    private void CmdOnCollisionEnter()
    {
        this.RpcCreateExplosion();
    }
}
