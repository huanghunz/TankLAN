using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class FireControlBase : NetworkBehaviour {

    public GameObject BulletPrefab;
    
    public Transform BulletSpwanPoint;

    public float BulletForce = 400f;

    protected int FireDamage = 5;

    public int MaxNumBullet = 10;

    public int NumBullerPerShooting = 1; 

    protected int NumBullet = 0;
    protected float BulletCoolDown = 3f;

    protected PlayerHUD PlayerUI;
    protected bool _isRefilling;

    protected const float REFILL_TIME = 3f;

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

    private void Awake()
    {
        this.PlayerUI = this.GetComponent<PlayerHUD>();
    }

    void Update()
    {
        if (!isLocalPlayer) return;
        var b = GetComponent<LocalPlayer>().isLocalPlayer;
        if (this.NumBullet == 0 && !_isRefilling)
        {
            this.BulletCoolDown = REFILL_TIME;
            _isRefilling = true;
            StartCoroutine(this.RefillBullet(REFILL_TIME));
        }

        this.BulletCoolDown -= Time.deltaTime;
        if (this.BulletCoolDown >= 0)
        {
            return;
        }

        this.Shoot();
    }

    protected virtual void Shoot()
    {
        if (Input.GetKeyDown("space"))
        {
            this.CmdShoot(FireDamage);
            this.NumBullet -= this.NumBullerPerShooting;
            this.PlayerUI.UpdateBulletCount(this.NumBullet);
        }
    }

    protected virtual void CreateBullet(int damage)
    {
        for (int i = 0; i < this.NumBullerPerShooting; ++i)
        {
            GameObject bullet = Instantiate(this.BulletPrefab, this.BulletSpwanPoint.position, this.BulletSpwanPoint.rotation);
            bullet.GetComponent<Rigidbody>().AddForce(this.BulletSpwanPoint.forward * this.BulletForce);
            bullet.GetComponent<BulletBase>().BulletDamage = damage;
        }
    }

    protected IEnumerator RefillBullet(float time)
    {
        var delay = new WaitForEndOfFrame();
        float elapsed = 0;
        while(elapsed < 1)
        {
            elapsed += Time.deltaTime / time;
            int count = Mathf.FloorToInt(elapsed * 10);
            this.PlayerUI.UpdateBulletCount(count);
            yield return delay;
        }

        this.NumBullet = 10;
        _isRefilling = false;
    }
}
