
using UnityEngine.Networking;
using UnityEngine;

public class FireControlSingle : FireControlBase {

    [Command]
    void CmdShoot()
    {
        this.RpcCreateBullet();
    }
    [ClientRpc]
    void RpcCreateBullet()
    {
        this.CreateBullet();
    }

    // Update is called once per frame
    void Update () {

        if (!isLocalPlayer) return;
       
		if (Input.GetKeyDown("space"))
        {
            // talk to the server
            this.CmdShoot();
        }
	}
   
    void CreateBullet()
    {
        //Debug.Log("isLocal? " + isLocalPlayer + " buffDam: " + this.FireDamageBuffer);
        //GameObject bullet = Instantiate(this.BulletPrefab, this.BulletSpwanPoint.position, this.BulletSpwanPoint.rotation);
        //bullet.GetComponent<Rigidbody>().AddForce(this.BulletSpwanPoint.forward * this.BulletForce);
        //bullet.GetComponent<BulletBase>().BulletDamage = this._fireDamage + this.FireDamageBuffer;
    }

   
}
