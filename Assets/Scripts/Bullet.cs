using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Bullet : BulletBase {
    
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            collision.transform.GetComponent<LocalPlayer>()
                .AddDamage(-this.BulletDamage);

            Vector3 force = collision.transform.position - this.transform.position;
            force = new Vector3(force.x, 0, force.z);
            collision.transform.GetComponent<Rigidbody>().AddForce(force.normalized * 200f);
        }

        GameObject e = Instantiate(this.Explosion, this.transform.position, Quaternion.identity);
        e.AddComponent<NetworkIdentity>();
        e.AddComponent<NetworkTransform>();
        //NetworkServer.Spawn(e);
        Destroy(this.gameObject, 0.1f);
        Destroy(e, 0.3f);
    }
}
