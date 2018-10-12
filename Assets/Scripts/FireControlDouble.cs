using UnityEngine.Networking;
using UnityEngine;


public class FireControlDouble : FireControlBase{

    // public GameObject BulletPrefab;
    [Header("Bullet Spawn Point")]
    public Transform BulletSpwanPointSecond;


    //// Update is called once per frame
    //void Update()
    //{
    //    if (!isLocalPlayer) return;

    //    if (Input.GetKeyDown("space"))
    //    {
    //        // talk to the server
    //        this.CmdShoot();
    //    }
    //}

    //[ClientRpc]
    //void RpcCreateBullet()
    //{
    //    this.CreateBullet();
    //}

    protected override void CreateBullet(int damage)
    {
        // GameObject bullet = Instantiate(this.BulletPrefab, this.BulletSpwanPoint.position, this.BulletSpwanPoint.rotation);
        // bullet.GetComponent<Rigidbody>().AddForce(this.BulletSpwanPoint.forward * this.BulletForce);
        //// Destroy(bullet, 3f);
        ///
        base.CreateBullet(damage);

        GameObject bullet2 = Instantiate(this.BulletPrefab, this.BulletSpwanPointSecond.position, this.BulletSpwanPointSecond.rotation);
        bullet2.GetComponent<Rigidbody>().AddForce(this.BulletSpwanPointSecond.forward * this.BulletForce);
        //Destroy(bullet2, 3f);
        bullet2.GetComponent<BulletBase>().BulletDamage = damage;
    }

    //[Command]
    //void CmdShoot()
    //{
    //    this.RpcCreateBullet();
        
    //}
}
