using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class FireControlBase : NetworkBehaviour {

    public GameObject BulletPrefab;

    [Header("Bullet Spawn Point")]
    public Transform BulletSpwanPoint;

    public float BulletForce = 400f;

    private int _fireDamage = 5;
    public  int DamageBuffer = 0;

    public bool IsBufferOn = false;

    [Command]
    protected void CmdShoot(int damage)
    {
        this.RpcCreateBullet(damage);
    }
    [ClientRpc]
    protected void RpcCreateBullet(int damage)
    {
        this.CreateBullet(damage);
    }

    void Update()
    {
        if (!isLocalPlayer) return;

        if (Input.GetKeyDown("space"))
        {
            this.CmdShoot(_fireDamage + this.DamageBuffer);
        }
    }

    protected virtual void CreateBullet(int damage)
    {
        GameObject bullet = Instantiate(this.BulletPrefab, this.BulletSpwanPoint.position, this.BulletSpwanPoint.rotation);
        bullet.GetComponent<Rigidbody>().AddForce(this.BulletSpwanPoint.forward * this.BulletForce);
        bullet.GetComponent<BulletBase>().BulletDamage = damage;
    }
}
