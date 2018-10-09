using UnityEngine.Networking;
using UnityEngine;


public class DoubleFireControl : NetworkBehaviour {

    public GameObject BulletPrefab;
    public Transform BulletSpwanPoint1;
    public Transform BulletSpwanPoint2;

    public float Bullet1Force = 400f;
    public float Bullet2Force = 400f;

    // Update is called once per frame
    void Update()
    {
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
    }

    void CreateBullet()
    {
        GameObject bullet = Instantiate(this.BulletPrefab, this.BulletSpwanPoint1.position, this.BulletSpwanPoint1.rotation);
        bullet.GetComponent<Rigidbody>().AddForce(this.BulletSpwanPoint1.forward * 400);
       // Destroy(bullet, 3f);

        GameObject bullet2 = Instantiate(this.BulletPrefab, this.BulletSpwanPoint2.position, this.BulletSpwanPoint2.rotation);
        bullet2.GetComponent<Rigidbody>().AddForce(this.BulletSpwanPoint2.forward * 400);
        //Destroy(bullet2, 3f);
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
