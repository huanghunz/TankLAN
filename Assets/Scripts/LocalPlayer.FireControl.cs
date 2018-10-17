using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public partial class LocalPlayer : NetworkBehaviour {

    // Fire control
    public GameObject BulletPrefab;

    public Transform[] BulletSpwanPoint;

    public float BulletForce = 400f;

    public int FireDamage = 5;

    public int MaxNumBullet = 10;

    public int NumBullerPerShooting = 1;

    private int _numBullet = 0;
    private float _bulletCoolDown = 3f;
    private bool _isRefilling;

    private const float REFILL_TIME = 3f;

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
    
    private void UpdateFireControl()
    {
        if (!isLocalPlayer) return;
        if (!_enableControl) return;

        if (_numBullet == 0 && !_isRefilling)
        {
            _bulletCoolDown = REFILL_TIME;
            _isRefilling = true;
            StartCoroutine(this.RefillBullet(REFILL_TIME));
        }

        _bulletCoolDown -= Time.deltaTime;
        if (_bulletCoolDown >= 0)
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
            _numBullet -= (this.BulletSpwanPoint.Length * this.NumBullerPerShooting);
            _HUD.UpdateBulletCount(_numBullet);
        }
    }

    protected virtual void CreateBullet(int damage)
    {
        for (int i = 0; i < this.BulletSpwanPoint.Length; ++i)
        {
            for (int j = 0; j < this.NumBullerPerShooting; ++j)
            {
                GameObject bullet = Instantiate(this.BulletPrefab, this.BulletSpwanPoint[i].position, this.BulletSpwanPoint[i].rotation);
                bullet.GetComponent<Rigidbody>().AddForce(this.BulletSpwanPoint[i].forward * this.BulletForce);
                bullet.GetComponent<BulletBase>().BulletDamage = damage;
            }
        }
    }

    protected IEnumerator RefillBullet(float time)
    {
        var delay = new WaitForEndOfFrame();
        float elapsed = 0;
        while(elapsed < 1)
        {
            elapsed += Time.deltaTime / time;
            int count = Mathf.FloorToInt(elapsed * this.MaxNumBullet);
            _HUD.UpdateBulletCount(count);
            yield return delay;
        }

        _numBullet = this.MaxNumBullet;
        _isRefilling = false;
    }
}
