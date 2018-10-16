using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModelData : MonoBehaviour {

    public GameObject BulletPrefab;
    public float BulletForces;
    public Transform[] BulletSpawnPositions;
    public int NumBulletPerShooting;
    public int MaxNumBullet;

    public void Awake()
    {
        
    }
}
