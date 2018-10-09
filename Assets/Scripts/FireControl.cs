
using UnityEngine.Networking;
using UnityEngine;

public class FireControl : NetworkBehaviour {

    public GameObject BulletPrefab;
    public Transform BulletSpwanPoint;

    public float BulletForce = 400f;

    // Update is called once per frame
    void Update () {

        if (!isLocalPlayer) return;
       
		if (Input.GetKeyDown("space"))
        {
            // talk to the server
            this.CmdShoot();
        }
	}

    [ClientRpc]
    void RpcCreateBullet()
    {
        this.CreateBullet();
        //if (!isServer)
        //{
        //    this.CreateBullet();
        //}
    }

    void CreateBullet()
    {
        GameObject bullet = Instantiate(this.BulletPrefab, this.BulletSpwanPoint.position, this.BulletSpwanPoint.rotation);
        bullet.GetComponent<Rigidbody>().AddForce(this.BulletSpwanPoint.forward * this.BulletForce);
        //Destroy(bullet, 3f);
    }

    [Command]
    void CmdShoot()
    {
        //this.CreateBullet();
        this.RpcCreateBullet();
        //GameObject bullet = Instantiate(this.BulletPrefab, this.BulletSpwanPoint.position, this.BulletSpwanPoint.rotation);
        //bullet.GetComponent<Rigidbody>().AddForce(this.BulletSpwanPoint.forward * 2000);

        ////Ask server to create bullet -- lagging
        //NetworkServer.Spawn(bullet);
        //Destroy(bullet, 3f);
    }
}
