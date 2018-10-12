using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Bullet : BulletBase {
    

    //[ClientRpc]
    //public void RpcCreateExplosion()
    //{
    //    GameObject e = Instantiate(this.Explosion, this.transform.position, Quaternion.identity);
    //    //e.transform.parent = this.transform;
    //    e.AddComponent<NetworkIdentity>();
    //    e.AddComponent<NetworkTransform>();

    //    Destroy(this.gameObject, 0.1f);
    //    Destroy(e, 0.3f);
    //}

    //[Command]
    //private void CmdOnCollisionEnter()
    //{
    //    this.RpcCreateExplosion();
    //}

    

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            collision.transform.GetComponent<LocalPlayer>()
                .AddDamage(-this.BulletDamage);
        }

        GameObject e = Instantiate(this.Explosion, this.transform.position, Quaternion.identity);
        e.AddComponent<NetworkIdentity>();
        e.AddComponent<NetworkTransform>();
        //NetworkServer.Spawn(e);
        Destroy(this.gameObject, 0.1f);
        Destroy(e, 0.3f);
    }
}
