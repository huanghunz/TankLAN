using UnityEngine.Networking;
using UnityEngine;


public class FireControlDouble : FireControlBase{

    // public GameObject BulletPrefab;
    [Header("Bullet Spawn Point")]
    public Transform BulletSpwanPointSecond;

    protected override void Shoot()
    {
        if (Input.GetKeyDown("space"))
        {
            this.CmdShoot(FireDamage);
            this.NumBullet -= 2;
            this.PlayerUI.UpdateBulletCount(this.NumBullet);
        }
    }

    protected override void CreateBullet(int damage)
    {
        base.CreateBullet(damage);

        GameObject bullet2 = Instantiate(this.BulletPrefab, this.BulletSpwanPointSecond.position, this.BulletSpwanPointSecond.rotation);
        bullet2.GetComponent<Rigidbody>().AddForce(this.BulletSpwanPointSecond.forward * this.BulletForce);
        //Destroy(bullet2, 3f);
        bullet2.GetComponent<BulletBase>().BulletDamage = damage;
    }
}
